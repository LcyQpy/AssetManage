using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AssetBundleManager : SingleAutoMono<AssetBundleManager>
{
    private Dictionary<string, AssetBundle> abDic  = new Dictionary<string, AssetBundle>();
    private AssetBundle MianABundle = null;
    private AssetBundleManifest mianfest = null;
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





    // 同步加载
    public void LoadRes(string abName, string resName)
    {
        if (MianABundle == null)
        {
            // 加载主包
            MianABundle = AssetBundle.LoadFromFile(Application.streamingAssetsPath + $"/AssetBundle/{MainABundleName}");
            mianfest = MianABundle.LoadAsset<AssetBundleManifest>("AssetBundleManifest");
        }
        // 获取主包依赖信息
        string[] ReferStr = mianfest.GetDirectDependencies
    }
    // 异步加载
    public void AsyncLoadRes()
    {

    }
    // 单个包卸载
    public void UnLoad()
    {

    }
    // 所有包卸载
    public void ClearAllRes()
    {

    }

}
