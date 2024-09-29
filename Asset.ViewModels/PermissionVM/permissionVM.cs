using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Asset.ViewModels.PermissionVM
{
    public class permissionVM
    {
        public permissionVM(int Id,string Name,bool Value)
        {
            id = Id;
            name = Name;
            value = Value;
        }
        public int id { get; set; }
        public string name { get; set; }
        public bool value { get; set; }
    }
}
