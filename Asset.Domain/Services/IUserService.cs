using Asset.ViewModels.RoleCategoryVM;
using Asset.ViewModels.UserVM;
using Asset.ViewModels.VisitVM;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Asset.Domain.Services
{
    public interface IUserService
    {
        Task<UserResultVM> GetAll(int first, int rows, SortSearchVM sortSearchVM);

    }
}
