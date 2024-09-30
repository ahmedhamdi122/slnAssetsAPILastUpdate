using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Asset.ViewModels.PermissionVM
{
    public class permissionVM
    {
        public permissionVM(int Id,string Name)
        {
            id = Id;
            name = Name;
        }
        public int id { get; set; }
        public string name { get; set; }
    }
}
