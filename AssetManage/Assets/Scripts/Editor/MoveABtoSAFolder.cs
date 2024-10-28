using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class MoveABtoSAFolder
{
    [MenuItem("ABTool/Move to StreamingAssets")]
    private static void MoveABtoSA()
    {
        Object[] selectionAssets =  Selection.GetFiltered(typeof(object),SelectionMode.DeepAssets);
        if (selectionAssets.Length == 0)
        {
            return ;
        }
        string abCompare = "";
        foreach (Object asset in selectionAssets) 
        {
            string asPath = AssetDatabase.GetAssetPath(asset);
            Debug.Log(asPath);
            // 截取路径当中的文件
            string fileName = asPath.Substring(asPath.LastIndexOf('/'));
            // 获取该文件的信息
            FileInfo fileInfo = new FileInfo(Application.streamingAssetsPath + fileName);
            // 判断是否为AB包文件
            if (fileInfo.Extension != "")
                continue;
            // 利用AssetDatabase中的API 将选中文件复制到指定路径
            AssetDatabase.CopyAsset(asPath, Application.streamingAssetsPath + "/"+ fileName);
            abCompare += fileInfo.Name + " " + fileInfo.Length + " " + CreateABCompare.GetMD5(asPath);
            abCompare += "|";
        }
        // 截取最后一个
        abCompare = abCompare.Substring(0, abCompare.Length - 1);
        // 存入本地
        File.WriteAllText(Application.streamingAssetsPath + "/ABCompareInfo.txt", abCompare);
        AssetDatabase.Refresh();
    }
}
