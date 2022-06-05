using Moonstorm;
using R2API.ScriptableObjects;
using RoR2;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace NebulousTrinkets.Modules
{
    public sealed class NebulousEquipment : EquipmentModuleBase
    {
        public static NebulousEquipment Instance { get; private set; }
        public static EquipmentDef[] NebulousEquipments { get => NebulousContent.Instance.SerializableContentPack.equipmentDefs; }
        public override R2APISerializableContentPack SerializableContentPack => NebulousContent.Instance.SerializableContentPack;

        public override void Initialize()
        {
            Instance = this;
            base.Initialize();
            NLog.Info($"Initializing Equipments...");
            GetEquipmentBases();
        }

        protected override IEnumerable<EquipmentBase> GetEquipmentBases()
        {
            base.GetEquipmentBases()
                .ToList()
                .ForEach(eqp => AddEquipment(eqp));
            return null;
        }
    }
}