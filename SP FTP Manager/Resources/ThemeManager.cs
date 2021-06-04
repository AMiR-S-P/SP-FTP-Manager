using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;

namespace SP_FTP_Manager.Resources
{


    public static class DarkColors
    {
        public static Color MainAccentColor { get;  } = App.Settings.AccentColor; /*(Color)ColorConverter.ConvertFromString("#Ff669166")*/
        public static Color MainBackgroundColor { get; } = (Color)ColorConverter.ConvertFromString("#FF222222");
        public static Color SecondaryBackgroundColor { get; } = (Color)ColorConverter.ConvertFromString("#FF111111");
        public static Color MainForegroundColor { get; } = App.Settings.TextColor;/*(System.Windows.Media.Colors.WhiteSmoke)*/

    }
    public static class LightColors
    {
        public static Color MainAccentColor { get; } = App.Settings.AccentColor; /*(Color)ColorConverter.ConvertFromString("#Ff669166");*/
        public static Color MainBackgroundColor { get; } = (Color)ColorConverter.ConvertFromString("#E1D1D1D1");
        public static Color SecondaryBackgroundColor { get; } = (Color)ColorConverter.ConvertFromString("#E1e1e1e1");
        public static Color MainForegroundColor { get; } = App.Settings.TextColor; /*(System.Windows.Media.Colors.WhiteSmoke);*/
    }

    public class Colors
    {
        public static event EventHandler<PropertyChangedEventArgs> StaticPropertyChanged=delegate{ };
        private static SolidColorBrush test=new SolidColorBrush(System.Windows.Media.Colors.Red);

        public static SolidColorBrush Test { get => test; set { test = value; StaticPropertyChanged?.Invoke(typeof(Colors), new PropertyChangedEventArgs("Test")); } }
        #region Colors
        public static Color MainAccentColor { get => App.Settings.IsLight ? LightColors.MainAccentColor : DarkColors.MainAccentColor; }
        public static Color MainBackgroundColor { get => App.Settings.IsLight ? LightColors.MainBackgroundColor : DarkColors.MainBackgroundColor; }
        public static Color SecondaryBackgroundColor { get => App.Settings.IsLight ? LightColors.SecondaryBackgroundColor : DarkColors.SecondaryBackgroundColor; }
        public static Color MainForegroundColor { get => App.Settings.IsLight ? LightColors.MainForegroundColor : DarkColors.MainForegroundColor; }


        public static Color ControlBackgroundColor
        {
            get =>
                App.Settings.IsLight ?
                new Color() { A = 143, R = LightColors.MainAccentColor.R, G = LightColors.MainAccentColor.G, B = LightColors.MainAccentColor.B } :
                new Color() { A = 143, R = DarkColors.MainAccentColor.R, G = DarkColors.MainAccentColor.G, B = DarkColors.MainAccentColor.B };
        }
        public static Color ControlBorderColor
        {
            get =>
                App.Settings.IsLight ?
                new Color() { A = 255, R = LightColors.MainAccentColor.R, G = LightColors.MainAccentColor.G, B = LightColors.MainAccentColor.B } :
                new Color() { A = 255, R = DarkColors.MainAccentColor.R, G = DarkColors.MainAccentColor.G, B = DarkColors.MainAccentColor.B };
        }
        public static Color ControlMouseOverBackgroundColor
        {
            get =>
                App.Settings.IsLight ?
                new Color() { A = 95, R = LightColors.MainAccentColor.R, G = LightColors.MainAccentColor.G, B = LightColors.MainAccentColor.B } :
                new Color() { A = 95, R = DarkColors.MainAccentColor.R, G = DarkColors.MainAccentColor.G, B = DarkColors.MainAccentColor.B };
        }
        public static Color ControlPressedBackgroundColor
        {
            get =>
                App.Settings.IsLight ?
                new Color() { A = 223, R = LightColors.MainAccentColor.R, G = LightColors.MainAccentColor.G, B = LightColors.MainAccentColor.B } :
                new Color() { A = 223, R = DarkColors.MainAccentColor.R, G = DarkColors.MainAccentColor.G, B = DarkColors.MainAccentColor.B };
        }
        public static Color ControlDisabledBackgroundColor
        {
            get => (Color)ColorConverter.ConvertFromString("2f916666");
        }
        public static Color ControlDisabledTextColor
        {
            get => (Color)ColorConverter.ConvertFromString("FF919191");
        }
        public static Color ControlDisabledBorderColor
        {
            get => App.Settings.IsLight ? LightColors.MainAccentColor : DarkColors.MainAccentColor;
        }
        #endregion

