using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BepInEx;
using BepInEx.Configuration;
using Moonstorm.Loaders;

namespace NebulousTrinkets
{
    public class NebulousConfig : ConfigLoader<NebulousConfig>
    {
        public const string items = "NT.Items";
        public const string equips = "NT.Equips";

        internal static ConfigFile itemConfig;
        internal static ConfigFile equipsConfig;
        public override BaseUnityPlugin MainClass => NebulousMain.Instance;
        public override bool CreateSubFolder => true;

        internal void Init()
        {
            itemConfig = CreateConfigFile(items);
            equipsConfig = CreateConfigFile(equips);
        }
    }
}