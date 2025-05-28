using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace RenderMentor.Utility.Helper
{
    public static class DateTimeHelper
    {
        public static string ToRfc3339String(this DateTime dateTime)
        {
            return dateTime.ToString("yyyy-MM-dd", DateTimeFormatInfo.InvariantInfo);
        }
        public static string ToRfc3339StringFull(this DateTime dateTime)
        {
            return dateTime.ToString("yyyy-MM-dd'T'HH:mm", DateTimeFormatInfo.InvariantInfo);
        }
    }
}
