using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace CFramework
{
    public class AppConst
    {
        //是否为调试模式
        //public static bool IS_DEBUG = false;
        public static bool IS_DEBUG = Application.platform == RuntimePlatform.WindowsEditor;
        //是否在search path 中查找读取lua文件。否则从外部设置过来bundel文件中读取lua文件
        public static bool LuaBundleMode = !IS_DEBUG;   
        //public static bool LuaBundleMode = false;
        public static int DesignHeight = 750;
        public static int DesignWidth = 1334;
        
    }
}
