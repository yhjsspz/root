
using LuaInterface;
using System;
using System.IO;
using UnityEngine;

namespace CFramework
{

    public class LuaManager : BaseManager<LuaManager>
    {
        private LuaState _lua;
        private LuaLoader _loader;
        private LuaLooper _loop = null;

        /// <summary>
        /// 
        /// </summary>
        public void Dispose()
        {
            this.Close();
        }

        /// <summary>
        /// 
        /// </summary>
        public override void Init()
        {
            this._loader = new LuaLoader();
            this._lua = new LuaState();
            this.OpenLibs();
            this._lua.LuaSetTop(0);

            LuaBinder.Bind(this._lua);
            DelegateFactory.Init();
            LuaCoroutine.Register(this._lua, this);
        }

        /// <summary>
        /// 
        /// </summary>
        private void OpenLibs()
        {

            this._lua.OpenLibs(LuaDLL.luaopen_pb);
            this._lua.OpenLibs(LuaDLL.luaopen_protobuf_c);
            this._lua.OpenLibs(LuaDLL.luaopen_lpeg);
#if UNITY_STANDALONE_OSX || UNITY_EDITOR_OSX
            this._lua.OpenLibs(LuaDLL.luaopen_bit);
#endif

            this.OpenCJson();
        }

        /// <summary>
        /// 
        /// </summary>
        private void OpenCJson()
        {
            this._lua.LuaGetField(LuaIndexes.LUA_REGISTRYINDEX, "_LOADED");
            this._lua.OpenLibs(LuaDLL.luaopen_cjson);
            this._lua.LuaSetField(-2, "cjson");

            this._lua.OpenLibs(LuaDLL.luaopen_cjson_safe);
            this._lua.LuaSetField(-2, "cjson.safe");
        }

        /// <summary>
        /// 
        /// </summary>
        public void Startup()
        {
            this._loader.beZip = AppConst.LuaBundleMode;

            this.InitLuaPath();
            
            this.InitLuaBundle();

            this._lua.Start();

            this.StartMain();
        }

        /// <summary>
        /// 
        /// </summary>
        private void InitLuaBundle()
        {
            if (this._loader.beZip == false)
            {
                return;
            }


            string[] files = null;

            if (Application.platform == RuntimePlatform.Android)
            {
                WWW www = new WWW(FileUtil.Instance.GetResPath("file.t"));
                while (!www.isDone) { }

                files = www.text.Split(new string[] { "\r\n" }, StringSplitOptions.None);

                www.Dispose();
            }
            else if (Application.platform == RuntimePlatform.IPhonePlayer) {
                WWW www = new WWW(FileUtil.Instance.GetResPath("file.t"));
                while (!www.isDone) { }

                DebugManager.Log("file.t content:"+ www.text);

                files = www.text.Split(new string[] { "\r\n" }, StringSplitOptions.None);

                DebugManager.Log("files content:" + www.text);
                www.Dispose();
            }
            else
            {
                files = File.ReadAllLines(FileUtil.Instance.GetResPath("file.t"));

            }

            DebugManager.Log("file.t Length:" + files.Length);

            for (int i = 0; i < files.Length; i++)
            {
                string[] keyValue = files[i].Split('|');
                string fileName = keyValue[0];

                DebugManager.Log("for file======>:" + fileName);
                if (Path.GetExtension(fileName) == ".bytes")
                {
                    string bundleName = "";

                    if (fileName.StartsWith("src/tolua/") == true)
                    {
                        bundleName = fileName.Replace("src/tolua/", string.Empty);
                    }
                    else if (fileName.StartsWith("src/gameapp/lua/") == true)
                    {
                        bundleName = fileName.Replace("src/gameapp/lua/", string.Empty);
                    }
                    else if (fileName.StartsWith("src/") == true)
                    {
                        bundleName = fileName.Replace("src/", string.Empty);
                    }
                    //DebugManager.Log("InitLuaBundle:"+ bundleName);
                    string abPath = FileUtil.Instance.GetWritePath(fileName);
                    this._loader.AddBundle(abPath, bundleName);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void InitLuaPath()
        {
            if (AppConst.IS_DEBUG)
            {
                this._loader.AddSearchPath(FileUtil.Instance.GetResPath("src"));
                this._loader.AddSearchPath(FileUtil.Instance.GetResPath("src/tolua"));
                this._loader.AddSearchPath(FileUtil.Instance.GetResPath("src/code"));
                this._loader.AddSearchPath(FileUtil.Instance.GetResPath("src/channel"));
                this._loader.AddSearchPath(FileUtil.Instance.GetResPath("src/config"));
                this._loader.AddSearchPath(FileUtil.Instance.GetResPath("src/gameapp/lua"));
            }
            else
            {
                this._loader.AddSearchPath(FileUtil.Instance.GetWritePath("src"));
                this._loader.AddSearchPath(FileUtil.Instance.GetWritePath("src/tolua"));
                this._loader.AddSearchPath(FileUtil.Instance.GetWritePath("src/code"));
                this._loader.AddSearchPath(FileUtil.Instance.GetWritePath("src/channel"));
                this._loader.AddSearchPath(FileUtil.Instance.GetWritePath("src/config"));
                this._loader.AddSearchPath(FileUtil.Instance.GetWritePath("src/gameapp/lua"));
                
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void StartMain()
        {
            this._lua.DoFile("main.lua");

            LuaFunction main = this._lua.GetFunction("Main");
            main.Call();

            main.Dispose();
            main = null;

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="filename"></param>
        public void DoFile(string filename)
        {
            this._lua.DoFile(filename);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="filename"></param>
        public LuaTable CreateLuaClass(string path, string luaClassName)
        {

            string strCreate = luaClassName + " = require('" + path + "." + luaClassName + "').create()";

            this._lua.DoString(strCreate);
            LuaTable luaClass = this._lua.GetTable(luaClassName);

            return luaClass;

        }

        public void TestLua()
        {
            
            //this._lua.DoFile("test.lua");
            this._lua.DoString(@"
                local cla = {1,2,3,4,5}
                function F1()

                 print(333333333333333333333)


                end

            ");

            //LuaFunction lf = this._lua.GetFunction("Main1");
            //lf.Call();
            
            DebugManager.Log("//////cla////////" + (this._lua.GetTable("cla") == null).ToString());
            //Debug.Log("//////////////" + (this._lua.GetTable("s1") == null).ToString());

            //return this._lua.GetTable("LoginScene");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="funcName"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public object[] CallFunction(string funcName, params object[] args)
        {
            LuaFunction func = this._lua.GetFunction(funcName);
            if (func != null)
            {
                return func.Invoke<object[], object[]>(args);
            }
            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        public void LuaGC()
        {
            this._lua.LuaGC(LuaGCOptions.LUA_GCCOLLECT);
        }

        /// <summary>
        /// 
        /// </summary>
        public void Close()
        {
            //DetachProfiler();

            if (this._loop != null)
            {
                this._loop.Destroy();
                this._loop = null;
            }
            
            if (this._lua != null)
            {
                this._lua.Dispose();
                this._lua = null;
                this._loader = null;
            }
        }
        
    }
}