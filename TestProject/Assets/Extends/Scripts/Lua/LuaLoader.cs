using LuaInterface;
using System.IO;
using UnityEngine;

using System.Text;

namespace CFramework
{
    public class LuaLoader : LuaFileUtils
    {
        /// <summary>
        /// 
        /// </summary>
        public LuaLoader()
        {
            instance = this;
        }

        /// <summary>
        /// 添加打入Lua代码的AssetBundle
        /// </summary>
        /// <param name="bundle"></param>
        public void AddBundle(string path, string bundleName)
        {
            AssetBundle bundle = null;

            if (Application.platform == RuntimePlatform.Android)
            {
                if (path.StartsWith("file://"))
                    path = path.Replace("file://", string.Empty);

                bundle = AssetBundle.LoadFromFile(path);
            }
            else if (Application.platform == RuntimePlatform.IPhonePlayer)
            {
                if (path.StartsWith("file://"))
                    path = path.Replace("file://", string.Empty);

                bundle = AssetBundle.LoadFromFile(path);
            }
            else
            {
                if (File.Exists(path) == true)

                    bundle = AssetBundle.LoadFromMemory(File.ReadAllBytes(path));
            }

            if (bundle != null)
            {
                base.AddSearchBundle(bundleName.ToLower(), bundle);
            }
            else {
                DebugManager.LogError("AddSearchBundle Error:" + path+","+ bundleName);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="bundleName"></param>
        /// <param name="asset"></param>
        public void AddBundle(string bundleName, AssetBundle bundle)
        {
            if (bundle != null)
            {
                base.AddSearchBundle(bundleName.ToLower(), bundle);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public override byte[] ReadFile(string fileName)
        {
            if (!beZip)
            {
                string path = FindFile(fileName);
                byte[] str = null;

                if (!string.IsNullOrEmpty(path) && File.Exists(path))
                {
#if !UNITY_WEBPLAYER
                    str = File.ReadAllBytes(path);
#else
                    throw new LuaException("can't run in web platform, please switch to other platform");
#endif
                }

                return str;
            }
            else
            {

                return this.GetLuaBytes(fileName);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        private byte[] GetLuaBytes(string fileName)
        {
            DebugManager.Log("GetLuaBytes:" + fileName);
            byte[] buffer = null;

            fileName = fileName.ToLower();

            if (!fileName.EndsWith(".lua"))
            {
                fileName += ".lua";
            }

            fileName = fileName.Replace(".lua", ".bytes");

            string assetName = fileName;

            int pos = fileName.LastIndexOf('/');

            if (pos > 0)
            {
                fileName = fileName.Substring(0, pos) + ".bytes";
                assetName = assetName.Substring(pos + 1);
            }

            AssetBundle zipFile = null;
            TextAsset luaCode = null;
            
            if (zipMap.ContainsKey(fileName) == true)
                zipFile = zipMap[fileName];

            if (zipFile == null)
            {
                string[] rootArr = new string[] { "tolua.bytes", "luaroot.bytes" };

                for (int i = 0; i < rootArr.Length; i++)
                {
                    zipFile = zipMap[rootArr[i]];
                    luaCode = zipFile.LoadAsset<TextAsset>(assetName);

                    if (luaCode != null)
                        break;
                }
            }
            else
            {
                luaCode = zipFile.LoadAsset<TextAsset>(assetName);
            }

            if (luaCode != null)
            {
                buffer = luaCode.bytes;
                Resources.UnloadAsset(luaCode);
            }

            return buffer;
        }
    }
}
