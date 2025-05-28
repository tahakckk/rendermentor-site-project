using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace RenderMentor.Utility.Helper
{
    public class StringProcessing
    {
        public static String ParseOutHTML(String htmlContent)
        {
            String parsedContent = String.Empty;
            Regex tagPattern = new Regex(@"(?<=^|>)[^><]+?(?=<|$)", RegexOptions.IgnoreCase);
            MatchCollection tagsRemoved = tagPattern.Matches(htmlContent);
            foreach (Match match in tagsRemoved)
                parsedContent += match.Value.Trim() + " ";
            return parsedContent;
        }
    }
}
