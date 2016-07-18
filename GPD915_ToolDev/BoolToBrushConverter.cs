using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;

namespace GPD915_ToolDev
{

    public class BoolToBrushConverter : IValueConverter
    {

        public Brush TrueBrush { get; set; }

        public Brush FalseBrush { get; set; }

        public object Convert(
            object value,
            Type targetType,
            object parameter,
            System.Globalization.CultureInfo culture)
        {
            if (!(value is bool)) { throw new ArgumentException("value"); }

            if((bool)value)
            {
                return TrueBrush;
            }
            else
            {
                return FalseBrush;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

    }

}