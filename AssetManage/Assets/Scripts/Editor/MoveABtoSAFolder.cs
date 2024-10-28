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
            // ��ȡ·�����е��ļ�
            string fileName = asPath.Substring(asPath.LastIndexOf('/'));
            // ��ȡ���ļ�����Ϣ
            FileInfo fileInfo = new FileInfo(Application.streamingAssetsPath + fileName);
            // �ж��Ƿ�ΪAB���ļ�
            if (fileInfo.Extension != "")
                continue;
            // ����AssetDatabase�е�API ��ѡ���ļ����Ƶ�ָ��·��
            AssetDatabase.CopyAsset(asPath, Application.streamingAssetsPath + "/"+ fileName);
            abCompare += fileInfo.Name + " " + fileInfo.Length + " " + CreateABCompare.GetMD5(asPath);
            abCompare += "|";
        }
        // ��ȡ���һ��
        abCompare = abCompare.Substring(0, abCompare.Length - 1);
        // ���뱾��
        File.WriteAllText(Application.streamingAssetsPath + "/ABCompareInfo.txt", abCompare);
        AssetDatabase.Refresh();
    }
}
