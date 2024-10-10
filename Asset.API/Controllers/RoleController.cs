using Asset.API.Helpers;
using Asset.Core.Services;
using Asset.Domain.Services;
using Asset.Models;
using Asset.Models.Models;
using Asset.ViewModels.ModuleVM;
using Asset.ViewModels.PagingParameter;
using Asset.ViewModels.PermissionVM;
using Asset.ViewModels.RoleCategoryVM;
using Asset.ViewModels.RoleVM;
using DocumentFormat.OpenXml.Bibliography;
using DocumentFormat.OpenXml.Drawing.Spreadsheet;
using DocumentFormat.OpenXml.Packaging;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Diagnostics;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Asset.API.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class RoleController : ControllerBase
    {

        RoleManager<ApplicationRole> _applicationRole;
        private readonly ApplicationDbContext _context;
        private IPagingService _pagingService;
        private IRoleCategoryService _roleCategoryService;
        private IRoleService RoleService;
        public RoleController(ApplicationDbContext context, RoleManager<ApplicationRole> applicationRole,
            IRoleCategoryService roleCategoryService, IPagingService pagingService,IRoleService roleService)
        {
            _context = context;
            _applicationRole = applicationRole;
            _roleCategoryService = roleCategoryService;
            _pagingService = pagingService;
            RoleService=roleService;
        }


        //[HttpPut]
        //[Route("ListRolesWithPages")]
        //public List<IndexRoleVM.GetData> Index(PagingParameter pageInfo)
        //{
        //    List<IndexRoleVM.GetData> lstRoles = new List<IndexRoleVM.GetData>();
        //    var rlst = _context.ApplicationRole.ToList();
        //    var roles = _pagingService.GetAll<ApplicationRole>(pageInfo, rlst);
        //    foreach (var item in roles)
        //    {
        //        IndexRoleVM.GetData roleObj = new IndexRoleVM.GetData();
        //        roleObj.Id = item.Id;
        //        roleObj.Name = item.Name;
        //        roleObj.DisplayName = item.DisplayName;
        //        roleObj.CategoryName = _roleCategoryService.GetById(item.RoleCategoryId).Name;
        //        lstRoles.Add(roleObj);
        //    }
        //    return lstRoles;
        //}


        [HttpPost]
        [Route("{first}/{rows}")]
        public async Task<IndexRoleVM> getAll(int first, int rows, SortSearchVM sortSearchObj)
        {
             return await RoleService.getAll( first,  rows,  sortSearchObj);
        }

                 
        [HttpGet]
        [Route("getcount")]
        public int count()
        {
            return _pagingService.Count<RoleCategory>();
        }
        [HttpPost]
        [Route("GetRoleByIdForEdit/{roleId}/{first}/{rows}")]
        public async Task<ActionResult<ModulesPermissionsWithSelectedPermissionIDsResult>> GetByIdForEdit(string roleId, int first, int rows, SortSearchVM sortSearchObj)
        {
            return await RoleService.getModulesPermissionsbyRoleIdForEdit(roleId, first, rows, sortSearchObj);
        }
        [HttpGet]
        [Route("GetRoleById/{roleId}")]
        public async Task<ActionResult<RoleVM>> ViewById(string roleId)
        {
            var res = await RoleService.getById(roleId);
            if (res == null)
                return StatusCode(StatusCodes.Status404NotFound, new Response { Status = "notFound", Message = "not found any role contain this ID", MessageAr = ".لا يوجد أي دور بهذا الرقم" });
            return res;
        }
        //[HttpGet]
        //[Route("GetRoleById/{roleId}")]
        //public async Task<ActionResult<ViewRoleWithModulePermissonsVM>> ViewById(string roleId)
        //{
        //    //return await _context.ApplicationRole.Include(r =>r.RoleModulePermissions).ThenInclude(r=>r.Permission).Include(m => m.RoleModulePermissions).ThenInclude(r => r.Module).Where(m=>m.RoleModulePermissions.Any()).Select(r => new ViewRoleWithModulePermissonsVM { Name =r.Name,DisplayName=r.DisplayName, ModulesWithPermissions = new ModuleNameWithPermissionsVM() { name = m.Name, nameAr = m.NameAr, Permissions = m.RoleModulePermissions.Where(r => r.RoleId == roleId).Select(p => p.Permission.Name).ToList() } });
        //}

        [HttpGet]
        [Route("GetRolesByRoleCategoryId/{catId}")]
        public async Task<List<ApplicationRole>> GetById(int catId)
        {
            var lstRoles = await _applicationRole.Roles.Where(a => a.RoleCategoryId == catId).ToListAsync();
            return lstRoles;
        }

        [HttpGet]
        [Route("AddRoleToListById/{id}")]
        public async Task<IActionResult> AddRoleToListById(string id)
        {
           
            var updateObj = await _applicationRole.FindByIdAsync(id);
            return Ok(updateObj);
        }




        [HttpPost]
        [Route("AddRole")]
        public async Task<IActionResult> Create(CreateRoleVM role)
        {
            var Exists = await RoleService.CheckRoleExists(role.Name,role.DisplayName);
            if(Exists !=null) 
            {
                if(Exists== "Name")
                {
                    return StatusCode(StatusCodes.Status409Conflict, new Response { Status = "roleExists", Message = "Role Name already exists.", MessageAr = ".اسم الدور موجود بالفعل" });
                }
                else if(Exists== "DisplayName")
                {
                    return StatusCode(StatusCodes.Status409Conflict, new Response { Status = "DisplayNameroleExists", Message = "Role DisplayName already exists.", MessageAr = ".اسم العرض للدور موجود بالفعل" });
                }
            }
            var error = await RoleService.ValidateModuleAndPermissionsAsync(role.ModuleIdsWithPermissions);
            if (error != null)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "notFound", Message = error, MessageAr = error });
            }
            await RoleService.AddRoleWithModulePermissionsAsync(role);
            return Ok();
        }

        [HttpPost("{roleId}/modules-permissions/{first}/{rows}")]
        public async Task<ActionResult<ModulesPermissionsResult>> getModulesPermissionsbyRoleId(string roleId, int first, int rows, SortSearchVM sortSearchObj)
        {
            return await RoleService.getModulesPermissionsbyRoleId(roleId,first, rows, sortSearchObj);

        }

        [HttpPut]
        [Route("UpdateRole")]
        public async Task<IActionResult> Update(ApplicationRole roleObj)
        {
            var updateObj = await _applicationRole.FindByIdAsync(roleObj.Id);
            updateObj.RoleCategoryId = roleObj.RoleCategoryId;
            updateObj.DisplayName = roleObj.DisplayName;
            updateObj.Name = roleObj.Name;
            await _applicationRole.UpdateAsync(updateObj);
            return Ok();
        }

        [HttpDelete]
        [Route("{RoleId}")]
        public async Task<ActionResult> Delete(string RoleId)
        {
            try
            {

                bool IsRoleAssignedToUsers = await RoleService.IsRoleAssignedToUsersService(RoleId);
                if (IsRoleAssignedToUsers) 
                {
                    return StatusCode(StatusCodes.Status409Conflict, new Response { Status = "IsRoleAssignedToUsers", Message = "You cannot delete this role while users are still assigned to it.", MessageAr = "لا يمكنك حذف هذا الدور بينما لا يزال هناك مستخدمون مرتبطون بها" });
                }

                await RoleService.DeleteRoleService(RoleId);
            }
            catch (DbUpdateConcurrencyException ex)
            {
                string msg = ex.Message;
                return BadRequest("Error in delete");
            }

            return Ok();
        }

    }
}
