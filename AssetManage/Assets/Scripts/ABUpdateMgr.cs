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
    // 带下载的AB包列表文件
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
        // 遍历字典的键
        foreach(string name in remoteABinfo.Keys)
        {
            // 放入待下载列表中
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
                // 成功的就弹出
                tempList.Remove(dowmLoadList[i]);
            }
        }
        
    }

    private void OnDestroy()
    {
        instance = null;
    }
    /// <summary>
    /// 下载AB包对比文件, 并将远端信息存入ABinfo
    /// </summary>
    public void DownLoadABCompareFile()
    {
        // 下载远端AB包对比文件到本地
        DownLoadFile("ABCompareInfo.txt", Application.persistentDataPath + "/ABCompareInfo.txt");
        // 获取本地对比文件中的 字符串信息进行对比
        string info = File.ReadAllText(Application.persistentDataPath + "/ABCompareInfo.txt");
        string[] strs = info.Split('|');
        string[] infos =null;
        for(int i = 0; i < strs.Length; i++)
        {
            infos = strs[i].Split(" ");
            // 记录每一个远端AB包的信息
            remoteABinfo.Add(infos[0], new ABInfo(infos[0], infos[1], infos[2]));
        }
        Debug.Log("远程对比文件下载完毕");
    }

    /// <summary>
    /// 根据文件名下载remote指定文件
    /// </summary>
    /// <param name="fileName"></param>
    /// <param name="loaclPath"></param>
    public bool DownLoadFile(string fileName, string loaclPath)
    {
        Debug.Log("fileName:" + fileName);
        try
        {
            // 创建FTP连接 用于下载
            FtpWebRequest req = FtpWebRequest.Create(new Uri("ftp://47.119.185.213/AssetBundle/Windows/" + fileName)) as FtpWebRequest;
            // 设置通信凭证 匿名账号可以不设置 实际开发不建议匿名账号
            //NetworkCredential n = new NetworkCredential("Linchengyu", "123456");
            //req.Credentials = n;

            // 其他代理null
            req.Proxy = null;
            // 请求完毕后 是否关闭控制连接
            req.KeepAlive = false;
            // 操作命令
            req.Method = WebRequestMethods.Ftp.DownloadFile;
            // ftp 流对象
            req.UseBinary = true;
            FtpWebResponse res = req.GetResponse() as FtpWebResponse;
            // 读取信息写入该流对象
            Stream downLoadStream = res.GetResponseStream();
            using (FileStream file = File.Create(loaclPath))
            { 
                // 一点一点下载内容
                byte[] bytes = new byte[2048];
                // contentLength > 0 文件内有数据
                int contentLength = downLoadStream.Read(bytes, 0, bytes.Length);
                // 循环上传
                while (contentLength > 0)
                {
                    // 写入流对象
                    file.Write(bytes, 0, contentLength);
                    // 写完再读
                    contentLength = downLoadStream.Read(bytes, 0, bytes.Length);
                }
                // 循环完毕证明上传结束
                file.Close();
                downLoadStream.Close();
                Debug.Log(fileName + "下载成功");
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
