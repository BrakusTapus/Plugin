﻿using ECommons.Configuration;
using ECommons.Events;
using ECommons.ExcelServices;
using ECommons.GameHelpers;
using Lumina.Excel.GeneratedSheets;
using System.IO;
using Path = System.IO.Path;
using Plugin.Helpers;

namespace Plugin.Data;

internal class DataStore
{
    internal const string FileName = "StaticData.json";
    internal uint[] Territories;
    internal Dictionary<TinyAetheryte, List<TinyAetheryte>> Aetherytes = [];
    internal string[] Worlds = Array.Empty<string>();
    internal string[] DCWorlds = Array.Empty<string>();
    internal StaticData StaticData;

    internal TinyAetheryte GetMaster(TinyAetheryte aetheryte)
    {
        foreach (var x in Aetherytes.Keys)
        {
            if (x.Group == aetheryte.Group) return x;
        }
        return default;
    }

    internal DataStore()
    {
        var terr = new List<uint>();
        StaticData = EzConfig.LoadConfiguration<StaticData>(System.IO.Path.Combine(Svc.PluginInterface.AssemblyLocation.DirectoryName, FileName), false);
        Svc.Data.GetExcelSheet<Aetheryte>().Each(x =>
        {
            if (x.AethernetGroup != 0)
            {
                if (x.IsAetheryte)
                {
                    Aetherytes[GetTinyAetheryte(x)] = [];
                    terr.Add(x.Territory.Value.RowId);
                }
            }
        });
        Svc.Data.GetExcelSheet<Aetheryte>().Each(x =>
        {
            if (x.AethernetGroup != 0)
            {
                if (!x.IsAetheryte)
                {
                    var a = GetTinyAetheryte(x);
                    Aetherytes[GetMaster(a)].Add(a);
                    terr.Add(x.Territory.Value.RowId);
                }
            }
        });
        foreach (var x in Aetherytes.Keys.ToArray())
        {
            Aetherytes[x] = [.. Aetherytes[x].OrderBy(x => GetAetheryteSortOrder(x.ID))];
        }
        Territories = [.. terr];
        if (ProperOnLogin.PlayerPresent)
        {
            //BuildWorlds();
        }
    }

    internal uint GetAetheryteSortOrder(uint id)
    {
        var ret = 10000u;
        if (StaticData.SortOrder.TryGetValue(id, out var x))
        {
            ret += x;
        }
        if (C.Favorites.Contains(id))
        {
            ret -= 10000u;
        }
        return ret;
    }

    //internal void BuildWorlds()
    //{
    //    BuildWorlds(Svc.ClientState.LocalPlayer.CurrentWorld.GameData.DataCenter.Value.RowId);
    //    if(Player.Settings)
    //    {
    //        bool AutoRetainerApi = false;
    //        if(P.AutoRetainerApi?.Ready == true && P.Configs.UseAutoRetainerAccounts)
    //        {
    //            var data = P.AutoRetainerApi.GetOfflineCharacterData(Player.CID);
    //            if(data != null)
    //            {
    //                P.Configs.ServiceAccounts[Player.NameWithWorld] = data.ServiceAccount;
    //            }
    //        }
    //        else if(!P.Configs.ServiceAccounts.ContainsKey(Player.NameWithWorld))
    //        {
    //            P.Configs.ServiceAccounts[Player.NameWithWorld] = -1;
    //        }
    //    }
    //}

    internal void BuildWorlds(uint dc)
    {
        Worlds = [.. Svc.Data.GetExcelSheet<World>().Where(x => x.DataCenter.Value.RowId == dc && x.IsPublic()).Select(x => x.Name.ToString()).Order()];
        PluginLog.Debug($"Built worlds: {Worlds.Print()}");
        DCWorlds = Svc.Data.GetExcelSheet<World>().Where(x => x.DataCenter.Value.RowId != dc && x.IsPublic() && (x.DataCenter.Value.Region == Player.Object.HomeWorld.GameData.DataCenter.Value.Region || x.DataCenter.Value.Region == 4)).Select(x => x.Name.ToString()).ToArray();
        PluginLog.Debug($"Built DCworlds: {DCWorlds.Print()}");
    }

    internal TinyAetheryte GetTinyAetheryte(Aetheryte aetheryte)
    {
        var AethersX = 0f;
        var AethersY = 0f;
        if (StaticData.CustomPositions.TryGetValue(aetheryte.RowId, out var pos))
        {
            AethersX = pos.X;
            AethersY = pos.Z;
        }
        else
        {
            var map = Svc.Data.GetExcelSheet<Map>().FirstOrDefault(m => m.TerritoryType.Row == aetheryte.Territory.Value.RowId);
            var scale = map.SizeFactor;
            var mapMarker = Svc.Data.GetExcelSheet<MapMarker>().FirstOrDefault(m => m.DataType == (aetheryte.IsAetheryte ? 3 : 4) && m.DataKey == (aetheryte.IsAetheryte ? aetheryte.RowId : aetheryte.AethernetName.Value.RowId));
            if (mapMarker != null)
            {
                AethersX = Utils.ConvertMapMarkerToRawPosition(mapMarker.X, scale);
                AethersY = Utils.ConvertMapMarkerToRawPosition(mapMarker.Y, scale);
            }
        }
        return new(new(AethersX, AethersY), aetheryte.Territory.Value.RowId, aetheryte.RowId, aetheryte.AethernetGroup);
    }
}
