using Asset.Models;
using Asset.ViewModels.ModuleVM;
using Asset.ViewModels.PermissionVM;
using DocumentFormat.OpenXml.Office2010.ExcelAc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using Asset.Domain.Services;

namespace Asset.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ModuleController : ControllerBase
    {
        private IModuleService _ModuleService;
        public ModuleController(IModuleService moduleService)
        {
            _ModuleService = moduleService;
        }

        [HttpPost]
        [Route("{First}/{Rows}")]
        public async Task<ModulesPermissionsResult> getAll(int First, int Rows,  SearchSortModuleVM SearchSortObj)
        {
            return await _ModuleService.getAll(First, Rows, SearchSortObj);
        }

    }
}
