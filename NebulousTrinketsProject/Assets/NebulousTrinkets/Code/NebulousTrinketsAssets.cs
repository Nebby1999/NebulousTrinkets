//------------------------------------------------------------------------------
// <auto-generated>
//     This code was auto-generated by com.TeamMoonstorm.MoonstormSharedUtils:SingleAssetBundleAssetsClassCodeGen
//     version 1.0.0
//
//     Feel free to remove this commment block.
// </auto-generated>
//------------------------------------------------------------------------------


using System;
using System.Collections.Generic;
using UnityEngine;
using MSU;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.IO;
using System.Collections;
using RoR2.ContentManagement;
using RoR2;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.AddressableAssets;
using Path = System.IO.Path;

namespace NebulousTrinkets
{
    //These enums represent a specific, non streamed scene bundle in your project. The exceptions are "Invalid", which represents an Invalid assetbundle, and the special enum "All", which is used to do a global search.
    public enum NebulousTrinketsBundle
    {
        Invalid,
        All,
        Main,
        Items,
        Equipments
    }

    //This is your assets class. it'll contain all of your mod's assetbundles.
    internal static class NebulousTrinketsAssets
    {
        private const string ASSET_BUNDLE_FOLDER_NAME = "assetbundles";

        //Try to keep a specific constant for each of your non streamed scene bundles.
        private const string MAIN = "nebuloustrinketsmain";
        private const string ITEMS = "nebuloustrinketsitems";
        private const string EQUIPMENTS = "nebuloustrinketsequipments";

        private static string AssetBundleFolderPath => Path.Combine(Path.GetDirectoryName(NebulousTrinketsMain.PluginInfo.Location), ASSET_BUNDLE_FOLDER_NAME);
        private static Dictionary<NebulousTrinketsBundle, AssetBundle> _assetBundles = new Dictionary<NebulousTrinketsBundle, AssetBundle>();
        private static AssetBundle[] _streamedSceneBundles = Array.Empty<AssetBundle>();
        private static List<Material> _swappedShaderMaterials = new List<Material>();

        public static AssetBundle GetAssetBundle(NebulousTrinketsBundle bundle) => _assetBundles[bundle];

        public static TAsset LoadAsset<TAsset>(string name, NebulousTrinketsBundle bundle) where TAsset : UnityEngine.Object
        {
            TAsset asset = null;
            if(bundle == NebulousTrinketsBundle.All)
            {
                asset = FindAsset(name, out NebulousTrinketsBundle foundInBundle);
#if DEBUG
                if(!asset)
                {
                    NebulousTrinketsLogger.Warning($"Could not find asset of type {typeof(TAsset).Name} with name {name} in any of the bundles.");
                }
                else
                {
                    NebulousTrinketsLogger.Info($"Asset of type {typeof(TAsset).Name} with name {name} was found inside bundle {foundInBundle}, it is recommended that you load the asset directly.");
                }
#endif
                return asset;
            }
            asset = _assetBundles[bundle].LoadAsset<TAsset>(name);
#if DEBUG
            if(!asset)
            {
                NebulousTrinketsLogger.Warning($"The method \"{GetCallingMethod()}\" is calling \"LoadAsset<TAsset>(string, NebulousTrinketsBundle)\" with the arguments \"{typeof(TAsset).Name}\", \"{name}\" and \"{bundle}\", however, the asset could not be found.\n A complete search of all the bundles will be done and the correct bundle enum will be logged.");
                return LoadAsset<TAsset>(name, NebulousTrinketsBundle.All);
            }
#endif
            return asset;

            TAsset FindAsset(string assetName, out NebulousTrinketsBundle foundInBundle)
            {
                foreach((var enumVal, var assetBundle) in _assetBundles)
                {
                    var loadedAsset = assetBundle.LoadAsset<TAsset>(assetName);
                    if(loadedAsset)
                    {
                        foundInBundle = enumVal;
                        return loadedAsset;
                    }
                }
                foundInBundle = NebulousTrinketsBundle.Invalid;
                return null;
            }
        }

        public static IEnumerator LoadAssetAsync<TAsset>(SingleBundleAssetRequest<TAsset> request) where TAsset : UnityEngine.Object
        {
            yield return request.Load();
        }

