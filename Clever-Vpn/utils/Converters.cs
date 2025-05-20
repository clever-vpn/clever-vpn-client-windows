using Microsoft.UI.Xaml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Clever_Vpn.utils
{
    public static class Converters
    {
        public static bool ConvertNullBoolToBool(bool? value)
        {
            if (value == null)
            {
                return false;
            }
            else
            {
                return (bool)value;
            }
        }

        public static bool ConvertObjectToBool(object? value)
        {
            if (value == null)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public static Visibility BoolToDisplay(bool value)
        {
            if (value)
            {
                return Visibility.Visible;
            }
            else
            {
                return Visibility.Collapsed;
            }
        }

        public static Visibility EqualToDisplay(object src, object dst)
        {
            return BoolToDisplay(src.Equals(dst));
        }

        public static Visibility NotEqualToDisplay(object src, object dst)
        {
            return BoolToDisplay(! src.Equals(dst));
        }

    }
}
