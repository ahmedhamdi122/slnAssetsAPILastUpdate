using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Asset.ViewModels.PermissionVM
{
    public class permissionVM
    {
        public permissionVM(int id,string name,bool value)
        {
            ID = id;
            Name = name;
            Value = value;
        }
        public int ID { get; set; }
        public string Name { get; set; }
        public bool Value { get; set; }
    }
}
