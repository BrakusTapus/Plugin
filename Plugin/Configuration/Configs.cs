using System.Collections.ObjectModel;
using ECommons.Configuration;
using Plugin.Features;
using Plugin.Utilities.Data;
using Plugin.Utilities.Enums;
using Plugin.Utilities.Helpers;

namespace Plugin.Configuration;

public class Configs : IEzConfig
{
    public ObservableCollection<string> EnabledTweaks = [];
    public TweakConfigs Tweaks = new();


    public int Version { get; set; } = 3;

    public bool Enable = true;
    internal bool AllowClosingESC2 = false;
    public int ButtonWidth = 10;
    public int ButtonHeightAetheryte = 1;
    public int ButtonHeightWorld = 5;
    public bool FixedPosition = false;
    public Vector2 Offset = Vector2.Zero;
    public bool UseMapTeleport = true;
    public bool HideAddon = true;
    public HashSet<string> HideAddonList = [.. Utils.DefaultAddons];
    //public BasePositionHorizontal PosHorizontal = BasePositionHorizontal.Middle;
    //public BasePositionVertical PosVertical = BasePositionVertical.Middle;
    public bool ShowAethernet = true;
    public bool ShowWorldVisit = true;
    public HashSet<uint> Favorites = [];
    public HashSet<uint> Hidden = [];
    public Dictionary<uint, string> Renames = [];
    public WorldChangeAetheryte WorldChangeAetheryte = WorldChangeAetheryte.Uldah;
    public bool Firmament = true;
    public bool WalkToAetheryte = true;
    public bool LeavePartyBeforeWorldChange = true;
    public bool AllowDcTransfer = true;
    public bool LeavePartyBeforeLogout = true;
    public bool TeleportToGatewayBeforeLogout = true;
    public bool NoProgressBar = false;
    public Dictionary<string, int> ServiceAccounts = [];
    public bool DCReturnToGateway = false;
    public bool WorldVisitTPToAethernet = false;
    public string WorldVisitTPTarget = "";
    public bool WorldVisitTPOnlyCmd = true;
    public bool UseAutoRetainerAccounts = true;
    public bool SlowTeleport = false;
    public int SlowTeleportThrottle = 0;
    public bool WaitForScreenReady = true;
    public bool ShowWards = true;
    internal bool ShowPlots = false;
    public List<AddressBookFolder> AddressBookFolders = [];
    public bool AddressNoPathing = false;
    public bool AddressApartmentNoEntry = false;
    public bool SingleBookMode = false;
    public List<MultiPath> MultiPathes = [];
    public string GameVersion = "";
    public Dictionary<uint, int> PublicInstances = [];
    public bool ShowInstanceSwitcher = true;
    public int InstanceButtonHeight = 10;
    public bool UseSprintPeloton = true;
    public bool DisplayChatTeleport = false;
    public bool DisplayPopupNotifications = true;


    // Migrated configs from Configs class
    // Main window options
    public bool IsMainWindowMovable { get; set; } = false;
    public bool IsMainWindowResizeable { get; set; } = false;
    public bool IsMainWindowNoTitleBar { get; set; } = false;
    public bool IsMainNoWindowScrollbar { get; set; } = false;
    public bool IsMainWindowNoScrollWithMouse { get; set; } = false;
    public bool IsMainWindowNoCollapseable { get; set; } = false;
    public bool IsMainWindowNoBackground { get; set; } = false;

    // Configs window options
    public bool IsConfigWindowMovable { get; set; } = true;
    public bool IsConfigWindowResizeable { get; set; } = true;
    public bool IsConfigWindowNoTitleBar { get; set; } = true;
    public bool IsConfigNoWindowScrollbar { get; set; } = true;
    public bool IsConfigWindowNoScrollWithMouse { get; set; } = true;
    public bool IsConfigWindowNoCollapseable { get; set; } = true;
    public bool IsConfigWindowNoBackground { get; set; } = true;
    public bool ShowToolTips { get; set; } = false;

    public bool SomePropertyToBeSavedAndWithADefault { get; set; } = false;

    // Change Instance Task related
    public bool InstanceSwitcherRepeat = true;
    public bool EnableFlydownInstance = true;
    public static bool UseFrameDelay = true;
    public int Delay = 200;
    public int FrameDelay = 8;

    // extra
    public bool EnableAutoDismount = false;


}

public class TweakConfigs
{
    //public AchievementTrackerConfiguration AchievementTrackerConfiguration { get; init; } = new();
    //public AddresBookConfiguration AddresBook { get; init; } = new();
    //public AddresBookDebugConfiguration AddresBookDebug { get; init; } = new();
    //public ARTurnInConfiguration ARTurnIn { get; init; } = new();
    //public ARxMBConfiguration ARxMB { get; init; } = new();
    //public AutoFollowConfiguration AutoFollow { get; init; } = new();
    //public AutoSelectGardeningConfiguration AutoSelectGardening { get; init; } = new();
    //public ClickToMoveConfiguration ClickToMove { get; init; } = new();
    //public CommandsConfiguration Commands { get; init; } = new();
    //public DateWithDestinyConfiguration DateWithDestiny { get; init; } = new();
    //public DebugToolsConfiguration DebugTools { get; init; } = new();
    //public EnhancedDutyStartEndConfiguration EnhancedDutyStartEnd { get; init; } = new();
    //public EnhancedTargetingConfiguration EnhancedTargeting { get; init; } = new();
    //public FcChestTabRenameConfiguration FCChestTabRename { get; init; } = new();
    //public GettingTooAttachedConfiguration GettingTooAttached { get; init; } = new();
    //public GMAlertConfiguration GMAlert { get; init; } = new();
    //public HuntRelayHelperConfiguration HuntRelayHelper { get; init; } = new();
    public AutoAdjustRetainerListingsConfiguration MarketAdjuster { get; init; } = new();
}
