using System;
namespace HJPT.Models
{
    public class User : Entity
    {
        public string StuID { get; set; }
        public string PasswordHash { get; set; }
        public string PassKey { get; set; }
        public string RegIP { get; set; }
        public string LastIP { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string SecurityStamp { get; set; } = new Guid().ToString();
        public string Token { get; set; }
        public DateTime TokenExpires { get; set; }
        public string RefreshToken { get; set; }
        public DateTime RefreshTokenExpires { get; set; }
        
    }
}
