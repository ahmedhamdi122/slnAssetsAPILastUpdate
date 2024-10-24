using Asset.Domain;
using Asset.Domain.Services;
using Asset.ViewModels.RoleCategoryVM;
using Asset.ViewModels.UserVM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Asset.Core.Services
{
    public class UserService : IUserService
    {
        private IUnitOfWork UnitOfWork;
        public UserService(IUnitOfWork unitOfWork)
        {
            UnitOfWork=unitOfWork;
        }
        public async Task<UserResultVM> GetAll(int first, int rows, SortSearchVM sortSearchVM)
        {
          return await UnitOfWork.UserRepository.GetAll(first,rows,sortSearchVM.SortField, sortSearchVM.SortOrder, sortSearchVM.search);
        }
    }
}
