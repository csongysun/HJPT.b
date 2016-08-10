using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HJPT.Models
{
    public class Entity
    {
        public Entity()
        {
            Id = Guid.NewGuid().ToString().Replace("-","");
        }
        public string Id { get; set; }
    }
}
