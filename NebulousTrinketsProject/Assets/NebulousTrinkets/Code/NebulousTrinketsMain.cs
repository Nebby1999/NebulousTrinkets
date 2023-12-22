using BepInEx;
using R2API;
using R2API.Networking;
using System;
using UnityEngine;
using System.Security.Permissions;
using System.Security;
using UObject = UnityEngine.Object;
using MSU;

[assembly: HG.Reflection.SearchableAttribute.OptIn]
#pragma warning disable CS0618
[assembly: SecurityPermission(SecurityAction.RequestMinimum, SkipVerification = true)]
#pragma warning restore CS0618
[module: UnverifiableCode]

namespace NebulousTrinkets
{
    [BepInDependency("com.TeamMoonstorm.MSU", BepInDependency.DependencyFlags.HardDependency)]
    [BepInPlugin(GUID, MODNAME, VERSION)]
    public class NebulousTrinketsMain : BaseUnityPlugin
    {
        public const string GUID = "com.Nebby.NebulousTrinkets";
        public const string MODNAME = "Nebulous Trinkets";
        public const string VERSION = "0.0.1";
        internal static NebulousTrinketsMain Instance { get; private set; }
        internal static PluginInfo PluginInfo { get; private set;}

        private void Awake()
        {
            Instance = this;
            PluginInfo = Info;
            new NebulousTrinketsLogger(Logger);
            new NebulousTrinketsContent();
        }
    }
}
