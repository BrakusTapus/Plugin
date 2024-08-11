using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CheapLoc;

namespace Plugin.Windows.AlphaMainWindow;

/// <summary>
/// Manage category filters for MainMenu.
/// </summary>
internal class PluginCategoryManager
{
    /// <summary>
    /// First categoryId for tag based categories.
    /// </summary>
    private const int FirstTagBasedCategoryId = 100;

    private readonly CategoryInfo[] categoryList =
    [
        new(CategoryKind.All, "special.all", () => Locs.Category_All),
        new(CategoryKind.About, "special.about", () => Locs.Category_About),
        new(CategoryKind.General, "special.general", () => Locs.Category_General),
        new(CategoryKind.QualityOfLife, "special.qualityOfLife", () => Locs.Category_QualityOfLife),
        new(CategoryKind.Shortcuts, "special.shortcuts", () => Locs.Category_Shortcuts),
        new(CategoryKind.Character, "special.character", () => Locs.Category_Character),
        new(CategoryKind.Automation, "special.Automation", () => Locs.Category_Automation),

        // order doesn't matter, all tag driven categories should have Id >= FirstTagBasedCategoryId
    ];

    private GroupInfo[] groupList =
    [
        new(GroupKind.MainMenu, () => Locs.Group_MainMenu, CategoryKind.About, CategoryKind.Character),
        new(GroupKind.Commands, () => Locs.Group_Commands, CategoryKind.All, CategoryKind.QualityOfLife, CategoryKind.Shortcuts, CategoryKind.Alias),
        new(GroupKind.Features, () => Locs.Group_Features, CategoryKind.All, CategoryKind.QualityOfLife),
        new(GroupKind.Tasks, () => Locs.Group_Tasks, CategoryKind.All, CategoryKind.Automation, CategoryKind.Activities)

        // order important, used for drawing, keep in sync with defaults for currentGroupIdx
    ];

    private int currentGroupIdx = 2;
    private CategoryKind currentCategoryKind = CategoryKind.All;
    private bool isContentDirty;

    //private Dictionary<IPluginManifest, CategoryKind[]> mapPluginCategories = new();
    private List<CategoryKind> highlightedCategoryKinds = new();

    /// <summary>
    /// Type of category group.
    /// </summary>
    public enum GroupKind
    {
        /// <summary>
        /// UI group: Main menu.
        /// </summary>
        MainMenu,

        /// <summary>
        /// UI group: Commands.
        /// </summary>
        Commands,

        /// <summary>
        /// UI group: AutoMation.
        /// </summary>
        Features,

        /// <summary>
        /// UI group: Tasks.
        /// </summary>
        Tasks,
    }

    /// <summary>
    /// Type of category.
    /// </summary>
    public enum CategoryKind
    {
        /// <summary>
        /// All
        /// </summary>
        All = 0,

        /// <summary>
        /// 
        /// </summary>
        About = 1,

        /// <summary>
        /// 
        /// </summary>
        General = 2,

        /// <summary>
        /// 
        /// </summary>
        QualityOfLife = 3,

        /// <summary>
        /// 
        /// </summary>
        Shortcuts = 4,

        /// <summary>
        /// 
        /// </summary>
        Character = 5,

        /// <summary>
        /// 
        /// </summary>
        Automation = 6,

        /// <summary>
        /// 
        /// </summary>
        Activities = 7,

        /// <summary>
        /// 
        /// </summary>
        Enabled = 8,

        /// <summary>
        /// 
        /// </summary>
        Alias = 9
    }

    /// <summary>
    /// Gets the list of all known categories.
    /// </summary>
    public CategoryInfo[] CategoryList => this.categoryList;

    /// <summary>
    /// Gets the list of all known UI groups.
    /// </summary>
    public GroupInfo[] GroupList => this.groupList;

    /// <summary>
    /// Gets or sets the current group kind.
    /// </summary>
    public GroupKind CurrentGroupKind
    {
        get => this.groupList[this.currentGroupIdx].GroupKind;
        set
        {
            var newIdx = Array.FindIndex(this.groupList, x => x.GroupKind == value);
            if (newIdx >= 0)
            {
                this.currentGroupIdx = newIdx;
                this.currentCategoryKind = this.CurrentGroup.Categories.First();
                this.isContentDirty = true;
            }
        }
    }

