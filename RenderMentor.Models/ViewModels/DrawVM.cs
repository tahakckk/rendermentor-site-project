using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace RenderMentor.Models.ViewModels
{
    public class DrawVM
    {
        public bool DrawOnline { get; set; }
        public bool HasWinner { get; set; }
        public ICollection<string> WinnerEmails { get; set; }

        public string DrawTitle { get; set; }
        public string DrawDesc { get; set; }
        public string DrawSmallDesc { get; set; }
    }
}
