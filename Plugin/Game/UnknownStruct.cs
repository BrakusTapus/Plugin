namespace Plugin.Game;

[System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Explicit, Size = 0x40)]
internal unsafe struct UnknownStruct
{
    [System.Runtime.InteropServices.FieldOffset(4)] public byte unk_4;
    [System.Runtime.InteropServices.FieldOffset(8)] public int SelectedItem;
}
