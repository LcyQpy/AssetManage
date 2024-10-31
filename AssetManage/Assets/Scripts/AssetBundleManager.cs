
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class AssetBundleManager : SingleAutoMono<AssetBundleManager>
{
    // 已加载的AB包
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
    /// 加载目标包及其依赖包
    /// </summary>
    /// <param name="abName"></param>
    private void LoadAB(string abName)
    {
        if (MianABundle == null)
        {
            // 加载AB包
            MianABundle = AssetBundle.LoadFromFile(Application.streamingAssetsPath + $"/{MainABundleName}");
            mianfest = MianABundle.LoadAsset<AssetBundleManifest>("AssetBundleManifest");
        }
        // 从主包获取目标包依赖信息
        string[] ReferStr = mianfest.GetDirectDependencies(abName);
        for (int i = 0; i < ReferStr.Length; i++)
        {
            // 判断依赖包是否被加载
            if (!abDic.ContainsKey(ReferStr[i]))
            {
                AssetBundle ab = AssetBundle.LoadFromFile(PerUrl + ReferStr[i]);
                abDic.Add(ReferStr[i], new AssetBundleInfo(ab));
            }
            abDic[ReferStr[i]].m_referAssetBuble.Add(abName);
        }
        // 加载目标包 需要先加载依赖包
        if (!abDic.ContainsKey(abName)) // 没被加载
        {
            AssetBundle tarAB = AssetBundle.LoadFromFile(PerUrl +abName);
            abDic.Add(abName, new AssetBundleInfo(tarAB));
        }
    }

    /// <summary>
    /// 同步加载在AB包资源
    /// </summary>
    /// <param name="abName"></param>
    /// <param name="resName"></param>
    /// <returns></returns>

    public Object LoadRes(string abName, string resName)
    {
        LoadAB(abName);
        // 加载成功
        var obj = abDic[abName].m_assetBundle.LoadAsset(resName);
        //abDic[abName].m_obj.Add(obj as Object);
        // 加载目标资源
        return abDic[abName].m_assetBundle.LoadAsset(resName);
    }
    /// <summary>
    /// 同步加载在AB包资源 根据资源类型
    /// </summary>
    /// <param name="abName"></param>
    /// <param name="resName"></param>
    /// <param name="resType"></param>
    /// <returns></returns>
    public Object LoadRes(string abName, string resName, System.Type resType)
    {
        LoadAB(abName);
        // 根据类型 加载目标资源
        var obj = abDic[abName].m_assetBundle.LoadAsset(resName, resType);
        abDic[abName].m_obj.Add(obj);
        return abDic[abName].m_assetBundle.LoadAsset(resName, resType);
    }

    /// <summary>
    /// 同步加载 根据泛型指定类型
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="abName"></param>
    /// <param name="resName"></param>
    /// <returns></returns>
    public T LoadRes<T>(string abName, string resName) where T :Object
    {
        LoadAB(abName);
        // 根据类型 加载目标资源
        var obj = abDic[abName].m_assetBundle.LoadAsset<T>(resName);
        abDic[abName].m_obj.Add(obj);
        return abDic[abName].m_assetBundle.LoadAsset<T>(resName);
    }
    // 异步加载 public目的是给外部启动协程
    public void LoadResAsync(string abName, string resName, UnityAction<Object> callBack)
    {
      StartCoroutine(ReallyLoadResAsync(abName, resName, callBack));
    }
    private IEnumerator ReallyLoadResAsync(string abName, string resName, UnityAction<Object> callBack)
    {
        LoadAB(abName);  // 加载依赖包
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
        LoadAB(abName);  // 加载依赖包
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
        LoadAB(abName);  // 加载依赖包
        AssetBundleRequest request = abDic[abName].m_assetBundle.LoadAssetAsync<T>(resName);
        yield return request;
        abDic[abName].m_obj.Add(request.asset as Object);
        callBack(request.asset);
    }

    // 单个包卸载
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

    // 所有包卸载
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
    public AssetBundle m_assetBundle; // 当前包
    public List<Object> m_obj = new();  // 加载对象
    public List<string> m_referAssetBuble = new(); // 被引用
    public AssetBundleInfo(AssetBundle assetBundle)
    {
        m_assetBundle = assetBundle;
    }
}
