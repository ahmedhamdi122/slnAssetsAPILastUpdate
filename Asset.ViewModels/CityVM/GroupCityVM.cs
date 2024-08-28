﻿using Asset.ViewModels.AssetDetailVM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Asset.ViewModels.CityVM
{
    public class GroupCityVM
    {
        public int Id { get; set; }
        public int CountAssets { get; set; }

        public string Name { get; set; }
        public string NameAr { get; set; }
        public List<IndexAssetDetailVM.GetData> AssetList { get; set; }
    }
}
