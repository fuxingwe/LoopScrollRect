using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace UnityEngine.UI
{
    [System.Serializable]
    public class LoopScrollPrefabSource 
    {
        public string prefabName;
        public int poolSize = 5;

        private bool inited = false;
        //同一帧缓存，回收再拿出来用，不需要改变父对象和设置显隐
        private Queue<GameObject> frameCacheStack = new Queue<GameObject>();
        public virtual GameObject GetObject(Transform content)
        {
            if (frameCacheStack.Count > 0)
            {
                //Debug.Log("from cache" + frameCacheStack.Count);
                return frameCacheStack.Dequeue();
            }
            if (!inited)
            {
                SG.ResourceManager.Instance.InitPool(prefabName, poolSize,SG.PoolInflationType.INCREMENT);
                inited = true;
            }
            GameObject go = SG.ResourceManager.Instance.GetObjectFromPool(prefabName);
            //Debug.Log(go.GetInstanceID() + "---GetObject---" + frameCacheStack.Count.ToString());
            go.transform.SetParent(content, false);
            //go.SetActive(true);
            return go;
        }

        public virtual void CacheObject(Transform go)
        {
            //Debug.Log(go.GetInstanceID() + "---CacheObject---" + frameCacheStack.Count.ToString());
            frameCacheStack.Enqueue(go.gameObject);
        }
        public virtual void ReturnObject(Transform go)
        {
            go.SendMessage("ScrollCellReturn", SendMessageOptions.DontRequireReceiver);
            SG.ResourceManager.Instance.ReturnObjectToPool(go.gameObject);
        }

        public virtual void ClearCache()
        {
            for (var e = frameCacheStack.GetEnumerator(); e.MoveNext();)
            {
                e.Current.SendMessage("ScrollCellReturn", SendMessageOptions.DontRequireReceiver);
                //Debug.Log(e.Current.GetInstanceID()+ "---ClearCache----" + frameCacheStack.Count.ToString());
                SG.ResourceManager.Instance.ReturnObjectToPool(e.Current);
            }
            frameCacheStack.Clear();
        }
    }
}
