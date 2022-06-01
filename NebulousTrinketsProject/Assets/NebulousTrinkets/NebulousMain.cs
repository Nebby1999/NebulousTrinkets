using BepInEx;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NebulousTrinkets
{
    [BepInDependency("com.TeamMoonstorm.MoonstormSharedUtils", BepInDependency.DependencyFlags.HardDependency)]
    [BepInPlugin(GUID, ModName, Version)]
    public class NebulousMain : BaseUnityPlugin
    {
        public const string ModName = "Nebulous Trinkets";
        public const string Version = "0.0.1";
        public const string GUID = "com.Nebby.NebulousTrinkets";

        public static NebulousMain Instance { get; private set; }
        private void Awake()
        {
            Instance = this;

        }
    }
}
