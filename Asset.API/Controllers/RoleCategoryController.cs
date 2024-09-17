using Asset.API.Helpers;
using Asset.Core.Services;
using Asset.Domain.Services;
using Asset.Models;
using Asset.ViewModels.HospitalVM;
using Asset.ViewModels.RoleCategoryVM;
using DocumentFormat.OpenXml.InkML;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Asset.API.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class RoleCategoryController : ControllerBase
    {
        private IRoleCategoryService _roleCategoryService;
        private ApplicationDbContext _context;
        private IRoleService _roleService;
        private readonly UserManager<ApplicationUser> _userManager;

        public RoleCategoryController(IRoleCategoryService roleCategoryService, UserManager<ApplicationUser> userManager, IRoleService roleService,ApplicationDbContext context)
        {
            _roleCategoryService = roleCategoryService;
            _userManager = userManager;
            _roleService = roleService;
            _context = context; 
        }

        [HttpGet]
        [Route("ListRoleCategories")]
        public async Task<IEnumerable<IndexCategoryVM.GetData>> GetAll()
        {
            return  await _roleCategoryService.GetAll();
        }


        [HttpGet]
        [Route("GetById/{id}")]
        public async Task<ActionResult<RoleCategory>> GetById(int id)
        {
            return await _roleCategoryService.GetById(id);
        }



        [HttpPut]
        [Route("UpdateRoleCategory")]
        public IActionResult Update(EditRoleCategory roleCategory)
        {
            try
            {
                int updatedRow = _roleCategoryService.Update(roleCategory);
            }
            catch (DbUpdateConcurrencyException ex)
            {
                string msg = ex.Message;
                return BadRequest("Error in update");
            }

            return Ok();
        }


        [HttpPost]
        [Route("AddRoleCategory")]
        public ActionResult<RoleCategory> Add(CreateRoleCategory roleCategory)
        {
            var savedId = _roleCategoryService.Add(roleCategory);
            return CreatedAtAction("GetById", new { id = savedId }, roleCategory);
        }

        [HttpDelete]
        [Route("DeleteRoleCategory/{id}")]
        public  async Task<ActionResult<RoleCategory>> Delete(int id)
        {
            try
            {

                var isRoleCategoryExists = await _roleCategoryService.isRoleCategoryExistsUsingId(id);
                if (!isRoleCategoryExists)
                {
                    return StatusCode(StatusCodes.Status404NotFound, new Response { Status = "notFound", Message = "Can't delete becuase not found", MessageAr = "لا يوجد " });
                }
                var isRoleWithRoleCategory = await _roleService.hasRoleWithRoleCategoryId(id);
                if (isRoleWithRoleCategory)
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "roleCategoryInUse", Message = "You cannot delete this category it has related data", MessageAr = "لا يمكنك مسح هذا العنصر وذلك لارتباط بيانات بها" });
                }

                var data = _context.ApplicationRole.ToList();
                int deletedRow = await _roleCategoryService.Delete(id);

            }
            catch (DbUpdateConcurrencyException ex)
            {
                string msg = ex.Message;
                return BadRequest("Error in delete");
            }

            return Ok();
        }



        [HttpPost]
        [Route("LoadRoleCategories/{first}/{rows}")]
        public async Task<IndexCategoryVM> LoadRoleCategories(int first, int rows, SortSearchVM SortSearchObj)
        {
            return await _roleCategoryService.LoadRoleCategories(first,rows, SortSearchObj.SortField, SortSearchObj.SortOrder, SortSearchObj.search);
        }


        [HttpGet]
        [Route("GenerateRoleCategoryOrderId")]
        public GenerateRoleCategoryOrderVM GenerateRoleCategoryOrderId()
        {
            return _roleCategoryService.GenerateRoleCategoryOrderId();
        }


    }
}
