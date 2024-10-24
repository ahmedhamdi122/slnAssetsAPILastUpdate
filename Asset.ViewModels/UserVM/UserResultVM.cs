using Asset.ViewModels.ModuleVM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Asset.ViewModels.UserVM
{
    public class UserResultVM
    {
        public IEnumerable<UserVM> results { get; set; }
        public int count { get; set; }
    }
}
