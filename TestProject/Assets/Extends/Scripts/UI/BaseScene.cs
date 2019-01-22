using System;
using System.Collections.Generic;
using UnityEngine;

namespace CFramework
{
    public class BaseScene : BaseView
    {
        public BaseScene(string id, string[] preloadList, Action<BaseScene> completeCallback, Action<int> progressCallback, bool isCache, Transform uiRoot) 
               : base(id, preloadList, isCache, uiRoot) {

            ProgressCallback = progressCallback;
            CompleteCallback = completeCallback;

            _modulelList = new Dictionary<string, BaseModule>();
            _windowlList = new Dictionary<string, BaseWindow>();
            _sysWindowlList = new Dictionary<string, BaseWindow>();
        }

        public virtual void OnInit()
        {

            InitLayer(_uiRoot);

        }

        protected Dictionary<string, BaseModule> _modulelList;
        protected Dictionary<string, BaseWindow> _windowlList;
        protected Dictionary<string, BaseWindow> _sysWindowlList;

        
        public Transform TipLayer { get; set; }
        public Transform GuideLayer { get; set; }
        public Transform WindowLayer { get; set; }
        public Transform SysLayer { get; set; }

        public string LoadingId { get; set; }
        public Action<int> ProgressCallback { get; }
        public Action<BaseScene> CompleteCallback { get; set; }
        
        public void AddModule(GameObject parent, string id, string abName, string assetName, Action<BaseModule> completeCallback) { }
        public void AddWindow(string id, string abName, string assetName, string[] preloadList, bool isModal, bool isCache, bool isClickExit, Action<BaseWindow> completeCallback, Action<int, int> progressCallback) {

            List<string> list = new List<string>(preloadList);
            list.Insert(0,abName);
            
            ResourceManager.Instance.LoadPrefabAsync(id, list.ToArray(), delegate (List<AssetBundle> assetList) {

                GameObject window = ResourceManager.Instance.GetAsset(assetList[0], assetName).gameObject;
            
                window.transform.SetParent(WindowLayer);
                window.transform.localScale = new Vector3(1f,1f,1f);
                window.GetComponent<RectTransform>().localPosition = new Vector3(0,0,0);

                window.name = id;

                BaseWindow baseWindow = new BaseWindow(id, null, false, window.transform);
                _windowlList.Add(id, baseWindow);

                completeCallback(baseWindow);
                
            });
        }
        public void AddTip(GameObject go) {
            
        }

        public BaseModule GetModule(string id) {
            return _modulelList[id];
        }
        public BaseWindow GetWindow(string id)
        {
            return _windowlList.ContainsKey(id) ? _windowlList[id] : null;
        }
        public BaseWindow GetSysWindow(string id)
        {
            return _sysWindowlList[id];
        }
        public BaseWindow GetTopWindow()
        {
            return null;
        }
        public bool IsTopWindow(string id) {
            return false;
        }

        public void RemoveAllWindow(bool removeNow = false) {

            foreach (string id in _windowlList.Keys) {
                _windowlList[id].Dispose();
                _windowlList.Remove(id);
            }

        }
        public void RemoveWindow(string id, bool removeNow = false) {
            GameObject.Destroy(_windowlList[id].UiRoot.gameObject);
            _windowlList.Remove(id);
        }
        public void RemoveModule(string id, bool removeNow = false) { }

        private void InitLayer(Transform transform) {
            
            foreach (Transform view in transform)
            {
                DebugManager.Log("InitLayer:" + view.name);
                if (view.name.Equals("TipLayer"))
                {
                    TipLayer = view;
                }
                else if (view.name.Equals("GuideLayer"))
                {
                    GuideLayer = view;
                }
                else if (view.name.Equals("WindowLayer"))
                {
                    WindowLayer = view;
                }
                else if (view.name.Equals("SysLayer"))
                {
                    SysLayer = view;
                }

                if (view.childCount > 0) {
                    InitLayer(view);
                }
            }
        }

        public void RemoveAllModule(bool removeNow = false) { }
        public void RemoveAllSysWindow(bool removeNow = false) { }
        public void RemoveAllTip() { }
        public void RemoveGuide(bool removeNow = false) { }
        public void RemoveSysWindow(string id, bool removeNow = false) { }
        public void RemoveTip(GameObject go) { }

        public override void Dispose()
        {

        }


    }
}
