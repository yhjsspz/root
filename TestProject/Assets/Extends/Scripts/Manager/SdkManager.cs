using LuaInterface;
using UnityEngine;

namespace CFramework
{
    public class SdkManager : BaseManager<SdkManager>
    {

        public override void Init()
        {
        }

        private LuaFunction callback;
        /// <summary>
        /// 
        /// </summary>
        public void CallSdk(int num, LuaFunction updateCallback)
        {
            callback = updateCallback;
            AndroidJavaClass jc = new AndroidJavaClass("com.test.UnityPlayerActivity");
            AndroidJavaObject jo = jc.GetStatic<AndroidJavaObject>("sdkManager");

            DebugManager.Log("调用sdk:" + jo.ToString());
            string r = jo.Call<string>("androidTest","GameManager", num.ToString(), "BeCallFunc");
            DebugManager.Log("sdk回返回:" + r);
        }

        //设置一个回掉方法
        private void BeCallFunc(string content)
        {
            DebugManager.Log("sdk回回调:" + content);
            callback.Call(content);
        }
    }
}
