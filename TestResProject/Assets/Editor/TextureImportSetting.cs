using UnityEngine;
using System.Collections;
using UnityEditor;

/// <summary>
/// AssetPostprocessor： 贴图、模型、声音等资源导入时调用，可自动设置相应参数
/// 导入图片时自动设置图片的参数
/// </summary>
public class TextureImportSetting : AssetPostprocessor
{

    /// <summary>
    /// 图片导入之前调用，可设置图片的格式、Tag……
    /// </summary>
    void OnPreprocessTexture()
    {
        TextureImporter importer = (TextureImporter)assetImporter;
        importer.textureType = TextureImporterType.Sprite; // 设置为Sprite类型
        importer.mipmapEnabled = false; // 禁用mipmap
        //importer.spritePackingTag = "tag"; // 设置Sprite的打包Tag

        Debug.Log("OnPreprocessTexture");
    }

    /// <summary>
    /// 图片已经被压缩、保存到指定目录下之后调用
    /// </summary>
    /// <param name="texture"></param>
    void OnPostprocessTexure(Texture2D texture)
    {
        Debug.Log(texture.name);
    }

    /// <summary>
    /// 所有资源被导入、删除、移动完成之后调用
    /// </summary>
    /// <param name="importedAssets"></param>
    /// <param name="deletedAssets"></param>
    /// <param name="movedAssets"></param>
    /// <param name="movedFromAssetPaths"></param>
    static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
    {
        foreach (string str in importedAssets)
        {
            Debug.Log("Reimported Asset: " + str);
        }
        foreach (string str in deletedAssets)
        {
            Debug.Log("Deleted Asset: " + str);
        }

        for (int i = 0; i < movedAssets.Length; i++)
        {
            Debug.Log("Moved Asset: " + movedAssets[i] + " from: " + movedFromAssetPaths[i]);
        }
    }
}