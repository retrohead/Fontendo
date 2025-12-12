using Fontendo;
using Fontendo.UI;
using System;
using System.ComponentModel;
using System.Windows;

public class objectAnimations
{
    public enum animTypes
    {
        appear,
        disappear
    }
    public enum animSpeed
    {
        normal,
        medium,
        fast,
        instant
    }
    public animTypes animType;
    public dynamic obj;
    public double opacity;
    public bool blockForm;
    public double alphaStep;
    private UI_MainWindow mainFrm;
    private BackgroundWorker bgwrk;
    private BackgroundWorker bgwrkAppearDelay;

    public dynamic? completeFunctionObj;
    public Action? completeFunctionDelegate;
    public object? completeFunctionParams;

    public dynamic? delayFunctionObj;
    public Action? delayFunctionDelegate;
    public object? delayFunctionParams;

    public void runCompleteFunction()
    {
        if (completeFunctionDelegate != null)
        {
            Type? t = completeFunctionObj?.GetType();

            if (completeFunctionParams == null)
                completeFunctionObj?.Dispatcher.BeginInvoke(completeFunctionDelegate, null);
            else
                completeFunctionObj?.Dispatcher.BeginInvoke(completeFunctionDelegate, completeFunctionParams);
        }
    }

    public objectAnimations(UI_MainWindow MainWindow, dynamic objToFade, animTypes animationType, animSpeed animSpeed, bool blockFormActions, dynamic onCompleteFunctionObj, Action onCompleteFunctionDelegate, object? onCompleteFunctionParams = null, double animSpeedMultiplyer = 1)
    {
        completeFunctionObj = onCompleteFunctionObj;
        completeFunctionDelegate = onCompleteFunctionDelegate;
        completeFunctionParams = onCompleteFunctionParams;

        if (bgwrk == null)
        {
            bgwrk = new BackgroundWorker();
            bgwrk.DoWork += bgwrk_DoWork;
            bgwrk.RunWorkerCompleted += bgwrk_RunWorkerCompleted;
        }

        if ((animationType == animTypes.appear))
        {
            if ((objToFade.Opacity != 1))
            {
                opacity = 0;
                objToFade.Opacity = 0;
            }
        }
        else if ((objToFade.Opacity != 0))
        {
            objToFade.Opacity = 1;
            opacity = 1;
        }


        mainFrm = MainWindow;
        animType = animationType;


        switch (animSpeed)
        {
            case animSpeed.normal:
                alphaStep = 0.04;
                break;
            case animSpeed.medium:
                alphaStep = 0.1;
                break;
            case animSpeed.fast:
                alphaStep = 0.25;
                break;
            case animSpeed.instant:
                alphaStep = 1.0;
                break;
            default:
                alphaStep = 0.04;
                break;
        }


        if (alphaStep < 1.0)
            alphaStep = alphaStep * animSpeedMultiplyer;

        if (alphaStep > 1.0)
            alphaStep = 1.0;

        obj = objToFade;
        blockForm = blockFormActions;

        if (bgwrkAppearDelay == null)
        {
            bgwrkAppearDelay = new BackgroundWorker();
            bgwrkAppearDelay.DoWork += bgwrkAppearDelay_DoWork;
            bgwrkAppearDelay.RunWorkerCompleted += bgwrkAppearDelay_RunWorkerCompleted;
        }
    }

    public void run()
    {
        if ((animType == animTypes.appear))
        {
            if ((obj.Opacity == 1))
            {
                runCompleteFunction();
                return;
            }
        }
        else if ((obj.Opacity == 0))
        {
            runCompleteFunction();
            return;
        }
        obj.Opacity = opacity;
        bgwrk.RunWorkerAsync();

        if ((blockForm))
        {
            if ((popUps.IsOpen() == false))
                mainFrm.enableForm(false);
            else
                mainFrm.enablePopUp(false);
            mainFrm.updateProgressLabel("Please Wait...");
            mainFrm.panelSmallProgress.Visibility = Visibility.Visible;
        }
    }

    private void bgwrk_DoWork(object? sender, System.ComponentModel.DoWorkEventArgs e)
    {
        System.Threading.Thread.Sleep(1);
    }

