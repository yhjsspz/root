using System.Collections.Generic;
using UnityEngine;

namespace CFramework
{
    public abstract class BaseView
    {

        protected string _id;
        protected bool _isCache;
        protected bool _isActive;
        protected string[] _preloadList;
        protected Transform _uiRoot;
        protected Dictionary<string, Transform> _controlList;
        protected MonoBehaviour _luaBehaviour;
        protected List<string> _abCacheList;


        public Transform UiRoot { get; }
        public bool IsActive { get; set; }
        public string[] PreloadList { get; set; }
        public bool IsCache { get; set; }
        public string Id { get; set; }
        public MonoBehaviour LuaBehaviour { get; set; }
        public Dictionary<string, Transform> ControlList { get; }

        public BaseView(string id, string[] preloadList, bool isCache, Transform go)
        {
            _id = id;
            _preloadList = preloadList;
            _isCache = IsCache;
            _uiRoot = go;

            Id = id;
            UiRoot = go;

            _controlList = new Dictionary<string, Transform>();
            ControlList = _controlList;

            initView(go.transform);
        }

        private void initView(Transform go) {

            foreach (Transform t in go) {

                if (!_controlList.ContainsKey(t.name)) {
                    _controlList.Add(t.name, t);
                }

                if (t.childCount > 0) {
                    initView(t);
                }

            }

        }

        public Transform AddChild(Transform go, string preStr = null) {
            
            return AddChild(UiRoot, go, preStr);
        }

        public Transform AddChild(Transform parent, Transform go, string preStr = null) {

            DebugManager.Log("AddChild:"+go.name+","+preStr);
            if (preStr != null)
            {
                go.name = preStr + "_" + go.name;
                formatChildName(go, preStr);
            }
            
            go.transform.parent = parent.transform;

            initView(go);

            LuaComponent luaComponent = _uiRoot.GetComponent<LuaComponent>();
            luaComponent.UpdateControlBind();

            return go;
        }

        private void formatChildName(Transform parent, string preStr) {

            foreach (Transform child in parent) {

                DebugManager.Log("child:" + child.name);
                child.name = preStr + "_" + child.name;

                if (child.childCount > 0) {
                    formatChildName(child, preStr);
                }
            }
        }

        public GameObject GetChild(string id) {
            return _controlList[id].gameObject;
        }
        public void RemoveAllChild(GameObject parent) {
            for (var i = 0; i < parent.transform.childCount; ++i) {
                var c = parent.transform.GetChild(i).gameObject;
                GameObject.Destroy(c);
            }
        }
        public void RemoveChild(GameObject go) {

            GameObject.Destroy(go);

        }

        public abstract void Dispose();


    }
}
