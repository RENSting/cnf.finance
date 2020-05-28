using System;

namespace Cnf.Finance.Web
{
    public class AccNumberFormatter : IFormatProvider, ICustomFormatter
    {
        public string Format(string format, object arg, IFormatProvider formatProvider)
        {
            if (arg == null)
                return string.Empty;

            if(arg is float || arg is decimal || arg is double || arg is int || arg is long)
            {
                if (Convert.ToDouble(arg) == 0D)
                    return string.Empty;
                else
                    return string.Format("{0:N}", arg);
            }
            else
            {
                throw new FormatException("AccNumberFormatter 不支持格式化非数字类型");
            }
        }

        public object GetFormat(Type formatType)
        {
            if(formatType == typeof(ICustomFormatter))
                return this;
            else
                return null;
        }
    }
}