    private void bgwrk_RunWorkerCompleted(object? sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
    {
        bool complete = false;
        if ((animType == animTypes.appear))
        {
            opacity = opacity + alphaStep;
            if ((opacity >= 1.0))
            {
                opacity = 1.0;
                complete = true;
            }
        }
        else
        {
            opacity = opacity - alphaStep;
            if ((opacity <= 0.0))
            {
                opacity = 0.0;
                complete = true;
                obj.Visibility = Visibility.Hidden;
            }
        }
        obj.Opacity = opacity;
        if ((complete == false))
            bgwrk.RunWorkerAsync();
        else
        {
            if ((blockForm))
            {
                mainFrm.panelSmallProgress.Visibility = Visibility.Hidden;

                if ((popUps.IsOpen() == false))
                    mainFrm.enableForm(true);
                else
                    mainFrm.enablePopUp(true);
            }
            runCompleteFunction();
        }
    }

    public static void prepareAppear(ref UI_MainWindow mainFrm, dynamic obj, animSpeed animSpeed, bool blockFormActions, dynamic onCompleteFunctionObj, Action onCompleteFunctionDelegate, object? onCompleteFunctionParams = null, double animSpeedMulitpler = 1)
    {
        if ((obj.Opacity != 1))
        {
            try
            {
                if ((obj.InvokeRequired))
                    obj.BeginInvoke(new UI_MainWindow.setObjectOpacityDelegate(UI_MainWindow.setObjectOpacity), 0);
                else
                    obj.Opacity = 0;
            }
            catch
            {
                obj.Opacity = 0;
            }
        }
        try
        {
            if ((obj.InvokeRequired))
                obj.BeginInvoke(new UI_MainWindow.setObjectVisibilityDelegate(UI_MainWindow.setObjectVisibility), Visibility.Visible);
            else
                obj.Visibility = Visibility.Visible;
        }
        catch
        {
            obj.Visibility = Visibility.Visible;
        }

        UI_MainWindow.objectAnim = new objectAnimations(mainFrm, obj, animTypes.appear, animSpeed, blockFormActions, onCompleteFunctionObj, onCompleteFunctionDelegate, onCompleteFunctionParams, animSpeedMulitpler);
    }

    private void bgwrkAppearDelay_DoWork(object? sender, System.ComponentModel.DoWorkEventArgs e)
    {
        System.Threading.Thread.Sleep(1);
    }

    private void bgwrkAppearDelay_RunWorkerCompleted(object? sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
    {
        // run the function after delay
        if (delayFunctionDelegate != null)
        {
            Type? t = delayFunctionObj?.GetType();
            if (delayFunctionParams == null)
                delayFunctionObj?.Dispatcher.BeginInvoke(delayFunctionDelegate);
            else
                delayFunctionObj?.Dispatcher.BeginInvoke(delayFunctionDelegate, delayFunctionParams);
        }

        // now make the item appear
        UI_MainWindow.objectAnim?.run();
    }

    public static void panelAppear(ref UI_MainWindow? mainFrm, object? obj, bool blockFormActions, dynamic onCompleteFunctionObj, Action onCompleteFunctionDelegate, object onCompleteFunctionParams, dynamic delayCompleteFunctionObj, Action delayCompleteFunctionDelegate, object delayCompleteFunctionParams)
    {
        prepareAppear(ref mainFrm, obj, (animSpeed)Fontendo.Properties.Settings.Default.AnimationSpeed, blockFormActions, onCompleteFunctionObj, onCompleteFunctionDelegate, onCompleteFunctionParams);

        if (UI_MainWindow.objectAnim != null)
        {
            UI_MainWindow.objectAnim.completeFunctionObj = onCompleteFunctionObj;
            UI_MainWindow.objectAnim.delayFunctionObj = delayCompleteFunctionObj;
            UI_MainWindow.objectAnim.delayFunctionDelegate = delayCompleteFunctionDelegate;
            UI_MainWindow.objectAnim.delayFunctionParams = delayCompleteFunctionParams;
            UI_MainWindow.objectAnim.bgwrkAppearDelay.RunWorkerAsync();
        }
    }

    public static void makeAppear(ref UI_MainWindow mainFrm, dynamic? obj, bool blockFormActions, dynamic? onCompleteFunctionObj, Action onCompleteFunctionDelegate, object? onCompleteFunctionParams = null, double animSpeedMulitpler = 1)
    {
        animSpeed animSpeed = (animSpeed)Fontendo.Properties.Settings.Default.AnimationSpeed;
        prepareAppear(ref mainFrm, obj, animSpeed,  blockFormActions, onCompleteFunctionObj, onCompleteFunctionDelegate, onCompleteFunctionParams, animSpeedMulitpler);
        UI_MainWindow.objectAnim?.run();
    }
    public static void makeDisappear(UI_MainWindow? mainFrm, dynamic? obj, bool blockFormActions, dynamic onCompleteFunctionObj, Action onCompleteFunctionDelegate, object? onCompleteFunctionParams = null, double animSpeedMulitpler = 1)
    {
        if (obj == null)
            return;
        if (obj.Opacity == 0)
        {
            // already hidden, just run the function
            if (!(onCompleteFunctionDelegate == null))
            {
                Type t = onCompleteFunctionObj.GetType();
                if ((onCompleteFunctionParams == null))
                    onCompleteFunctionObj.Dispatcher.BeginInvoke(onCompleteFunctionDelegate);
                else
                    onCompleteFunctionObj.Dispatcher.BeginInvoke(onCompleteFunctionDelegate, onCompleteFunctionParams);
            }
            return;
        }
        obj.Opacity = 1;
        obj.Visibility = Visibility.Visible;
        animSpeed animSpeed = (animSpeed)Fontendo.Properties.Settings.Default.AnimationSpeed;
        UI_MainWindow.objectAnim = new objectAnimations(mainFrm, obj, animTypes.disappear, animSpeed, blockFormActions, onCompleteFunctionObj, onCompleteFunctionDelegate, onCompleteFunctionParams, animSpeedMulitpler);
        UI_MainWindow.objectAnim.run();
    }

}
