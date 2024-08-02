//TODO: If migrating to Ecommons EzConfig is succesfol then this can be removed

//using Dalamud.Configuration;
//using Dalamud.Plugin;
//using ECommons.Configuration;
//using Plugin.MyServices;
//using System;

//namespace Plugin.Configurations;

//[Serializable]
//public class Configs : IEzConfig
//{
//    private readonly Plugin plugin;

//    public Configs(Plugin plugin)
//    {
//        this.plugin = plugin;
//    }

//    public int Version { get; set; } = 1;

//    // Main window options
//    public bool IsMainWindowMovable { get; set; } = true;
//    public bool IsMainWindowResizeable { get; set; } = true;
//    public bool IsMainWindowNoTitleBar { get; set; } = true;
//    public bool IsMainNoWindowScrollbar { get; set; } = true;
//    public bool IsMainWindowNoScrollWithMouse { get; set; } = true;
//    public bool IsMainWindowNoCollapseable { get; set; } = true;
//    public bool IsMainWindowNoBackground { get; set; } = true;

//    // EzConfigs window options
//    public bool IsConfigWindowMovable { get; set; } = true;
//    public bool IsConfigWindowResizeable { get; set; } = true;
//    public bool IsConfigWindowNoTitleBar { get; set; } = true;
//    public bool IsConfigNoWindowScrollbar { get; set; } = true;
//    public bool IsConfigWindowNoScrollWithMouse { get; set; } = true;
//    public bool IsConfigWindowNoCollapseable { get; set; } = true;
//    public bool IsConfigWindowNoBackground { get; set; } = true;
//    public bool ShowToolTips { get; set; } = true;

//    public bool SomePropertyToBeSavedAndWithADefault { get; set; } = true;

//    //TODO: If migrating to Ecommons EzConfig is succesfol then this can be removed
//    /// <summary>
//    /// Easy way of saving!
//    /// </summary>
//    //public void Save()  
//    //{
//    //    MyServices.Services.PluginInterface.SavePluginConfig(this);
//    //    MyServices.Services.PluginLog.Debug("Plugin configs saved!");
//    //}

//    //public void ToggleConfigWindow()
//    //{
//    //    Plugin.ToggleConfigWindow();
//    //}

//    //public void ToggleMainWindow() => plugin.ToggleMainWindow();

//    public bool InstanceSwitcherRepeat = true;
//    public bool EnableFlydownInstance = true;
//    public static bool UseFrameDelay = true;
//    public int Delay = 200;
//    public int FrameDelay = 8;

//}
