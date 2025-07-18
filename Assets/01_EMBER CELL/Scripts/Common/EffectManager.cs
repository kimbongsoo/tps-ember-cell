using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TEC
{
    public class EffectManager : MonoBehaviour
    {
    public static EffectManager Instance { get; private set;}

    [System.Serializable]
    public class EffectData
    {
        public string id;
        public GameObject prefab;
    }

    public List<EffectData> effectDatas = new List<EffectData>();
    private void Awake()
    {
        Instance = this;
    }

    public bool GetEffect(string id, out GameObject result)
    {
        EffectData targetData = effectDatas.Find(data => data.id.Equals(id));
        if(targetData == null)
        {
            result = null;
            return false;
        }

        result = Instantiate(targetData.prefab);
        result.SetActive(true);
        return true;
    }

    }

}
