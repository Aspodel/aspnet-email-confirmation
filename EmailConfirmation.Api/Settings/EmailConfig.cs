using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EmailConfirmation.Api.Settings
{
    public class EmailConfig
    {
        public string SendGridUser { get; set; }
        public string SendGridKey { get; set; }
    }
}
