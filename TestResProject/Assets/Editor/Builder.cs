using UnityEngine;
using System.Collections;
using UnityEditor;
using System.IO;
using System.Collections.Generic;
using System;

/// <summary>  
/// 把Resource下的资源打包成.unity3d 到StreamingAssets目录下  
/// </summary>  
public class Builder : Editor
{
    public static string sourcePath = Application.dataPath + "/res";
    const string AssetBundlesOutputPath = "Assets/StreamingAssets";

    [MenuItem("Tools/AssetBundle    Move")]
    public static void MoveAssetBundle()
    {

        Debug.Log("开始移动");
        string U3D_CODE_ROOT = Environment.GetEnvironmentVariable("U3D_CODE_ROOT");
        string U3D_RES_ROOT = Environment.GetEnvironmentVariable("U3D_RES_ROOT");

        try {

            FileTool.DeleteAll(U3D_CODE_ROOT + "/StreamingAssets");
        }
        catch (Exception e) {

            Debug.Log(e.Message);
        }
        
        FileTool.CopyFolderTo(U3D_RES_ROOT + "/StreamingAssets", U3D_CODE_ROOT + "/StreamingAssets");


        Debug.Log("移动完成");
    }

    [MenuItem("Tools/AssetBundle    Build Windows64")]
    public static void BuildAssetBundleWindows()
    {
        BuildAssetBundle(BuildTarget.StandaloneWindows64);
    }

    [MenuItem("Tools/AssetBundle    Build Android")]
    public static void BuildAssetBundleAndroid()
    {
        BuildAssetBundle(BuildTarget.Android);
    }

    [MenuItem("Tools/AssetBundle    Build IOS")]
    public static void BuildAssetBundleIOS()
    {
        BuildAssetBundle(BuildTarget.iOS);
    }

    public static void BuildAssetBundle(BuildTarget buildTarget)
    {
        Debug.Log("开始打包");
        ClearAssetBundlesName();

        Pack(sourcePath);

        string outputPath = AssetBundlesOutputPath;
        if (!Directory.Exists(outputPath))
        {
            Directory.CreateDirectory(outputPath);
        }

        //根据BuildSetting里面所激活的平台进行打包  
        BuildPipeline.BuildAssetBundles(outputPath, 0, buildTarget);
        
        AssetDatabase.Refresh();

        Debug.Log("打包完成");
        MoveAssetBundle();
    }

    /// <summary>  
    /// 清除之前设置过的AssetBundleName，避免产生不必要的资源也打包  
    /// 之前说过，只要设置了AssetBundleName的，都会进行打包，不论在什么目录下  
    /// </summary>  
    static void ClearAssetBundlesName()
    {
        int length = AssetDatabase.GetAllAssetBundleNames().Length;
        //Debug.Log(length);
        string[] oldAssetBundleNames = new string[length];
        for (int i = 0; i < length; i++)
        {
            oldAssetBundleNames[i] = AssetDatabase.GetAllAssetBundleNames()[i];
        }

        for (int j = 0; j < oldAssetBundleNames.Length; j++)
        {
            AssetDatabase.RemoveAssetBundleName(oldAssetBundleNames[j], true);
        }
        length = AssetDatabase.GetAllAssetBundleNames().Length;
        //Debug.Log(length);
    }

    static void Pack(string source)
    {
        DirectoryInfo folder = new DirectoryInfo(source);
        FileSystemInfo[] files = folder.GetFileSystemInfos();
        int length = files.Length;
        for (int i = 0; i < length; i++)
        {

            if (files[i] is DirectoryInfo)
            {
                Pack(files[i].FullName);
            }
            else
            {
                if (!IsBlockedByExtension(files[i].Name) && !IsBlockedByBlackList(files[i].Name))
                {
                    //Debug.Log("**********"+files[i].FullName);
                    file(files[i].FullName);
                }
            }
        }
    }

    static void file(string source)
    {
        string _source = Replace(source);
        string _assetPath = "Assets" + _source.Substring(Application.dataPath.Length);
        string _assetPath2 = _source.Substring(Application.dataPath.Length + 1);
        //Debug.Log("----------------------------------------------");
        Debug.Log("_source:" + _source);
        Debug.Log("_assetPath:" + _assetPath);
        Debug.Log("_assetPath2:" + _assetPath2);

        if (_assetPath2.StartsWith(""))
        {

        }

        //在代码中给资源设置AssetBundleName  
        AssetImporter assetImporter = AssetImporter.GetAtPath(_assetPath);
        string assetName = _assetPath2;
        assetName = assetName.Replace(Path.GetExtension(assetName), ".unity3d");

        //处理多语言
        assetName = assetName.Replace("Default/","");

        //处理字体、动画打包
        if (assetName.Contains("/Font/") || assetName.Contains("/SpineAni/") || assetName.Contains("/UI/")) {
            assetName = assetName.Substring(0, assetName.LastIndexOf("/")) + ".unity3d";
        }

        Debug.Log("assetName:" + assetName);

        assetImporter.assetBundleName = assetName;
    }

    static string Replace(string s)
    {
        return s.Replace("\\", "/");
    }

     // 黑名单关键字列表，遍历文件夹时忽略   
    static public string[] skip_dirs_all = { "chartlet_chuzheng_03.png" };
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
                //Debug.Log("---------------Compare:" + path+","+ folderNames[i]);
                if (string.Compare(path, folderNames[i], true) == 0)
                    return true;
            }
        }
        return false;
    }

    // 黑名单后缀名列表，遍历文件夹时忽略   
    static public string[] skip_extensions_all = { ".cs", ".meta" };
    /// <summary>  
    /// 判断是否被黑名单后缀名列表过滤  
    /// </summary>  
    static bool IsBlockedByExtension(string filePath)
    {
        List<string> ExtBlackList = new List<string>(skip_extensions_all);
        string extension = Path.GetExtension(filePath);
        foreach (string ext in ExtBlackList)
        {
            if (string.Compare(extension.ToLower(), ext, true) == 0)
                return true;
        }
        return false;
    }
}

public class Platform
{
    public static string GetPlatformFolder(BuildTarget target)
    {
        switch (target)
        {
            case BuildTarget.Android:
                return "Android";
            case BuildTarget.iOS:
                return "IOS";
            //case BuildTarget.WebPlayer:
            //    return "WebPlayer";
            case BuildTarget.StandaloneWindows:
            case BuildTarget.StandaloneWindows64:
                return "Windows";
            //case BuildTarget.StandaloneOSXIntel:
            //case BuildTarget.StandaloneOSXIntel64:
            case BuildTarget.StandaloneOSX:
                return "OSX";
            default:
                return null;
        }
    }
}