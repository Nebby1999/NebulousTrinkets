using MSU;
using R2API.ScriptableObjects;
using RoR2;
using RoR2.ContentManagement;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NebulousTrinkets
{
    public class NebulousTrinketsContent : IContentPackProvider
    {
        public string identifier => NebulousTrinketsMain.GUID;

        public static R2APISerializableContentPack SerializableContentPack { get; private set; }
        public static ContentPack RuntimeContentPack { get; private set; }

        private static Func<IEnumerator>[] _loadDispatchers;

        public IEnumerator FinalizeAsync(FinalizeAsyncArgs args)
        {
            args.ReportProgress(1f);
            yield break;
        }

        public IEnumerator GenerateContentPackAsync(GetContentPackAsyncArgs args)
        {
            ContentPack.Copy(RuntimeContentPack, args.output);
            args.ReportProgress(1f);
            yield break;
        }

        public IEnumerator LoadStaticContentAsync(LoadStaticContentAsyncArgs args)
        {
            yield return NebulousTrinketsAssets.Initialize(args);

            var contentPackRequest = new NebulousTrinketsAssets.SingleBundleAssetRequest<R2APISerializableContentPack>("ContentPack", NebulousTrinketsBundle.Main);
            yield return contentPackRequest.Load();

            SerializableContentPack = contentPackRequest.Asset;
            var main = NebulousTrinketsMain.Instance;

            for(int i = 0; i < _loadDispatchers.Length; i++)
            {
                args.ReportProgress(Util.Remap(i + 1, 0f, _loadDispatchers.Length, 0f, 0.05f));
                yield return _loadDispatchers[i]();
            }

            RuntimeContentPack = SerializableContentPack.GetOrCreateContentPack();
            RuntimeContentPack.identifier = identifier;
        }

        internal NebulousTrinketsContent()
        {
            ContentManager.collectContentPackProviders += AddSelf;
        }

        static NebulousTrinketsContent()
        {
            NebulousTrinketsMain main = NebulousTrinketsMain.Instance;
            _loadDispatchers = new Func<IEnumerator>[]
            {
                () =>
                {
                    ItemModule.AddProvider(main, ContentUtil.AnalyzeForContentPieces<ItemDef>(main, SerializableContentPack));
                    return ItemModule.InitializeItems(main);
                },
                () =>
                {
                    EquipmentModule.AddProvider(main, ContentUtil.AnalyzeForContentPieces<EquipmentDef>(main, SerializableContentPack));
                    return EquipmentModule.InitialzeEquipments(main);
                }
            };
        }

        private void AddSelf(ContentManager.AddContentPackProviderDelegate addContentPackProvider)
        {
            addContentPackProvider(this);
        }
    }
}