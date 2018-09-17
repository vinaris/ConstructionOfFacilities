using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static System.String;

namespace ConstructionOfFacilities.Views
{
    public static class ConvertFunctions
    {
        public static string DateToStringConvertion(DateTime? dateTime)
        {
            var formatDate = "";
            if (dateTime.HasValue)
            {
                formatDate = Format("{0:00}", dateTime.Value.Day) + "." + Format("{0:00}", dateTime.Value.Month) + "." + Format("{0:0000}", dateTime.Value.Year);
            }
            formatDate = formatDate == "01.01.0001" ? "" : formatDate;
            return formatDate;
        }
        public static string DecimalToStringConvertion(decimal? x)
        {
            return x == null || x == 0 ? "" : Format("{0:0,0.00}", x);
        }
        public static string StringToNullMessageConvertion(string x)
        {
            if (IsNullOrEmpty(x))
            {
                x = "Не указано.";
            }
            return x;
        }
        public static string StringToNullMessageConvertion(DateTime? x)
        {
            var str = "";
            if (!x.HasValue || x == DateTime.MinValue)
            {
                str = "Не указано.";
            }
            else
            {
                str = Convert.ToString(x);
            }
            return str;
        }


        public static string ClearAddressString(string str)
        {
            var newStr = str.ToLower();
            newStr = newStr.Replace("ул.", "");
            newStr = newStr.Replace("пр.", "");
            newStr = newStr.Replace("д.", "");
            newStr = newStr.Replace("№", "");
            newStr = newStr.Replace(" ", "");
            newStr = newStr.Replace(".", "");
            newStr = newStr.Replace(",", "");
            newStr = newStr.Replace("«", "");
            newStr = newStr.Replace("»", "");
            return newStr;
        }

    }
}