        public static TAsset[] LoadAllAssets<TAsset>(NebulousTrinketsBundle bundle) where TAsset : UnityEngine.Object
        {
            List<TAsset> loadedAssets = new List<TAsset>();
            if(bundle == NebulousTrinketsBundle.All)
            {
                FindAssets(loadedAssets);
#if DEBUG
                if(loadedAssets.Count == 0)
                {
                    NebulousTrinketsLogger.Warning($"Could not find any asset of type {typeof(TAsset).Name} in any of the bundles.");
                }
#endif
                return loadedAssets.ToArray();
            }
            loadedAssets = _assetBundles[bundle].LoadAllAssets<TAsset>().ToList();
#if DEBUG
            if(loadedAssets.Count == 0)
            {
                NebulousTrinketsLogger.Warning($"Could not find any asset of type {typeof(TAsset)} inside the bundle {bundle}");
            }
#endif
            return loadedAssets.ToArray();

            void FindAssets(List<TAsset> output)
            {
                foreach((var _, var bndle) in _assetBundles)
                {
                    output.AddRange(bndle.LoadAllAssets<TAsset>());
                }
                return;
            }
        }

        public static IEnumerator LoadAllAssetsAsync<TAsset>(MultipleBundleAssetRequest<TAsset> request) where TAsset : UnityEngine.Object
        {
            yield return request.Load();
        }

        //This method gets called during ContentPack loading, which will ensure assets load asynchronously properly.
        internal static IEnumerator Initialize(LoadStaticContentAsyncArgs args)
        {
            NebulousTrinketsLogger.Info("Initializing Assets");

            yield return LoadAssetBundles(args);
            yield return SwapShaders(args);
            yield return SwapAddressableShaders(args);
        }

        private static IEnumerator LoadAssetBundles(LoadStaticContentAsyncArgs args)
        {
            var bundlePaths = GetAssetBundlePaths();
            for(int i = 0; i < bundlePaths.Length; i++)
            {
                //In this switch statement we proceed to tie a bundle name to a specific enum. there's a special case for default if the loaded bundle is a streamed scene bundle.
                string path = bundlePaths[i];
                string fileName = Path.GetFileName(path);
                switch(fileName)
                {
                    case MAIN: yield return LoadAndAssign(path, NebulousTrinketsBundle.Main, i); break;
                    case ITEMS: yield return LoadAndAssign(path, NebulousTrinketsBundle.Items, i); break;
                    case EQUIPMENTS: yield return LoadAndAssign(path, NebulousTrinketsBundle.Equipments, i); break;
                    default: yield return HandleDefaultCase(path, i); break;
                }
            }

            IEnumerator LoadAndAssign(string path, NebulousTrinketsBundle bundleEnum, int index)
            {
                var request = AssetBundle.LoadFromFileAsync(path);
                while(!request.isDone)
                {
                    args.ReportProgress(Util.Remap(request.progress + index, 0f, bundlePaths.Length, 0f, 0.4f));
                    yield return null;
                }

                try
                {
                    AssetBundle bundle = request.assetBundle;
                    if(!bundle)
                    {
                        throw new FileLoadException("AssetBundle.LoadFromFile did not return an asset bundle.");
                    }
                    if(_assetBundles.ContainsKey(bundleEnum))
                    {
                        throw new InvalidOperationException($"AssetBundle in path loaded succesfully, but the assetbundles dictionary already contains an entry for {bundleEnum}");
                    }

                    _assetBundles[bundleEnum] = bundle;
                }
                catch(Exception e)
                {
                    NebulousTrinketsLogger.Error($"Could not load assetbundle at path {path} andd assign to enum {bundleEnum}. {e}");
                }
            }

            IEnumerator HandleDefaultCase(string path, int index)
            {
                var request = AssetBundle.LoadFromFileAsync(path);
                while(!request.isDone)
                {
                    args.ReportProgress(Util.Remap(request.progress + index, 0f, bundlePaths.Length, 0f, 0.4f));
                    yield return null;
                }

                try
                {
                    AssetBundle bundle = request.assetBundle;
                    if(!bundle)
                    {
                        throw new FileLoadException("AssetBundle.LoadFromFile did not return an asset bundle. (Path={path})");
                    }
                    if(!bundle.isStreamedSceneAssetBundle)
                    {
                        throw new Exception($"AssetBundle in specified path is not a streamed scene bundle, but its file name was not found in the Switch statement. have you forgotten to setup the enum and file name in your assets class? (Path={path})");
                    }
                    else
                    {
                        HG.ArrayUtils.ArrayAppend(ref _streamedSceneBundles, bundle);
                    }
                    NebulousTrinketsLogger.Warning($"Invalid or Unexpected file in the AssetBundles folder (Path={path})");
                }
                catch(Exception e)
                {
                    NebulousTrinketsLogger.Error($"Default statement on bundle loading method hit, Exception thrown. {e}");
                }
            }
        }

