using System;

namespace Gpm.Common.Util
{
    public static class Miscellaneous
    {
        public static T CheckNotNull<T>(T value)
        {
            if (IsNull(value) == true)
            {
                throw new ArgumentNullException();
            }

            return value;
        }

        public static T CheckNotNull<T>(T value, string paramName)
        {
            if (IsNull(value) == true)
            {
                throw new ArgumentNullException(paramName);
            }

            return value;
        }

        private static bool IsNull<T>(T value)
        {
            var type = typeof(T);

            if (type == typeof(string))
            {
                if (string.IsNullOrEmpty(Convert.ToString(value).Trim()) == true)
                {
                    return true;
                }
            }
            else
            {
                if (value == null)
                {
                    return true;
                }
            }

            return false;
        }
    }
}