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

namespace Asset.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ModuleController : ControllerBase
    {
        private ApplicationDbContext _context;
        public ModuleController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<List<ModuleWithPermissionsVM>>> getAll()
        {
            return await _context.Modules.Include(m => m.Permissions).Select(m => new ModuleWithPermissionsVM(m.Id, m.Name, m.NameAr, m.Permissions.Select(p => new permissionVM(p.Id, p.Name)).ToList())).ToListAsync();
        }
    }
}
