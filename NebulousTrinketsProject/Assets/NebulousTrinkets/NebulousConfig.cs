using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BepInEx;
using Moonstorm.Loaders;

namespace NebulousTrinkets
{
    public class NebulousConfig : ConfigLoader<NebulousConfig>
    {
        public override BaseUnityPlugin MainClass => NebulousMain.Instance;

        public override bool CreateSubFolder => true;
    }
}