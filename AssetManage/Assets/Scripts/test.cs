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
     * 1.�����ļ�·�� ����ȡ�ļ�������Ϣ
     * 2.����md5�����������Ϣ �����MD5��(�ֽ�����ģʽ)
     * 3.���ֽ�������ʽ��MD5��תΪ 16�����ַ���
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