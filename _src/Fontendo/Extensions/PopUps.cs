using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.ComponentModel;
using Fontendo;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows;
using System.Windows.Shapes;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using Fontendo.Popups;

public class popUps
{
    public class popUpType
    {
        public popUpType(MainWindow MainWindow, ref dynamic objectToShow, bool max_size)
        {
            maxSize = max_size;
            bgWorkDelay = new BackgroundWorker();
            appearDelayCompleteFunctionDelegate = null;
            appearDelayCompleteFunctionParams = null;
            cancelFunctionDelegate = null;
            cancelFunctionParams = null;
            obj = objectToShow;
            mainFrm = MainWindow;
            objectToShow.popUpObj = this;
            createPopUpElements(maxSize);
            Objects.Add(this);
        }
        private bool maxSize;
        private dynamic obj;
        public Action? appearDelayCompleteFunctionDelegate;
        public object ? appearDelayCompleteFunctionParams;
        public Action? cancelFunctionDelegate;
        public object ? cancelFunctionParams;
        public MainWindow mainFrm;
        public bool IsOpen = false;

        public void appear(Action? delayCompleteFunctionDelegate = null, object? delayCompleteFunctionParams = null)
        {
            if ((IsOpen))
            {
                if ((delayCompleteFunctionDelegate == null))
                    return;
                else
                    // Application.MyMsgBox("WARNING - Trying to run popup appear on object that is already appeared but asking for a delay function to be ran too!")
                    return;
            }
            appearDelayCompleteFunctionDelegate = delayCompleteFunctionDelegate;
            appearDelayCompleteFunctionParams = delayCompleteFunctionParams;
            bgWorkDelay?.RunWorkerAsync();
        }

