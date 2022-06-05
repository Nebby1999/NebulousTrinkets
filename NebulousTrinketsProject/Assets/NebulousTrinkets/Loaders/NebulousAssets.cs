using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moonstorm.Loaders;
using UnityEngine;

namespace NebulousTrinkets
{
    public class NebulousAssets : AssetsLoader<NebulousAssets>
    {
        public override AssetBundle MainAssetBundle => _assetBundle;
        private AssetBundle _assetBundle;
        public const string AssetBundleName = "nebulousassets";
        public const string AssetBundleFolder = "assetbundles";
        public static string AssetBundleLocation { get => Path.Combine(NebulousMain.InstalledDirectory, AssetBundleFolder, AssetBundleName); }
        internal void LoadAssetbundle()
        {
            _assetBundle = AssetBundle.LoadFromFile(AssetBundleLocation);
        }

        internal void SwapShaders()
        {
            SwapShadersFromMaterials(_assetBundle.LoadAllAssets<Material>().Where(m => m.shader.name.StartsWith("Stubbed")));
        }
    }
}