    /// <summary>
    /// Gets information about currently selected group.
    /// </summary>
    public GroupInfo CurrentGroup => this.groupList[this.currentGroupIdx];

    /// <summary>
    /// Gets or sets the current category kind.
    /// </summary>
    public CategoryKind CurrentCategoryKind
    {
        get => this.currentCategoryKind;
        set
        {
            if (this.currentCategoryKind != value)
            {
                this.currentCategoryKind = value;
                this.isContentDirty = true;
            }
        }
    }

    /// <summary>
    /// Gets information about currently selected category.
    /// </summary>
    public CategoryInfo CurrentCategory => this.categoryList.First(x => x.CategoryKind == this.currentCategoryKind);

    /// <summary>
    /// Gets a value indicating whether current group + category selection changed recently.
    /// Changes in Settings group should be followed with <see cref="GetCurrentCategoryContent"/>, everything else can use <see cref="ResetContentDirty"/>.
    /// </summary>
    public bool IsContentDirty => this.isContentDirty;

    /// <summary>
    /// Gets a value indicating whether <see cref="CurrentCategoryKind"/> and <see cref="CurrentGroupKind"/> are valid.
    /// </summary>
    public bool IsSelectionValid =>
        (this.currentGroupIdx >= 0) &&
        (this.currentGroupIdx < this.groupList.Length) &&
        this.groupList[this.currentGroupIdx].Categories.Contains(this.currentCategoryKind);

    /// <summary>
    /// Rebuild available categories based on currently available plugins.
    /// </summary>
    /// <param name="availablePlugins">list of all available plugin manifests to install.</param>
    //public void BuildCategories(IEnumerable<PluginManifest> availablePlugins)
    //{
    //    // rebuild map plugin name -> categoryIds
    //    this.mapPluginCategories.Clear();

    //    var groupAvail = Array.Find(this.groupList, x => x.GroupKind == GroupKind.AutoMation);
    //    var prevCategoryIds = new List<CategoryKind>();
    //    prevCategoryIds.AddRange(groupAvail.Categories);

    //    var categoryList = new List<CategoryKind>();
    //    var allCategoryIndices = new List<int>();

    //    foreach (var manifest in availablePlugins)
    //    {
    //        categoryList.Clear();

    //        var pluginCategoryTags = this.GetCategoryTagsForManifest(manifest);
    //        if (pluginCategoryTags != null)
    //        {
    //            foreach (var tag in pluginCategoryTags)
    //            {
    //                // only tags from whitelist can be accepted
    //                var matchIdx = Array.FindIndex(this.CategoryList, x => x.Tag.Equals(tag, StringComparison.InvariantCultureIgnoreCase));
    //                if (matchIdx >= 0)
    //                {
    //                    var categoryKind = this.CategoryList[matchIdx].CategoryKind;
    //                    if ((int)categoryKind >= FirstTagBasedCategoryId)
    //                    {
    //                        categoryList.Add(categoryKind);

    //                        if (!allCategoryIndices.Contains(matchIdx))
    //                        {
    //                            allCategoryIndices.Add(matchIdx);
    //                        }
    //                    }
    //                }
    //            }
    //        }

    //        //if (PluginManager.HasTestingVersion(manifest) || manifest.IsTestingExclusive)
    //        //    categoryList.Add(CategoryKind.AvailableForTesting);

    //        // always add, even if empty
    //        this.mapPluginCategories.Add(manifest, categoryList.ToArray());
    //    }

    //    // sort all categories by their loc name
    //    allCategoryIndices.Sort((idxX, idxY) => this.CategoryList[idxX].Name.CompareTo(this.CategoryList[idxY].Name));
    //    allCategoryIndices.Insert(0, 2); // "Settings for testing"

