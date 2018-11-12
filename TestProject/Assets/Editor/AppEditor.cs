using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using UnityEditor;
using UnityEngine;

namespace CFramework
{
    class AppEditor:Editor
    {
        [MenuItem("GameTools/Create pb config", false, 5)]
        static void GenLuaAll() {
            
            Thread thread = new Thread(new ThreadStart(CreatePBStart));
            thread.Start();
            
        }
        private static void CreatePBStart()
        {

            string U3D_PB_ROOT = Environment.GetEnvironmentVariable("U3D_PB_ROOT");
            string U3D_CODE_ROOT = Environment.GetEnvironmentVariable("U3D_CODE_ROOT");
            UnityEngine.Debug.Log(U3D_PB_ROOT);

            string cmd = "cd " + U3D_PB_ROOT + "\n";

            foreach (FileInfo file in new DirectoryInfo(U3D_PB_ROOT+"\\bin").GetFiles("*.proto"))
            {
                cmd += ".\\protoc3 --descriptor_set_out " + U3D_CODE_ROOT + "\\GameApp\\Protobuf\\" +file.Name.Replace(".proto",".pb") +" bin/"+ file.Name+ " \n";
            }

            UnityEngine.Debug.Log(cmd);
            RunCommand(cmd);
            UnityEngine.Debug.Log("pb文件生成完成");

            try
            {

                string[] lines = File.ReadAllLines(U3D_PB_ROOT + "\\bin\\message.proto");

                string command = "";

                foreach (string line in lines)
                {
                    if (line.EndsWith("IRequest") || line.EndsWith("IResponse")) {
                        string strs = line.Split(' ')[1];
                        command += strs.Split('_')[2].Trim() +","+ strs.Trim() + ",";
                    }
                }
                command = command.Substring(0, command.Length - 1);

                string configPath = U3D_CODE_ROOT + "/GameApp/Protobuf/proto.config";
                Debug.Log("configPath:"+ configPath);
                FileInfo fileInfo = new FileInfo(configPath);
                if (!File.Exists(configPath))
                {
                    fileInfo.Create();
                }
                StreamWriter writer = fileInfo.CreateText();
                writer.WriteLine(command);

                writer.Close();
                UnityEngine.Debug.Log("pb配置生成完成");

            }
            catch (Exception e)
            {

                UnityEngine.Debug.Log(e.Message);
            }
            //try {
            //    string classNames = "";
            //    foreach (FileInfo file in new DirectoryInfo(U3D_CODE_ROOT + "/GameApp/Protobuf").GetFiles("*.pb"))
            //    {

            //        UnityEngine.Debug.Log("pb class:"+ file.Name);
            //        if (!classNames.Equals(""))
            //        {
            //            classNames += ",";
            //        }

            //        classNames += file.Name.Split('.')[0];
            //    }
            //    classNames += "^";

            //    string[] lines = File.ReadAllLines(U3D_PB_ROOT + "\\config.txt");

            //    string command = "";

            //    foreach (string line in lines)
            //    {
            //        if (!command.Equals(""))
            //        {
            //            command += ",";
            //        }
            //        string[] lineData = line.Split('=');
            //        command += lineData[0] + "," + lineData[1];
            //    }

            //    string configPath = U3D_CODE_ROOT + "/GameApp/Protobuf/proto.config";

            //    if (File.Exists(configPath))
            //    {
            //        File.Delete(configPath);
            //    }
            //    StreamWriter writer = new StreamWriter(File.Create(configPath));
            //    writer.WriteLine(classNames + command);

            //    writer.Close();
            //    UnityEngine.Debug.Log("pb配置生成完成");

            //}
            //catch (Exception e)
            //{

            //    UnityEngine.Debug.Log(e.Message);
            //}
        }

