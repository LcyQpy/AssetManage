using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using UnityEngine;

public class ABUpdateMgr : MonoBehaviour
{
    private static ABUpdateMgr instance;
    private Dictionary<string, ABInfo> remoteABinfo = new();
    // �����ص�AB���б��ļ�
    private List<string> dowmLoadList = new();
    public static ABUpdateMgr Instance
    {
        get
        {
            if (instance == null)
            {
                GameObject obj = new GameObject("ABUpdateMgr");
                instance = obj.AddComponent<ABUpdateMgr>();
            }
            return instance;
        }
    }

    public void Test()
    {
        Debug.Log("Test ABMgr");
    }
    public async void DownLoadABFile()
    {
        // �����ֵ�ļ�
        foreach(string name in remoteABinfo.Keys)
        {
            // ����������б���
            dowmLoadList.Add(name);
        }
        var tempList = dowmLoadList;
        string localPath = Application.persistentDataPath + "/";
        for (int i = 0; i < dowmLoadList.Count; i++)
        {
            bool isOver = false;
            await Task.Run(() =>{
                isOver = DownLoadFile(dowmLoadList[i], localPath +  dowmLoadList[i]);
            });
            if (isOver)
            {
                // �ɹ��ľ͵���
                tempList.Remove(dowmLoadList[i]);
            }
        }
        
    }

    private void OnDestroy()
    {
        instance = null;
    }
    /// <summary>
    /// ����AB���Ա��ļ�, ����Զ����Ϣ����ABinfo
    /// </summary>
    public void DownLoadABCompareFile()
    {
        // ����Զ��AB���Ա��ļ�������
        DownLoadFile("ABCompareInfo.txt", Application.persistentDataPath + "/ABCompareInfo.txt");
        // ��ȡ���ضԱ��ļ��е� �ַ�����Ϣ���жԱ�
        string info = File.ReadAllText(Application.persistentDataPath + "/ABCompareInfo.txt");
        string[] strs = info.Split('|');
        string[] infos =null;
        for(int i = 0; i < strs.Length; i++)
        {
            infos = strs[i].Split(" ");
            // ��¼ÿһ��Զ��AB������Ϣ
            remoteABinfo.Add(infos[0], new ABInfo(infos[0], infos[1], infos[2]));
        }
        Debug.Log("Զ�̶Ա��ļ��������");
    }

    /// <summary>
    /// �����ļ�������remoteָ���ļ�
    /// </summary>
    /// <param name="fileName"></param>
    /// <param name="loaclPath"></param>
    public bool DownLoadFile(string fileName, string loaclPath)
    {
        Debug.Log("fileName:" + fileName);
        try
        {
            // ����FTP���� ��������
            FtpWebRequest req = FtpWebRequest.Create(new Uri("ftp://47.119.185.213/AssetBundle/Windows/" + fileName)) as FtpWebRequest;
            // ����ͨ��ƾ֤ �����˺ſ��Բ����� ʵ�ʿ��������������˺�
            //NetworkCredential n = new NetworkCredential("Linchengyu", "123456");
            //req.Credentials = n;

            // ��������null
            req.Proxy = null;
            // ������Ϻ� �Ƿ�رտ�������
            req.KeepAlive = false;
            // ��������
            req.Method = WebRequestMethods.Ftp.DownloadFile;
            // ftp ������
            req.UseBinary = true;
            FtpWebResponse res = req.GetResponse() as FtpWebResponse;
            // ��ȡ��Ϣд���������
            Stream downLoadStream = res.GetResponseStream();
            using (FileStream file = File.Create(loaclPath))
            { 
                // һ��һ����������
                byte[] bytes = new byte[2048];
                // contentLength > 0 �ļ���������
                int contentLength = downLoadStream.Read(bytes, 0, bytes.Length);
                // ѭ���ϴ�
                while (contentLength > 0)
                {
                    // д��������
                    file.Write(bytes, 0, contentLength);
                    // д���ٶ�
                    contentLength = downLoadStream.Read(bytes, 0, bytes.Length);
                }
                // ѭ�����֤���ϴ�����
                file.Close();
                downLoadStream.Close();
                Debug.Log(fileName + "���سɹ�");
            }
            return true;
        }
        catch(Exception ex)
        {
            Debug.LogError(ex.Message);
            return false;
        }
        
    }   
    private class ABInfo
    {
        public string name;
        public long size;
        public string md5;
        public ABInfo(string name, string size, string md5)
        {
            this.name = name;
            this.size = long.Parse(size);
            this.md5 = md5;
        }
    }
}
