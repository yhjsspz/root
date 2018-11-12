using System;
using UnityEngine;

namespace CFramework
{
    public class BaseModule : BaseView
    {

        public BaseModule(string id, string[] preloadList, bool isCache, Transform go) : base(id, preloadList, isCache, go) {
            
        }

        public override void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}
