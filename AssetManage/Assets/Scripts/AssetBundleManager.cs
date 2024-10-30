
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class AssetBundleManager : SingleAutoMono<AssetBundleManager>
{
    // 已加载的AB包
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
            // 判断包是否被加载
            if (!abDic.ContainsKey(ReferStr[i]))
            {
                AssetBundle ab = AssetBundle.LoadFromFile(PerUrl + ReferStr[i]);
                abDic.Add(ReferStr[i], ab);
            }
        }
        // 加载目标包 需要先加载依赖包
        if (!abDic.ContainsKey(abName))
        {
            AssetBundle tarAB = AssetBundle.LoadFromFile(PerUrl +abName);
            abDic.Add(abName, tarAB);
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
        // 加载目标资源
        return abDic[abName].LoadAsset(resName);
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
        return abDic[abName].LoadAsset(resName, resType);
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
        return abDic[abName].LoadAsset<T>(resName);
    }
    // 异步加载 public目的是给外部启动协程
    public void LoadResAsync(string abName, string resName, UnityAction<Object> callBack)
    {
      StartCoroutine(ReallyLoadResAsync(abName, resName, callBack));
    }
    private IEnumerator ReallyLoadResAsync(string abName, string resName, UnityAction<Object> callback)
    {
        LoadAB(abName);
        // 加载目标资源
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
        // 加载目标资源
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
        // 加载目标资源
        yield return abDic[abName].LoadAssetAsync<T>(resName); ;
    }

    // 单个包卸载
    public void UnLoad(string abName)
    {
        if (abDic.ContainsKey(abName))
        {
            abDic[abName].Unload(false);
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
