﻿using Asset.ViewModels.PermissionVM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Asset.ViewModels.ModuleVM
{
    public class ModulesPermissionsResult
    {
        public IEnumerable<ModuleWithPermissionsVM> results { get; set; }
        public int count { get; set; }

    }
}

