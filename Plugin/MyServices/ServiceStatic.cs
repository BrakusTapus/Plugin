using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Plugin.Services;

namespace Plugin.MyServices;
public static class ServiceStatic
{
    //public static SelectWorldWindow SelectWorldWindow { get; private set; }
    public static TeleportService TeleportService { get; private set; }
    public static InstanceHandler InstanceHandler { get; private set; }
}
