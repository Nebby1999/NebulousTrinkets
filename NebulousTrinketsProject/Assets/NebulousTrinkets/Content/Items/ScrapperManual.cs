using Moonstorm;
using RoR2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NebulousTrinkets.Items
{
    public class ScrapperManual : ItemBase
    {
        public override ItemDef ItemDef => NebulousAssets.LoadAsset<ItemDef>("ScrapperManual");
        public override void Initialize()
        {

        }
    }
}
