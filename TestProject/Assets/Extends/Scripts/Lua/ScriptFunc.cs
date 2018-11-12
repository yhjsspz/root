using LuaInterface;
using Spine.Unity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace CFramework
{
    public class ScriptFunc : Singleton<ScriptFunc>
    {

        /// <summary>
        /// 添加计时器
        /// </summary>
        /// <param name="tid"></param>
        /// <param name="interval"></param>
        /// <param name="updateCallback"></param>
        /// <param name="endCallback"></param>
        public void AddTimer(string tid, float interval, LuaFunction updateCallback, bool isNow = false)
        {
            TimerManager.Instance.AddTimer(tid, interval, 
                delegate ()
                {
                    if (updateCallback != null)
                    {
                        updateCallback.Call();
                    }
                }, isNow);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tid"></param>
        public void RemoveTimer(string tid)
        {
            TimerManager.Instance.RemoveTimer(tid);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="viewId"></param>
        /// <param name="parent"></param>
        /// <param name="abName"></param>
        /// <param name="assetName"></param>
        /// <param name="callback"></param>
        public void NewSpineAni(string viewId, GameObject parent, string abName, string assetName, LuaFunction callback, string actionName = "", bool isLoop = true)
        {
            ResourceManager.Instance.LoadPrefabAsync(viewId, abName, delegate (AssetBundle assetBundle)
            {
                if (assetBundle == null )
                {
                    DebugManager.LogError("NewSpineAni Error:下载资源失败!" + abName + "," + assetName);
                    return;
                }

                //foreach (UnityEngine.Object obj in assetBundle.LoadAllAssets()) {
                //    DebugManager.Log(obj.name);
                //}

                SkeletonAnimation animation = createSpineAni( parent,  assetBundle,  assetName,  actionName ,  isLoop );

                if (callback != null)
                {
                    callback.Call(animation);
                    callback.Dispose();
                }
                
            });
        }

        public SkeletonAnimation NewSpineAniSync(string viewId, GameObject parent, string abName, string assetName, string actionName = null, bool isLoop = true)
        {
            AssetBundle assetBundle = ResourceManager.Instance.LoadPrefab(viewId, abName);

            return createSpineAni(parent, assetBundle, assetName, actionName, isLoop);
        }

        public SkeletonAnimation createSpineAni(GameObject parent, AssetBundle assetBundle, string assetName, string actionName = "", bool isLoop = true) {

            SkeletonDataAsset skeletonDataAsset = assetBundle.LoadAsset<SkeletonDataAsset>(assetName + "_SkeletonData");

            SkeletonAnimation animation = SkeletonAnimation.NewSkeletonAnimationGameObject(skeletonDataAsset);

            animation.gameObject.name = "Spi_" + assetName;

            if (parent != null)
            {
                animation.gameObject.transform.SetParent(parent.transform);
            }
            
            if (actionName != null)
                animation.state.SetAnimation(0, actionName, isLoop);
            
            return animation;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="state"></param>
        /// <param name="eventType"></param>
        /// <param name="callback"></param>
        public void SetSpineEvent(Spine.AnimationState state, int eventType, LuaFunction callback)
        {
            if (state == null)
                return;

            if (eventType == 0)
            {
                state.Complete += delegate (Spine.TrackEntry trackEntry)
                {
                    callback.Call(trackEntry);
                };
            }
            else
            {
                state.Event += delegate (Spine.TrackEntry trackEntry, Spine.Event e)
                {
                    callback.Call(e.Data.Name);
                };
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="abName"></param>
        /// <param name="assetName"></param>
        /// <param name="actionName"></param>
        /// <param name="isLoop"></param>
        /// <returns></returns>
        public void NewUISpineAni(string viewId, GameObject parent, string abName, string assetName, LuaFunction callback, string actionName = "", bool isLoop = true, bool hasButton = false, string name = null)
        {
            ResourceManager.Instance.LoadPrefabAsync(viewId, abName, delegate (AssetBundle assetBundle)
            {
                if (assetBundle == null)
                {
                    DebugManager.LogError("NewUISpineAni Error:下载资源失败!" + abName + "," + assetName);
                    return;
                }

                SkeletonDataAsset skeletonDataAsset = assetBundle.LoadAsset<SkeletonDataAsset>(assetName + "_SkeletonData");
                SkeletonGraphic graphic = SkeletonGraphic.NewSkeletonGraphicGameObject(skeletonDataAsset, parent.transform);
                graphic.material = new Material(Shader.Find("Spine/SkeletonGraphic (Premultiply Alpha)"));

                if (actionName != "")
                    graphic.AnimationState.SetAnimation(0, actionName, isLoop);

                graphic.gameObject.transform.localPosition = new Vector3(0, 0, 0);

                if (name != null)
                {
                    graphic.gameObject.name = name;
                }
                else {
                    graphic.gameObject.name = "Spi_"+assetName;
                }

                if (hasButton == true)
                {
                    graphic.gameObject.AddComponent<UnityEngine.UI.Button>();
                }

                if (callback != null)
                {
                    callback.Call(graphic);
                    callback.Dispose();
                }
            });
        }

        public void LoadPrefab(string viewId, string abName, string assetName, LuaFunction func) {
            ResourceManager.Instance.LoadPrefabAsync(viewId, abName, delegate(AssetBundle assetBundle) {

                if (assetBundle == null)
                {

                    DebugManager.LogError("LoadPrefab Error result is null:" + abName + "," + assetName.ToString());
                    func.Dispose();
                    func = null;
                    return;
                }
                else {

                    if (func != null) {

                        if (assetName.EndsWith("_sprite"))
                        {
                            string name = assetName.Substring(0, assetName.LastIndexOf("_sprite"));
                            
                            func.Call(TextureToSprite( ResourceManager.Instance.GetTexture2DAsset(assetBundle, name)));
                        }
                        else
                        {
                            func.Call(ResourceManager.Instance.GetAsset(assetBundle, assetName));
                        }
                        
                        func.Dispose();
                        func = null;
                    }


                }

            });
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="go"></param>
        public Sprite TextureToSprite(Texture2D tex)
        {
            return Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), Vector2.zero);
        }

        public DG.Tweening.Ease ConvertToEaseType(int type)
        {
            return (DG.Tweening.Ease)type;
        }
    }
}
