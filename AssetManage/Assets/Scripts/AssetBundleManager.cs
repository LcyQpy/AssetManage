
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class AssetBundleManager : SingleAutoMono<AssetBundleManager>
{
    // �Ѽ��ص�AB��
    private Dictionary<string, AssetBundle> abDic  = new Dictionary<string, AssetBundle>();
    private AssetBundle MianABundle = null;
    private AssetBundleManifest mianfest = null;
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
            // �жϰ��Ƿ񱻼���
            if (!abDic.ContainsKey(ReferStr[i]))
            {
                AssetBundle ab = AssetBundle.LoadFromFile(PerUrl + ReferStr[i]);
                abDic.Add(ReferStr[i], ab);
            }
        }
        // ����Ŀ��� ��Ҫ�ȼ���������
        if (!abDic.ContainsKey(abName))
        {
            AssetBundle tarAB = AssetBundle.LoadFromFile(PerUrl +abName);
            abDic.Add(abName, tarAB);
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
        // ����Ŀ����Դ
        return abDic[abName].LoadAsset(resName);
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
        return abDic[abName].LoadAsset(resName, resType);
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
        return abDic[abName].LoadAsset<T>(resName);
    }
    // �첽���� publicĿ���Ǹ��ⲿ����Э��
    public void LoadResAsync(string abName, string resName, UnityAction<Object> callBack)
    {
      StartCoroutine(ReallyLoadResAsync(abName, resName, callBack));
    }
    private IEnumerator ReallyLoadResAsync(string abName, string resName, UnityAction<Object> callback)
    {
        LoadAB(abName);
        // ����Ŀ����Դ
        yield return abDic[abName].LoadAssetAsync(resName); ;
    }
    // Type
    public void LoadResAsync(string abName, string resName, System.Type type, UnityAction<Object> callBack)
    {
        StartCoroutine(ReallyLoadResAsync(abName, resName,type, callBack));
    }
    private IEnumerator ReallyLoadResAsync(string abName, string resName, System.Type type, UnityAction<Object> callback)
    {
        LoadAB(abName);
        // ����Ŀ����Դ
        yield return abDic[abName].LoadAssetAsync(resName, type); 
    }
    // T
    public void LoadResAsync<T>(string abName, string resName, UnityAction<Object> callBack) where T : Object
    {
        StartCoroutine(ReallyLoadResAsync<T>(abName, resName, callBack));
    }
    private IEnumerator ReallyLoadResAsync<T>(string abName, string resName, UnityAction<Object> callback)
    {
        LoadAB(abName);
        // ����Ŀ����Դ
        yield return abDic[abName].LoadAssetAsync<T>(resName); ;
    }

    // ������ж��
    public void UnLoad(string abName)
    {
        if (abDic.ContainsKey(abName))
        {
            abDic[abName].Unload(false);
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
