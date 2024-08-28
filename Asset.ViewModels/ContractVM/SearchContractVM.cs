using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Asset.ViewModels.ContractVM
{
    public class SearchContractVM
    {

        public int Id { get; set; }
        public string UserId { get; set; }
        public string ContractNumber { get; set; }
        public string Subject { get; set; }

        public string BarCode { get; set; }
        public string SerialNumber { get; set; }
        public string Model { get; set; }



        public DateTime? ContractDate { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string Start { get; set; }
        public string End { get; set; }

        public int? GovernorateId { get; set; }
        public int? CityId { get; set; }
        public int? OrganizationId { get; set; }
        public int? SubOrganizationId { get; set; }
        public int? HospitalId { get; set; }
        public int? DepartmentId { get; set; }
        public int? BrandId { get; set; }
        public int? MasterAssetId { get; set; }
        public int? SupplierId { get; set; }
        public int? OriginId { get; set; }
        public int SelectedContractType { get; set; }


    }
}
