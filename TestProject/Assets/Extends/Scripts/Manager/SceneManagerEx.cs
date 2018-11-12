using LuaInterface;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace CFramework
{
    public class EventData
    {
        public Spine.AnimationState.TrackEntryEventDelegate eventObj;
        public LuaFunction luaFunc;
        public GameObject obj;
    }

    public class SceneManagerEx:BaseManager<SceneManagerEx>
    {
        private bool _isLoad = true;
        private string _id;
        private bool _isCache;
        private string[] _preloadList;
        private Action<BaseScene> _completeCallback;
        private Action<int> _progressCallback;
        private BaseScene _loadBaseScene;
        private AsyncOperation _prog;
        private bool _preloadIsEnd;
        private LuaTable _luaSceneClass;

        private Dictionary<string, Dictionary<string, EventData>> _eventList = new Dictionary<string, Dictionary<string, EventData>>();

        public override void Init()
        {

        }

        public BaseScene CurrScene { get; set; }
        /// <summary>
        /// 进入指定场景
        /// </summary>
        /// <param name="id"></param>
        /// <param name="preloadList"></param>
        /// <param name="completeCallback"></param>
        /// <param name="errorCallback"></param>
        /// <param name="isCache"></param>
        public void EnterScene(string id, string[] preloadList, LuaTable luaSceneClass, LuaFunction progressCallback,string loadingWindowId, 
                    LuaTable loadingWindowClass , bool isCache = false)
        {
            EnterScene(id, preloadList, luaSceneClass, delegate (BaseScene baseScene)
            {
                //加载完成

            }, delegate (int progress)
            {
                //加载进度
                if (progressCallback != null)
                {
                    progressCallback.Call(progress);
                }
            }, loadingWindowId, loadingWindowClass, isCache);
        }


        public void EnterScene(string id, string[] preloadList, LuaTable luaSceneClass, Action<BaseScene> completeCallback, Action<int> progressCallback, string loadingWindowId,
                    LuaTable loadingWindowClass, bool isCache = false)
        {
            _preloadIsEnd = false;
            _isLoad = false;
            _id = id;
            _preloadList = preloadList;
            _completeCallback = completeCallback;
            _progressCallback = progressCallback;
            _isCache = isCache;
            _luaSceneClass = luaSceneClass;

            SceneManager.LoadScene("LoadingScene");
            StartCoroutine(StartLoadingScene(id, loadingWindowId, loadingWindowClass));
        }

        /// <summary>
        /// 启动loading场景
        /// </summary>
        /// <returns></returns>
        private IEnumerator StartLoadingScene(string sceneId, string loadingWindowId,
                    LuaTable loadingWindowClass) {

            Scene loadingScene = SceneManager.GetSceneByName("LoadingScene");

            yield return new WaitUntil(delegate() {
                return loadingScene.isLoaded;
            });
            
            Debug.Log("LoadingScene complete");

            string assetName = ("prefabs/" + loadingWindowId + ".unity3d").ToLower();

            ResourceManager.Instance.LoadPrefabAsync("LoadingScene", assetName, delegate(AssetBundle asset) {

                //先显示加载窗口
                GameObject loadingWindowAsset = ResourceManager.Instance.GetAsset(asset, loadingWindowId).gameObject;

                loadingWindowAsset.AddComponent<LuaComponent>();
                GameObject loadingWindow = Instantiate(loadingWindowAsset, GameObject.Find("WindowLayer").transform);
                LuaComponent luaComponent = loadingWindow.GetComponent<LuaComponent>();
                luaComponent.New(loadingWindowClass, new BaseWindow(loadingWindowId, null, false, loadingWindow.transform));
                loadingWindow.name = loadingWindowId;


                //启动预加载资源加载
                ResourceManager.Instance.LoadPrefabAsync(_id, _preloadList, delegate(List<AssetBundle> assetBundle) {
                    _preloadIsEnd = true;

                    //启动场景加载
                    _prog = SceneManager.LoadSceneAsync(sceneId);
                    _prog.allowSceneActivation = false;  //如果加载完成，也不进入场景  
                });

                //启动加载进度条
                StartCoroutine(LoadingScene(loadingWindowClass));

            });
        }

        /// <summary>
        /// 加载场景
        /// </summary>
        /// <param name="loadingWindowClass"></param>
        /// <returns></returns>
        private IEnumerator LoadingScene(LuaTable loadingWindowClass)
        {
            
            int showProgress = 80;

            while (showProgress < 100) {

                if (showProgress < 30)
                {
                    showProgress++;

                    if (_progressCallback != null)
                    {
                        _progressCallback(showProgress);
                        loadingWindowClass.Call("OnProgress", loadingWindowClass, showProgress);
                        yield return new WaitForEndOfFrame(); //等待一帧  
                    }
                }
                else if (showProgress < 90)
                {

                    yield return new WaitUntil(delegate ()
                    {
                        return _preloadIsEnd;
                    });

                    showProgress++;

                    if (_progressCallback != null)
                    {
                        _progressCallback(showProgress);
                        loadingWindowClass.Call("OnProgress", loadingWindowClass, showProgress);
                        yield return new WaitForEndOfFrame(); //等待一帧  
                    }

                }
                else {

                    yield return new WaitUntil(delegate ()
                    {
                        return _prog.progress >= 0.9f;
                    });

                    showProgress++;
                    if (_progressCallback != null)
                    {
                        _progressCallback(showProgress);
                        loadingWindowClass.Call("OnProgress", loadingWindowClass, showProgress);
                        yield return new WaitForEndOfFrame(); //等待一帧  
                    }

                }
                
            }

            //-----------------------------预加载资源加载完成，进入场景------------------------------
            _prog.allowSceneActivation = true;  //如果加载完成，可以进入场景  

            Scene scene = SceneManager.GetSceneByName(_id);
            yield return new WaitUntil(delegate() {
                return scene.isLoaded;
            });
            
            DebugManager.Log("enter scene:"+ scene.name);
            
            GameObject[] objs = scene.GetRootGameObjects();
            
            foreach (GameObject go in objs)
            {
                DebugManager.Log("go.name:" + go.name);
                if (go.name.Equals("Canvas"))
                {
                    _loadBaseScene = new BaseScene(_id, _preloadList, _completeCallback, _progressCallback, _isCache, go.transform);
                    LuaComponent luaComponent = go.AddComponent<LuaComponent>();
                    
                    luaComponent.New(_luaSceneClass, _loadBaseScene);

                    _completeCallback(_loadBaseScene);

                    this.CurrScene = _loadBaseScene;

                    _loadBaseScene = null;
                    _id = null;
                    break;
                }
            }



        }

        /// <summary>
        /// 添加一个场景窗口
        /// </summary>
        /// <param name="id"></param>
        /// <param name="preloadList"></param>
        /// <param name="luaWindowClass"></param>
        /// <param name="progressCallback"></param>
        /// <param name="isCahce"></param>
        public void AddWindow(string id, string abName, string assetName, string[] preloadList, bool isModal, LuaTable luaWindowClass, 
                        LuaFunction progressCallback, bool isClickExit, bool isCache = false)
        {
            if (this.CurrScene == null)
            {
                DebugManager.LogError("AddPopup Error:当前没有可用的场景！");

                return;
            }
            
            this.CurrScene.AddWindow(id,abName,assetName,preloadList,isModal,isCache,isClickExit,delegate(BaseWindow window) {

                LuaComponent luaComponent = window.UiRoot.gameObject.AddComponent<LuaComponent>();
                luaComponent.New(luaWindowClass, window);

                if (progressCallback != null)
                {
                    progressCallback.Call();
                }

            },delegate(int count, int total) {

            });
            
        }


        /// <summary>
        /// 添加控件事件
        /// </summary>
        /// <param name="viewId"></param>
        /// <param name="eventId"></param>
        /// <param name="target"></param>
        /// <param name="callback"></param>
        public void AddEvent(string viewId, int eventId, Transform target, LuaFunction callback, string fullEventId = null)
        {
            if (target == null || callback == null)
            {
                DebugManager.LogError("AddEvent Error:target or callback is null！" + viewId + "," + eventId);

                return;
            }

            Dictionary<string, EventData> viewEventList = null;

            this._eventList.TryGetValue(viewId, out viewEventList);

            if (viewEventList == null)
            {
                viewEventList = new Dictionary<string, EventData>();
                this._eventList.Add(viewId, viewEventList);
            }

            if (fullEventId == null)
                fullEventId = viewId+"_"+target.name + "_" + eventId.ToString();

            EventData eventData = new EventData();
            eventData.obj = target.gameObject;
            eventData.luaFunc = callback;

            if (viewEventList.ContainsKey(fullEventId) == true)
            {
                DebugManager.LogError("AddEvent Error:重复的事件ID注册！" + fullEventId);

                return;
            }

            viewEventList.Add(fullEventId, eventData);

            switch (eventId)
            {
                case 1:
                    //click事件
                    Button btn = target.GetComponent<Button>();

                    if (btn == null)
                    {
                        DebugManager.LogError("AddEvent Error:对象没有添加Button组件！" + target.name);
                        return;
                    }

                    btn.onClick.AddListener(
                        delegate ()
                        {
                            callback.Call(target);
                        });
                    break;

                case 2:
                    //Scroll OnValueChanged
                    ScrollRect scrollRect = target.GetComponent<ScrollRect>();

                    if (scrollRect == null)
                    {
                        DebugManager.LogError("AddEvent Error:对象没有添加ScrollRect组件！" + target.name);
                        return;
                    }

                    scrollRect.onValueChanged.AddListener(
                        delegate (Vector2 v)
                        {
                            callback.Call(target, v);
                        });

                    break;

                case 3:

                    Toggle toggle = target.GetComponent<Toggle>();

                    if (toggle == null)
                    {
                        DebugManager.LogError("AddEvent Error:对象没有添加Toggle组件！" + target.name);
                        return;
                    }

                    toggle.onValueChanged.AddListener(
                        delegate (bool check)
                        {
                            callback.Call(target, check);
                        }
                    );

                    break;

                case 4:

                    Slider slider = target.GetComponent<Slider>();

                    if (slider == null)
                    {
                        DebugManager.LogError("AddEvent Error:对象没有添加Slider组件！" + target.name);
                        return;
                    }

                    slider.onValueChanged.AddListener(
                        delegate (float v)
                        {
                            callback.Call(target, v);
                        }
                    );

                    break;

                case 5:

                    //SkeletonGraphic skeleton = target.GetComponent<SkeletonGraphic>();

                    //if (skeleton == null)
                    //{
                    //    DebugManager.Instance.LogError("AddEvent Error:对象没有添加SkeletonGraphic组件！"+ target.name);

                    //    return;
                    //}     

                    //skeleton.AnimationState.Event += delegate (Spine.TrackEntry trackEntry, Spine.Event e)
                    //{
                    //    callback.Call(target, e.Data.Name);
                    //};

                    //skeleton.AnimationState.Complete += delegate (Spine.TrackEntry trackEntry)
                    //{
                    //    callback.Call(target, "End");
                    //};

                    break;

                case 6:
                    break;

                case 7:

                    InputField field = target.GetComponent<InputField>();

                    if (field == null)
                    {
                        DebugManager.LogError("AddEvent Error:对象没有添加InputField组件！" + target.name);

                        return;
                    }

                    field.onEndEdit.AddListener(delegate (string txt) {
                        callback.Call(target, txt);
                    });

                    break;

                case 8:

                    InputField field1 = target.GetComponent<InputField>();

                    if (field1 == null)
                    {
                        DebugManager.LogError("AddEvent Error:对象没有添加InputField组件！" + target.name);

                        return;
                    }

                    field1.onValueChanged.AddListener(delegate (string txt) {
                        callback.Call(target, txt);
                    });

                    break;

                case 9:

                    Dropdown dropdown = target.GetComponent<Dropdown>();

                    if (dropdown == null)
                    {
                        DebugManager.LogError("AddEvent Error:对象没有添加Dropdown组件！" + target.name);

                        return;
                    }

                    dropdown.onValueChanged.AddListener(delegate (int v) {
                        callback.Call(target, v);
                    });

                    break;
            }
        }


        /// <summary>
        /// 移除控件事件
        /// </summary>
        /// <param name="viewId"></param>
        /// <param name="eventId"></param>
        /// <param name="target"></param>
        public void RemoveEvent(string viewId, int eventId, Transform target, string fullEventId = null)
        {
            if (target == null)
            {
                DebugManager.LogError("RemoveEvent Error:target is null！");

                return;
            }

            Dictionary<string, EventData> viewEventList = null;

            this._eventList.TryGetValue(viewId, out viewEventList);

            if (fullEventId == null)
                fullEventId = viewId+"_"+target.name + "_" + eventId.ToString();

            if (viewEventList != null && viewEventList.ContainsKey(fullEventId) == true)
            {
                switch (eventId)
                {
                    case 1:
                        //click事件
                        target.GetComponent<Button>().onClick.RemoveAllListeners();
                        break;

                    case 2:
                        //Scroll OnValueChanged
                        target.GetComponent<ScrollRect>().onValueChanged.RemoveAllListeners();
                        break;

                    case 3:

                        target.GetComponent<Toggle>().onValueChanged.RemoveAllListeners();
                        break;

                    case 4:

                        target.GetComponent<Slider>().onValueChanged.RemoveAllListeners();
                        break;

                    case 5:
                        break;

                    case 6:
                        break;

                    case 7:

                        target.GetComponent<InputField>().onEndEdit.RemoveAllListeners();
                        break;

                    case 8:

                        target.GetComponent<InputField>().onValueChanged.RemoveAllListeners();
                        break;

                    case 9:

                        target.GetComponent<Dropdown>().onValueChanged.RemoveAllListeners();
                        break;
                }

                EventData eventData = viewEventList[fullEventId];
                eventData.obj = null;

                if (eventData.luaFunc != null)
                {
                    //eventData.luaFunc.Dispose();
                    eventData.luaFunc = null;
                }

                viewEventList.Remove(fullEventId);
            }
        }

        /// <summary>
        /// 获取当前弹出窗口
        /// </summary>
        /// <param name="id"></param>
        public BaseWindow GetWindow(string id)
        {
            if (this.CurrScene == null)
            {
                DebugManager.LogError("GetPopup Error:当前没有可用的场景！");

                return null;
            }
            
            return this.CurrScene.GetWindow(id);
        }


        /// <summary>
        /// 移除弹出窗口
        /// </summary>
        /// <param name="id"></param>
        public void RemoveWindow(string id)
        {
            if (this.CurrScene == null)
            {
                DebugManager.LogError("RemovePopup Error:当前没有可用的场景！");

                return;
            }

            this.CurrScene.RemoveWindow(id);
            RemoveViewAllEvent(id);
        }

        /// <summary>
        /// 移除所有弹出窗口
        /// </summary>
        public void RemoveAllWindow()
        {
            if (this.CurrScene == null)
            {
                DebugManager.LogError("RemoveAllPopup Error:当前没有可用的场景！");

                return;
            }

            this.CurrScene.RemoveAllWindow();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="viewId"></param>
        private void RemoveViewAllEvent(string viewId)
        {
            Dictionary<string, EventData> viewEventList = null;

            this._eventList.TryGetValue(viewId, out viewEventList);

            if (viewEventList == null)
                return;

            foreach (EventData eventData in viewEventList.Values)
            {
                eventData.obj = null;

                if (eventData.luaFunc != null)
                {
                    //eventData.luaFunc.Dispose();
                    eventData.luaFunc = null;
                }
            }

            viewEventList.Clear();

            this._eventList.Remove(viewId);
        }

        public void EnterWaitScene() { }
        public virtual void OnSceneUnloaded(string id) { }
        public virtual void OnViewChildDispose(string viewId, GameObject go) { }
        public virtual void OnViewDispose(string id) { }
        public void StartPreload(Action<float, int> progressCallback, Action completeCallback) {

        }
        
        public void Dispose() { }
    }
}
