using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualBasic;
using System.Windows.Controls.Primitives;
using System.Windows;
using System.Windows.Media;
using System.Windows.Controls;
using Fontendo;
using Fontendo.UI;

public class Theme
{
    public enum themeType
    {
        infinity_blue,
        dark,
        light,
        grey_sky,
    }
    public static themeType themeObj = themeType.grey_sky;
    public static int themeCol = 0;
    public static int selectedTheme = -1;
    public static int loadedTheme = -1;
    public class ThemeColorsType
    {
        public string ? ColorDark;
        public string ? ColorMedium;
        public string ? ColorLight;
        public string ? ColorSelected;
        public string ? ColorHighlight;
        public string ? ColorPanelAlpha;
        public string ? ColorHeaderText;
        public string ? ColorLabelText;
        public string ? ColorActiveText;
        public string ? ColorInactiveText;
        public string ? ColorControlBorder;
        public string ? ColorControlHighlightBorder;
        public string ? ColorNegativeText;
        public string ? ColorPositiveText;
    }

    public static object[] lightThemeColors = new[] { new string[] { "#021284", "#2F2F2F" }, new string[] { "#00B5E4", "#2F2F2F" }, new string[] { "#15A58A", "#2F2F2F" }, new string[] { "#03844A", "#2F2F2F" }, new string[] { "#7EA515", "#2F2F2F" }, new string[] { "#FF9C23", "#2F2F2F" }, new string[] { "#A86003", "#2F2F2F" }, new string[] { "#840101", "#2F2F2F" }, new string[] { "#610084", "#2F2F2F" }, new string[] { "#960193", "#2F2F2F" } };

    public static ThemeColorsType selectedThemeColours = new ThemeColorsType();
    public static List<ThemeColorsType> defaultThemeColours = new List<ThemeColorsType>();

    public static ThemeColorsType getThemeColorsFromWindowResources(Control window)
    {
        ThemeColorsType themeColours = new ThemeColorsType();
        themeColours.ColorDark = getResourceVal(window, "ColorDark");
        themeColours.ColorMedium = getResourceVal(window, "ColorMedium");
        themeColours.ColorLight = getResourceVal(window, "ColorLight");
        themeColours.ColorPanelAlpha = getResourceVal(window, "ColorPanelAlpha");
        themeColours.ColorSelected = getResourceVal(window, "ColorSelected");
        themeColours.ColorHighlight = getResourceVal(window, "ColorHighlight");
        themeColours.ColorHeaderText = getResourceVal(window, "ColorHeaderText");
        themeColours.ColorLabelText = getResourceVal(window, "ColorLabelText");
        themeColours.ColorActiveText = getResourceVal(window, "ColorActiveText");
        themeColours.ColorInactiveText = getResourceVal(window, "ColorInactiveText");
        themeColours.ColorControlBorder = getResourceVal(window, "ColorControlBorder");
        themeColours.ColorControlHighlightBorder = getResourceVal(window, "ColorControlHighlightBorder");
        themeColours.ColorPositiveText = getResourceVal(window, "ColorPositiveText");
        themeColours.ColorNegativeText = getResourceVal(window, "ColorNegativeText");
        return themeColours;
    }