    //    // rebuild all categories in group, leaving first entry = All intact and always on top
    //    if (groupAvail.Categories.Count > 1)
    //    {
    //        groupAvail.Categories.RemoveRange(1, groupAvail.Categories.Count - 1);
    //    }

    //    foreach (var categoryIdx in allCategoryIndices)
    //    {
    //        groupAvail.Categories.Add(this.CategoryList[categoryIdx].CategoryKind);
    //    }

    //    // Hidden at the end
    //    groupAvail.Categories.Add(CategoryKind.Alias);

    //    // compare with prev state and mark as dirty if needed
    //    var noCategoryChanges = prevCategoryIds.SequenceEqual(groupAvail.Categories);
    //    if (!noCategoryChanges)
    //    {
    //        this.isContentDirty = true;
    //    }
    //}

    /// <summary>
    /// Filters list of available plugins based on currently selected category.
    /// Resets <see cref="IsContentDirty"/>.
    /// </summary>
    /// <param name="plugins">List of available plugins to install.</param>
    /// <returns>Filtered list of plugins.</returns>
    //public List<PluginManifest> GetCurrentCategoryContent(IEnumerable<PluginManifest> plugins)
    //{
    //    var result = new List<PluginManifest>();

    //    if (this.IsSelectionValid)
    //    {
    //        var groupInfo = this.groupList[this.currentGroupIdx];

    //        var includeAll = this.currentCategoryKind == CategoryKind.All ||
    //                         this.currentCategoryKind == CategoryKind.Hidden ||
    //                         groupInfo.GroupKind != GroupKind.AutoMation;

    //        if (includeAll)
    //        {
    //            result.AddRange(plugins);
    //        }
    //        else
    //        {
    //            var selectedCategoryInfo = Array.Find(this.categoryList, x => x.CategoryKind == this.currentCategoryKind);

    //            foreach (var plugin in plugins)
    //            {
    //                if (this.mapPluginCategories.TryGetValue(plugin, out var pluginCategoryIds))
    //                {
    //                    var matchIdx = Array.IndexOf(pluginCategoryIds, selectedCategoryInfo.CategoryKind);
    //                    if (matchIdx >= 0)
    //                    {
    //                        result.Add(plugin);
    //                    }
    //                }
    //            }
    //        }
    //    }

    //    this.ResetContentDirty();
    //    return result;
    //}

    /// <summary>
    /// Clears <see cref="IsContentDirty"/> flag, indicating that all cached values about currently selected group + category have been updated.
    /// </summary>
    public void ResetContentDirty()
    {
        this.isContentDirty = false;
    }

    /// <summary>
    /// Sets category highlight based on list of plugins. Used for searching.
    /// </summary>
    /// <param name="plugins">List of plugins whose categories should be highlighted.</param>
    //public void SetCategoryHighlightsForPlugins(IEnumerable<PluginManifest> plugins)
    //{
    //    ArgumentNullException.ThrowIfNull(plugins);

    //    this.highlightedCategoryKinds.Clear();

    //    foreach (var entry in plugins)
    //    {
    //        if (this.mapPluginCategories.TryGetValue(entry, out var pluginCategories))
    //        {
    //            foreach (var categoryKind in pluginCategories)
    //            {
    //                if (!this.highlightedCategoryKinds.Contains(categoryKind))
    //                {
    //                    this.highlightedCategoryKinds.Add(categoryKind);
    //                }
    //            }
    //        }
    //    }
    //}

    /// <summary>
    /// Checks if category should be highlighted.
    /// </summary>
    /// <param name="categoryKind">CategoryKind to check.</param>
    /// <returns>true if highlight is needed.</returns>
    public bool IsCategoryHighlighted(CategoryKind categoryKind) => this.highlightedCategoryKinds.Contains(categoryKind);

    //private IEnumerable<string> GetCategoryTagsForManifest(PluginManifest pluginManifest)
    //{
    //    if (pluginManifest.CategoryTags != null)
    //    {
    //        return pluginManifest.CategoryTags;
    //    }

    //    return null;
    //}

