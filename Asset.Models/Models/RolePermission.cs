using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Asset.Models
{
    public class RoleModule
    {
        public string RoleId { get; set; }
        public virtual  ApplicationRole Role { get; set; }

        public int ModuleId { get; set; }
        public virtual Module Module { get; set; }  
    }
}
