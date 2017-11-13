using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using System.Globalization;
using NoteKeeper.Model;

namespace NoteKeeperClient.Converters
{
    class StringCollectionToString:IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int limit = Int32.Parse((string)parameter);
            string result = String.Join(", ", (IEnumerable<String>)value);
            if (result.Length > limit)
            {
                result = result.Substring(0, Math.Min(limit, result.Length)) + "...";
            }
            return result;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