        private static void RunCommand(string command)
        {
            System.Diagnostics.Process process = new System.Diagnostics.Process();
            process.StartInfo.FileName = "powershell";
            process.StartInfo.Arguments = command;

            process.StartInfo.CreateNoWindow = true; // 获取或设置指示是否在新窗口中启动该进程的值（不想弹出powershell窗口看执行过程的话，就=true）
            process.StartInfo.ErrorDialog = true; // 该值指示不能启动进程时是否向用户显示错误对话框
            process.StartInfo.UseShellExecute = false;

            process.Start();

            //process.WaitForExit();
            process.Close();
        }

        [MenuItem("GameTools/BuildCode Windows64")]
        static void BuildWindows64()
        {
            Build(BuildTarget.StandaloneWindows64);
        }

        [MenuItem("GameTools/BuildCode Android")]
        static void BuildAndroid()
        {
            Build(BuildTarget.Android);
        }

        [MenuItem("GameTools/BuildCode IOS")]
        static void BuildIOS()
        {
            Build(BuildTarget.iOS);
        }

        static void Build(BuildTarget buildTarget)
        {

            string U3D_CODE_ROOT = Environment.GetEnvironmentVariable("U3D_CODE_ROOT");
            string outputPath = Replace((U3D_CODE_ROOT + "/StreamingAssets/src"));
            string tempPath = Replace((U3D_CODE_ROOT + "/luaroot"));

            FileTool.CopyLuaFolderTo(Replace(U3D_CODE_ROOT + "/GameApp/Lua"), tempPath);
            FileTool.CopyLuaFolderTo(Replace(U3D_CODE_ROOT + "/Extends/Plugins/ToLua/Lua"), tempPath + "/tolua");

            AssetDatabase.Refresh();
            try
            {
                //检查是否存在目的目录  
                if (Directory.Exists(outputPath))
                {
                    FileTool.DeleteAll(outputPath);
                    
                    AssetDatabase.Refresh();
                }
                Directory.CreateDirectory(outputPath);
            }
            catch (Exception e)
            {
                DebugManager.Log(e.Message);
            }

            //FileTool.CopyFolderTo(U3D_CODE_ROOT + "/GameApp/Lua", U3D_CODE_ROOT + "/StreamingAssets/src/lua");
            //FileTool.CopyFolderTo(U3D_CODE_ROOT + "/GameApp/Lua", U3D_CODE_ROOT + "/StreamingAssets/src/lua");
            FileTool.CopyFolderTo(U3D_CODE_ROOT + "/GameApp/Protobuf", U3D_CODE_ROOT + "/StreamingAssets/src/protobuf");

            PackLuaDir(tempPath);

            //根据BuildSetting里面所激活的平台进行打包  
            BuildPipeline.BuildAssetBundles(outputPath, 0, buildTarget);

            AssetDatabase.Refresh();
            //FileTool.DeleteAll(tempPath);

            //生成file文件
            string filePaht = U3D_CODE_ROOT + "/StreamingAssets/file.t";
            if (File.Exists(filePaht)) {
                File.Delete(filePaht);
            }

            StreamWriter streamWriter =  File.CreateText(filePaht);
            
            writeFile(U3D_CODE_ROOT + "\\StreamingAssets", streamWriter, Replace( U3D_CODE_ROOT + "/StreamingAssets/"));
            

            streamWriter.Close();

            //复制json文件
            FileTool.CopyLuaFolderTo(tempPath + "/config/battleSkill", outputPath+ "/gameapp/lua/battleskill");

            //复制pb文件


            Debug.Log("生成lua完成");
        }

        static void PackLuaDir(string source)
        {
            DirectoryInfo folder = new DirectoryInfo(source);
            
            if (folder.Name.Equals("battleSkill")) {
                Debug.Log("PING BI:"+folder.Name);
            }

            FileSystemInfo[] files = folder.GetFileSystemInfos();
            int length = files.Length;
            for (int i = 0; i < length; i++)
            {

                if (files[i] is DirectoryInfo)
                {
                    PackLuaDir(files[i].FullName);
                }
                else
                {
                    if (!IsBlockedByExtension(files[i].Name) && !IsBlockedByBlackList(files[i].Name))
                    {
                        //Debug.Log("**********" + files[i].FullName);
                        PackLuaFile(files[i].FullName);
                    }
                }
            }
        }

