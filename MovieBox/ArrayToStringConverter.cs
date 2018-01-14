using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;

namespace MovieBox
{
    public class ArrayToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value != null)
            {
                IEnumerable<object> collection = (IEnumerable<object>)value;
                string merged = "";

                foreach (object item in collection)
                {
                    if (item == null)
                        break;
                    merged += item.ToString();
                    merged += ", ";
                }
                
                if(merged.Length >= 2)
                    merged = merged.Remove(merged.Length - 2, 2);
                return merged;
            }
            return null;
        }
        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