    public static void initTheme(Window mainFrm)
    {
        // standard theme
        defaultThemeColours.Add(getThemeColorsFromWindowResources(mainFrm));

        // dark theme
        ThemeColorsType themeColour1 = new ThemeColorsType();
        themeColour1.ColorDark = "#FF201F1F";
        themeColour1.ColorMedium = "#FF2D2C2C";
        themeColour1.ColorLight = "#FF383838";
        themeColour1.ColorPanelAlpha = "#44000000";
        themeColour1.ColorSelected = "#FF568C37";
        themeColour1.ColorHighlight = "#9F568C37";
        themeColour1.ColorHeaderText = "#E6E6E6";
        themeColour1.ColorLabelText = "#C4C4C4";
        themeColour1.ColorActiveText = "#E6E6E6";
        themeColour1.ColorInactiveText = "#C4C4C4";
        themeColour1.ColorControlBorder = "#33FFFFFF";
        themeColour1.ColorControlHighlightBorder = "#CCFFFFFF";
        themeColour1.ColorPositiveText = "#407D00";
        themeColour1.ColorNegativeText = "#BF0000";
        defaultThemeColours.Add(themeColour1);

        // light theme
        ThemeColorsType themeColour2 = new ThemeColorsType();
        themeColour2.ColorDark = "#FFEDECEC";
        themeColour2.ColorMedium = "#FFCFCFCF";
        themeColour2.ColorLight = "#FFDEDEDE";
        themeColour2.ColorPanelAlpha = "#70F2F2F2";
        themeColour2.ColorSelected = "#FF0AAFD1";
        themeColour2.ColorHighlight = "#C70DC1E6";
        themeColour2.ColorHeaderText = "#FF525252";
        themeColour2.ColorLabelText = "#FF545454";
        themeColour2.ColorActiveText = "#FFFFFFFF";
        themeColour2.ColorInactiveText = "#FF696969";
        themeColour2.ColorControlBorder = "#BA787878";
        themeColour2.ColorControlHighlightBorder = "#93595959";
        themeColour2.ColorPositiveText = "#FF407D00";
        themeColour2.ColorNegativeText = "#FFBF0000";
        defaultThemeColours.Add(themeColour2);

        // dark sky theme
        ThemeColorsType themeColour3 = new ThemeColorsType();
        themeColour3.ColorDark = "#FF1F2633";
        themeColour3.ColorMedium = "#FF293140";
        themeColour3.ColorLight = "#FF333A47";
        themeColour3.ColorPanelAlpha = "#612D3647";
        themeColour3.ColorSelected = "#FF485670";
        themeColour3.ColorHighlight = "#4A546482";
        themeColour3.ColorHeaderText = "#FFE6E6E6";
        themeColour3.ColorLabelText = "#FFC4C4C4";
        themeColour3.ColorActiveText = "#FFE6E6E6";
        themeColour3.ColorInactiveText = "#FFC4C4C4";
        themeColour3.ColorControlBorder = "#7E728096";
        themeColour3.ColorControlHighlightBorder = "#C4546482";
        themeColour3.ColorPositiveText = "#FF7A964F";
        themeColour3.ColorNegativeText = "#FFBD3F3F";
        defaultThemeColours.Add(themeColour3);


        ThemeColorsType defaultTheme = themeColour1;
        selectedThemeColours.ColorDark = defaultTheme.ColorDark;
        selectedThemeColours.ColorMedium = defaultTheme.ColorMedium;
        selectedThemeColours.ColorLight = defaultTheme.ColorLight;
        selectedThemeColours.ColorPanelAlpha = defaultTheme.ColorPanelAlpha;
        selectedThemeColours.ColorSelected = defaultTheme.ColorSelected;
        selectedThemeColours.ColorHighlight = defaultTheme.ColorHighlight;
        selectedThemeColours.ColorHeaderText = defaultTheme.ColorHeaderText;
        selectedThemeColours.ColorLabelText = defaultTheme.ColorLabelText;
        selectedThemeColours.ColorActiveText = defaultTheme.ColorActiveText;
        selectedThemeColours.ColorInactiveText = defaultTheme.ColorInactiveText;
        selectedThemeColours.ColorControlBorder = defaultTheme.ColorControlBorder;
        selectedThemeColours.ColorControlHighlightBorder = defaultTheme.ColorControlHighlightBorder;
        selectedThemeColours.ColorPositiveText = defaultTheme.ColorPositiveText;
        selectedThemeColours.ColorNegativeText = defaultTheme.ColorNegativeText;
    }
    public static string getResourceVal(Control? control, string name)
    {
        ResourceDictionary? res = control?.Resources;
        return res?[name].ToString() ?? "";
    }

