using System.IO;
using System.Security.Cryptography;
using System.Text;
using UnityEditor;
using UnityEngine;
public class CreateABCompare
{
    [MenuItem("ABTool/CreateABCompareFile")]
    private static void CreateCompareFile()
    {
        // ��ȡ�ļ�����Ϣ
        DirectoryInfo directory = Directory.CreateDirectory(Application.dataPath + "/AssetBundle/Windows");
        // ��ȡ��Ŀ¼�µ��ļ���Ϣ
        FileInfo[] fileInfos = directory.GetFiles();
        // ���ڴ洢��Ϣ���ַ���
        string abCompareInfo = "";
        foreach (FileInfo info in fileInfos)
        {
            // �޺�׺�Ĳ��ǰ���Դ
            if (info.Extension == "")
            {
                //Debug.Log(info.Name);
                // ƴ��һ��AB������Ϣ
                abCompareInfo += info.Name + " " + info.Length + " " + GetMD5(info.FullName);
                // �ָ���
                abCompareInfo += "|";
            }
        }
        // ȥ�����һ���ָ���
        abCompareInfo = abCompareInfo.Substring(0, abCompareInfo.Length - 1); // Substring ��ͷ����β // 
        //Debug.Log(abCompareInfo);
        // �洢ƴ�Ӻõ��ļ��ַ�����Ϣ
        File.WriteAllText(Application.dataPath + "/AssetBundle/Windows/ABCompareInfo.txt", abCompareInfo);
        // �༭��ˢ��
        AssetDatabase.Refresh();
        Debug.Log("AB���Ա��ļ����ɳɹ�");
    }
    public static string GetMD5(string filePath)
    {
        // ���ļ������ķ�ʽ��
        using (FileStream file = new FileStream(filePath, FileMode.Open))
        {
            // ����һ��MD5������������MD5��
            MD5 md5 = new MD5CryptoServiceProvider();
            // ����API�õ����ݵ�MD5�� 16���ֽ� ����
            byte[] md5Info = md5.ComputeHash(file);
            StringBuilder sb = new StringBuilder();
            // �ر���
            file.Close();
            // 16�ֽ�ת16����ƴ�ӳ��ַ���
            for (int i = 0; i < md5Info.Length; i++)
            {
                sb.Append(md5Info[i].ToString("x2"));
            }
            return sb.ToString();
        }
    }

}
