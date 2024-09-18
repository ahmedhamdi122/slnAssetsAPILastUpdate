using System;
using System.Collections.Generic;

#nullable disable

namespace Asset.API.Models
{
    public partial class Problem
    {
        public Problem()
        {
            SubProblems = new HashSet<SubProblem>();
        }

        public int Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string NameAr { get; set; }
        public int? MasterAssetId { get; set; }

        public virtual ICollection<SubProblem> SubProblems { get; set; }
    }
}
