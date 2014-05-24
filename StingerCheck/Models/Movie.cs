using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace StingerCheck.Models
{
    public class Movie
    {
        public long Id { get; set; }
        [Index(IsUnique = true)]
        public string TomatoId { get; set; }
        public string Title { get; set; }
        public string TomatoUrl { get; set; }

        public bool? HasMidStinger { get; set; }
        public bool? HasFinalStinger { get; set; }

        public Int16? MidTeaser { get; set; }
        public Int16? MidClosure { get; set; }
        public Int16? MidGag { get; set; }
        public Int16? MidEgg { get; set; }

        public Int16? FinalTeaser { get; set; }
        public Int16? FinalClosure { get; set; }
        public Int16? FinalGag { get; set; }
        public Int16? FinalEgg { get; set; }
    }
}