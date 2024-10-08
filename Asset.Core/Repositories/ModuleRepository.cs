using Asset.Domain.Repositories;
using Asset.Models;
using Asset.ViewModels.MasterAssetVM;
using Asset.ViewModels.ModuleVM;
using Asset.ViewModels.PermissionVM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.EntityFrameworkCore;

using System.Threading.Tasks;

namespace Asset.Core.Repositories
{
    public class ModuleRepository:IModuleRepository
    {
        private ApplicationDbContext _context;
        public ModuleRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<ModulesPermissionsResult> getAll(int First, int Rows, SearchSortModuleVM SearchSortObj)
        {

            IQueryable<Module> Modules = _context.Modules.Include(m => m.Permissions);



            #region Search Criteria

            if (!string.IsNullOrEmpty(SearchSortObj.Name))
            {
                Modules = Modules.Where(x => x.Name.Contains(SearchSortObj.Name));
            }
            if (!string.IsNullOrEmpty(SearchSortObj.NameAr))
            {
                Modules = Modules.Where(x => x.NameAr.Contains(SearchSortObj.NameAr));
            }
            #endregion

            #region Sort Criteria

            if (SearchSortObj.SortFiled != null)
            {
                switch (SearchSortObj.SortFiled)
                {
                    case "Name":
                        if (SearchSortObj.SortOrder == 1)
                        {
                            Modules = Modules.OrderBy(m => m.Name);
                        }
                        else
                        {
                            Modules = Modules.OrderBy(m => m.Name);
                        }
                        break;
                    case "NameAr":
                        if (SearchSortObj.SortOrder == 1)
                        {
                            Modules = Modules.OrderBy(m => m.NameAr);
                        }
                        else
                        {
                            Modules = Modules.OrderBy(m => m.NameAr);
                        }
                        break;
                }
            }


            #endregion

            #region Represent data by Paging and count
            ModulesPermissionsResult modulesWithPermissions = new ModulesPermissionsResult();
            modulesWithPermissions.count = Modules.Count();
            if (SearchSortObj.SortOrder == 1) modulesWithPermissions.results = await Modules.Skip(First).Take(Rows).OrderBy(m => m.Id).Select(m => new ModuleWithPermissionsVM() {id= m.Id, name=m.Name,nameAr= m.NameAr,Permissions= m.Permissions.Select(p => new permissionVM() { id = p.Id, name = p.Name }).ToList() } ).ToListAsync();
            else modulesWithPermissions.results = await Modules.Skip(First).Take(Rows).OrderByDescending(m => m.Id).Select(m => new ModuleWithPermissionsVM() { id = m.Id, name = m.Name, nameAr = m.NameAr, Permissions = m.Permissions.Select(p => new permissionVM() { id = p.Id, name = p.Name }).ToList() }) .ToListAsync();


            #endregion

            return modulesWithPermissions;
        }
    }
}
