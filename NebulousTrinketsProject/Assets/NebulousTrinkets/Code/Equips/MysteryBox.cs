using MSU;
using R2API;
using R2API.ScriptableObjects;
using RoR2;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace NebulousTrinkets.Equipments
{
    /*
     * 1st Stage: White
     * 2nd Stage: White
     * 3rd Stage: Green
     * 4th Stage: Green
     * 5th Stage: Red
     * Commencement: Lunar
     * Moment Fractured: Beads
     * Moment Whole: Nothing
     * Bazaar: 2 Lunar Coins
     * Bulwark's Ambry: Artifact Shard
     * Gilded Coast: Enough money for everything
     * Void Fields: T1 Potential
     * Void Locus: T2 Potential
     * The Planetarium: T3 Potential
     */
    public sealed class MysteryBox : IEquipmentContentPiece, IContentPackModifier
    {
        public NullableRef<GameObject> ItemDisplayPrefab => null;

        public EquipmentDef Asset => _asset;
        private EquipmentDef _asset;
        private EquipmentDef _mysteryBoxConsumed;
        private GameObject _potentialPrefab;

        public bool Execute(EquipmentSlot slot)
        {
            Run run = Run.instance;

            if (!run)
                return false;

            if(run is InfiniteTowerRun itr)
            {
                return ExecuteInfiniteTowerRun(slot);
            }
            else
            {
                return ExecuteNormal(slot);
            }
        }

        private bool ExecuteNormal(EquipmentSlot slot)
        {
            Stage stage = Stage.instance;
            if (!stage)
                return false;

            SceneDef def = stage.sceneDef;

            switch (def.stageOrder)
            {
                case 1:
                case 2:
                    if (SpawnItem(ItemTier.Tier1, slot))
                    {
                        ReplaceEquipment(slot);
                        return true;
                    }
                    return false;
                case 3:
                case 4:
                    if (SpawnItem(ItemTier.Tier2, slot))
                    {
                        ReplaceEquipment(slot);
                        return true;
                    }
                    return false;
                case 5:
                    if (SpawnItem(ItemTier.Tier3, slot))
                    {
                        ReplaceEquipment(slot);
                        return true;
                    }
                    return false;
                default:
                    break;
            }

            DirectorAPI.StageInfo stageInfo = DirectorAPI.StageInfo.ParseInternalStageName(def.baseSceneName);

            switch (stageInfo.stage)
            {
                case DirectorAPI.Stage.Commencement:
                    if (SpawnItem(ItemTier.Lunar, slot))
                    {
                        ReplaceEquipment(slot);
                        return true;
                    }
                    return false;
                case DirectorAPI.Stage.MomentFractured:
                    if (SpawnItem(RoR2Content.Items.LunarTrinket, slot))
                    {
                        ReplaceEquipment(slot);
                        return true;
                    }
                    return false;
                case DirectorAPI.Stage.MomentWhole:
                    ReplaceEquipment(slot);
                    return true;
                case DirectorAPI.Stage.Bazaar:
                    if (SpawnLunarCoins(6, slot))
                    {
                        ReplaceEquipment(slot);
                        return true;
                    }
                    return false;
                case DirectorAPI.Stage.ArtifactReliquary:
                    if (SpawnItem(RoR2Content.Items.ArtifactKey, slot))
                    {
                        ReplaceEquipment(slot);
                        return true;
                    }
                    return false;
                case DirectorAPI.Stage.GildedCoast:
                    if(GoldenCoast(slot))
                    {
                        ReplaceEquipment(slot);
                        return true;
                    }
                    return false;
                case DirectorAPI.Stage.VoidCell:
                    if (SpawnPotential(ItemTier.Tier1, slot))
                    {
                        ReplaceEquipment(slot);
                        return true;
                    }
                    return false;
                case DirectorAPI.Stage.VoidLocus:
                    if (SpawnPotential(ItemTier.Tier2, slot))
                    {
                        ReplaceEquipment(slot);
                        return true;
                    }
                    return false;
                case DirectorAPI.Stage.ThePlanetarium:
                    if (SpawnPotential(ItemTier.Tier3, slot))
                    {
                        ReplaceEquipment(slot);
                        return true;
                    }
                    return false;
                default:
                    return true;
            }
        }

        private bool SpawnItem(ItemTier itemTier,  EquipmentSlot slot)
        {
            PickupIndex index = PickupIndex.none;
            switch(itemTier)
            {
                case ItemTier.Tier1:
                    index = Run.instance.treasureRng.NextElementUniform(Run.instance.availableTier1DropList);
                    break;
                case ItemTier.Tier2:
                    index = Run.instance.treasureRng.NextElementUniform(Run.instance.availableTier2DropList);
                    break;
                case ItemTier.Tier3:
                    index = Run.instance.treasureRng.NextElementUniform(Run.instance.availableTier3DropList);
                    break;
                case ItemTier.Lunar:
                    index = Run.instance.treasureRng.NextElementUniform(Run.instance.availableLunarCombinedDropList);
                    break;
            }

            if (index == PickupIndex.none)
                return false;

            PickupDropletController.CreatePickupDroplet(new GenericPickupController.CreatePickupInfo
            {
                pickupIndex = index,
                position = slot.transform.position,
                rotation = Quaternion.identity
            }, slot.transform.position, Vector3.up * 3);
            return true;
        }

        private bool SpawnItem(ItemDef itemDef, EquipmentSlot slot)
        {
            PickupIndex index = PickupCatalog.FindPickupIndex(itemDef.itemIndex);
            if (index == PickupIndex.none)
                return false;

            PickupDropletController.CreatePickupDroplet(new GenericPickupController.CreatePickupInfo
            {
                pickupIndex = index,
                position = slot.transform.position,
                rotation = Quaternion.identity
            }, slot.transform.position, Vector3.up * 3);
            return true;
        }

        private bool SpawnPotential(ItemTier tier, EquipmentSlot slot)
        {
            bool includeVoids = Run.instance.IsExpansionEnabled(DLC1Content.Items.AttackSpeedAndMoveSpeed.requiredExpansion);

            var treasureRNG = Run.instance.treasureRng;

            PickupIndex pickupIndex = PickupIndex.none;
            PickupPickerController.Option[] options = null;
            switch(tier)
            {
                case ItemTier.Tier1:
                    pickupIndex = PickupCatalog.FindPickupIndex(ItemTier.Tier1);
                    options = new PickupPickerController.Option[3];
                    for(int i = 0; i < 3; i++)
                    {
                        PickupIndex index = PickupIndex.none;
                        if(includeVoids)
                        {
                            index = (treasureRNG.nextInt % 2 == 0) ? treasureRNG.NextElementUniform(Run.instance.availableTier1DropList) : treasureRNG.NextElementUniform(Run.instance.availableVoidTier1DropList);
                        }
                        else
                        {
                            index = treasureRNG.NextElementUniform(Run.instance.availableTier1DropList);
                        }
                        options[i] = new PickupPickerController.Option
                        {
                            available = true,
                            pickupIndex = index
                        };
                    }
                    break;
                case ItemTier.Tier2:
                    pickupIndex = PickupCatalog.FindPickupIndex(ItemTier.Tier2);
                    options = new PickupPickerController.Option[3];
                    for (int i = 0; i < 3; i++)
                    {
                        PickupIndex index = PickupIndex.none;
                        if (includeVoids)
                        {
                            index = (treasureRNG.nextInt % 2 == 0) ? treasureRNG.NextElementUniform(Run.instance.availableTier2DropList) : treasureRNG.NextElementUniform(Run.instance.availableVoidTier2DropList);
                        }
                        else
                        {
                            index = treasureRNG.NextElementUniform(Run.instance.availableTier2DropList);
                        }
                        options[i] = new PickupPickerController.Option
                        {
                            available = true,
                            pickupIndex = index
                        };
                    }
                    break;
                case ItemTier.Tier3:
                    pickupIndex = PickupCatalog.FindPickupIndex(ItemTier.Tier3);
                    options = new PickupPickerController.Option[3];
                    for (int i = 0; i < 3; i++)
                    {
                        PickupIndex index = PickupIndex.none;
                        if (includeVoids)
                        {
                            index = (treasureRNG.nextInt % 2 == 0) ? treasureRNG.NextElementUniform(Run.instance.availableTier3DropList) : treasureRNG.NextElementUniform(Run.instance.availableVoidTier3DropList);
                        }
                        else
                        {
                            index = treasureRNG.NextElementUniform(Run.instance.availableTier3DropList);
                        }
                        options[i] = new PickupPickerController.Option
                        {
                            available = true,
                            pickupIndex = index
                        };
                    }
                    break;
                case ItemTier.Lunar:
                    pickupIndex = PickupCatalog.FindPickupIndex(ItemTier.Lunar);
                    options = new PickupPickerController.Option[3];
                    for (int i = 0; i < 3; i++)
                    {
                        PickupIndex index = PickupIndex.none;
                        index = treasureRNG.NextElementUniform(Run.instance.availableLunarCombinedDropList);

                        options[i] = new PickupPickerController.Option
                        {
                            available = true,
                            pickupIndex = index
                        };
                    }
                    break;
                default:
                    break;
            }

            if (options == null)
                return false;

            PickupDropletController.CreatePickupDroplet(new GenericPickupController.CreatePickupInfo
            {
                pickerOptions = options,
                position = slot.transform.position,
                rotation = Quaternion.identity,
                prefabOverride = _potentialPrefab,
                pickupIndex = pickupIndex
            }, slot.transform.position, Vector3.up * 10);
            return true;
        }

        private bool SpawnLunarCoins(int amount, EquipmentSlot slot)
        {
            PickupIndex index = PickupCatalog.FindPickupIndex(RoR2Content.MiscPickups.LunarCoin.miscPickupIndex);

            if (index == PickupIndex.none)
                return false;

            for(int i = 0; i < amount; i++)
            {
                PickupDropletController.CreatePickupDroplet(new GenericPickupController.CreatePickupInfo
                {
                    pickupIndex = index,
                    position = slot.transform.position,
                    rotation = Quaternion.identity
                }, slot.transform.position, Vector3.up * 3);
            }
            return true;
        }

        private bool GoldenCoast(EquipmentSlot slot)
        {
            int num = 11;
            if(!slot.characterBody.master)
            {
                return false;
            }
            var master = slot.characterBody.master;
            master.GiveMoney((uint)Run.instance.GetDifficultyScaledCost(25 * 11));
            return true;
        }

        private void ReplaceEquipment(EquipmentSlot slot)
        {
            var body = slot.characterBody;
            if(body.inventory)
            {
                CharacterMasterNotificationQueue.SendTransformNotification(body.master, body.inventory.currentEquipmentIndex, _mysteryBoxConsumed.equipmentIndex, CharacterMasterNotificationQueue.TransformationType.Default);
                body.inventory.SetEquipmentIndex(_mysteryBoxConsumed.equipmentIndex);
            }
        }


        private bool ExecuteInfiniteTowerRun(EquipmentSlot slot)
        {
            Stage stage = Stage.instance;
            if (!stage)
                return false;

            DirectorAPI.Stage stageEnum = DirectorAPI.GetStageEnumFromSceneDef(stage.sceneDef);

            switch(stageEnum)
            {
                case DirectorAPI.Stage.AbandonedAqueductSimulacrum:
                case DirectorAPI.Stage.AphelianSanctuarySimulacrum:
                case DirectorAPI.Stage.TitanicPlainsSimulacrum:
                    if(SpawnPotential(ItemTier.Tier1, slot))
                    {
                        ReplaceEquipment(slot);
                        return true;
                    }
                    return false;
                case DirectorAPI.Stage.RallypointDeltaSimulacrum:
                case DirectorAPI.Stage.AbyssalDepthsSimulacrum:
                    if(SpawnPotential(ItemTier.Tier2, slot))
                    {
                        ReplaceEquipment(slot);
                        return true;
                    }
                    return false;
                case DirectorAPI.Stage.SkyMeadowSimulacrum:
                    if(SpawnPotential(ItemTier.Tier3, slot))
                    {
                        ReplaceEquipment(slot);
                        return true;
                    }
                    return false;
                case DirectorAPI.Stage.CommencementSimulacrum:
                    if(SpawnPotential(ItemTier.Lunar, slot))
                    {
                        ReplaceEquipment(slot);
                        return true;
                    }
                    return false;
                default:
                    return true;
            }
        }
        public void Initialize()
        {
        }

        public bool IsAvailable()
        {
            return true;
        }

        public IEnumerator LoadContentAsync()
        {
            var equipmentRequest = new NebulousTrinketsAssets.SingleBundleAssetRequest<EquipmentDef>("MysteryBox", NebulousTrinketsBundle.Equipments);
            yield return equipmentRequest.Load();
            _asset = equipmentRequest.Asset;

            equipmentRequest = new NebulousTrinketsAssets.SingleBundleAssetRequest<EquipmentDef>("MysteryBoxConsumed", NebulousTrinketsBundle.Equipments);
            yield return equipmentRequest.Load();
            _mysteryBoxConsumed = equipmentRequest.Asset;

            var addressablesRequest = Addressables.LoadAssetAsync<GameObject>("RoR2/DLC1/OptionPickup/OptionPickup.prefab");
            while (!addressablesRequest.IsDone)
                yield return null;
            _potentialPrefab = addressablesRequest.Result;
        }

        public void OnEquipmentObtained(CharacterBody body)
        {
        }

        public void ModifyContentPack(R2APISerializableContentPack contentPack)
        {
            HG.ArrayUtils.ArrayAppend(ref contentPack.equipmentDefs, _mysteryBoxConsumed);
        }
    }
}
