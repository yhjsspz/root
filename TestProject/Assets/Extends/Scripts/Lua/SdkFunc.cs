using UnityEngine;

namespace CFramework
{
    public class SdkFunc : Singleton<SdkFunc>
    {

        /// <summary>
        /// 添加计时器
        /// </summary>
        public void CsTest(string title)
        {
            AndroidJavaClass aj = new AndroidJavaClass("com.test.SdkManager");
            int r = aj.CallStatic<int>("androidTest", title);
            DebugManager.Log(r);
        }
        
    }
}
