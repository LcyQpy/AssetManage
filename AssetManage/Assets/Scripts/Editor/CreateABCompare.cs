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
        // 获取文件夹信息
        DirectoryInfo directory = Directory.CreateDirectory(Application.dataPath + "/AssetBundle/Windows");
        // 获取该目录下的文件信息
        FileInfo[] fileInfos = directory.GetFiles();
        // 用于存储信息的字符串
        string abCompareInfo = "";
        foreach (FileInfo info in fileInfos)
        {
            // 无后缀的才是包资源
            if (info.Extension == "")
            {
                //Debug.Log(info.Name);
                // 拼接一个AB包的信息
                abCompareInfo += info.Name + " " + info.Length + " " + GetMD5(info.FullName);
                // 分隔符
                abCompareInfo += "|";
            }
        }
        // 去除最后一个分隔符
        abCompareInfo = abCompareInfo.Substring(0, abCompareInfo.Length - 1); // Substring 顾头不顾尾 // 
        //Debug.Log(abCompareInfo);
        // 存储拼接好的文件字符串信息
        File.WriteAllText(Application.dataPath + "/AssetBundle/Windows/ABCompareInfo.txt", abCompareInfo);
        // 编辑器刷新
        AssetDatabase.Refresh();
        Debug.Log("AB包对比文件生成成功");
    }
    public static string GetMD5(string filePath)
    {
        // 将文件以流的方式打开
        using (FileStream file = new FileStream(filePath, FileMode.Open))
        {
            // 声明一个MD5对象用于生成MD5码
            MD5 md5 = new MD5CryptoServiceProvider();
            // 利用API得到数据的MD5码 16个字节 数组
            byte[] md5Info = md5.ComputeHash(file);
            StringBuilder sb = new StringBuilder();
            // 关闭流
            file.Close();
            // 16字节转16进制拼接成字符串
            for (int i = 0; i < md5Info.Length; i++)
            {
                sb.Append(md5Info[i].ToString("x2"));
            }
            return sb.ToString();
        }
    }

}
