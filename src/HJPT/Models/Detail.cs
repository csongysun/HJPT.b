using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HJPT.Models
{
    public class Detail
    {
        public int Id { get; set; }
        public ApplicationUser Owner { get; set; }
        public bool Anonymous { get; set; }
        public bool Visible { get; set; }
        public string IMDbUrl { get; set; }
        public string Title { get; set; }
        public string SubTitle { get; set; }
        public SubCategory Category { get; set; }
        public DateTimeOffset AddDate { get; set; }
        public DateTimeOffset EditDate { get; set; }
        public Torrent Torrent { get; set; }

    }

    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
    public class SubCategory
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public Category PCategory { get; set; }
    }
    public class Promotion
    {
        public int Id { get; set; }
        public float DownFold { get; set; }
        public float UpFold { get; set; }
    }
}
