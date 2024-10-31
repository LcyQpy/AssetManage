using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using Unity.VisualScripting;
using UnityEngine;


public class test : MonoBehaviour
{
    GameObject Camera1;
    #region
    /**
     * 1.�����ļ�·�� ����ȡ�ļ�������Ϣ
     * 2.����md5�����������Ϣ �����MD5��(�ֽ�����ģʽ)
     * 3.���ֽ�������ʽ��MD5��תΪ 16�����ַ���
     */
    #endregion 
    private void Start()
    {
        GameObject loadedGameObject = new GameObject();
        AssetBundleManager.GetInstance().LoadResAsync("prefabs", "TestCube", obj =>
        {
            loadedGameObject = obj as GameObject;
            if (loadedGameObject != null)
            {
                loadedGameObject.transform.position = -Vector3.up;
                Instantiate(loadedGameObject);
            }
        });
        Debug.Log(AssetBundleManager.GetInstance().dicGet());
        GameObject TestSphere =  AssetBundleManager.GetInstance().LoadRes("prefabs", "TestSphere") as GameObject;
        Destroy(loadedGameObject);
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