    /// <summary>
    /// Plugin installer category info.
    /// </summary>
    public struct CategoryInfo
    {
        /// <summary>
        /// Unique Id number of category, tag match based should be greater of equal <see cref="FirstTagBasedCategoryId"/>.
        /// </summary>
        public CategoryKind CategoryKind;

        /// <summary>
        /// Tag from plugin manifest to match.
        /// </summary>
        public string Tag;

        private Func<string> nameFunc;

        /// <summary>
        /// Initializes a new instance of the <see cref="CategoryInfo"/> struct.
        /// </summary>
        /// <param name="categoryKind">Kind of the category.</param>
        /// <param name="tag">Tag to match.</param>
        /// <param name="nameFunc">Function returning localized name of category.</param>
        /// <param name="condition">Condition to be checked when deciding whether this category should be shown.</param>
        public CategoryInfo(CategoryKind categoryKind, string tag, Func<string> nameFunc, AppearCondition condition = AppearCondition.None)
        {
            this.CategoryKind = categoryKind;
            this.Tag = tag;
            this.nameFunc = nameFunc;
            this.Condition = condition;
        }

        /// <summary>
        /// Conditions for categories.
        /// </summary>
        public enum AppearCondition
        {
            /// <summary>
            /// Check no conditions.
            /// </summary>
            None,

            /// <summary>
            /// Check if plugin testing is enabled.
            /// </summary>
            DoPluginTest,

            /// <summary>
            /// Check if there are any hidden plugins.
            /// </summary>
            AnyHiddenPlugins,
        }

        /// <summary>
        /// Gets or sets the condition to be checked when rendering.
        /// </summary>
        public AppearCondition Condition { get; set; }

        /// <summary>
        /// Gets the name of category.
        /// </summary>
        public string Name => this.nameFunc();
    }

    /// <summary>
    /// Plugin installer UI group, a container for categories.
    /// </summary>
    public struct GroupInfo
    {
        /// <summary>
        /// Type of group.
        /// </summary>
        public GroupKind GroupKind;

        /// <summary>
        /// List of categories in container.
        /// </summary>
        public List<CategoryKind> Categories;

        private Func<string> nameFunc;

        /// <summary>
        /// Initializes a new instance of the <see cref="GroupInfo"/> struct.
        /// </summary>
        /// <param name="groupKind">Type of group.</param>
        /// <param name="nameFunc">Function returning localized name of category.</param>
        /// <param name="categories">List of category Ids to hardcode.</param>
        public GroupInfo(GroupKind groupKind, Func<string> nameFunc, params CategoryKind[] categories)
        {
            this.GroupKind = groupKind;
            this.nameFunc = nameFunc;

            this.Categories = new();
            this.Categories.AddRange(categories);
        }

        /// <summary>
        /// Gets the name of UI group.
        /// </summary>
        public string Name => this.nameFunc();
    }

    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:Elements should be documented", Justification = "locs")]
    internal static class Locs
    {
        #region UI groups

        public static string Group_MainMenu => Loc.Localize("MainWindowMainMenu", "Main Menu");

        public static string Group_Commands => Loc.Localize("MainWindowCommands", "Commands");

        public static string Group_Features => Loc.Localize("MainWindowFeatures", "AutoMation");

        public static string Group_Tasks => Loc.Localize("MainWindowTasks", "Tasks");

        #endregion

        #region Categories

        public static string Category_All => Loc.Localize("MainWindowCategoryAll", "All");

        public static string Category_About => Loc.Localize("MainWindowCategoryAbout", "Character");

        public static string Category_General => Loc.Localize("MainWindowCategoryGeneral", "General");

        public static string Category_QualityOfLife => Loc.Localize("MainWindowCategoryQualityOfLife", "QoL");

        public static string Category_Shortcuts => Loc.Localize("MainWindowCatagoryShortcuts", "Shortcuts");

        public static string Category_Character => "MainWindowCatagory/Character Character";

        public static string Category_Automation => Loc.Localize("MainWindowCategoryAutomation", "Automation");

        public static string Category_Activities => Loc.Localize("MainWindowCategoryActivities", "Activities");

        #endregion
    }

}
