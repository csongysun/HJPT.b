using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace HJPT.Model
{
    public class User
    {
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int UID { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
        public string StuID { get; set; }

    }


}
