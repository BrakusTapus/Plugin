using Dalamud.Configuration;
using Dalamud.Plugin;
using System;

namespace Plugin.Configurations;

[Serializable]
public class Configs : IPluginConfiguration
{
    public int Version { get; set; } = 0;

    public bool IsConfigWindowMovable { get; set; } = true;
    public bool IsConfigWindowResizeable { get; set; } = true;
    public bool IsConfigWindowNoTitleBar { get; set; } = true;
    public bool IsConfigNoWindowScrollbar { get; set; } = true;
    public bool IsConfigWindowNoScrollWithMouse { get; set; } = true;
    public bool IsConfigWindowNoCollapseable { get; set; } = true;
    public bool IsConfigWindowNoBackground { get; set; } = true;

    public bool SomePropertyToBeSavedAndWithADefault { get; set; } = true;

    public void Save()
    {
        Services.PluginInterface.SavePluginConfig(this);
        Services.PluginLog.Debug("Config Saved!");
    }
}