        private static string[] GetAssetBundlePaths() => Directory.GetFiles(AssetBundleFolderPath).Where(filePath => !filePath.EndsWith(".manifest")).ToArray();

        private static IEnumerator SwapShaders(LoadStaticContentAsyncArgs args)
        {
#if DEBUG
            NebulousTrinketsLogger.Debug("Swapping stubbed shaders from AssetBundles");
#endif
            var request = new MultipleBundleAssetRequest<Material>(NebulousTrinketsBundle.All);
            yield return LoadAllAssetsAsync(request);

            var materials = request.Assets.Where(mat => mat.shader.name.StartsWith("Stubbed")).ToArray();
            for(int i = 0; i < materials.Length; i++)
            {
                var material = materials[i];
#if DEBUG
                NebulousTrinketsLogger.Debug($"Swapping the shader of material \"{material}\"");
#endif
                AsyncOperationHandle<Shader> asyncOp = default;
                try
                {
                    var shaderName = material.shader.name.Substring("Stubbed".Length);
                    var addressablePath = $"{shaderName}.shader";
                    asyncOp = Addressables.LoadAssetAsync<Shader>(addressablePath);
                }
                catch(Exception e)
                {
                    NebulousTrinketsLogger.Error($"Faileed to swap the shader of material \"{material}\". {e}");
                    continue;
                }
                while(!asyncOp.IsDone)
                {
                    args.ReportProgress(Util.Remap(asyncOp.PercentComplete + i, 0f, 1f, 0.4f, 0.6f));
                    yield return null;
                }
                material.shader = asyncOp.Result;
                _swappedShaderMaterials.Add(material);
            }
        }

        private static IEnumerator SwapAddressableShaders(LoadStaticContentAsyncArgs args)
        {
#if DEBUG
            NebulousTrinketsLogger.Debug("Finalizing materials with AddressableMaterial shaders from AssetBundles");
#endif
            var request = new MultipleBundleAssetRequest<Material>(NebulousTrinketsBundle.All);
            yield return LoadAllAssetsAsync(request);

            var materials = request.Assets.Where(mat => mat.shader.name == "AddressableMaterialShader").ToArray();
            for(int i = 0; i < materials.Length; i++)
            {
                var material = materials[i];
#if DEBUG
                NebulousTrinketsLogger.Debug($"Finalizing material {material}");
#endif

                AsyncOperationHandle<Material> asyncOp = default;
                try
                {
                    var shaderKeywords = material.shaderKeywords;
                    var address = shaderKeywords[0];
                    asyncOp = Addressables.LoadAssetAsync<Material>(address);
                }
                catch(Exception e)
                {
                    NebulousTrinketsLogger.Error($"Failed to finalize the material \"{material}\", {e}");
                    continue;
                }

                while(!asyncOp.IsDone)
                {
                    args.ReportProgress(Util.Remap(asyncOp.PercentComplete + i, 0f, 1f, 0.6f, 0.8f));
                    yield return null;
                }

                var loadedMat = asyncOp.Result;
                material.shader = loadedMat.shader;
                material.CopyPropertiesFromMaterial(loadedMat);
                _swappedShaderMaterials.Add(material);
            }
        }
#if DEBUG
        private static string GetCallingMethod()
        {
            var stackTrace = new StackTrace();

            for(int stackFrameIndex = 0; stackFrameIndex < stackTrace.FrameCount; stackFrameIndex++)
            {
                var frame = stackTrace.GetFrame(stackFrameIndex);
                var method = frame.GetMethod();
                if(method == null)
                continue;

                var declaringType = method.DeclaringType;
                if(declaringType.IsGenericType && declaringType.DeclaringType == typeof(NebulousTrinketsAssets))
                continue;

                if(declaringType == typeof(NebulousTrinketsAssets))
                continue;

                var fileName = frame.GetFileName();
                var fileLineNumber = frame.GetFileLineNumber();
                var fileColumnNumber = frame.GetFileColumnNumber();

                return $"{declaringType.FullName}.{method.Name}({GetMethodParams(method)}) (fileName={fileName}, Location=L{fileLineNumber} C{fileColumnNumber})";
            }
            return "[COULD NOT GET CALLING METHOD]";
        }

