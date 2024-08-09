using System.IO;
using MyServices;

namespace Plugin.Utilities.UI;

public class UiPaths
{
    public UiPaths(string baseUiPath)
    {
        BasePath = baseUiPath;
        Initialize();
    }

    public string BasePath { get; }
    public static string? AccountManagementPath { get; private set; }
    public static string? AdvancedOptionsPath { get; private set; }
    public static string? AncientSpellPath { get; private set; }
    public static string? ArceuusSpellPath { get; private set; }
    public static string? BankPath { get; private set; }
    public static string? BondsPouchPath { get; private set; }
    public static string? ButtonPath { get; private set; }
    public static string? ChatboxPath { get; private set; }
    public static string? ClansTabPath { get; private set; }
    public static string? CombatPath { get; private set; }
    public static string? CombatAchievementsPath { get; private set; }
    public static string? CrossSpritesPath { get; private set; }
    public static string? DialogPath { get; private set; }
    public static string? EmotePath { get; private set; }
    public static string? EquipmentPath { get; private set; }
    public static string? FixedModePath { get; private set; }
    public static string? GePath { get; private set; }
    public static string? ImplingPath { get; private set; }
    public static string? LoginScreenPath { get; private set; }
    public static string? LunarSpellPath { get; private set; }
    public static string? NormalSpellPath { get; private set; }
    public static string? OptionsPath { get; private set; }
    public static string? OtherPath { get; private set; }
    public static string? PrayerPath { get; private set; }
    public static string? QuestsTabPath { get; private set; }
    public static string? ResizeableModePath { get; private set; }
    public static string? ScrollbarPath { get; private set; }
    public static string? SkillPath { get; private set; }
    public static string? StatsPath { get; private set; }
    public static string? TabPath { get; private set; }
    public static string? WelcomeScreenPath { get; private set; }



    public void Initialize()
    {
        AccountManagementPath = Path.Combine(BasePath, "account_management");
        AdvancedOptionsPath = Path.Combine(BasePath, "advanced_options");
        AncientSpellPath = Path.Combine(BasePath, "ancient_spell");
        ArceuusSpellPath = Path.Combine(BasePath, "arceuus_spell");
        BankPath = Path.Combine(BasePath, "bank");
        BondsPouchPath = Path.Combine(BasePath, "bonds_pouch");
        ButtonPath = Path.Combine(BasePath, "button");
        ChatboxPath = Path.Combine(BasePath, "chatbox");
        ClansTabPath = Path.Combine(BasePath, "clans_tab");
        CombatPath = Path.Combine(BasePath, "combat");
        CombatAchievementsPath = Path.Combine(BasePath, "combat_achievements");
        CrossSpritesPath = Path.Combine(BasePath, "cross_sprites");
        DialogPath = Path.Combine(BasePath, "dialog");
        EmotePath = Path.Combine(BasePath, "emote");
        EquipmentPath = Path.Combine(BasePath, "equipment");
        FixedModePath = Path.Combine(BasePath, "fixed_mode");
        GePath = Path.Combine(BasePath, "ge");
        ImplingPath = Path.Combine(BasePath, "impling");
        LoginScreenPath = Path.Combine(BasePath, "login_screen");
        LunarSpellPath = Path.Combine(BasePath, "lunar_spell");
        NormalSpellPath = Path.Combine(BasePath, "normal_spell");
        OptionsPath = Path.Combine(BasePath, "options");
        OtherPath = Path.Combine(BasePath, "other");
        PrayerPath = Path.Combine(BasePath, "prayer");
        QuestsTabPath = Path.Combine(BasePath, "quests_tab");
        ResizeableModePath = Path.Combine(BasePath, "resizeable_mode");
        ScrollbarPath = Path.Combine(BasePath, "scrollbar");
        SkillPath = Path.Combine(BasePath, "skill");
        StatsPath = Path.Combine(BasePath, "stats");
        TabPath = Path.Combine(BasePath, "tab");
        WelcomeScreenPath = Path.Combine(BasePath, "welcome_screen");

        Services.PluginLog.Debug("UI paths successfully initialized!");
    }
}
