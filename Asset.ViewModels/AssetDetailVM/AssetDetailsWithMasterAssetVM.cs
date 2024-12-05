using Asset.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Asset.ViewModels.AssetDetailVM
{
    public class AssetDetailsWithMasterAssetVM
    {
        public string Barcode { get; set; }

        public int Id { get; set; }

        public string MasterAsseName { get; set; }

        public string MasterAsseNameAr { get; set; }
        public string MasterAsseBrandName { get; set; }

        public string MasterAsseBrandNameAr { get; set; }

        public string SerialNumber { get; set; }
        public string MasterAsseModelNumbe { get; set; }
        public string MasterAssetCode { get; set; }

        
   
    }
}
