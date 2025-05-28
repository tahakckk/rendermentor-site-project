using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using RenderMentor.Models;

namespace RenderMentor.Models.ViewModels
{
    public class TrialOnlineVM
    {
        public bool TrialOnline { get; set; }
        public IEnumerable<Subscription> Subscriptions { get; set; }
    }
}
