using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace CFramework
{
    public class StartUpManager : BaseManager<StartUpManager>
    {

        private int _copyNum;
        private int _copyNumCur;
        private Text _Txt_msg;

        public override void Init()
        {
        }

        public void StartUp()
        {
            
            SceneManager.LoadScene("InitScene");
            StartCoroutine(EnterInitScene());
            
        }

        IEnumerator EnterInitScene()
        {
            Scene initScene = SceneManager.GetSceneByName("InitScene");
            yield return new WaitUntil(delegate() {
                return initScene.isLoaded;
            });

            _Txt_msg = GameObject.Find("Txt_msg").GetComponent<Text>();

            //先解压资源
            Compress();
        }

        /// <summary>
        /// 解压安装包资源
        /// </summary>
        public void Compress() {


            //if (Application.platform == RuntimePlatform.WindowsEditor)
            //{
            //    _Txt_msg.text = "编辑模式不拷贝文件,不更新，直接进入程序初始化";
            //    DebugManager.Log(_Txt_msg.text);
            //    InitApp();
            //    return;
            //}

            _Txt_msg.text = "开始解压安装包资源";
            DebugManager.Log(_Txt_msg.text);

            FileInfo fi = new FileInfo(FileUtil.Instance.GetWritePath("file.t"));

            DebugManager.Log("是否初始化：" + fi.Exists);
            DebugManager.Log("file：" + fi.FullName);
            //判断文件是否存在  
            if (!fi.Exists)
            {
                DebugManager.Log("================read file.t====================");
                DebugManager.Log(FileUtil.Instance.GetResPath("file.t"));
                WWW www = new WWW(FileUtil.Instance.GetResPath("file.t"));
                while (!www.isDone) { }

                string[] files = www.text.Split(new string[] { "\r\n" }, StringSplitOptions.None);

                www.Dispose();

                _copyNum = files.Length;
                _copyNumCur = 0;

                CopyStreamingFileToPersistentDataPath(files, delegate() {
                    //资源解压完成

                    //最后复制file.t
                    string[] tmp = new string[1];
                    tmp[0] = "file.t";
                    CopyStreamingFileToPersistentDataPath(tmp, delegate() {
                        StartCoroutine(StartUpdate());

                    });

                    
                });
            }
            else
            {

                //TODO 检查更新
                if (Application.platform == RuntimePlatform.WindowsEditor)
                {
                    StartCoroutine(StartUpdate());
                    //InitApp();
                }
                else
                {
                    StartCoroutine(StartUpdate());
                }
            }

            
        }

        /// <summary>  
        /// 检查游戏更新
        /// </summary> 
        IEnumerator StartUpdate() {

            _Txt_msg.text = "解压安装包资源完成, 开始检查游戏更新";
            DebugManager.Log(_Txt_msg.text);

            string cdnUrl = "file://F:/LocalSvn/TestCdnServer/";

            WWW www = new WWW(cdnUrl + "version.t");

            yield return www;

            DebugManager.Log("server version:"+www.text);
            
            string serverVersion = www.text;

            www.Dispose();

            www = new WWW(FileUtil.Instance.GetWritePath("version.t"));
            
            yield return www;

            if (serverVersion.Equals(www.text))
            {
                //版本一致，进入下一步
                _Txt_msg.text = "更新版本完成";
                DebugManager.Log(_Txt_msg.text);
                InitApp();
            }
            else {

                _Txt_msg.text = "检测到新版本，准备更新";
                DebugManager.Log(_Txt_msg.text);
                CopyFiles(cdnUrl, new string[] { "file.t" } , delegate() {

                    StartCoroutine(CheckAndUpdateFile());
                });
            }
            www.Dispose();
        }

        /// <summary>  
        /// 检测md5并更新文件
        /// </summary> 
        IEnumerator CheckAndUpdateFile() {

            //从服务器下载file.t, 检测文件是否存在和有没改变过，如果有变化则从服务器重新下载该文件并覆盖
            string cdnUrl = "file://F:/LocalSvn/TestCdnServer/";

            WWW www = new WWW(FileUtil.Instance.GetWritePath("file.t"));

            yield return www;

            DebugManager.Log("打开file.t:" + FileUtil.Instance.GetWritePath("file.t"));
            DebugManager.Log("打开file.t:" + www.text);
            string[] files = www.text.Split(new string[] { "\r\n" }, StringSplitOptions.None);
            //TODO 检验md5
            DebugManager.Log("检测md5并更新文件:"+ www.text);
            CopyFiles(cdnUrl, files, delegate() {
                InitApp();
            });

            www.Dispose(); 
        }


        /// <summary>  
        /// 初始化游戏
        /// </summary> 
        public void InitApp() {

            _Txt_msg.text = "开始初始化游戏，准备进入游戏";
            DebugManager.Log(_Txt_msg.text);

            SceneManagerEx.Instance.Init();
            ResourceManager.Instance.Init();
            LuaManager.Instance.Init();
            NetworkManager.Instance.Init();
            TimerManager.Instance.Init();

            LuaManager.Instance.Startup();


            //NetworkManager.Instance.ConnectServer("127.0.0.1", 8088);
            //NetworkManager.Instance.SendMessage(10011, "");

        }

        /// <summary>  
        /// 将StreamingAssets里的文件复制到PersistentDataPath下  
        /// </summary>  
        public void CopyStreamingFileToPersistentDataPath(string[] files, Action completeAction) {
            CopyFiles(FileUtil.Instance.GetResPath(""), files, completeAction);
        }

        public void CopyFiles(string sourcePath, string[] files, Action completeAction)
        {
            if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer)                    //如果是Android平台  
            {
                foreach (string path in files)
                {
                    StartCoroutine(CopyFile_Android(sourcePath, path.Split('|')[0], completeAction));
                }
            }
            else if (Application.platform == RuntimePlatform.WindowsEditor || Application.platform == RuntimePlatform.WindowsPlayer)          //如果是Windows平台  
            {
                sourcePath = sourcePath.Replace("file://", "");
                foreach (string path in files)
                {
                    string _path = path.Split('|')[0];
                    if (path.Equals("")) {
                        continue;
                    }
                    try
                    {
                        DebugManager.Log("Start CopyFile ======>"+ sourcePath +_path + " to:"+FileUtil.Instance.GetWritePath(_path));

                        string toPath = FileUtil.Instance.GetWritePath(_path);
                        string toDir = toPath.Substring(0, toPath.LastIndexOf("/"));

                        if (!Directory.Exists(toDir))
                        {
                            Directory.CreateDirectory(toDir);
                        }

                        File.Copy(sourcePath+ _path, FileUtil.Instance.GetWritePath( _path), true);
                    }
                    catch (Exception e)
                    {
                        DebugManager.LogError("Copy file Error:" + path);
                        DebugManager.LogError(e.Message);
                    }
                    DebugManager.Log("CopyFile Success! ======> " + FileUtil.Instance.GetWritePath(_path));
                }
                completeAction();
            }

        }
        
        /// <summary>  
        /// 安卓端复制文件  
        /// </summary>  
        /// <param name="fileName">文件路径</param>  
        /// <returns></returns>  
        IEnumerator CopyFile_Android(string path, string fileName, Action completeAction)
        {

            string sourcePath = path+fileName;
            string toPath = FileUtil.Instance.GetWritePath(fileName);

            DebugManager.Log("Start CopyFile ======>" + sourcePath + " to:" + toPath);

            WWW w = new WWW(sourcePath);

            yield return w;
            
            if (w.error == null)
            {
                
                FileInfo fi = new FileInfo(toPath);
                DirectoryInfo dictionary = fi.Directory;

                if (!dictionary.Exists)
                {
                    DebugManager.Log("create dir:"+ dictionary.Name);
                    dictionary.Create();
                }


                //判断文件是否存在  
                if (!fi.Exists)
                {

                    FileStream fs = fi.Create();

                    fs.Write(w.bytes, 0, w.bytes.Length);

                    fs.Flush();

                    fs.Close();

                    fs.Dispose();

                    DebugManager.Log("----- CopyFile Success! ======> " + fi.FullName);

                }

            }
            else
            {
                DebugManager.Log("Error : ======> " + w.error);
            }
            _copyNumCur++;
            DebugManager.Log("+++++++++++++++++++++++progress:" + _copyNumCur + "/" + _copyNum);
            if (_copyNumCur == _copyNum) {
                completeAction();
            }
            w.Dispose();
        }
    }
}
