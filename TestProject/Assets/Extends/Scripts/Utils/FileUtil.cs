using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;

namespace CFramework
{
    public class FileUtil:Singleton<FileUtil>
    {

        /// <summary>
        /// asset
        /// </summary>
        /// <param name="path">文件名.格式名</param>
        /// <returns></returns>
        public string GetAssetsPath(string path) {

            string filePath = "";
            if (Application.platform == RuntimePlatform.Android)
            {
                filePath = Application.dataPath + "/" + path;
            }
            else if (Application.platform == RuntimePlatform.IPhonePlayer)
            {
                filePath = Application.dataPath + "/" + path;
            }
            else
            {
                filePath = "file://" + Application.dataPath + "/" + path;
            }

            return filePath;

        }
        /// <summary>
        /// asset
        /// </summary>
        /// <param name="path">文件名.格式名</param>
        /// <returns></returns>
        public string GetResPath(string path)
        {
            string filePath = "";
            if (Application.platform == RuntimePlatform.Android)
            {
                filePath = Application.streamingAssetsPath + "/" + path;
            }
            else if (Application.platform == RuntimePlatform.IPhonePlayer)
            {
                filePath = "file://" + Application.streamingAssetsPath + "/" + path;
            }
            else {
                filePath = Application.streamingAssetsPath + "/" + path;
            }

            return filePath;
        }
        
        /// <summary>
        /// asset
        /// </summary>
        /// <param name="path">文件名.格式名</param>
        /// <returns></returns>
        public string GetWritePath(string path)
        {

            string filePath = "";
            if (Application.platform == RuntimePlatform.Android)
            {
                filePath = Application.persistentDataPath + "/" + path;
            }
            else if (Application.platform == RuntimePlatform.IPhonePlayer)
            {
                filePath = Application.persistentDataPath + "/" + path;
            }
            else
            {
                filePath = Application.persistentDataPath + "/" + path;
            }

            return filePath;

        }


        /// <summary>
        /// 从一个目录将其内容复制到另一目录
        /// </summary>
        /// <param name="directorySource">源目录</param>
        /// <param name="directoryTarget">目标目录</param>
        public void CopyFolderTo(string directorySource, string directoryTarget)
        {
            if (directorySource.StartsWith("/")) {
                directorySource = directorySource.Substring(1);
            }
            //检查是否存在目的目录  
            if (!Directory.Exists(directorySource))
            {
#if UNITY_ANDROID
#elif UNITY_IPHONE
#else
                DebugManager.Log("==========================创建源目录：");
                Directory.CreateDirectory(directorySource);
#endif
            }
            if (!Directory.Exists(directoryTarget))
            {
                Directory.CreateDirectory(directoryTarget);
            }

            DebugManager.Log("CopyFolderSource:" + directorySource);
            DebugManager.Log("CopyFolderTo:" + directoryTarget);

            //先来复制文件  
            DirectoryInfo directoryInfo = new DirectoryInfo(directorySource);
            FileInfo[] files = directoryInfo.GetFiles();
            //复制所有文件  
            foreach (FileInfo file in files)
            {
                string toPath = directoryTarget;
                if (!Directory.Exists(toPath))
                {
                    Directory.CreateDirectory(toPath);
                }
                file.CopyTo(Path.Combine(toPath, file.Name), true);
            }
            //最后复制目录  
            DirectoryInfo[] directoryInfoArray = directoryInfo.GetDirectories();
            foreach (DirectoryInfo dir in directoryInfoArray)
            {
                CopyFolderTo(Path.Combine(directorySource, dir.Name), Path.Combine(directoryTarget, dir.Name));
            }
        }
    }
}
