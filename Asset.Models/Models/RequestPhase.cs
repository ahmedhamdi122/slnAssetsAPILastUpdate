using System;
using System.Collections.Generic;

#nullable disable

namespace Asset.API.Models
{
    public partial class RequestPhase
    {
        public int Id { get; set; }
        public int PhaseId { get; set; }
        public int RequestId { get; set; }
        public int EmployeeId { get; set; }

        public virtual Employee Employee { get; set; }
        public virtual Phase Phase { get; set; }
        public virtual Request Request { get; set; }
    }
}
