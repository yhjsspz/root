
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using LuaInterface;
using CFramework;

class CellEventButton
{
    public string targetName;
    public string fullEventId;
    public GameObject go;    
    public LuaFunction callback;
}

public class UITableViewCell : MonoBehaviour
{
    private string _viewId;
    private string _tableViewId;
    private int _cellIndex;    

    private Dictionary<string, GameObject> _childList = new Dictionary<string, GameObject>();
    private Dictionary<string, CellEventButton> _eventTargetList = new Dictionary<string, CellEventButton>();

    private void Awake()
    {
        Transform[] childList = this.GetComponentsInChildren<Transform>(true);        

        foreach (Transform transform in childList)
        {
            if (transform.gameObject.name.ToLower().StartsWith("Tmp_") == true)
            {
                continue;
            }
            else if (transform.parent && transform.parent.name.StartsWith("Parent_") == true)
            {
                transform.gameObject.name = transform.parent.name.Split('_')[1] + "_" + transform.gameObject.name;
                _childList.Add(transform.gameObject.name, transform.gameObject);
            }
            else
            {
                if (_childList.ContainsKey(transform.gameObject.name) == true)
                {
                    DebugManager.LogError("UITableViewCell addViewCell Error:重复的子控件ID " + transform.gameObject.name);

                    continue;
                }

                _childList.Add(transform.gameObject.name, transform.gameObject);
            }
        }
    }

    private void OnDestroy()
    {
        this.Dispose();

        if (_childList!= null)
        {
            _childList.Clear();
            _childList = null;
        }        
    }

    public void Dispose()
    {
        if (this._eventTargetList == null)
            return;

        foreach(CellEventButton button in this._eventTargetList.Values)
        {
            (SceneManagerEx.Instance as SceneManagerEx).RemoveEvent(this._viewId, 1, button.go.transform, button.fullEventId);
        }

        this._eventTargetList.Clear();
        this._eventTargetList = null;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="viewId"></param>
    /// <param name="targetName"></param>
    /// <param name="callback"></param>
    public void AddClickEvent(string viewId, string tableViewId, string targetName, LuaFunction callback)
    {
        this._viewId = viewId;
        this._tableViewId = tableViewId;

        GameObject target = this.GetChild(targetName);

        if (target == null)
        {
            DebugManager.LogError("UITableView Cell AddClickEvent Error: target is null." + targetName);

            return;
        }

        CellEventButton btnTarget = null;

        this._eventTargetList.TryGetValue(targetName, out btnTarget);

        if (btnTarget == null)
        {
            btnTarget = new CellEventButton();
            btnTarget.targetName = targetName;            
            btnTarget.go = target;
            btnTarget.callback = callback;

            this._eventTargetList.Add(targetName, btnTarget);
        }
        else
        {
            (SceneManagerEx.Instance as SceneManagerEx).RemoveEvent(viewId, 1, target.transform, btnTarget.fullEventId);            
        }

        btnTarget.fullEventId = this._tableViewId + "_" + targetName + "_" + this._cellIndex.ToString();

        (SceneManagerEx.Instance as SceneManagerEx).AddEvent(viewId, 1, target.transform, callback, btnTarget.fullEventId);
    }

    public GameObject GetChild(string childName)
    {
        GameObject child = null;

        _childList.TryGetValue(childName, out child);

        return child;
    }

    public int CellIndex
    {
        get
        {
            return this._cellIndex;
        }

        set
        {
            foreach(CellEventButton btnTarget in this._eventTargetList.Values)
            {
                (SceneManagerEx.Instance as SceneManagerEx).RemoveEvent(this._viewId, 1, btnTarget.go.transform, btnTarget.fullEventId);

                btnTarget.fullEventId = this._tableViewId + "_" + btnTarget.targetName + "_" + value;

                (SceneManagerEx.Instance as SceneManagerEx).RemoveEvent(this._viewId, 1, btnTarget.go.transform, btnTarget.fullEventId);
                (SceneManagerEx.Instance as SceneManagerEx).AddEvent(this._viewId, 1, btnTarget.go.transform, btnTarget.callback, btnTarget.fullEventId);
            }

            this._cellIndex = value;
        }
    }
}

