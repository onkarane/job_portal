using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace CIS665Project.Models
{
    public class Search
    {
        public string WildCard { get; set; }
        public int Category { get; set; }

        public static string StaticWildCard { get; set; }
        public static int StaticCategory { get; set; }

    }
}
