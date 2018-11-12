using UnityEngine;

namespace CFramework
{

    public class UnitySingleton<T> : MonoBehaviour where T : UnitySingleton<T>
    {
        private static UnitySingleton<T> _instance;

        public static UnitySingleton<T> _Instance
        {
            get
            {
                if (_instance != null)
                {
                    return _instance;
                    //T[] compoments = GameObject.FindObjectsOfType(typeof(T)) as T[];

                    //if (compoments != null && compoments.Length == 1)
                    //{
                    //    return compoments[0];
                    //}
                    //else if(compoments != null && compoments.Length > 1)
                    //{
                    //    DebugManager.LogError("compoments"+ compoments.Length);
                    //    for (int c = 0; c < compoments.Length - 1; ++c)
                    //    {
                    //        DestroyImmediate(compoments[c].gameObject);
                    //    }
                    //    return compoments[compoments.Length - 1];
                    //}
                }

                GameObject go = GameObject.Find("GameManager");
                if (go != null)
                {

                    _instance = go.AddComponent<T>();
                    return _instance;
                }
                else {
                    DebugManager.LogError("========================="+ typeof(T).ToString() + " init fail, GameManager is null");
                    return null;
                }

            }
            set { _instance = value as T; }
        }
    }
}

