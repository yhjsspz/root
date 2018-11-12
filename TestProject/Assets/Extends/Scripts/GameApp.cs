using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CFramework
{
    public class GameApp :Singleton<GameApp>
    {
        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="appName">名称</param>
        /// <param name="appVer">版本</param>
        /// <param name="frameRate">帧率</param>
        /// <param name="designWidth">宽</param>
        /// <param name="designHeight">高</param>
        /// <param name="assetDir"></param>
        /// <param name="isDebug"></param>
        /// <param name="luaBundleMode"></param>
        public void Init(string appName, string appVer, int frameRate, int designWidth, int designHeight, string assetDir, bool isDebug, bool luaBundleMode)
        {
            //初始化管理类

            AppConst.IS_DEBUG = isDebug;
            DebugManager.Instance.Init();
            

        }

        /// <summary>
        /// 启动游戏
        /// </summary>
        public void StartUp()
        {
            OnStartUpComplete();
        }

        /// <summary>
        /// 
        /// </summary>
        public void OnStartUpComplete()
        {
            LuaManager.Instance.Startup();
        }

        /// <summary>
        /// 
        /// </summary>
        public void Dispose()
        {

            DebugManager.Instance.Dispose();
            
        }


    }
}
