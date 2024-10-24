using Asset.ViewModels.UserVM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Asset.Domain.Repositories
{
    public interface IUserRepository
    {
       Task<UserResultVM> GetAll(int first, int rows, string SortField, int SortOrder, string search);
    }
}
