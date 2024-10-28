using NUnit.Framework.Constraints;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

public class Lesson_FTPServer
{
    private static string platForm = "Windows";
    // 47.119.185.213 公网IP
    // 上传
    [MenuItem("ABTool/Upload")]
    private static void UpLoadAllFile()
    {
        // 获取文件夹
        DirectoryInfo directory = Directory.CreateDirectory(Application.dataPath + "/AssetBundle/" + platForm);
        // 获取文件信息
        FileInfo[] fileInfos = directory.GetFiles();
        
        foreach (FileInfo info in fileInfos)
        {
            // 后缀为空以及资源对比文件.txt||info.Name
            if (info.Extension == "" || info.Extension == ".txt")
            {
                // 文件上传服务器
                FTPUpLoadFile(info.FullName, info.Name);
            }
        }
        Debug.Log("上传文件成功");
    }
    // 下载
    private static void DownLoadAllFile()
    {

    }
    /// <summary>
    /// 文件上传服务器
    /// </summary>
    /// <param name="filePath">文件路径</param>
    /// <param name="fileName">文件名</param>
    private async static void FTPUpLoadFile(string filePath, string fileName)
    {
        await Task.Run(() =>
        {
            try
            {
                // 创建FTP连接 用于上传
                FtpWebRequest req = FtpWebRequest.Create(new Uri("ftp://47.119.185.213/AssetBundle/Windows/" + fileName)) as FtpWebRequest;
                // 设置通信凭证
                NetworkCredential n = new NetworkCredential("Linchengyu", "123456");
                req.Credentials = n;
                // 其他代理null
                req.Proxy = null;
                // 请求完毕后 是否关闭控制连接
                req.KeepAlive = false;
                // 操作命令
                req.Method = WebRequestMethods.Ftp.UploadFile;
                // 上传文件类型 bytes
                req.UseBinary = true;
                // ftp 流对象
                Stream upLoadStream = req.GetRequestStream();
                // 读取信息写入该流对象
                using (FileStream file = File.OpenRead(filePath))
                {
                    // 一点一点上传内容
                    byte[] bytes = new byte[2048];
                    // contentLength > 0 文件内有数据
                    int contentLength = file.Read(bytes, 0, bytes.Length);
                    // 循环上传
                    while (contentLength > 0)
                    {
                        // 写入流对象
                        upLoadStream.Write(bytes, 0, contentLength);
                        // 写完再读
                        contentLength = file.Read(bytes, 0, bytes.Length);
                    }
                    // 循环完毕证明上传结束
                    file.Close();
                    upLoadStream.Close();
                    Debug.Log(fileName + "上传成功");
                }
            }
            catch (Exception e) // 异常捕获
            {
                Debug.LogError(fileName + "上传失败：" + e.Message);
            }
        });
    }
}
