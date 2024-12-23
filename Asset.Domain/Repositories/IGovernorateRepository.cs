using System.Collections.Generic;
using System.Threading.Tasks;
using Asset.Models;
using Asset.ViewModels.GovernorateVM;


namespace Asset.Domain.Repositories
{
  public  interface IGovernorateRepository
    {
        IEnumerable<Governorate> GetAllGovernorates();
        Task<IEnumerable<IndexGovernorateVM.GetData>> GetAll();
        EditGovernorateVM GetById(int id);
        EditGovernorateVM GetGovernorateByName(string govName);
        int Add(CreateGovernorateVM governorateObj); 
        int Update(EditGovernorateVM governorateObj);
        int Delete(int id);
        public IEnumerable<GovernorateWithHospitalsVM> GetGovernorateWithHospitals();
    }
}
