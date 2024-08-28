﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Asset.ViewModels.SupplierVM
{
    public class IndexSupplierVM
    {

        public List<GetData> Results { get; set; }
        public int Count { get; set; }

        public class GetData
        {
            public int Id { get; set; }
            public string Code { get; set; }
            public string Name { get; set; }
            public string NameAr { get; set; }
            public string Mobile { get; set; }
            public string Website { get; set; }
            public string EMail { get; set; }
            public string Address { get; set; }
            public string AddressAr { get; set; }
            public string ContactPerson { get; set; }
            public string Fax { get; set; }
            public string Notes { get; set; }

            public int CountAssets { get; set; }
            public decimal? SumPrices { get; set; }
        }
    }
}
