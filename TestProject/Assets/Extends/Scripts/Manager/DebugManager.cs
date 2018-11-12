using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;

namespace CFramework
{
    public class DebugManager : BaseManager<DebugManager>
    {

        public override void Init()
        {
        }

        public static void Log(int msg)
        {
            Log(msg.ToString());
        }

        public static void Log(bool msg)
        {
            Log(msg.ToString());
        }

        public static void Log(string msg)
        {
            Log(msg, "1789B8FF");
        }

        public static void LogWarning(string msg)
        {
            Log(msg, "ffff00");
        }

        public static void LogError(string msg)
        {
            Log(msg, "ff0000");
        }
        
        public static void Log(string msg, string color)
        {
            //if (AppConst.IS_DEBUG == false)  
            //{
            //    return;
            //}
            string msg2 = string.Format("<color=#{0}>{2} ===>{1}</color>", color, msg, System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            Debug.Log(msg2);
            
        }

        internal void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}