        #region Brushes
        public static SolidColorBrush MainAccentBrush
        {
            get => App.Settings.IsLight ?
                new SolidColorBrush() { Color = LightColors.MainAccentColor } :
                new SolidColorBrush() { Color = DarkColors.MainAccentColor };
        }
        public static SolidColorBrush MainBackgroundBrush
        {
            get => App.Settings.IsLight ?
                    new SolidColorBrush() { Color = LightColors.MainBackgroundColor } :
                    new SolidColorBrush() { Color = DarkColors.MainBackgroundColor };
        }
        public static SolidColorBrush SecondaryBackgroundBrush
        {
            get => App.Settings.IsLight ?
                    new SolidColorBrush() { Color = LightColors.SecondaryBackgroundColor } :
                    new SolidColorBrush() { Color = DarkColors.SecondaryBackgroundColor };
        }
        public static SolidColorBrush MainForegroundBrush
        {
            get => App.Settings.IsLight ?
                    new SolidColorBrush() { Color = LightColors.MainForegroundColor } :
                    new SolidColorBrush() { Color = DarkColors.MainForegroundColor };
        }

        public static LinearGradientBrush WindowsBackgroundBrush
        {
            get
            {
                LinearGradientBrush linear = new LinearGradientBrush();
                linear.StartPoint = new System.Windows.Point(0.5, 0);
                linear.EndPoint = new System.Windows.Point(0.5, 1);
                linear.GradientStops.Add(new GradientStop()
                {
                    Color = MainBackgroundColor,
                    Offset = 0
                }); linear.GradientStops.Add(new GradientStop()
                {
                    Color = SecondaryBackgroundColor,
                    Offset = 1
                });
                return linear;
            }
        }
        public static LinearGradientBrush JobTemplateBackground
        {
            get
            {
                LinearGradientBrush linear = new LinearGradientBrush();
                linear.StartPoint = new System.Windows.Point(0.5, 1);
                linear.EndPoint = new System.Windows.Point(0.5, 0);
                linear.GradientStops.Add(new GradientStop()
                {
                    Color = (Color)ColorConverter.ConvertFromString("#FFdfdfdf"),
                    Offset = 0
                });
                linear.GradientStops.Add(new GradientStop()
                {
                    Color = (Color)ColorConverter.ConvertFromString("#FFdfdfdf"),
                    Offset = 1
                });
                linear.GradientStops.Add(new GradientStop()
                {
                    Color = (System.Windows.Media.Colors.WhiteSmoke),
                    Offset = 0.5
                });
                return linear;
            }
        }

        public static SolidColorBrush ControlBackgroundBrush
        {
            get => App.Settings.IsLight ?
                new SolidColorBrush()
                {
                    Color = new Color() { A = 143, R = LightColors.MainAccentColor.R, G = LightColors.MainAccentColor.G, B = LightColors.MainAccentColor.B }
                } :
                new SolidColorBrush()
                {
                    Color = new Color() { A = 143, R = DarkColors.MainAccentColor.R, G = DarkColors.MainAccentColor.G, B = DarkColors.MainAccentColor.B }
                };
        }
        public static SolidColorBrush ControlBorderBrush
        {
            get =>
                App.Settings.IsLight ?
                 new SolidColorBrush()
                 {
                     Color = new Color() { A = 255, R = LightColors.MainAccentColor.R, G = LightColors.MainAccentColor.G, B = LightColors.MainAccentColor.B }
                 } :
                new SolidColorBrush()
                {
                    Color = new Color() { A = 255, R = DarkColors.MainAccentColor.R, G = DarkColors.MainAccentColor.G, B = DarkColors.MainAccentColor.B }
                };
        }
        public static SolidColorBrush ControlMouseOverBackgroundBrush
        {
            get =>
                App.Settings.IsLight ?
                new SolidColorBrush() { Color = new Color() { A = 95, R = LightColors.MainAccentColor.R, G = LightColors.MainAccentColor.G, B = LightColors.MainAccentColor.B } } :
                new SolidColorBrush()
                {
                    Color = new Color() { A = 95, R = DarkColors.MainAccentColor.R, G = DarkColors.MainAccentColor.G, B = DarkColors.MainAccentColor.B }
                };
        }
        public static SolidColorBrush ControlPressedBackgroundBrush
        {
            get =>
                App.Settings.IsLight ?
                new SolidColorBrush()
                {
                    Color = new Color() { A = 223, R = LightColors.MainAccentColor.R, G = LightColors.MainAccentColor.G, B = LightColors.MainAccentColor.B }
                } :
                new SolidColorBrush()
                {
                    Color = new Color() { A = 223, R = DarkColors.MainAccentColor.R, G = DarkColors.MainAccentColor.G, B = DarkColors.MainAccentColor.B }
                };
        }
        public static SolidColorBrush ControlDisabledBackgroundBrush
        {
            get => new SolidColorBrush()
            {
                Color = (Color)ColorConverter.ConvertFromString("2f916666")
            };
        }
        public static SolidColorBrush ControlDisabledTextBrush
        {
            get => new SolidColorBrush()
            {
                Color = (Color)ColorConverter.ConvertFromString("FF919191")
            };
        }
        public static SolidColorBrush ControlDisabledBorderBrush
        {
            get => App.Settings.IsLight ?
                new SolidColorBrush()
                {
                    Color = LightColors.MainAccentColor
                } :
                new SolidColorBrush()
                {
                    Color = DarkColors.MainAccentColor
                };
        }

        #endregion
    }


}
