
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class AssetBundleManager : SingleAutoMono<AssetBundleManager>
{
    // �Ѽ��ص�AB��
    private Dictionary<string, AssetBundleInfo> abDic  = new();
    private AssetBundle MianABundle = null;
    private AssetBundleManifest mianfest = null;

    public Dictionary<string, AssetBundleInfo> dicGet() { 
        return abDic;
    }
    private string PerUrl
    {
        get
        {
            return Application.streamingAssetsPath + "/";
        }
    }
    private string MainABundleName
    {
        get
        {
#if UNITY_IOS
    return "IOS";
#elif UNIY_ANDRODID
    return "Amdroid";
#else
    return "Windows";
#endif
        }
    }

    /// <summary>
    /// ����Ŀ�������������
    /// </summary>
    /// <param name="abName"></param>
    private void LoadAB(string abName)
    {
        if (MianABundle == null)
        {
            // ����AB��
            MianABundle = AssetBundle.LoadFromFile(Application.streamingAssetsPath + $"/{MainABundleName}");
            mianfest = MianABundle.LoadAsset<AssetBundleManifest>("AssetBundleManifest");
        }
        // ��������ȡĿ���������Ϣ
        string[] ReferStr = mianfest.GetDirectDependencies(abName);
        for (int i = 0; i < ReferStr.Length; i++)
        {
            // �ж��������Ƿ񱻼���
            if (!abDic.ContainsKey(ReferStr[i]))
            {
                AssetBundle ab = AssetBundle.LoadFromFile(PerUrl + ReferStr[i]);
                abDic.Add(ReferStr[i], new AssetBundleInfo(ab));
            }
            abDic[ReferStr[i]].m_referAssetBuble.Add(abName);
        }
        // ����Ŀ��� ��Ҫ�ȼ���������
        if (!abDic.ContainsKey(abName)) // û������
        {
            AssetBundle tarAB = AssetBundle.LoadFromFile(PerUrl +abName);
            abDic.Add(abName, new AssetBundleInfo(tarAB));
        }
    }

    /// <summary>
    /// ͬ��������AB����Դ
    /// </summary>
    /// <param name="abName"></param>
    /// <param name="resName"></param>
    /// <returns></returns>

    public Object LoadRes(string abName, string resName)
    {
        LoadAB(abName);
        // ���سɹ�
        var obj = abDic[abName].m_assetBundle.LoadAsset(resName);
        //abDic[abName].m_obj.Add(obj as Object);
        // ����Ŀ����Դ
        return abDic[abName].m_assetBundle.LoadAsset(resName);
    }
    /// <summary>
    /// ͬ��������AB����Դ ������Դ����
    /// </summary>
    /// <param name="abName"></param>
    /// <param name="resName"></param>
    /// <param name="resType"></param>
    /// <returns></returns>
    public Object LoadRes(string abName, string resName, System.Type resType)
    {
        LoadAB(abName);
        // �������� ����Ŀ����Դ
        var obj = abDic[abName].m_assetBundle.LoadAsset(resName, resType);
        abDic[abName].m_obj.Add(obj);
        return abDic[abName].m_assetBundle.LoadAsset(resName, resType);
    }

    /// <summary>
    /// ͬ������ ���ݷ���ָ������
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="abName"></param>
    /// <param name="resName"></param>
    /// <returns></returns>
    public T LoadRes<T>(string abName, string resName) where T :Object
    {
        LoadAB(abName);
        // �������� ����Ŀ����Դ
        var obj = abDic[abName].m_assetBundle.LoadAsset<T>(resName);
        abDic[abName].m_obj.Add(obj);
        return abDic[abName].m_assetBundle.LoadAsset<T>(resName);
    }
    // �첽���� publicĿ���Ǹ��ⲿ����Э��
    public void LoadResAsync(string abName, string resName, UnityAction<Object> callBack)
    {
      StartCoroutine(ReallyLoadResAsync(abName, resName, callBack));
    }
    private IEnumerator ReallyLoadResAsync(string abName, string resName, UnityAction<Object> callBack)
    {
        LoadAB(abName);  // ����������
        AssetBundleRequest request = abDic[abName].m_assetBundle.LoadAssetAsync(resName);
        yield return request;
        abDic[abName].m_obj.Add(request.asset as Object);
        callBack(request.asset);
    }
    // Type
    public void LoadResAsync(string abName, string resName, System.Type type, UnityAction<Object> callBack)
    {
        StartCoroutine(ReallyLoadResAsync(abName, resName,type, callBack));
    }
    private IEnumerator ReallyLoadResAsync(string abName, string resName, System.Type type, UnityAction<Object> callBack)
    {
        LoadAB(abName);  // ����������
        AssetBundleRequest request = abDic[abName].m_assetBundle.LoadAssetAsync(resName, type);
        yield return request;
        abDic[abName].m_obj.Add(request.asset as Object);
        callBack(request.asset);
    }
    // T
    public void LoadResAsync<T>(string abName, string resName, UnityAction<Object> callBack) where T : Object
    {
        StartCoroutine(ReallyLoadResAsync<T>(abName, resName, callBack));
    }
    private IEnumerator ReallyLoadResAsync<T>(string abName, string resName, UnityAction<Object> callBack) where T : Object
    {
        LoadAB(abName);  // ����������
        AssetBundleRequest request = abDic[abName].m_assetBundle.LoadAssetAsync<T>(resName);
        yield return request;
        abDic[abName].m_obj.Add(request.asset as Object);
        callBack(request.asset);
    }

    // ������ж��
    public void UnLoad(string abName)
    {
        var m_list = abDic[abName];
        // obj
        foreach (Object obj in m_list.m_obj)
        {
            if (obj) return;
            abDic[abName].m_obj.Remove(obj);
        }
        foreach (string str in m_list.m_referAssetBuble)
        {
            if (abDic.ContainsKey(str)) return;
            abDic[abName].m_referAssetBuble.Remove(str);
        }
        if (!m_list.m_obj.Any() && !m_list.m_referAssetBuble.Any())
        {
            m_list.m_assetBundle.Unload(false);
            abDic.Remove(abName);
        }
    }

    // ���а�ж��
    public void ClearAllRes()
    {
        AssetBundle.UnloadAllAssetBundles(false);
        abDic.Clear();
        mianfest = null;
        MianABundle = null;
    } 
}

public class AssetBundleInfo
{
    public AssetBundle m_assetBundle; // ��ǰ��
    public List<Object> m_obj = new();  // ���ض���
    public List<string> m_referAssetBuble = new(); // ������
    public AssetBundleInfo(AssetBundle assetBundle)
    {
        m_assetBundle = assetBundle;
    }
}
