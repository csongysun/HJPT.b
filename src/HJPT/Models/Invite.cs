using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HJPT.Models
{
    public class Invite : Entity
    {
        public User Inviter { get; set; }
        public User Invitee { get; set; }
        public string Hash { get; set; }
        public DateTimeOffset? InvitedTime { get; set; }
    }
}
