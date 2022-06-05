using Moonstorm;
using RoR2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace NebulousTrinkets.Items
{
    public class EquipmentCooldownIncrease : ItemBase
    {
        public override ItemDef ItemDef => NebulousAssets.LoadAsset<ItemDef>("EquipmentCooldownIncrease");

        public override void Initialize()
        {
            On.RoR2.Inventory.CalculateEquipmentCooldownScale += Inventory_CalculateEquipmentCooldownScale;
        }

        private float Inventory_CalculateEquipmentCooldownScale(On.RoR2.Inventory.orig_CalculateEquipmentCooldownScale orig, Inventory self)
        {
            if(self.)
        }
    }
}
