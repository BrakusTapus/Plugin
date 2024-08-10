using System;
using System.Collections.Generic;
using System.Linq;
using Plugin.Windows;

public class CategoryManager
{
    public enum GroupKind
    {
        About,
        Features,
        Automation,
        Commands,
        Links
    }

    public enum CategoryKind
    {
        About,
        Info,
        General,
        QoL,
        Combat,
        Hunts,
        Retainers,
        Misc,
        Teleports,
        Savage,
        Ultimates
    }

    public class GroupInfo
    {
        public GroupKind GroupKind { get; set; }
        public string Name { get; set; }
        public List<CategoryKind> Categories { get; set; }
    }

    public class CategoryInfo
    {
        public CategoryKind CategoryKind { get; set; }
        public string Name { get; set; }
    }

    public GroupKind CurrentGroupKind { get; set; }
    public CategoryKind CurrentCategoryKind { get; set; }
    public List<GroupInfo> GroupList { get; private set; }
    public List<CategoryInfo> CategoryList { get; private set; }

    public CategoryManager()
    {
        GroupList = new List<GroupInfo>();
        CategoryList = new List<CategoryInfo>();
        InitializeCategories();
    }

    private void InitializeCategories()
    {
        // Define all categories and their respective groups
        CategoryList = new List<CategoryInfo>
        {
            // About Categories
            new CategoryInfo { CategoryKind = CategoryKind.About, Name = "About" },
            new CategoryInfo { CategoryKind = CategoryKind.Info, Name = "Info" },

            // Feature Categories
            new CategoryInfo { CategoryKind = CategoryKind.General, Name = "General" },
            new CategoryInfo { CategoryKind = CategoryKind.QoL, Name = "QoL" },
            new CategoryInfo { CategoryKind = CategoryKind.Combat, Name = "Combat" },

            // Automation Categories
            new CategoryInfo { CategoryKind = CategoryKind.Hunts, Name = "Hunts" },
            new CategoryInfo { CategoryKind = CategoryKind.Retainers, Name = "Retainers" },
            new CategoryInfo { CategoryKind = CategoryKind.Misc, Name = "Misc" },

            // Command Categories
            new CategoryInfo { CategoryKind = CategoryKind.Teleports, Name = "Teleports" },

            // Links Categories
            new CategoryInfo { CategoryKind = CategoryKind.General, Name = "General" },
            new CategoryInfo { CategoryKind = CategoryKind.Savage, Name = "Savage" },
            new CategoryInfo { CategoryKind = CategoryKind.Ultimates, Name = "Ultimates" }
        };

        GroupList = new List<GroupInfo>
        {
            new GroupInfo
            {
                GroupKind = GroupKind.About,
                Name = "About",
                Categories = new List<CategoryKind>
                {
                    CategoryKind.About,
                    CategoryKind.Info
                }
            },
            new GroupInfo
            {
                GroupKind = GroupKind.Features,
                Name = "AutoMation",
                Categories = new List<CategoryKind>
                {
                    CategoryKind.General,
                    CategoryKind.QoL,
                    CategoryKind.Combat
                }
            },
            new GroupInfo
            {
                GroupKind = GroupKind.Automation,
                Name = "Automation",
                Categories = new List<CategoryKind>
                {
                    CategoryKind.Hunts,
                    CategoryKind.Retainers,
                    CategoryKind.Misc
                }
            },
            new GroupInfo
            {
                GroupKind = GroupKind.Commands,
                Name = "Commands",
                Categories = new List<CategoryKind>
                {
                    CategoryKind.Teleports
                }
            },
            new GroupInfo
            {
                GroupKind = GroupKind.Links,
                Name = "Links",
                Categories = new List<CategoryKind>
                {
                    CategoryKind.General,
                    CategoryKind.Savage,
                    CategoryKind.Ultimates
                }
            }
        };
    }

    internal GroupInfo GetGroupInfoByHeader(CategoryTabHeaders header)
    {
        return GroupList.FirstOrDefault(g => g.GroupKind.ToString() == header.ToString());
    }

    public CategoryInfo GetCategoryInfo(CategoryKind categoryKind)
    {
        return CategoryList.FirstOrDefault(c => c.CategoryKind == categoryKind);
    }

    public bool IsSelectionValid => CategoryList.Any(c => c.CategoryKind == CurrentCategoryKind);

    public void ResetContentDirty()
    {
        // Implement logic to reset the content dirty flag if needed
    }
}