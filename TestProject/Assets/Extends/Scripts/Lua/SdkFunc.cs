using LuaInterface;
using UnityEngine;

namespace CFramework
{
    public class SdkFunc : Singleton<SdkFunc>
    {

        private LuaFunction callback;
        /// <summary>
        /// 
        /// </summary>
        public void CallSdk(int num, LuaFunction updateCallback)
        {
            callback = updateCallback;
            AndroidJavaClass jc = new AndroidJavaClass("com.test.SdkManager");
            AndroidJavaObject jo = jc.GetStatic<AndroidJavaObject>("currentActivity");
            int r = jo.Call<int>("androidTest", num, "BeCallFunc");
            DebugManager.Log(r);
        }

        //设置一个回掉方法
        private void BeCallFunc(string content)
        {
            callback.Call(content);
        }
    }
}
