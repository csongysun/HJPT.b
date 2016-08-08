using Microsoft.AspNetCore.Http;
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
        public string InviteToken { get; set; }

    }

    public class UploadForm
    {
        public bool Anonymous { get; set; }
        public string IMDbUrl { get; set; }
        public string Title { get; set; }
        public string SubTitle { get; set; }
        public int CategoryId { get; set; }
        public IFormFile torrent { get; set; }
    }


}
