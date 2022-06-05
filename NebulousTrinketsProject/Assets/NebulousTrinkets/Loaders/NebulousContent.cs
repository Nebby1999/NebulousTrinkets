using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moonstorm.Loaders;
using R2API.ScriptableObjects;
using RoR2;
using NebulousTrinkets.Modules;

namespace NebulousTrinkets
{
    public class NebulousContent : ContentLoader<NebulousContent>
    {
        public static class Equipments
        {
            public static EquipmentDef BrotherRing;
        }
        public override string identifier => NebulousMain.GUID;

        public override R2APISerializableContentPack SerializableContentPack { get; protected set; } = NebulousAssets.LoadAsset<R2APISerializableContentPack>("NebulousContent");
        public override Action[] LoadDispatchers { get; protected set; }
        public override Action[] PopulateFieldsDispatchers { get; protected set; }

        public override void Init()
        {
            base.Init();
            LoadDispatchers = new Action[]
            {
                () => new NebulousEquipment().Initialize()
            };
            PopulateFieldsDispatchers = new Action[]
            {
                () => PopulateTypeFields(typeof(Equipments), ContentPack.equipmentDefs),
            };
        }
    }
}
