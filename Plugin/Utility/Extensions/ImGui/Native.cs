﻿using System.Runtime.InteropServices;

namespace ImGuiExtensions;

[StructLayout(LayoutKind.Explicit)]
public struct ImGuiWindow
{
    [FieldOffset(0xC)] public ImGuiWindowFlags Flags;

    [FieldOffset(0xD5)] public byte HasCloseButton;

    // 0x118 is the start of ImGuiWindowTempData
    [FieldOffset(0x130)] public Vector2 CursorMaxPos;
}

public static partial class Native
{
    [LibraryImport("cimgui")]
    [UnmanagedCallConv(CallConvs = [ typeof(System.Runtime.CompilerServices.CallConvCdecl) ])]
    private static partial nint igGetCurrentWindow();
    public static unsafe ImGuiWindow* GetCurrentWindow() => (ImGuiWindow*)igGetCurrentWindow();
    public static unsafe ImGuiWindowFlags GetCurrentWindowFlags() => GetCurrentWindow()->Flags;
    public static unsafe bool CurrentWindowHasCloseButton() => GetCurrentWindow()->HasCloseButton != 0;
}