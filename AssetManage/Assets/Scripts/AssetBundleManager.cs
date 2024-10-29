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





    // ͬ������
    public void LoadRes(string abName, string resName)
    {
        if (MianABundle == null)
        {
            // ��������
            MianABundle = AssetBundle.LoadFromFile(Application.streamingAssetsPath + $"/AssetBundle/{MainABundleName}");
            mianfest = MianABundle.LoadAsset<AssetBundleManifest>("AssetBundleManifest");
        }
        // ��ȡ����������Ϣ
        string[] ReferStr = mianfest.GetDirectDependencies
    }
    // �첽����
    public void AsyncLoadRes()
    {

    }
    // ������ж��
    public void UnLoad()
    {

    }
    // ���а�ж��
    public void ClearAllRes()
    {

    }

}
