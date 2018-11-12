using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace CFramework
{
    public class TimerManager : BaseManager<TimerManager>
    {

        public float time;
        internal Dictionary<string, TimerItem> timerDic;

        public override void Init()
        {

            time = Time.time;
            timerDic = new Dictionary<string, TimerItem>();
            StartCoroutine(Run());
        }

        IEnumerator<WaitForSeconds> Run() {

            while (true) {

                yield return new WaitForSeconds(0.1f);

                foreach (TimerItem item in timerDic.Values)
                {
                    item.Run(Time.time);
                }
            }


        }

        public void AddTimer(string id, float delayTime, Action callback, bool isNowRun = true)
        {
            if (!timerDic.ContainsKey(id))
            {
                TimerItem timerItem = new TimerItem(time, delayTime, callback, isNowRun);
                timerDic.Add(id, timerItem);
            }
        }
        public void RemoveTimer(string id)
        {
            if (timerDic.ContainsKey(id))
            {
                timerDic.Remove(id);
            }
        }

        internal void Dispose()
        {
            throw new NotImplementedException();
        }
    }

    internal class TimerItem
    {

        /// 当前时间
        public float currentTime;
        /// 延迟时间
        public float delayTime;
        /// 回调函数
        public Action callback;
        /// 是否立即执行一次
        public bool isNowRun;

        public TimerItem(float time, float delayTime, Action callback, bool isNowRun = true)
        {
            this.currentTime = time;
            this.delayTime = delayTime;
            this.callback = callback;
            this.isNowRun = isNowRun;

            if (isNowRun) {
                callback();
            }

        }

        public void Run(float time)
        {
            // 计算差值
            float offsetTime = time - this.currentTime;
            // 如果差值大等于延迟时间
            if (offsetTime >= this.delayTime)
            {
                float count = offsetTime / this.delayTime - 1;
                float mod = offsetTime % this.delayTime;
                for (int index = 0; index < count; index++)
                {
                    this.callback();
                }
                this.currentTime = time - mod;
            }
        }
    }
}
