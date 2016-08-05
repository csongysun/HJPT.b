using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HJPT.Models
{
    public class HttpRequestModel
    {

    }

    public class LoginForm
    {
        public string UserName { get; set; }
        public string Password { get; set; }

    }

    public class SignUpForm
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public string StuID { get; set; }

    }


}
