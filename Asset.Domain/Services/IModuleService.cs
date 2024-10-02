using Asset.ViewModels.ModuleVM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Asset.Domain.Services
{
    public interface IModuleService
    {
        Task<ModulesPermissionsResult> getAll(int First, int Rows,SearchSortModuleVM SearchSortObj);
    }
}
