using MSU;
using RoR2;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NebulousTrinkets.Equipments
{
    public sealed class BulwarksRing : IEquipmentContentPiece
    {
        public NullableRef<GameObject> ItemDisplayPrefab => null;

        public EquipmentDef Asset => _asset;
        private EquipmentDef _asset;

        public bool Execute(EquipmentSlot slot)
        {
            return false;
        }

        public void Initialize()
        {
            On.RoR2.GenericPickupController.AttemptGrant += BlockGrant;
            On.RoR2.GenericPickupController.GetContextString += ChangeMessage;
            On.RoR2.PurchaseInteraction.GetInteractability += GetInteractability;
            R2API.RecalculateStatsAPI.GetStatCoefficients += StatIncrease;
        }

        public bool IsAvailable()
        {
            return true;
        }

        public IEnumerator LoadContentAsync()
        {
            var request = new NebulousTrinketsAssets.SingleBundleAssetRequest<EquipmentDef>("BulwarksRing", NebulousTrinketsBundle.Equipments);
            yield return request.Load();
            _asset = request.Asset;
        }

        public void OnEquipmentObtained(CharacterBody body)
        {
        }


        private void StatIncrease(CharacterBody sender, R2API.RecalculateStatsAPI.StatHookEventArgs args)
        {
            var slot = sender.equipmentSlot;
            if (!slot)
                return;

            if (slot.equipmentIndex != Asset.equipmentIndex)
                return;

            args.attackSpeedMultAdd += 0.2f;
            args.cooldownMultAdd -= 0.1f;
            args.critDamageMultAdd += 0.2f;
            args.damageMultAdd += 0.2f;
            args.healthMultAdd += 0.2f;
            args.moveSpeedMultAdd += 0.2f;
            args.regenMultAdd += 0.2f;
        }

        private Interactability GetInteractability(On.RoR2.PurchaseInteraction.orig_GetInteractability orig, PurchaseInteraction self, Interactor activator)
        {
            Interactability vanilla = orig(self, activator);

            if (self.costType != CostTypeIndex.Equipment)
                return vanilla;

            if(activator.TryGetComponent<CharacterBody>(out var body) && body.equipmentSlot && body.equipmentSlot.equipmentIndex == Asset.equipmentIndex)
            {
                return Interactability.ConditionsNotMet;
            }
            return vanilla; ;
        }

        private string ChangeMessage(On.RoR2.GenericPickupController.orig_GetContextString orig, GenericPickupController self, Interactor activator)
        {
            string vanillaMessage = orig(self, activator);
            PickupDef pickupDef = PickupCatalog.GetPickupDef(self.pickupIndex);

            if (pickupDef.equipmentIndex == EquipmentIndex.None)
                return vanillaMessage;

            if (activator.TryGetComponent<CharacterBody>(out var body) && body.equipmentSlot && body.equipmentSlot.equipmentIndex == Asset.equipmentIndex)
            {
                return Language.GetString("NEBBY_EQUIP_BULWARKSRING_DENIED");
            }
            return vanillaMessage;
        }

        private void BlockGrant(On.RoR2.GenericPickupController.orig_AttemptGrant orig, GenericPickupController self, CharacterBody body)
        {
            PickupDef pickupDef = PickupCatalog.GetPickupDef(self.pickupIndex);

            if(pickupDef.equipmentIndex == EquipmentIndex.None)
            {
                orig(self, body);
                return;
            }

            if(body.equipmentSlot && body.equipmentSlot.equipmentIndex == Asset.equipmentIndex)
            {
                return;
            }

            orig(self, body);
        }
    }
}
