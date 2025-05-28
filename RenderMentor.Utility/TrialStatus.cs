using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace RenderMentor.Utility
{
    public enum TrialStatus
    {
        [Display(Name = "Tanımlanmamış")]
        InActive = 10,

        [Display(Name = "Kullanıma Hazır")]
        Ready = 20,

        [Display(Name = "Yeni Başladı")]
        Started = 30,

        [Display(Name = "Aktif")]
        Active = 40,

        [Display(Name = "Süresi Doldu")]
        Expired = 50,

        [Display(Name = "Yeni Bitti")]
        Completed = 60
    }
}
