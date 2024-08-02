using ECommons.Configuration;
using ECommons.ExcelServices.TerritoryEnumeration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plugin.Utility.Data;

public class HousingData : IEzConfig
{
    public Dictionary<uint, List<PlotInfo>> Data = [];
}
