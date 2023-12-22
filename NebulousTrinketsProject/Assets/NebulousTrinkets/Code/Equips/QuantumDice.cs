using MSU;
using RoR2;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace NebulousTrinkets.Equipments
{
    public class QuantumDice : IEquipmentContentPiece
    {
        public NullableRef<GameObject> ItemDisplayPrefab => null;

        public EquipmentDef Asset => _asset;
        private EquipmentDef _asset;

        private Dictionary<PickupDef, int> _pickupDefToAvailableItemsIndex = new Dictionary<PickupDef, int>();
        private List<PickupDef> _availableItems = new List<PickupDef>();

        private Dictionary<PickupDef, int> _pickupDefToAvailableEquipmentsIndex = new Dictionary<PickupDef, int>();
        private List<PickupDef> _availableEquipments = new List<PickupDef>();

        public bool Execute(EquipmentSlot slot)
        {
            slot.UpdateTargets(RoR2Content.Equipment.Recycle.equipmentIndex, false);
            GenericPickupController controller = slot.currentTarget.pickupController;

            if(!controller || controller.TryGetComponent<QuantumDiceToken>(out _))
            {
                return false;
            }

            PickupIndex initialPickupIndex = controller.pickupIndex;
            PickupDef def = PickupCatalog.GetPickupDef(initialPickupIndex);

            if(def.itemIndex != ItemIndex.None && _pickupDefToAvailableItemsIndex.TryGetValue(def, out var itemValue))
            {
                itemValue--;
                if(itemValue < 0)
                {
                    itemValue = _availableItems.Count - 1;
                }
                def = _availableItems[itemValue];
            }
            else if(def.equipmentIndex != EquipmentIndex.None && _pickupDefToAvailableEquipmentsIndex.TryGetValue(def, out var equipmentValue))
            {
                equipmentValue--;
                if(equipmentValue < 0)
                {
                    equipmentValue = _availableEquipments.Count - 1;
                }
                def = _availableEquipments[equipmentValue];
            }
            else
            {
                return false;
            }

            controller.gameObject.AddComponent<QuantumDiceToken>();
            controller.NetworkpickupIndex = def.pickupIndex;
            slot.InvalidateCurrentTarget();
            return true;
        }

        public void Initialize()
        {
            Run.onRunStartGlobal += BuildMap;
        }

        private void BuildMap(Run obj)
        {
            _pickupDefToAvailableItemsIndex.Clear();
            _availableItems.Clear();

            _pickupDefToAvailableEquipmentsIndex.Clear();
            _availableEquipments.Clear();

            for(int i = 0; i < ItemCatalog.itemCount; i++)
            {
                var itemDef = ItemCatalog.allItemDefs[i];

                if (itemDef.tier == ItemTier.NoTier)
                    continue;

                if(!itemDef.IsAvailable())
                {
                    continue;
                }

                if (itemDef.ContainsTag(ItemTag.WorldUnique) || 
                    itemDef.ContainsTag(ItemTag.Scrap) || 
                    itemDef.ContainsTag(ItemTag.PriorityScrap))
                {
                    continue;
                }

                var pickupDef = PickupCatalog.GetPickupDef(PickupCatalog.FindPickupIndex(itemDef.itemIndex));

                if (!pickupDef.pickupIndex.isValid)
                    continue;

                _availableItems.Add(pickupDef);
                _pickupDefToAvailableItemsIndex.Add(pickupDef, _availableItems.Count - 1);
            }

            for(int i = 0; i < EquipmentCatalog.equipmentCount; i++)
            {
                var equipmentDef = EquipmentCatalog.equipmentDefs[i];

                if (!equipmentDef.IsAvailable())
                    continue;

                if (!equipmentDef.canDrop)
                    continue;

                var pickupIndex = PickupCatalog.FindPickupIndex(equipmentDef.equipmentIndex);
                if (!pickupIndex.isValid)
                    continue;

                var pickupDef = PickupCatalog.GetPickupDef(pickupIndex);

                _availableEquipments.Add(pickupDef);
                _pickupDefToAvailableEquipmentsIndex.Add(pickupDef, _availableEquipments.Count - 1);
            }
        }

        public bool IsAvailable()
        {
            return true;
        }

        public IEnumerator LoadContentAsync()
        {
            var request = new NebulousTrinketsAssets.SingleBundleAssetRequest<EquipmentDef>("QuantumDice", NebulousTrinketsBundle.Equipments);
            yield return request.Load();
            _asset = request.Asset;
        }

        public void OnEquipmentObtained(CharacterBody body)
        {
        }

        private class QuantumDiceToken : MonoBehaviour
        {

        }
    }
}