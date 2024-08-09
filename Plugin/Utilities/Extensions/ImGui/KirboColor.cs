using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lumina.Excel.GeneratedSheets;
using Plugin.Utilities;

namespace ImGuiExtensions;

internal struct KirboColor
{ 
    public float R { get; set; }
    public float G { get; set; }
    public float B { get; set; }
    public float A { get; set; }

    public KirboColor()
    {
    }

    public KirboColor(float r, float g, float b, float a = 1)
    {
        R = r;
        G = g;
        B = b;
        A = a;
    }

    public KirboColor(Vector4 vec) : this(vec.X, vec.Y, vec.Z, vec.W)
    {
    }

    public KirboColor(uint col) : this(ImGui.ColorConvertU32ToFloat4(col))
    {
    }

    public readonly KirboColor WithRed(float r)
        => new(r, G, B, A);

    public readonly KirboColor WithGreen(float g)
        => new(R, g, B, A);

    public readonly KirboColor WithBlue(float b)
        => new(R, G, b, A);

    public readonly KirboColor WithAlpha(float a)
        => new(R, G, B, a);

    public static KirboColor From(float r, float g, float b, float a = 1)
        => new() { R = r, G = g, B = b, A = a };

    public static KirboColor From(Vector4 vec)
        => From(vec.X, vec.Y, vec.Z, vec.W);

    public static KirboColor From(uint col)
        => From(ImGui.ColorConvertU32ToFloat4(col));

    public static KirboColor From(ImGuiCol col)
        => From(ImGui.GetColorU32(col));

    public static KirboColor FromABGR(uint abgr)
        => From(abgr.Reverse());

    public static KirboColor FromUiForeground(uint id)
        => FromABGR(Excel.GetRow<UIColor>(id)!.UIForeground);

    public static KirboColor FromUiGlow(uint id)
        => FromABGR(Excel.GetRow<UIColor>(id)!.UIGlow);

    public static KirboColor FromStain(uint id)
        => From(Excel.GetRow<Stain>(id)!.Color.Reverse() >> 8).WithAlpha(1);

    public static implicit operator Vector4(KirboColor col)
        => new(col.R, col.G, col.B, col.A);

    public static implicit operator uint(KirboColor col)
        => ImGui.ColorConvertFloat4ToU32(col);
}
