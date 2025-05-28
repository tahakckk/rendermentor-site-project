using System;
using System.Collections.Generic;
using System.Text;

namespace RenderMentor.Utility
{
    public static class SD
    {
        public const string Role_User_Individual = "Student";
        public const string Role_User_Company = "Company";
        public const string Role_Admin = "Admin";
        public const string Role_Instructor = "Instructor";

        public const string ssShoppingCart = "Shopping Cart Session";

        public const string StatusPending = "Bekleniyor";
        public const string StatusApproved = "Onaylandı";
        public const string StatusInProcess = "İşleniyor";
        public const string StatusShipped = "Gönderildi";
        public const string StatusCancelled = "İptal edildi";
        public const string StatusRefunded = "İade edildi";

        public const string PendingQuestions = "Unanswered Questions";

        public static string ConvertToRawHtml(string source)
        {
            char[] array = new char[source.Length];
            int arrayIndex = 0;
            bool inside = false;

            for (int i = 0; i < source.Length; i++)
            {
                char let = source[i];
                if (let == '<')
                {
                    inside = true;
                    continue;
                }
                if (let == '>')
                {
                    inside = false;
                    continue;
                }
                if (!inside)
                {
                    array[arrayIndex] = let;
                    arrayIndex++;
                }
            }
            return new string(array, 0, arrayIndex);
        }

        public static string RemoveSpecialCharacters(this string str)
        {
            StringBuilder sb = new StringBuilder();
            foreach (char c in str)
            {
                if ((c >= '0' && c <= '9') || (c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z') || c == '.' || c == '_')
                {
                    sb.Append(c);
                }
            }
            return sb.ToString();
        }
    }
}
