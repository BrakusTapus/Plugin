using ECommons.EzIpcManager;
#nullable disable

namespace Plugin.IPC
{
    internal class IPCProvider
    {
        internal IPCProvider()
        {
            EzIPC.Init(this);
        }

        [EzIPC] public void Run(uint territoryType, int loops) => Plugin.P.Run(territoryType, loops);
        [EzIPC] public void Start(bool startFromZero = true) => Plugin.P.StartNavigation(startFromZero);
        [EzIPC] public void Stop() => Plugin.P.StopAndResetALL();
    }
}