        public void popUpBgFade_Complete()
        {
            try
            {
                Grid.SetRow((System.Windows.UIElement)obj, 1);
                dynamic elm = (System.Windows.UIElement)obj;
                window.panelPopupContent?.Children.Add((System.Windows.UIElement)obj);
                if(maxSize)
                {
                    elm.Width = Double.NaN;
                    elm.Height = Double.NaN;
                    elm.MaxHeight = window.panelPopupContent?.ActualHeight ?? 0;
                }
                window.btnCancelPopUp?.Focus();
                obj.load();
            }
            catch (Exception ex)
            {
                App.CustomMessageBox.Show("Fatal Error after pop up bg has faded in:\r\n\r\n" + ex.Message, "Fatal Error", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
        }

        public void resize(double newsize)
        {
            if (!maxSize)
                return;
            dynamic elm = (System.Windows.UIElement)obj;
            elm.MaxHeight = newsize - 180;
            obj.resize(newsize, MainWindow.Self?.ActualWidth ?? 0);
        }

        public void popUpBgFade2_Complete()
        {
            window.btnCancelPopUp?.Focus();
        }

        private BackgroundWorker? _bgWorkDelay;

        public BackgroundWorker? bgWorkDelay
        {
            [MethodImpl(MethodImplOptions.Synchronized)]
            get
            {
                return _bgWorkDelay;
            }

            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                if (_bgWorkDelay != null)
                {
                    _bgWorkDelay.DoWork -= bgWorkDelay_DoWork;
                    _bgWorkDelay.RunWorkerCompleted -= bgWorkDelay_RunWorkerCompleted;
                }

                _bgWorkDelay = value;
                if (_bgWorkDelay != null)
                {
                    _bgWorkDelay.DoWork += bgWorkDelay_DoWork;
                    _bgWorkDelay.RunWorkerCompleted += bgWorkDelay_RunWorkerCompleted;
                }
            }
        }

        private void bgWorkDelay_DoWork(object? sender, DoWorkEventArgs e)
        {
            System.Threading.Thread.Sleep(5);
        }
        private void bgWorkDelay_RunWorkerCompleted(object? sender, RunWorkerCompletedEventArgs e)
        {
            if(mainFrm == null)
            {
               throw new Exception("Fatal Error: Main window is null when trying to open a pop up");
            }
            try
            {
                if (!maxSize)
                {
                    if (window != null && window.panelPopupBoxGrid != null)
                    {
                        window.panelPopupBoxGridWrapper.Width = ((Control)window.panelPopupContent.Children[0]).MinWidth + 20;
                        window.panelPopupBoxGridWrapper.Height = ((Control)window.panelPopupContent.Children[0]).MinHeight + 10 + window.panelPopupBoxGrid.RowDefinitions[0].Height.Value;
                    }
                }
                Border bd = (Border)VisualTreeHelper.GetChild(window.panelPopupBoxGridWrapper, 0);
                if (window.panelPopupBoxGrid != null && bd != null)
                {
                    bd.Width = Double.NaN;
                    bd.Height = Double.NaN;
                    bd.Margin = new Thickness(0, 0, 0, 0);
                }
                IsOpen = true;


                if (!(appearDelayCompleteFunctionDelegate == null))
                {
                    Type t = obj.GetType();

                    if ((appearDelayCompleteFunctionParams == null))
                        obj.Dispatcher.BeginInvoke(appearDelayCompleteFunctionDelegate);
                    else
                        obj.Dispatcher.BeginInvoke(appearDelayCompleteFunctionDelegate, appearDelayCompleteFunctionParams);
                }
                if (maxSize)
                    obj.resize(MainWindow.Self?.ActualHeight ?? 0, MainWindow.Self?.ActualWidth ?? 0);
                

                objectAnimations.makeAppear(ref mainFrm, window, false, mainFrm, popUpBgFade2_Complete);
            }
            catch (Exception ex)
            {
                App.CustomMessageBox.Show("Fatal Error after after pop up appears:\r\n\r\n" + ex.Message, "Fatal Error", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
        }

        public void closePopUp(Action? completeFunctionDelegate, object? completeFunctionParams)
        {
            appearDelayCompleteFunctionParams = completeFunctionParams;
            appearDelayCompleteFunctionDelegate = completeFunctionDelegate;
            objectAnimations.makeDisappear(mainFrm, window, false, mainFrm, popUpBgFadeOut_Complete);
        }

        public void popUpBgFadeOut_Complete()
        {
            window.panelPopupContent?.Children.RemoveRange(0, window.panelPopupContent.Children.Count);
            objectAnimations.makeDisappear(mainFrm, panelPopupBackground, false, mainFrm, popUpBgFadeOut2_Complete);
        }

        public void popUpBgFadeOut2_Complete()
        {
            cancelFunctionDelegate = null;
            cancelFunctionParams = null;
            if ((Objects.Count > 1))
                mainFrm.enablePopUp(true);
            else
                mainFrm.enableForm(true);
            if (!(appearDelayCompleteFunctionDelegate == null))
            {
                Type t = obj.GetType();
                if ((appearDelayCompleteFunctionParams == null))
                    obj.Dispatcher.BeginInvoke(appearDelayCompleteFunctionDelegate);
                else
                    obj.Dispatcher.BeginInvoke(appearDelayCompleteFunctionDelegate, appearDelayCompleteFunctionParams);
            }
            deletePopUpElements();
            IsOpen = false;
            Objects.Remove(this);
        }

        public void cancel(object sender, RoutedEventArgs e)
        {
            if ((cancelFunctionDelegate == null))
                closePopUp(null, null);
            else
            {
                Type t = obj.GetType();

                if ((appearDelayCompleteFunctionParams == null))
                    obj.Dispatcher.BeginInvoke(cancelFunctionDelegate);
                else
                    obj.Dispatcher.BeginInvoke(cancelFunctionDelegate, cancelFunctionParams);
            }
        }

        public DockPanel? panelPopupBackground;
        public PopupWindow? window;

        private int defaultZindex = 2000;

        private void createPopUpBackgroundFader()
        {
            panelPopupBackground = new DockPanel();
            panelPopupBackground.Name = "panelPopUpFader_" + Objects.Count;
            panelPopupBackground.Background = new SolidColorBrush(Color.FromArgb(50, 0, 0, 0));
            panelPopupBackground.Opacity = 0;
            panelPopupBackground.Visibility = Visibility.Hidden;
            var zindex = defaultZindex + Objects.Count;
            Panel.SetZIndex(panelPopupBackground, zindex);
            Grid.SetColumn(panelPopupBackground, 0);
            Grid.SetColumnSpan(panelPopupBackground, 2);
            Grid.SetRow(panelPopupBackground, 1);
            KeyboardNavigation.SetTabNavigation(panelPopupBackground, KeyboardNavigationMode.Cycle);
            KeyboardNavigation.SetControlTabNavigation(panelPopupBackground, KeyboardNavigationMode.Cycle);
            KeyboardNavigation.SetDirectionalNavigation(panelPopupBackground, KeyboardNavigationMode.Cycle);

            StackPanel stackPanel = new StackPanel();
            stackPanel.VerticalAlignment = VerticalAlignment.Center;
            stackPanel.HorizontalAlignment = HorizontalAlignment.Center;
            stackPanel.Height = double.NaN;
            stackPanel.Width = double.NaN;

            panelPopupBackground.Children.Add(stackPanel);
            mainFrm.gridMain.Children.Add(panelPopupBackground);
        }

        private void createPopUpBox(bool maxSize)
        {
            window = new PopupWindow();

            var zindex = defaultZindex + Objects.Count + 1;
            Panel.SetZIndex(window, zindex);
            window.btnCancelPopUp.AddHandler(Button.ClickEvent, new RoutedEventHandler(cancel));
            mainFrm.gridMain.Children.Add(window);
        }

        //private void createPopUpBox(bool maxSize)
        //{
        //    panelPopupContentPanel = new DockPanel();
        //    panelPopupContentPanel.Name = "panelPopupContentPanel_" + Objects.Count;
        //    panelPopupContentPanel.Background = new SolidColorBrush(Color.FromArgb(50, 0, 0, 0));
        //    panelPopupContentPanel.Opacity = 1;
        //    panelPopupContentPanel.Visibility = Visibility.Hidden;
        //    panelPopupContentPanel.VerticalAlignment = VerticalAlignment.Stretch;

        //    var zindex = defaultZindex + Objects.Count + 1;
        //    Panel.SetZIndex(panelPopupContentPanel, zindex);
        //    Grid.SetColumn(panelPopupContentPanel, 0);
        //    Grid.SetRow(panelPopupContentPanel, 1);
        //    Grid.SetColumnSpan(panelPopupContentPanel, 2);
        //    KeyboardNavigation.SetTabNavigation(panelPopupContentPanel, KeyboardNavigationMode.Cycle);
        //    KeyboardNavigation.SetControlTabNavigation(panelPopupContentPanel, KeyboardNavigationMode.Cycle);
        //    KeyboardNavigation.SetDirectionalNavigation(panelPopupContentPanel, KeyboardNavigationMode.Cycle);


        //    panelPopupBoxGrid = new Grid();
        //    panelPopupBoxGrid.Name = "panelPopupBoxGrid_" + Objects.Count;
        //    panelPopupBoxGrid.Margin = new Thickness(0, 0, 0, 0);
        //    panelPopupBoxGrid.Background = (Brush)mainFrm.FindResource("WindowBackgroundBrushMedium");
        //    if (!maxSize)
        //        panelPopupBoxGrid.VerticalAlignment = VerticalAlignment.Center;
        //    panelPopupBoxGrid.HorizontalAlignment = HorizontalAlignment.Stretch;

        //    RowDefinition rowDef = new RowDefinition();
        //    rowDef.Height = new GridLength(35);
        //    panelPopupBoxGrid.RowDefinitions.Add(rowDef);
        //    RowDefinition rowDef2 = new RowDefinition();
        //    panelPopupBoxGrid.RowDefinitions.Add(rowDef2);

        //    imgPopUpIcon = new Rectangle();
        //    imgPopUpIcon.Name = "imgPopUpIcon_" + Objects.Count;
        //    imgPopUpIcon.HorizontalAlignment = HorizontalAlignment.Left;
        //    imgPopUpIcon.VerticalAlignment = VerticalAlignment.Center;
        //    imgPopUpIcon.Margin = new Thickness(10, 0, 10, 0);
        //    imgPopUpIcon.Width = 22;
        //    imgPopUpIcon.Height = 22;
        //    imgPopUpIcon.Visibility = Visibility.Collapsed;

        //    Grid.SetRow(imgPopUpIcon, 0);
        //    panelPopupBoxGrid.Children.Add(imgPopUpIcon);

        //    lblPopUpHeader = new Label();
        //    lblPopUpHeader.Name = "lblPopUpHeader_" + Objects.Count;
        //    lblPopUpHeader.Content = "[No Header Loaded]";
        //    lblPopUpHeader.HorizontalAlignment = HorizontalAlignment.Left;
        //    lblPopUpHeader.VerticalAlignment = VerticalAlignment.Center;
        //    lblPopUpHeader.Width = 300;
        //    lblPopUpHeader.Height = double.NaN;
        //    lblPopUpHeader.Foreground = (Brush)mainFrm.FindResource("HeaderTextBrush");
        //    lblPopUpHeader.FontFamily = (FontFamily) mainFrm.FindResource("FontendoFont");
        //    lblPopUpHeader.FontSize = 14;
        //    lblPopUpHeader.Margin = new Thickness(30, 3, 0, 0);
        //    Grid.SetRow(lblPopUpHeader, 0);
        //    panelPopupBoxGrid.Children.Add(lblPopUpHeader);

        //    Grid gridItem = new Grid();
        //    gridItem.HorizontalAlignment = HorizontalAlignment.Right;
        //    gridItem.VerticalAlignment = VerticalAlignment.Center;
        //    gridItem.Margin = new Thickness(0, 0, 5, 0);
        //    Grid.SetRow(gridItem, 0);

        //    btnCancelPopUp = new Button();
        //    btnCancelPopUp.Name = "btnCancelPopUp_" + Objects.Count;
        //    btnCancelPopUp.Width = 25;
        //    btnCancelPopUp.Height = 25;
        //    btnCancelPopUp.Cursor = Cursors.Hand;
        //    btnCancelPopUp.Style = (Style)mainFrm.FindResource("ButtonStyleImage");
        //    btnCancelPopUp.AddHandler(Button.ClickEvent, new RoutedEventHandler(cancel));

        //    Image btnImg = new Image();
        //    btnImg.VerticalAlignment = VerticalAlignment.Center;
        //    btnImg.HorizontalAlignment = HorizontalAlignment.Left;
        //    btnImg.Margin = new Thickness(0);
        //    btnImg.Source = appImages.getImageFromResources("cancel_round.png");
        //    btnImg.Width = 16;
        //    btnImg.Height = 16;

        //    btnCancelPopUp.Content = btnImg;
        //    btnCancelPopUp.Focus();
        //    gridItem.Children.Add(btnCancelPopUp);
        //    panelPopupBoxGrid.Children.Add(gridItem);

        //    panelPopupContent = new StackPanel();
        //    panelPopupContent.Name = "panelPopupContent_" + Objects.Count;

        //    if (!maxSize)
        //        panelPopupContent.VerticalAlignment = VerticalAlignment.Center;
        //    else
        //        panelPopupContent.VerticalAlignment = VerticalAlignment.Stretch;
        //    panelPopupContent.Width = double.NaN;
        //    panelPopupContent.Height = double.NaN;
        //    panelPopupContent.Margin = new Thickness(5, 0, 5, 5);
        //    panelPopupContent.Background = (Brush)mainFrm.FindResource("WindowBackgroundBrushDark");
        //    Panel.SetZIndex(panelPopupContent, 0);
        //    Grid.SetRow(panelPopupContent, 1);

        //    panelPopupBoxGrid.Children.Add(panelPopupContent);

        //    // add a drop shadow the panelPopupBoxGrid
        //    panelPopupBoxGridWrapper = new Grid();
        //    if (!maxSize)
        //    {
        //        panelPopupBoxGridWrapper.VerticalAlignment = VerticalAlignment.Center;
        //        panelPopupBoxGridWrapper.HorizontalAlignment = HorizontalAlignment.Center;
        //    }
        //    else
        //    {
        //        panelPopupBoxGridWrapper.Margin = new Thickness(50);
        //    }

        //    Border border = new Border();
        //    border.BorderThickness = new Thickness(1);
        //    border.BorderBrush = (Brush)mainFrm.FindResource("WindowBackgroundBrushDark");
        //    border.VerticalAlignment = VerticalAlignment.Stretch;

        //    System.Windows.Media.Effects.DropShadowEffect fx = new System.Windows.Media.Effects.DropShadowEffect();
        //    fx.Opacity = 1;
        //    fx.ShadowDepth = 1;
        //    fx.BlurRadius = 5;
        //    fx.Color = (Color)mainFrm.FindResource("ColorShadowAlpha");
        //    border.Effect = fx;

        //    panelPopupBoxGridWrapper.Children.Add(border);
        //    panelPopupBoxGridWrapper.Children.Add(panelPopupBoxGrid);
        //    panelPopupContentPanel.Children.Add(panelPopupBoxGridWrapper);
        //    mainFrm.gridMain.Children.Add(panelPopupContentPanel);
        //}
        private void createPopUpElements(bool maxSize)
        {
            createPopUpBackgroundFader();
            createPopUpBox(maxSize);
        }
        private void deletePopUpElements()
        {
            mainFrm.gridMain.Children.Remove(panelPopupBackground);
            mainFrm.gridMain.Children.Remove(window.panelPopupContentPanel);
        }
    }

    public static List<popUpType> Objects = new List<popUpType>();

    public static bool IsOpen()
    {
        foreach (popUpType item in Objects)
        {
            if ((item.IsOpen))
            {
                return true;
            }
        }
        return false;
    }

    public static void resize(double newsize)
    {
        foreach (popUpType item in Objects)
        {
            if ((item.IsOpen))
            {
                item.resize(newsize);
            }
        }
    }

    public static bool Exist()
    {
        if ((Objects.Count > 0))
           return true;
        return false;
    }

    public static void loadPopUp(MainWindow? mainFrm, string header, string icon, ref dynamic popUpWindow, bool themedIcon = false, bool maxSize = false)
    {
        if (mainFrm == null)
            throw new Exception("Main window cannot be null when loading a pop up");
        popUpType newObject = new popUpType(mainFrm, ref popUpWindow, maxSize);
        mainFrm.enableForm(false);
        if(newObject.window.lblPopUpHeader != null)
            newObject.window.lblPopUpHeader.Content = header;

        if ((icon != ""))
        {
            ImageBrush imgBrush = new ImageBrush();
            imgBrush.ImageSource = appImages.getImageFromResources(icon);
            imgBrush.Stretch = Stretch.Uniform;
            if (newObject.window.imgPopUpIcon != null)
            {
                newObject.window.imgPopUpIcon.Visibility = Visibility.Visible;
                if (themedIcon)
                {
                    newObject.window.imgPopUpIcon.OpacityMask = imgBrush;
                    newObject.window.imgPopUpIcon.Fill = (Brush)mainFrm.FindResource("HeaderTextBrush");
                }
                else
                {
                    newObject.window.imgPopUpIcon.Fill = imgBrush;
                    newObject.window.imgPopUpIcon.Height = 16;
                }
            }
        }
        else
        {
            if(newObject.window.lblPopUpHeader != null)
                newObject.window.lblPopUpHeader.Margin = new Thickness(10, 3, 0, 0);
        }

        newObject.window.panelPopupContent?.Children.RemoveRange(0, newObject.window.panelPopupContent.Children.Count);
        if(newObject.window != null)
            newObject.window.Opacity = 0;
        if(newObject.window.panelPopupBoxGrid != null)
            newObject.window.panelPopupBoxGrid.MinWidth = popUpWindow.MinWidth + 10;
        if (newObject.window.panelPopupContent != null)
            newObject.window.panelPopupContent.MinHeight = popUpWindow.MinHeight;
        objectAnimations.makeAppear(ref mainFrm, newObject.panelPopupBackground, false, mainFrm, newObject.popUpBgFade_Complete);
    }
}