        static void PackLuaFile(string source)
        {
            string _source = Replace(source);
            string _assetPath = "Assets" + _source.Substring(Application.dataPath.Length);
            string _assetPath2 = _source.Substring(Application.dataPath.Length + 1);
            //Debug.Log("----------------------------------------------");
            //Debug.Log("_source:" + _source);
            //Debug.Log("_assetPath:" + _assetPath);
            //Debug.Log("_assetPath2:" + _assetPath2);

            //在代码中给资源设置AssetBundleName  
            AssetImporter assetImporter = AssetImporter.GetAtPath(_assetPath);
            string assetName = _assetPath2;

            if (assetName.Contains("/app/") || assetName.Contains("/cframework/") || assetName.Contains("/config/"))
            {
                assetName = assetName.ToString().Replace("luaroot/", "gameapp/lua/");
            }
            else if(assetName.Contains("/tolua/") )
            {
                assetName = assetName.ToString().Replace("luaroot/", "");
            }
            assetName = assetName.Substring(0, assetName.LastIndexOf("/")) + ".bytes";

            //Debug.Log("assetName:" + assetName);

            assetImporter.assetBundleName = assetName;
            
        }

        // 黑名单关键字列表，遍历文件夹时忽略   
        static public string[] skip_dirs_all = { "chartlet_chuzheng_03.png", "battleskill" };
        /// <summary>  
        /// 判断是否被黑名单文件夹列表过滤  
        /// </summary>  
        static bool IsBlockedByBlackList(string filePath)
        {
            List<string> PathblackList = new List<string>(skip_dirs_all);
            string[] folderNames = filePath.Split('/');
            foreach (string path in PathblackList)
            {
                for (int i = 0; i < folderNames.Length; ++i)
                {
                    //Debug.Log("---------------Compare:" + path + "," + folderNames[i]);
                    if (string.Compare(path, folderNames[i], true) == 0)
                        return true;
                }
            }
            return false;
        }
        // 黑名单后缀名列表，遍历文件夹时忽略   
        static public string[] skip_extensions_all = { ".cs", ".json", ".meta" };
        /// <summary>  
        /// 判断是否被黑名单后缀名列表过滤  
        /// </summary>  
        static bool IsBlockedByExtension(string filePath)
        {
            List<string> ExtBlackList = new List<string>(skip_extensions_all);
            string extension = Path.GetExtension(filePath);
            foreach (string ext in ExtBlackList)
            {
                if (string.Compare(extension.ToLower(), ext, true) == 0) {

                    //Debug.Log("BlockedByExtension:" + filePath);
                    return true;
                }
            }
            return false;
        }

        static void writeFile(string dirPath, StreamWriter streamWriter, string preStr) {

            DirectoryInfo directory = new DirectoryInfo(dirPath);
            foreach (FileInfo file in directory.GetFiles()) {
                if (Path.GetExtension(file.Name).Equals(".meta") || 
                    Path.GetExtension(file.Name).Equals(".manifest") ||
                    file.Name.Equals("file.t") ||
                    Path.GetExtension(file.Name).Equals(".json")) {
                    continue;
                }
                streamWriter.WriteLine(Replace(file.FullName).Replace(preStr, "")+"|"+FileTool.GetMD5HashFromFile(file.FullName));
            }

            foreach (DirectoryInfo _directoryInfo in directory.GetDirectories())
            {
                writeFile(_directoryInfo.FullName, streamWriter, preStr);//递归遍历  
            }
        }

        static string Replace(string s)
        {
            return s.Replace("\\", "/");
        }
    }
}
