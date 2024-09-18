using System;
using System.Collections.Generic;

#nullable disable

namespace Asset.API.Models
{
    public partial class Phase
    {
        public Phase()
        {
            RequestPhases = new HashSet<RequestPhase>();
        }

        public int Id { get; set; }
        public string PhaseName { get; set; }

        public virtual ICollection<RequestPhase> RequestPhases { get; set; }
    }
}
