using System;
using UnityEngine;

namespace CFramework
{
    public class BaseWindow : BaseView
    {
        public BaseWindow(string id, string[] preloadList, bool isCache, Transform go) : base(id, preloadList, isCache, go) {
            
        }

        public override void Dispose()
        {
            ResourceManager.Instance.DisposeViewCache(_id);
        }
    }
}