    public static void overwriteResource(Control control, string name, string? value)
    {
        if (value == "")
            return;
        ResourceDictionary res = control.Resources;
        res[name] = ColorConverter.ConvertFromString(value);
    }

    public static void loadTheme(Window window, string themeFile)
    {
        Uri Uri = new Uri("/Fontendo;component/Resources/Themes/" + themeFile, UriKind.Relative);
        ResourceDictionary rs;
        rs = (ResourceDictionary)Application.LoadComponent(Uri);
        window.Resources.MergedDictionaries.Add(rs);
    }

    public static void applyThemeColors(Window window, ThemeColorsType themeColours)
    {
        overwriteResource(window, "ColorDark", themeColours.ColorDark);
        overwriteResource(window, "ColorMedium", themeColours.ColorMedium);
        overwriteResource(window, "ColorLight", themeColours.ColorLight);
        overwriteResource(window, "ColorPanelAlpha", themeColours.ColorPanelAlpha);
        overwriteResource(window, "ColorSelected", themeColours.ColorSelected);
        overwriteResource(window, "ColorHighlight", themeColours.ColorHighlight);
        overwriteResource(window, "ColorHeaderText", themeColours.ColorHeaderText);
        overwriteResource(window, "ColorLabelText", themeColours.ColorLabelText);
        overwriteResource(window, "ColorActiveText", themeColours.ColorActiveText);
        overwriteResource(window, "ColorInactiveText", themeColours.ColorInactiveText);
        overwriteResource(window, "ColorControlBorder", themeColours.ColorControlBorder);
        overwriteResource(window, "ColorControlHighlightBorder", themeColours.ColorControlHighlightBorder);
        overwriteResource(window, "ColorPositiveText", themeColours.ColorPositiveText);
        overwriteResource(window, "ColorNegativeText", themeColours.ColorNegativeText);
    }

    public static void applyTheme(Control control)
    {
        overwriteResource(control, "ColorDark", selectedThemeColours.ColorDark);
        overwriteResource(control, "ColorMedium", selectedThemeColours.ColorMedium);
        overwriteResource(control, "ColorLight", selectedThemeColours.ColorLight);
        overwriteResource(control, "ColorPanelAlpha", selectedThemeColours.ColorPanelAlpha);
        overwriteResource(control, "ColorSelected", selectedThemeColours.ColorSelected);
        overwriteResource(control, "ColorHighlight", selectedThemeColours.ColorHighlight);
        overwriteResource(control, "ColorHeaderText", selectedThemeColours.ColorHeaderText);
        overwriteResource(control, "ColorLabelText", selectedThemeColours.ColorLabelText);
        overwriteResource(control, "ColorActiveText", selectedThemeColours.ColorActiveText);
        overwriteResource(control, "ColorInactiveText", selectedThemeColours.ColorInactiveText);
        overwriteResource(control, "ColorControlBorder", selectedThemeColours.ColorControlBorder);
        overwriteResource(control, "ColorControlHighlightBorder", selectedThemeColours.ColorControlHighlightBorder);
        overwriteResource(control, "ColorPositiveText", selectedThemeColours.ColorPositiveText);
        overwriteResource(control, "ColorNegativeText", selectedThemeColours.ColorNegativeText);
    }


    public static bool compareThemeColours(ThemeColorsType theme1, ThemeColorsType theme2)
    {
        if ((theme1.ColorDark == theme2.ColorDark & theme1.ColorMedium == theme2.ColorMedium & theme1.ColorLight == theme2.ColorLight & theme1.ColorPanelAlpha == theme2.ColorPanelAlpha & theme1.ColorSelected == theme2.ColorSelected & theme1.ColorHighlight == theme2.ColorHighlight & theme1.ColorHeaderText == theme2.ColorHeaderText & theme1.ColorLabelText == theme2.ColorLabelText & theme1.ColorActiveText == theme2.ColorActiveText & theme1.ColorInactiveText == theme2.ColorInactiveText & theme1.ColorControlBorder == theme2.ColorControlBorder & theme1.ColorControlHighlightBorder == theme2.ColorControlHighlightBorder & theme1.ColorPositiveText == theme2.ColorPositiveText & theme1.ColorNegativeText == theme2.ColorNegativeText))
            return true;
        return false;
    }