        private static string GetMethodParams(MethodBase methodBase)
        {
            var parameters = methodBase.GetParameters();
            if(parameters.Length == 0)
            return string.Empty;

            StringBuilder stringBuilder = new StringBuilder();
            foreach(var parameter in parameters)
            {
                stringBuilder.Append(parameter.ToString() + ", ");
            }
            return stringBuilder.ToString();
        }
#endif

        public class SingleBundleAssetRequest<TAsset> where TAsset : UnityEngine.Object
        {
            public TAsset Asset { get; private set; }
            public NebulousTrinketsBundle BundleEnum { get; private set; }
            private string assetName;

            internal IEnumerator Load()
            {
                if(BundleEnum == NebulousTrinketsBundle.All)
                {
                    yield return FindAsset();

#if DEBUG
                    if(!Asset)
                    {
                        BundleEnum = NebulousTrinketsBundle.Invalid;
                        NebulousTrinketsLogger.Warning($"Could not find asset of type {typeof(TAsset).Name} with name {assetName} in any of the bundles.");
                    }
                    else
                    {
                        NebulousTrinketsLogger.Info($"Asset of type {typeof(TAsset).Name} with name {assetName} was found inside bundle {BundleEnum}, it is recommended that you load the asset directly.");
                    }
#endif
                    yield break;
                }

                var request = _assetBundles[BundleEnum].LoadAssetAsync<TAsset>(assetName);
                while(!request.isDone)
                {
                    yield return null;
                }

                Asset = (TAsset)request.asset;

#if DEBUG
                if(!Asset)
                {
                    NebulousTrinketsLogger.Warning($"The method \"{GetCallingMethod()}\" is calling \"LoadAssetAsync<TAsset>(SingleBundleAssetRequest<TAsset>)\" with the arguments \"{typeof(TAsset).Name}\", \"{assetName}\" and \"{BundleEnum}\", however, the asset could not be found.\n A complete search of all the bundles will be done and the correct bundle enum will be logged.");
                }
#endif
                yield break;
            }

            private IEnumerator FindAsset()
            {
                foreach((var enumVal, var assetBundle) in _assetBundles)
                {
                    var request = assetBundle.LoadAssetAsync<TAsset>(assetName);
                    while(request.isDone)
                    {
                        yield return null;
                    }
                    Asset = (TAsset)request.asset;
                    if(Asset)
                    {
                        BundleEnum = enumVal;
                        yield break;
                    }
                }
            }

            public SingleBundleAssetRequest(string name, NebulousTrinketsBundle bundleEnum)
            {
                assetName = name;
                BundleEnum = bundleEnum;
            }
        }

        public class MultipleBundleAssetRequest<TAsset> where TAsset : UnityEngine.Object
        {
            public TAsset[] Assets { get; private set; }
            public NebulousTrinketsBundle BundleEnum { get; private set; }

            internal IEnumerator Load()
            {
                List<TAsset> assets = new List<TAsset>();
                if(BundleEnum == NebulousTrinketsBundle.All)
                {
                    yield return FindAssets(assets);

#if DEBUG
                    if(assets.Count == 0)
                    {
                        NebulousTrinketsLogger.Warning($"Could not find any asset of type {typeof(TAsset).Name} in any of the bundles.");
                    }
#endif

                    Assets = assets.ToArray();
                    yield break;
                }

                var request = _assetBundles[BundleEnum].LoadAllAssetsAsync<TAsset>();
                while(!request.isDone)
                {
                    yield return null;
                }
                assets.AddRange(request.allAssets.OfType<TAsset>());
                Assets = assets.ToArray();

#if DEBUG
                if(Assets.Length == 0)
                {
                    NebulousTrinketsLogger.Warning($"Could not find any asset of type {typeof(TAsset)} inside the bundle {BundleEnum}");
                }
#endif

                yield break;
            }

            private IEnumerator FindAssets(List<TAsset> output)
            {
                foreach((var _, var bundle) in _assetBundles)
                {
                    var request = bundle.LoadAllAssetsAsync<TAsset>();
                    while(!request.isDone)
                    {
                        yield return null;
                    }
                    output.AddRange(request.allAssets.OfType<TAsset>());
                }
                yield break;
            }

            public MultipleBundleAssetRequest(NebulousTrinketsBundle bundleEnum)
            {
                BundleEnum = bundleEnum;
            }
        }

    }
}
