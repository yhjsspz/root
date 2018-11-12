using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

public class FileTool:Editor{
    
    /// <summary>
    /// 返回指定目录下的所有文件信息
    /// </summary>
    /// <param name="strDirectory"></param>
    /// <returns></returns>
    public static List<FileInfo> GetAllFilesInDirectory(string strDirectory)
    {
        List<FileInfo> listFiles = new List<FileInfo>(); //保存所有的文件信息  
        DirectoryInfo directory = new DirectoryInfo(strDirectory);
        DirectoryInfo[] directoryArray = directory.GetDirectories();
        FileInfo[] fileInfoArray = directory.GetFiles();
        if (fileInfoArray.Length > 0) listFiles.AddRange(fileInfoArray);
        foreach (DirectoryInfo _directoryInfo in directoryArray)
        {
            DirectoryInfo directoryA = new DirectoryInfo(_directoryInfo.FullName);
            //DirectoryInfo[] directoryArrayA = directoryA.GetDirectories();
            FileInfo[] fileInfoArrayA = directoryA.GetFiles();
            if (fileInfoArrayA.Length > 0) listFiles.AddRange(fileInfoArrayA);
            GetAllFilesInDirectory(_directoryInfo.FullName);//递归遍历  
        }
        return listFiles;
    }

    public static void DeleteAll(string directorySource) {

        Directory.Delete(directorySource, true);
        //File.Delete();
    }


    /// <summary>
    /// 从一个目录将其内容复制到另一目录
    /// </summary>
    /// <param name="directorySource">源目录</param>
    /// <param name="directoryTarget">目标目录</param>
    public static void CopyFolderTo(string directorySource, string directoryTarget)
    {
        //检查是否存在目的目录  
        if (!Directory.Exists(directorySource))
        {
            Directory.CreateDirectory(directorySource);
        }
        if (!Directory.Exists(directoryTarget))
        {
            Directory.CreateDirectory(directoryTarget);
        }
        //先来复制文件  
        DirectoryInfo directoryInfo = new DirectoryInfo(directorySource);
        FileInfo[] files = directoryInfo.GetFiles();
        //复制所有文件  
        foreach (FileInfo file in files)
        {
            string toPath = directoryTarget;
            //Debug.Log("toPath:" + toPath);
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
    /// <summary>
    /// 从一个目录将其内容移动到另一目录  
    /// </summary>
    /// <param name="directorySource">源目录</param>
    /// <param name="directoryTarget">目标目录</param>
    public static void MoveFolderTo(string directorySource, string directoryTarget)
    {
        //检查是否存在目的目录  
        if (!Directory.Exists(directoryTarget))
        {
            Directory.CreateDirectory(directoryTarget);
        }
        //先来移动文件  
        DirectoryInfo directoryInfo = new DirectoryInfo(directorySource);
        FileInfo[] files = directoryInfo.GetFiles();
        //移动所有文件  
        foreach (FileInfo file in files)
        {
            //如果自身文件在运行，不能直接覆盖，需要重命名之后再移动  
            if (File.Exists(Path.Combine(directoryTarget, file.Name)))
            {
                if (File.Exists(Path.Combine(directoryTarget, file.Name + ".bak")))
                {
                    File.Delete(Path.Combine(directoryTarget, file.Name + ".bak"));
                }
                File.Move(Path.Combine(directoryTarget, file.Name), Path.Combine(directoryTarget, file.Name + ".bak"));

            }
            file.MoveTo(Path.Combine(directoryTarget, file.Name));

        }
        //最后移动目录  
        DirectoryInfo[] directoryInfoArray = directoryInfo.GetDirectories();
        foreach (DirectoryInfo dir in directoryInfoArray)
        {
            MoveFolderTo(Path.Combine(directorySource, dir.Name), Path.Combine(directoryTarget, dir.Name));
        }
    }
}