    internal static void applyCustomTheme(int themeData, string ColoursData)
    {
        // apply custom theme
        selectedTheme = themeData;
        if ((selectedTheme == -1))
        {
            themeObj = themeType.dark;
            themeCol = 0;
        }
        else if ((selectedTheme >= lightThemeColors.Count()))
        {
            themeObj = themeType.dark;
            themeCol = 0;
        }
        else
        {
            themeObj = themeType.light;
            themeCol = selectedTheme;

            // apply selected theme default colours, but change the header to the old flavour for backwards compatability
            selectedThemeColours.ColorDark = defaultThemeColours[(int)themeType.light].ColorDark;
            selectedThemeColours.ColorMedium = defaultThemeColours[(int)themeType.light].ColorMedium;
            selectedThemeColours.ColorLight = defaultThemeColours[(int)themeType.light].ColorLight;
            selectedThemeColours.ColorPanelAlpha = defaultThemeColours[(int)themeType.light].ColorPanelAlpha;
            selectedThemeColours.ColorSelected = defaultThemeColours[(int)themeType.light].ColorSelected;
            selectedThemeColours.ColorHighlight = defaultThemeColours[(int)themeType.light].ColorHighlight;
            selectedThemeColours.ColorHeaderText = lightThemeColors[(int)themeCol].ToString();
            selectedThemeColours.ColorLabelText = defaultThemeColours[(int)themeType.light].ColorLabelText;
            selectedThemeColours.ColorActiveText = defaultThemeColours[(int)themeType.light].ColorActiveText;
            selectedThemeColours.ColorInactiveText = defaultThemeColours[(int)themeType.light].ColorInactiveText;
            selectedThemeColours.ColorControlBorder = defaultThemeColours[(int)themeType.light].ColorControlBorder;
            selectedThemeColours.ColorControlHighlightBorder = defaultThemeColours[(int)themeType.light].ColorControlHighlightBorder;
            selectedThemeColours.ColorPositiveText = defaultThemeColours[(int)themeType.light].ColorPositiveText;
            selectedThemeColours.ColorNegativeText = defaultThemeColours[(int)themeType.light].ColorNegativeText;
        }


        // apply custom colours
        if ((ColoursData != ""))
        {
            string[] cols = Strings.Split(ColoursData, "|");
            try
            {
                selectedThemeColours.ColorDark = cols[0];
                selectedThemeColours.ColorMedium = cols[1];
                selectedThemeColours.ColorLight = cols[2];
                selectedThemeColours.ColorPanelAlpha = cols[3];
                selectedThemeColours.ColorSelected = cols[4];
                selectedThemeColours.ColorHighlight = cols[5];
                selectedThemeColours.ColorHeaderText = cols[6];
                selectedThemeColours.ColorLabelText = cols[7];
                selectedThemeColours.ColorActiveText = cols[8];
                selectedThemeColours.ColorInactiveText = cols[9];
                selectedThemeColours.ColorControlBorder = cols[10];
                selectedThemeColours.ColorControlHighlightBorder = cols[11];
                selectedThemeColours.ColorPositiveText = cols[12];
                selectedThemeColours.ColorNegativeText = cols[13];
            }
            catch
            {
            }
        }
    }
}


public class ThemeCodeResources : ResourceDictionary
{
    public void Thumb_DragDelta(object sender, DragCompletedEventArgs e)
    {
        GridViewColumnHeader header = ((dynamic)sender).TemplatedParent;
        if ((header.Column.ActualWidth < 30))
            header.Column.Width = 30;
    }
}
