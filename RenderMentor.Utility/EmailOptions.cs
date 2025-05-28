using System;
using System.Collections.Generic;
using System.Text;

namespace RenderMentor.Utility
{
    public class EmailOptions
    {
        public string Host { get; set; }
        public int Port { get; set; }
        public bool EnableSSL { get; set; }
        public string mailAddress { get; set; }
        public string supportAddress { get; set; }
        public string Password { get; set; }
    }
}
