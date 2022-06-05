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
    public sealed class BrotherRing : EquipmentBase
    {
        public override EquipmentDef EquipmentDef => NebulousAssets.LoadAsset<EquipmentDef>(nameof(BrotherRing));

        public override void Initialize()
        {
            On.RoR2.GenericPickupController.AttemptGrant += BlockGrant;
            On.RoR2.GenericPickupController.GetContextString += ChangeMessage;
            On.RoR2.PurchaseInteraction.GetInteractability += GetInteractability;
            R2API.RecalculateStatsAPI.GetStatCoefficients += StatIncrease;
        }

        private Interactability GetInteractability(On.RoR2.PurchaseInteraction.orig_GetInteractability orig, PurchaseInteraction self, Interactor activator)
        {
            Interactability vanilla = orig(self, activator);
            //Cost is not equipment? return vanilla
            //Anything set to lunaritem/equipment can be used to remove the ring
            if(self.costType != CostTypeIndex.Equipment)
            {
                return vanilla;
            }
            var charBody = activator.GetComponent<CharacterBody>();
            //Body has the ring? interactable is disabled.
            if(charBody && charBody.equipmentSlot && charBody.equipmentSlot.equipmentIndex == EquipmentDef.equipmentIndex)
            {
                return Interactability.ConditionsNotMet;
            }
            //Otherwise, return vanilla.
            return vanilla;
        }

        private string ChangeMessage(On.RoR2.GenericPickupController.orig_GetContextString orig, GenericPickupController self, Interactor activator)
        {
            string vanillaMessage = orig(self, activator);
            PickupDef pickupDef = PickupCatalog.GetPickupDef(self.pickupIndex);

            //Not an equip? display vanilla
            if (pickupDef.equipmentIndex == EquipmentIndex.None)
                return vanillaMessage;

            //Equip, and body has the ring? display custom message
            var charBody = activator.GetComponent<CharacterBody>();
            if(charBody && charBody.equipmentSlot && charBody.equipmentSlot.equipmentIndex == EquipmentDef.equipmentIndex)
            {
                return Language.GetString($"NT_EQUIP_BROTHERRING_DENIED");
            }
            //Equip but no ring? display vanilla.
            return vanillaMessage;
        }

        private void StatIncrease(CharacterBody sender, R2API.RecalculateStatsAPI.StatHookEventArgs args)
        {
            if (sender && sender.inventory && sender.inventory.currentEquipmentIndex == EquipmentDef.equipmentIndex)
            {
                args.attackSpeedMultAdd += 0.1f;
                args.cooldownMultAdd += 0.1f;
                args.critDamageMultAdd += 0.1f;
                args.damageMultAdd += 0.1f;
                args.healthMultAdd += 0.1f;
                args.jumpPowerMultAdd += 0.1f;
                args.moveSpeedMultAdd += 0.1f;
                args.regenMultAdd += 0.1f;
                args.shieldMultAdd += 0.1f;
                args.critAdd += 10f;
                args.armorAdd += sender.baseArmor * 0.1f;
            }
        }

        private void BlockGrant(On.RoR2.GenericPickupController.orig_AttemptGrant orig, GenericPickupController self, CharacterBody body)
        {
            PickupDef pickupDef = PickupCatalog.GetPickupDef(self.pickupIndex);
            //If the droplet is not an equipment droplet, just continue.
            if (pickupDef.equipmentIndex == EquipmentIndex.None)
            {
                orig(self, body);
            }
            //Else, the droplet is an equipment, check if body has the ring as its equipment.
            if(body.equipmentSlot && body.equipmentSlot.equipmentIndex == EquipmentDef.equipmentIndex)
            {
                //Body has the ring, return.
                return;
            }
            //The droplet is an equipment, but the body does not have the ring, grant anyways.
            orig(self, body);
        }

        public override bool FireAction(EquipmentSlot slot)
        {
            return false;
        }
    }
}