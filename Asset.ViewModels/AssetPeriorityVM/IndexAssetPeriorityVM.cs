using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Asset.ViewModels.AssetPeriorityVM
{
    public class IndexAssetPeriorityVM
    {

        public List<GetData> Results { get; set; }


        public class GetData
        {
            public int Id { get; set; }
            public string Code { get; set; }
            public string Name { get; set; }
            public string NameAr { get; set; }
            public string Color { get; set; }






            public int HighCount { get; set; }
            public int MediumCount { get; set; }
            public int NormalCount { get; set; }
            public int MedicalCount { get; set; }
            public int ProductionCount { get; set; }
            public int TotalCount { get; set; }
        }
    }
}
