using System.Numerics;

namespace Plugin.Utility.UI;

/// <summary>
/// A set of fancy color for use in plugins.
/// </summary>
public static class Colours
{
    /// <summary>
    /// Converts RGB color to <see cref="Vector4"/> for ImGui
    /// </summary>
    /// <param name="col">Color in format 0xRRGGBB</param>
    /// <param name="alpha">Optional transparency value between 0 and 1</param>
    /// <returns>Color in <see cref="Vector4"/> format ready to be used with <see cref="ImGui"/> functions</returns>
    public unsafe static Vector4 Vector4FromRGB(uint col, float alpha = 1.0f)
    {
        byte* bytes = (byte*)&col;
        return new Vector4((float)bytes[2] / 255f, (float)bytes[1] / 255f, (float)bytes[0] / 255f, alpha);
    }

    // Color Collection
    public readonly static Vector4 Red = Vector4FromRGB(0xAA0000);
    public readonly static Vector4 RedBright = Vector4FromRGB(0xFF0000);
    public readonly static Vector4 Peach = Vector4FromRGB(0xFF6E59);
    public readonly static Vector4 Maroon = Vector4FromRGB(0x800000);
    public readonly static Vector4 Orange = Vector4FromRGB(0xAA5400);
    public readonly static Vector4 DalamudOrangeVector = Vector4FromRGB(0xFFB500, 1);
    public readonly static Vector4 Bronze = Vector4FromRGB(0xCD7F32);
    public readonly static Vector4 Indigo = Vector4FromRGB(0x4B0082);
    public readonly static Vector4 DarkType = Vector4FromRGB(0x5e1196);
    public readonly static Vector4 GhostType = Vector4FromRGB(0xFF6E59FF);
    public readonly static Vector4 BrightGhostType = Vector4FromRGB(0xab57ff);
    public readonly static Vector4 Puurple = Vector4FromRGB(0x9E35FF, 1);
    public readonly static Vector4 Violet = Vector4FromRGB(0xAA00AA);
    public readonly static Vector4 Purple = Vector4FromRGB(0xAA0058);
    public readonly static Vector4 Fuchsia = Vector4FromRGB(0xAD0066);
    public readonly static Vector4 VioletBright = Vector4FromRGB(0xFF00FF);
    public readonly static Vector4 PastelPurple = Vector4FromRGB(0xd557ff);
    public readonly static Vector4 Pink = Vector4FromRGB(0xFF6FFF);
    public readonly static Vector4 PinkLight = Vector4FromRGB(0xFFABD6);
    public readonly static Vector4 Blue = Vector4FromRGB(0x0000AA);
    public readonly static Vector4 BlueBright = Vector4FromRGB(0x0000FF);
    public readonly static Vector4 BlueSea = Vector4FromRGB(0x0058AA);
    public readonly static Vector4 BlueSky = Vector4FromRGB(0x0085FF);
    public readonly static Vector4 Cyan = Vector4FromRGB(0x00AAAA);
    public readonly static Vector4 CyanBright = Vector4FromRGB(0x00FFFF);
    public readonly static Vector4 LightBlue = Vector4FromRGB(0xADD8E6);
    public readonly static Vector4 Lavender = Vector4FromRGB(0xE6E6FA);
    public readonly static Vector4 Green = Vector4FromRGB(0x00AA00);
    public readonly static Vector4 Olive = Vector4FromRGB(0x808000);
    public readonly static Vector4 GreenBright = Vector4FromRGB(0x00FF00);
    public readonly static Vector4 GreenLight = Vector4FromRGB(0xCCFF99);
    public readonly static Vector4 Yellow = Vector4FromRGB(0xAAAA00);
    public readonly static Vector4 Gold = Vector4FromRGB(0xFFD700);
    public readonly static Vector4 YellowBright = Vector4FromRGB(0xFFFF00);
    public readonly static Vector4 Lemon = Vector4FromRGB(0xFFFF00);
    public readonly static Vector4 Black = Vector4FromRGB(0x000000);
    public readonly static Vector4 Silver = Vector4FromRGB(0xC0C0C0);
    public readonly static Vector4 White = Vector4FromRGB(0xFFFFFF);

    // Plugin options
    public readonly static Vector4 TextSelected = Vector4FromRGB(0xFFFFFF);
    public readonly static Vector4 TextNotSelected = Vector4FromRGB(0xC0C0C0);
    public readonly static Vector4 TextHovered =  Vector4FromRGB(0xFFFFFF, 0.2f);
    public readonly static Vector4 Transparent = Vector4FromRGB(0xFFFFFF, 0.0f);
    public readonly static Vector4 SelectableSelected = Vector4FromRGB(0x333333, 0.9f);
    public readonly static Vector4 ButtonActive = Vector4FromRGB(0x4D4D4D, 0.3f);

    // Dalamud UI Colors
    public readonly static Vector4 DalamudRed = new(1f, 0f, 0f, 1f);
    public readonly static Vector4 DalamudGrey = new(0.7f, 0.7f, 0.7f, 1f);
    public readonly static Vector4 DalamudGrey2 = new(0.7f, 0.7f, 0.7f, 1f);
    public readonly static Vector4 DalamudGrey3 = new(0.5f, 0.5f, 0.5f, 1f);
    public readonly static Vector4 DalamudWhite = new(1f, 1f, 1f, 1f);
    public readonly static Vector4 DalamudWhite2 = new(0.878f, 0.878f, 0.878f, 1f);
    public readonly static Vector4 DalamudOrange = new(1f, 0.709f, 0f, 1f);
    public readonly static Vector4 DalamudYellow = new(1f, 1f, 0.4f, 1f);
    public readonly static Vector4 DalamudViolet = new(0.77f, 0.7f, 0.965f, 1f);

    // Job Role Colors
    public readonly static Vector4 TankBlue = new(0f, 0.6f, 1f, 1f);
    public readonly static Vector4 HealerGreen = new(0f, 0.8f, 0.1333333f, 1f);
    public readonly static Vector4 DPSRed = new(0.7058824f, 0f, 0f, 1f);

    // FFLogs Parse Colors
    public readonly static Vector4 ParsedGrey = new(0.4f, 0.4f, 0.4f, 1f);
    public readonly static Vector4 ParsedGreen = new(0.117f, 1f, 0f, 1f);
    public readonly static Vector4 ParsedBlue = new(0f, 0.439f, 1f, 1f);
    public readonly static Vector4 ParsedPurple = new(0.639f, 0.207f, 0.933f, 1f);
    public readonly static Vector4 ParsedOrange = new(1f, 0.501f, 0f, 1f);
    public readonly static Vector4 ParsedPink = new(0.886f, 0.407f, 0.658f, 1f);
    public readonly static Vector4 ParsedGold = new(0.898f, 0.8f, 0.501f, 1f);

}
