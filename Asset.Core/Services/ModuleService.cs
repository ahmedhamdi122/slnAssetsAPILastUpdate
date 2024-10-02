using Asset.Domain;
using Asset.Domain.Services;
using Asset.ViewModels.ModuleVM;
using Asset.ViewModels.PermissionVM;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Asset.Core.Services
{
    public class ModuleService : IModuleService
    {
        private IUnitOfWork _unitOfWork;
        public ModuleService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;

        }
        public async Task<ModulesPermissionsResult> getAll(int First, int Rows, SearchSortModuleVM SearchSortObj)
        {
           return await _unitOfWork.ModuleRepository.getAll(First,Rows,SearchSortObj);
         
        }

    }
}
