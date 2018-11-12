using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace CFramework
{
    public class ResourceManager : BaseManager<ResourceManager>
    {
        private AssetBundle _bundle;
        private AssetBundleManifest _manifest;
        private Dictionary<string, List<string>> _dependDic;// 已经加载的ab资源和他的依赖资源

        private Dictionary<string, AssetBundle> _bundleDic;// 已经加载的ab资源缓存
        private List<string> _loadCache;//正在加载的ab资源
        
        public override void Init()
        {
            _dependDic = new Dictionary<string, List<string>>();
            _bundleDic = new Dictionary<string,AssetBundle>();
            _loadCache = new List<string>();
            //加载那个打包时额外打出来的总包
            if (AppConst.IS_DEBUG) {

                _bundle = AssetBundle.LoadFromFile(FileUtil.Instance.GetResPath("StreamingAssets"));
            }
            else {

                _bundle = AssetBundle.LoadFromFile(FileUtil.Instance.GetWritePath("StreamingAssets"));
            }

            //   var bundle = AssetBundle.LoadFromFile("Assets/AssetBundles/AssetBundles.manifest");//不能这样加载manifest。
            _manifest = _bundle.LoadAsset("AssetBundleManifest") as AssetBundleManifest;
            //AssetBundleManifest不是某个文件的名字，是固定的一个东西
            
        }

        //释放资源
        public void DisposeView(string viewId)
        {
            
            if (!_dependDic.ContainsKey(viewId)) {
                return;
            }

            List<string> disAbs = _dependDic[viewId];
            
            foreach (string key in _dependDic.Keys)
            {
                List<string> viewAbs = _dependDic[key];
                disAbs = disAbs.Except(viewAbs).ToList();
            }

            foreach (var key in disAbs) {
                _bundleDic[key].Unload(false);
            }
        }

        /// <summary>
        /// 实例化预制体
        /// </summary>
        /// <param name="name"></param>
        public Transform GetAsset(string abName, string name)
        {
            //ShowAbList();
            AssetBundle assetBundle;
            _bundleDic.TryGetValue(FormatPath(abName), out assetBundle);

            if (assetBundle == null)
            {
                DebugManager.LogError("GetAsset assetBundle is null     " + abName +"," + name);
                return null;
            }
            return GetAsset(assetBundle, name);
        }

        private void ShowAbList() {

            DebugManager.Log("-----------------showAbList---------------");
            foreach (string key in _bundleDic.Keys) {
                DebugManager.Log(key);
            }
            DebugManager.Log("-----------------showAbList   end---------------");
        }

        /// <summary>
        /// 实例化预制体
        /// </summary>
        /// <param name="name"></param>
        public Transform GetAsset(AssetBundle assetBundle, string name)
        {

            if (assetBundle == null)
            {
                DebugManager.LogError("GetAsset assetBundle is null     " + name);
                return null;
            }

            GameObject go = Instantiate(assetBundle.LoadAsset<GameObject>(name));
            go.name = go.name.Replace("(Clone)", "");
            //GameObject go = assetBundle.LoadAsset<GameObject>(name);
            
            return go.transform;
        }

        /// <summary>
        /// 实例化预制体
        /// </summary>
        /// <param name="name"></param>
        public Texture2D GetTexture2DAsset(AssetBundle assetBundle, string name)
        {

            if (assetBundle == null)
            {
                DebugManager.LogError("GetAsset assetBundle is null     " + name);
                return null;
            }

            foreach (Texture2D t in assetBundle.LoadAllAssets<Texture2D>()) {
                
                if (t.name.Equals(name)) {

                    return assetBundle.LoadAsset<Texture2D>(name);
                }
            }

            return null;
        }

        /// <summary>
        /// 异步加载预制体
        /// </summary>
        /// <param name="viewId"></param>
        /// <param name="path"></param>
        /// <param name="callback"></param>
        public void LoadPrefabAsync(string viewId, string[] pathList, Action<List<AssetBundle>> callback)
        {
            StartCoroutine(LoadListAsyncCoroutine(viewId, pathList, delegate(List<AssetBundle> assetBundle) {
                DebugManager.Log("-------------------load list end:");
                callback(assetBundle);
            }));
        }


        public IEnumerator LoadListAsyncCoroutine(string viewId, string[] pathList, Action<List<AssetBundle>> callback = null)
        {
            List<AssetBundle> list = new List<AssetBundle>();
            foreach (string path in pathList)
            {
                string path1 = FormatPath(path);
                yield return LoadAsyncCoroutine(viewId, path1);
                list.Add(_bundleDic[path1]);
            }
            if (callback != null)
            {
                callback(list);
            }
        }

        /// <summary>
        /// 同步加载预制体
        /// </summary>
        /// <param name="viewId"></param>
        /// <param name="path"></param>
        public List<AssetBundle> LoadPrefabList(string viewId, string[] pathList) {

            List<AssetBundle> list = new List<AssetBundle>();
            for (int i = 0; i < pathList.Length; ++i)
            {
                list.Add( LoadPrefab(viewId, pathList[i]));
            }

            return list;
        }

        /// <summary>
        /// 同步加载预制体
        /// </summary>
        /// <param name="viewId"></param>
        /// <param name="path"></param>
        public AssetBundle LoadPrefab(string viewId, string path)
        {
            path = FormatPath(path);

            if (_bundleDic.ContainsKey(path))
            {
                return _bundleDic[path];
            }

            string[] deps = _manifest.GetAllDependencies(path);
            for (int i = 0; i < deps.Length; ++i)
            {
                LoadPrefab(viewId, deps[i]);
            }

            AssetBundle assetBundle;
            if (AppConst.IS_DEBUG)
            {
                assetBundle = AssetBundle.LoadFromFile(FileUtil.Instance.GetResPath(path));
            }
            else {
                assetBundle = AssetBundle.LoadFromFile(FileUtil.Instance.GetWritePath(path));
            }

            AddCache(viewId, path, assetBundle);

            return assetBundle;
        }

        /// <summary>
        /// 异步加载预制体
        /// </summary>
        /// <param name="viewId"></param>
        /// <param name="path"></param>
        /// <param name="callback"></param>
        public void LoadPrefabAsync(string viewId, string path, Action<AssetBundle> callback)
        {
            StartCoroutine(LoadAsyncCoroutine(viewId, path, callback));
        }

        public IEnumerator LoadAsyncCoroutine(string viewId, string path, Action<AssetBundle> callback = null){

            path = FormatPath(path);

            //DebugManager.Log("*********LoadPrefabAsync start:" + path+","+ _bundleDic.ContainsKey(path));
            if (_bundleDic.ContainsKey(path))
            {
                //DebugManager.Log("----------load cache:" + path);
                if (callback != null) {
                    callback(_bundleDic[path]);
                }
                yield break;
            }

            if (_loadCache.Contains(path))
            {
                //加载一半的ab资源
                yield return new WaitWhile(delegate() {
                    return _loadCache.Contains(path);
                });

                if (callback != null)
                {
                    callback(_bundleDic[path]);
                }

            }
            else {

                //未加载过的ab资源
                _loadCache.Add(path);
                string[] deps = _manifest.GetAllDependencies(path);

                for (int i = 0; i < deps.Length; ++i)
                {
                    yield return LoadAsyncCoroutine(viewId, deps[i]);
                }

                string abPath = AppConst.IS_DEBUG ? FileUtil.Instance.GetResPath(path) : FileUtil.Instance.GetWritePath(path);
               
                AssetBundleCreateRequest abcr = AssetBundle.LoadFromFileAsync(abPath);
                yield return abcr;

                AddCache(viewId, path, abcr.assetBundle);
                _loadCache.Remove(path);

                if (callback != null)
                {
                    callback(abcr.assetBundle);
                }
            }

        }

        private void AddCache(string viewId, string path, AssetBundle assetBundle) {

            DebugManager.Log("----------load prefab success:" + path+","+(assetBundle == null));

            if (!_dependDic.ContainsKey(viewId))
            {
                _dependDic.Add(viewId, new List<string>());
            }

            _dependDic[viewId].Add(path);
            _bundleDic.Add(path, assetBundle);
        }

        public void DisposeViewCache(string viewId) {

            DebugManager.Log("DisposeViewCache:" + viewId);

            if ("LoadingWindow".Equals(viewId)) {
                return;
            }

            List<string> unDisposeList = new List<string>();

            foreach (string id in _dependDic.Keys) {
                if (id != viewId) {

                    foreach (string path in _dependDic[id]) {
                        unDisposeList.Add(path);
                    }

                }
            }

            foreach (string path in _dependDic[viewId].Except(unDisposeList)) {

                if (_bundleDic.ContainsKey(path)) {
                    AssetBundle ab = _bundleDic[path];
                    _bundleDic.Remove(path);
                    DebugManager.Log("释放资源：" + path);
                    ab.Unload(true);
                }

            }
            _dependDic.Remove(viewId);

        }

        private string FormatPath(string path) {

            if (!path.StartsWith("res/"))
            {
                path = "res/" + path;
            }

            if (!path.EndsWith(".unity3d"))
            {
                path += ".unity3d";
            }
            return path.ToLower();
        }

        public void Dispose()
        {
            _bundle.Unload(true);
        }
    }
}
