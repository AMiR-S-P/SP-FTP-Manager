using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace SP_FTP_Manager.Converters
{
    public class ByteToSizeUnitConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) { return ""; }
            try
            {
                double val = double.Parse(value.ToString());
                int divideCount = 0;

                if (val < 1024)
                {
                    return $"{val.ToString("F2")} B";
                }

                do
                {
                    val = val / 1024;
                    divideCount++;
                } while (val > 1024);

                switch (divideCount)
                {
                    case 1:
                        {
                            return $"{val.ToString("F2")} KB";
                        }
                    case 2:
                        {
                            return $"{val.ToString("F2")} MB";
                        }
                    case 3:
                        {
                            return $"{val.ToString("F2")} GB";
                        }
                    case 4:
                        {
                            return $"{val.ToString("F2")} TB";
                        }
                }
                return $"{val.ToString("F2")} B";
            }
            catch (Exception ex)
            {
                return "";
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }
    }
}
