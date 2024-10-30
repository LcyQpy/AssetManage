using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;


public class test : MonoBehaviour
{
    public GameObject Camera1;
    #region
    /**
     * 1.根据文件路径 ，获取文件的流信息
     * 2.利用md5对象根据流信息 计算出MD5码(字节数组模式)
     * 3.将字节数组形式的MD5码转为 16进制字符串
     */
    #endregion 
    private void Start()
    {
        //ABUpdateMgr.Instance.DownLoadABCompareFile();
        //ABUpdateMgr.Instance.DownLoadABFile();

        //AssetBundle ab =  AssetBundle.LoadFromFile(Application.dataPath + "/AssetBundle/Windows/prefabs");
        //GameObject obj = ab.LoadAsset("TestSphere") as GameObject;
        //obj.transform.SetParent(Camera1.transform);
        //Instantiate(obj);
        GameObject TestCube = AssetBundleManager.GetInstance().LoadRes("prefabs", "TestCube") as GameObject;
        GameObject TestSphere =  AssetBundleManager.GetInstance().LoadRes("prefabs", "TestSphere") as GameObject;
        TestCube.transform.position = -Vector3.up;
        Instantiate(TestCube);
        Instantiate(TestSphere);
    }
    private string GetMD5(string filePath)
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