using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moonstorm;
using RoR2;
using UnityEngine;

namespace NebulousTrinkets.Equipments
{
    public class BrotherRing : EquipmentBase
    {
        public override EquipmentDef EquipmentDef => throw new NotImplementedException();

        public override void Initialize()
        {
            On.RoR2.GenericPickupController.AttemptGrant += BlockGrant;
            R2API.RecalculateStatsAPI.GetStatCoefficients += StatIncrease;
        }

        private void StatIncrease(CharacterBody sender, R2API.RecalculateStatsAPI.StatHookEventArgs args)
        {
            if(sender.inventory.currentEquipmentIndex == EquipmentDef.equipmentIndex)
            {
                args.levelMultAdd += 0.10f;
            }
        }

        private void BlockGrant(On.RoR2.GenericPickupController.orig_AttemptGrant orig, GenericPickupController self, CharacterBody body)
        {
            PickupDef pickupDef = PickupCatalog.GetPickupDef(self.pickupIndex);
            if(pickupDef.equipmentIndex != EquipmentIndex.None && pickupDef.equipmentIndex == EquipmentDef.equipmentIndex)
            {
                Debug.Log($"Cockblocked");
                return;
            }
            orig(self, body);
        }

        public override bool FireAction(EquipmentSlot slot)
        {
            return false;
        }
    }
}