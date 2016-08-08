using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HJPT.Options
{
    public class SiteOptions
    {
        public SignUpOption SignUp { get; set; } = 0;
    }

    public enum SignUpOption
    {
        Reject = 0,
        Invite = 1,
        Open = 2
    }
}
