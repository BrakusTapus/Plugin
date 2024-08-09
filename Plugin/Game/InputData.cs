namespace Plugin.Game;

[System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Explicit, Size = 0x40)]
internal unsafe struct InputData
{
    [System.Runtime.InteropServices.FieldOffset(0)] internal fixed byte RawDump[0x40];
    [System.Runtime.InteropServices.FieldOffset(8)] internal nint* unk_8;
    [System.Runtime.InteropServices.FieldOffset(16)] internal int unk_16;
    [System.Runtime.InteropServices.FieldOffset(24)] internal byte unk_24;

    internal UnknownStruct* unk_8s => (UnknownStruct*)*unk_8;

    internal readonly Span<byte> RawDumpSpan
    {
        get
        {
            fixed(byte* ptr = RawDump)
            {
                return new Span<byte>(ptr, sizeof(InputData));
            }
        }
    }
}
