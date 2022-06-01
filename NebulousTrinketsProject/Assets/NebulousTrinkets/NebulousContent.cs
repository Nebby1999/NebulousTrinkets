using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moonstorm.Loaders;
using R2API.ScriptableObjects;

namespace NebulousTrinkets
{
    public class NebulousContent : ContentLoader<NebulousContent>
    {
        public override string identifier => NebulousMain.GUID;

        public override R2APISerializableContentPack SerializableContentPack { get; protected set; }
        public override Action[] LoadDispatchers { get; protected set; }
    }
}
