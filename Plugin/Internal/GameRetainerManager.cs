using FFXIVClientStructs.FFXIV.Client.Game;
using Plugin.Utilities;

namespace Plugin.Internal
{
    public static unsafe class GameRetainerManager
    {
        public static bool IsReady => RetainerManager.Instance()->Ready != 0;

        public static Retainer[] GetRetainers()
        {
            var rawRetainers = RetainerManager.Instance()->Retainers.ToArray();
            return rawRetainers
                .Where(x => x.RetainerId != 0 && x.Name[0] != 0)
                .Select(x => new Retainer(x))
                .ToArray();
        }

        public static int RetainerCount => GetRetainers().Length;

        private static readonly Dictionary<RetainerManager.RetainerTown, string> TownIconPaths = new()
        {
            { RetainerManager.RetainerTown.LimsaLominsa, "ui/icon/060000/060881.tex" },
            { RetainerManager.RetainerTown.Gridania, "ui/icon/060000/060882.tex" },
            { RetainerManager.RetainerTown.Uldah, "ui/icon/060000/060883.tex" },
            { RetainerManager.RetainerTown.Ishgard, "ui/icon/060000/060884.tex" },
            { RetainerManager.RetainerTown.Kugane, "ui/icon/060000/060885.tex" },
            { RetainerManager.RetainerTown.Crystarium, "ui/icon/060000/060886.tex" },
            { RetainerManager.RetainerTown.OldSharlayan, "ui/icon/060000/060887.tex" }
            //{ RetainerManager.RetainerTown.Tuliyollal, "ui/icon/060000/060888.tex" }
        };

        public static string GetTownIconPath(RetainerManager.RetainerTown town)
        {
            return TownIconPaths.TryGetValue(town, out var path) ? path : null;
        }

        public static int GetTownIconID(RetainerManager.RetainerTown town)
        {
            return town switch {
                RetainerManager.RetainerTown.LimsaLominsa => 060881,
                RetainerManager.RetainerTown.Gridania => 060882,
                RetainerManager.RetainerTown.Uldah => 060883,
                RetainerManager.RetainerTown.Ishgard => 060884,
                RetainerManager.RetainerTown.Kugane => 060885,
                RetainerManager.RetainerTown.Crystarium => 060886,
                RetainerManager.RetainerTown.OldSharlayan => 060887,
              //RetainerManager.RetainerTown.Tuliyollal => 060888,
                _ => 60071 // Return an question mark icon if the town is not recognized
            };
        }

        public class Retainer
        {
            private readonly RetainerManager.Retainer _handle;

            public string Name { get; }
            public uint VentureID => _handle.VentureId;
            public bool IsAvailable => _handle.ClassJob != 0 && _handle.Available;
            public DateTime VentureCompletionDate => Utils.DateFromTimeStamp(_handle.VentureComplete);
            public ulong RetainerID => _handle.RetainerId;
            public uint Gil => _handle.Gil;
            public uint VentureCompleteTimeStamp => _handle.VentureComplete;
            public int ItemCount => _handle.ItemCount;
            public int MarketItemCount => _handle.MarketItemCount;
            public uint MarketExpire => _handle.MarketExpire;
            public int Level => _handle.Level;
            public uint ClassJob => _handle.ClassJob;
            public RetainerManager.RetainerTown Town => _handle.Town;

            public Retainer(RetainerManager.Retainer handle)
            {
                _handle = handle;
                Name = handle.Name.Read();
            }
        }
    }
}
