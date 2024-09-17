using Asset.Domain;
using Asset.Domain.Services;
using Asset.ViewModels.RoleCategoryVM;
using Asset.ViewModels.RoleVM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Asset.Core.Services
{
    public class RoleService: IRoleService
    {
        private IUnitOfWork _unitOfWork;
        public RoleService(IUnitOfWork unitOfWork)
        {
            _unitOfWork=unitOfWork;
        }
        public async Task<bool> hasRoleWithRoleCategoryId(int id)
        {
            return await _unitOfWork.Role.hasRoleWithRoleCategoryId(id);
        }
        public Task<IndexRoleVM> getAll(int first, int rows, SortSearchVM sortSearchObj)
        {
            return _unitOfWork.Role.getAll(first, rows, sortSearchObj);
        }
    }
}
