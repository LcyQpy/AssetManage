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
    // 47.119.185.213 ����IP
    // �ϴ�
    [MenuItem("ABTool/Upload")]
    private static void UpLoadAllFile()
    {
        // ��ȡ�ļ���
        DirectoryInfo directory = Directory.CreateDirectory(Application.dataPath + "/AssetBundle/" + platForm);
        // ��ȡ�ļ���Ϣ
        FileInfo[] fileInfos = directory.GetFiles();
        
        foreach (FileInfo info in fileInfos)
        {
            // ��׺Ϊ���Լ���Դ�Ա��ļ�.txt||info.Name
            if (info.Extension == "" || info.Extension == ".txt")
            {
                // �ļ��ϴ�������
                FTPUpLoadFile(info.FullName, info.Name);
            }
        }
        Debug.Log("�ϴ��ļ��ɹ�");
    }
    // ����
    private static void DownLoadAllFile()
    {

    }
    /// <summary>
    /// �ļ��ϴ�������
    /// </summary>
    /// <param name="filePath">�ļ�·��</param>
    /// <param name="fileName">�ļ���</param>
    private async static void FTPUpLoadFile(string filePath, string fileName)
    {
        await Task.Run(() =>
        {
            try
            {
                // ����FTP���� �����ϴ�
                FtpWebRequest req = FtpWebRequest.Create(new Uri("ftp://47.119.185.213/AssetBundle/Windows/" + fileName)) as FtpWebRequest;
                // ����ͨ��ƾ֤
                NetworkCredential n = new NetworkCredential("Linchengyu", "123456");
                req.Credentials = n;
                // ��������null
                req.Proxy = null;
                // ������Ϻ� �Ƿ�رտ�������
                req.KeepAlive = false;
                // ��������
                req.Method = WebRequestMethods.Ftp.UploadFile;
                // �ϴ��ļ����� bytes
                req.UseBinary = true;
                // ftp ������
                Stream upLoadStream = req.GetRequestStream();
                // ��ȡ��Ϣд���������
                using (FileStream file = File.OpenRead(filePath))
                {
                    // һ��һ���ϴ�����
                    byte[] bytes = new byte[2048];
                    // contentLength > 0 �ļ���������
                    int contentLength = file.Read(bytes, 0, bytes.Length);
                    // ѭ���ϴ�
                    while (contentLength > 0)
                    {
                        // д��������
                        upLoadStream.Write(bytes, 0, contentLength);
                        // д���ٶ�
                        contentLength = file.Read(bytes, 0, bytes.Length);
                    }
                    // ѭ�����֤���ϴ�����
                    file.Close();
                    upLoadStream.Close();
                    Debug.Log(fileName + "�ϴ��ɹ�");
                }
            }
            catch (Exception e) // �쳣����
            {
                Debug.LogError(fileName + "�ϴ�ʧ�ܣ�" + e.Message);
            }
        });
    }
}
