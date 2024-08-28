using Asset.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Asset.ViewModels.AssetStatusVM
{
    public class IndexAssetStatusVM
    {

        public List<GetData> Results { get; set; }

        public int? CountNeedRepair { get; set; }
        public int? CountInActive { get; set; }
        public int? CountWorking { get; set; }
        public int? CountUnderMaintenance { get; set; }
        public int? CountUnderInstallation { get; set; }
        public int? CountNotWorking { get; set; }
        public int? CountShutdown { get; set; }
        public int? CountExecluded { get; set; }
        public int? CountHold { get; set; }
        public int? TotalCount { get; set; }



     
        public int? lstStatus10 { get; set; }
        public int?  lstStatus11  { get; set; }
        public int?  lstStatus12  { get; set; }
        public int?  lstStatus13  { get; set; }
        public int?  lstStatus14  { get; set; }
        public int?  lstStatus15  { get; set; }
        public int?  lstStatus16  { get; set; }
        public int?  lstStatus17  { get; set; }
        public int?  lstStatus18  { get; set; }
        public int?  lstStatus19  { get; set; }




        public List<AssetStatu> ListStatus { get; set; }


        public class GetData
        {
            public int Id { get; set; }
            public string Code { get; set; }
            public string Name { get; set; }
            public string NameAr { get; set; }




            public int? HospitalId { get; set; }
            public int? GovernorateId { get; set; }
            public int? CityId { get; set; }
            public int? OrganizationId { get; set; }
            public int? SubOrganizationId { get; set; }
            public List<AssetStatu> ListStatus { get; set; }
            public int? CountNeedRepair { get; set; }
            public int? CountInActive { get; set; }
            public int? CountWorking { get; set; }
            public int? CountUnderMaintenance { get; set; }
            public int? CountUnderInstallation { get; set; }
            public int? CountNotWorking { get; set; }
            public int? CountShutdown { get; set; }
            public int? CountExecluded { get; set; }
            public int? CountHold { get; set; }

            public int? TotalCount { get; set; }
        }
    }
}
