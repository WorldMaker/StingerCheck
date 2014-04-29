using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace StingerCheck.Models
{
    public class Stinger
    {
        public long Id { get; set; }

        public bool HasMidStinger { get; set; }
        public bool HasFinalStinger { get; set; }

        public Int16 MidTeaser { get; set; }
        public Int16 MidClosure { get; set; }
        public Int16 MidGag { get; set; }
        public Int16 MidEgg { get; set; }

        public Int16 FinalTeaser { get; set; }
        public Int16 FinalClosure { get; set; }
        public Int16 FinalGag { get; set; }
        public Int16 FinalEgg { get; set; }

        public virtual User User { get; set; }
        public virtual Movie Movie { get; set; }
    }
}