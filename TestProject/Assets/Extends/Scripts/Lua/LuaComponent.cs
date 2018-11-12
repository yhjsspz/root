
using CFramework;
using LuaInterface;
using System.Collections.Generic;
using UnityEngine;

namespace CFramework
{
    public class LuaComponent : MonoBehaviour
    {
        private LuaTable _table;
        private BaseView _baseView;

        /// <summary>
        /// 
        /// </summary>
        public void New(LuaTable table, BaseView baseView)
        {
            this._table = table;
            this._baseView = baseView;

            if (this._baseView != null)
                this._baseView.LuaBehaviour = this;
            
            if (this._table != null)
            {

                DebugManager.Log(_baseView.Id + " Awake");
                LuaFunction fun = this._table.GetLuaFunction("OnAwake");

                if (fun != null)
                    fun.Call(this._table, baseView);
            }

            this.UpdateControlBind();
        }

        /// <summary>
        /// 
        /// </summary>
        void Start()
        {

            if (this._table != null)
            {
                DebugManager.Log(_baseView.Id + " Start");
                LuaFunction fun = this._table.GetLuaFunction("OnStart");
                
                if (fun != null)
                    fun.Call(this._table);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        void OnDestroy()
        {
            if (this._table != null)
            {
                DebugManager.Log(_baseView.Id + " Destroy");
                LuaFunction fun = this._table.GetLuaFunction("Destroy");

                if (fun != null)
                    fun.Call(this._table);

                this._table.Dispose();
                this._table = null;
            }

            if (this._baseView != null)
            {
                this._baseView.Dispose();
                this._baseView = null;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="focus"></param>
        void OnApplicationFocus(bool focus)
        {
            if (this._table != null)
            {
                LuaFunction fun = this._table.GetLuaFunction("OnFocus");

                if (fun != null)
                    fun.Call(this._table);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pause"></param>
        void OnApplicationPause(bool pause)
        {
            if (this._table != null)
            {
                LuaFunction fun = this._table.GetLuaFunction("OnPause");

                if (fun != null)
                    fun.Call(this._table);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        void OnApplicationQuit()
        {
            if (this._table != null)
            {
                LuaFunction fun = this._table.GetLuaFunction("OnQuit");

                if (fun != null)
                    fun.Call(this._table);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void UpdateControlBind()
        {
            if (this._table != null && this._baseView != null)
            {
                LuaFunction fun = this._table.GetLuaFunction("UpdateUIControl");

                List<string> idList = new List<string>();
                List<Transform> objList = new List<Transform>();

                foreach (KeyValuePair<string, Transform> kv in this._baseView.ControlList)
                {
                    if (kv.Key.StartsWith("Tmp")) {
                        continue;
                    }
                    idList.Add(kv.Key);
                    objList.Add(kv.Value);
                }

                if (fun != null)
                    fun.Call(this._table, idList.ToArray(), objList.ToArray());
            }
        }
    }
}