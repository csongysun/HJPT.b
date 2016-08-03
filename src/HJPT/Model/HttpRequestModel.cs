using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HJPT.Model
{
    public class HttpRequestModel
    {

    }

    public class LoginForm
    {
        public string Username { get; set; }
        public string Password { get; set; }

    }

    public class SignUpForm
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public string StuID { get; set; }

        //public bool RequireCheck()
        //{
        //    if(user)
        //}
    }

}
