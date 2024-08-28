﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Asset.ViewModels.CityVM
{
    public class IndexCityVM
    {

        public List<GetData> Results { get; set; }


        public class GetData
        {
            public int Id { get; set; }
            public int CountAssets { get; set; }

            public string Code { get; set; }
            public string Name { get; set; }
            public string NameAr { get; set; }
            public decimal? Latitude { get; set; }
            public decimal? Longtitude { get; set; }
        }
    }
}
