using CFramework;
using UnityEngine;

namespace CFramework
{
    public abstract class BaseManager<T> : UnitySingleton<T> where T : UnitySingleton<T>
    {
        public static T Instance
        {
            get { return _Instance as T; }
            set { _Instance = value; }
        }

        public abstract void Init();
    }
}
