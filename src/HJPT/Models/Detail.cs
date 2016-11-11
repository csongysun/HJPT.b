using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HJPT.Models
{
    public class Topic : Entity
    {
        public User Owner { get; set; }
        public bool Anonymous { get; set; }
        public bool Visible { get; set; }
        public string IMDbUrl { get; set; }
        public string Title { get; set; }
        public string SubTitle { get; set; }
        public int ParentCategoryId { get; set; }
        public Category ParentCategory { get; set; }
        public int CategoryId { get; set; }
        public Category Category { get; set; }
        public DateTimeOffset AddDate { get; set; }
        public DateTimeOffset EditDate { get; set; }
        public Torrent Torrent { get; set; }

    }

    public class Category : NEntity
    {
        public string Name { get; set; }
        public Category PCategory { get; set; }
    }
    public class Promotion : NEntity
    {
        public float DownFold { get; set; }
        public float UpFold { get; set; }
    }
}
