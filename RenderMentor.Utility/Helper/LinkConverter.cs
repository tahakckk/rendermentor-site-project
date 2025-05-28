namespace RenderMentor.Utility.Helper
{
    public class LinkConverter
    {
        public static string CreateUrl(string sentence)
        {
            string temp = "";
            temp = sentence.ToLower().Trim();
            temp = temp.Replace("  ", "-");
            temp = temp.Trim();
            temp = temp.Replace(" ", "-");
            temp = temp.Replace("ı", "i");
            temp = temp.Replace("ö", "o");
            temp = temp.Replace("ü", "u");
            temp = temp.Replace("ç", "c");
            temp = temp.Replace("ğ", "g");
            temp = temp.Replace("'", "");
            temp = temp.Replace("&", "-ve-");
            temp = temp.Replace("ş", "s");
            temp = temp.Replace("#", "sharp");
            temp = temp.Replace("+", "");
            temp = temp.Replace(".", "");
            temp = temp.Replace("!", "");
            temp = temp.Replace("\"", "");
            temp = temp.Replace("/", "");
            temp = temp.Replace("’", "");
            temp = temp.Replace("m²", "metrekare");
            temp = temp.Replace("%", "");
            temp = temp.Replace("½", "");
            temp = temp.Replace("(", "");
            temp = temp.Replace(")", "");
            temp = temp.Replace(",", "-");
            temp = temp.Replace("--", "-");
            temp = temp.Replace("---", "-");
            temp = temp.Replace("?", "");
            temp = temp.Replace(":", "");
            return temp;
        }
    }
}
