
using CFramework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// 
/// </summary>
public class UITableView : MonoBehaviour, IEndDragHandler
{
    public bool useLoopItems = false;

    private List<UITableViewCell> m_items = new List<UITableViewCell>();    
    private RectTransform _content;
    private LayoutGroup _LayoutGroup;
    private RectOffset _oldPadding;

    private ScrollRect _scrollRect;
    private RectTransform _tranScrollRect;
    private int _itemSpace;          //每个Item的空间
    private int _viewItemCount;        //可视区域内Item的数量（向上取整）
    private bool _isVertical;          //是否是垂直滚动方式，否则是水平滚动

    private int _startIndex;           //数据数组渲染的起始下标

    private int _dataCount;

    private float _newLoadingLen;
    private bool _newLoadingState = false;

    private float _scrollRectTopY;
    private float _scrollRectEndY;

    private GameObject _itemSkin;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="scrollView"></param>
    /// <param name="dataCount"></param>
    public static UITableView CreateTableView(GameObject scrollView, float itemHeight, float newLoadingLen =0, bool useLoopItems = true)
    {
        UITableView tableView = scrollView.GetComponent<UITableView>();

        if (tableView == null)
            tableView = scrollView.AddComponent<UITableView>();

        tableView._newLoadingLen = newLoadingLen;     
        tableView.useLoopItems = useLoopItems;
        tableView._LayoutGroup = scrollView.GetComponentInChildren<LayoutGroup>();
        tableView._content = tableView._LayoutGroup.gameObject.GetComponent<RectTransform>();

        if (tableView._LayoutGroup != null)
            tableView._oldPadding = tableView._LayoutGroup.padding;

        tableView._scrollRect = scrollView.GetComponentInParent<ScrollRect>();

        if (tableView._scrollRect != null && tableView._LayoutGroup != null)
        {
            tableView._scrollRect.decelerationRate = 0.2f;
            tableView._tranScrollRect = tableView._scrollRect.GetComponent<RectTransform>();
            tableView._isVertical = tableView._scrollRect.vertical;

            if (tableView.useLoopItems)
            {
                tableView._scrollRect.onValueChanged.AddListener(tableView.OnScroll);
            }

            tableView._itemSpace = (int)(itemHeight + (int)(tableView._LayoutGroup as HorizontalOrVerticalLayoutGroup).spacing);
            tableView._viewItemCount = Mathf.CeilToInt(tableView.ViewSpace / tableView._itemSpace);
        }
        else
        {
            Debug.LogError("scrollRect is null or verticalLayoutGroup is null");
        }

        return tableView;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="go"></param>
    public void SetItemSkin(GameObject go)
    {
        this._itemSkin = go;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="count"></param>
    public void SetItemCount(int count)
    {
        this._dataCount = count;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="itemHeight"></param>
    public void SetItemHeight(float itemHeight)
    {
        this._itemSpace = (int)(itemHeight + (int)(this._LayoutGroup as HorizontalOrVerticalLayoutGroup).spacing);
        this._viewItemCount = Mathf.CeilToInt(this.ViewSpace / this._itemSpace);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="data"></param>
    private void OnScroll(Vector2 data)
    {
        if (this.m_items.Count == 0)
            return;
        
        var value = (ContentSpace - ViewSpace) * (_isVertical ? data.y : 1 - data.x);
        var start = ContentSpace - value - ViewSpace;
        var startIndex = Mathf.FloorToInt(start / _itemSpace) * ConstraintCount;
        startIndex = Mathf.Max(0, startIndex);

        if (startIndex != _startIndex)
        {
            _startIndex = startIndex;

            _UpdateView(false);
        }

        this._scrollRectTopY = start;
        this._scrollRectEndY = value;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="eventData"></param>
    public void OnEndDrag(PointerEventData eventData)
    {
        if (this._newLoadingState == false)
        {
            if (this._scrollRectTopY < -this._newLoadingLen)
            {
                if (this.m_newLoadingDelegate != null)
                {
                    this.m_newLoadingDelegate(true);
                    this._newLoadingState = true;
                }
            }
            else if (this._scrollRectEndY < -this._newLoadingLen)
            {
                if (this.m_newLoadingDelegate != null)
                {
                    this.m_newLoadingDelegate(false);
                    this._newLoadingState = true;
                }
            }
        }
    }

    /// <summary>
    /// 重置滚动位置，
    /// </summary>
    /// <param name="top">true则跳转到顶部，false则跳转到底部</param>
    public void ResetScrollPosition(bool top = true)
    {
        int index = top ? 0 : this._dataCount;

        ResetScrollPosition(index);
    }

    /// <summary>
    /// 重置滚动位置，如果同时还要赋值新的Data，请在赋值之前调用本方法
    /// </summary>
    public void ResetScrollPosition(int index)
    {
        if (this.m_items.Count == 0)
            return;

        StartCoroutine(_ResetScrollPosition(index));
    }

    IEnumerator _ResetScrollPosition(int index)
    {
        yield return 0;

        if (index < 0)
            index = 0;

        var unitIndex = Mathf.Clamp(index / ConstraintCount, 0, DataUnitCount - _viewItemCount > 0 ? DataUnitCount - _viewItemCount : 0);
        var value = (unitIndex * _itemSpace) / (Mathf.Max(ViewSpace, ContentSpace - ViewSpace));
        value = Mathf.Clamp01(value);

        //特殊处理无法使指定条目置顶的情况——拉到最后
        if (unitIndex != index / ConstraintCount)
            value = 1;

        if (_scrollRect)
        {
            if (_isVertical)
                _scrollRect.verticalNormalizedPosition = 1 - value;
            else
                _scrollRect.horizontalNormalizedPosition = value;
        }

        _startIndex = unitIndex * ConstraintCount;

        int itemLength = useLoopItems ? _viewItemCount * ConstraintCount + CacheCount : _dataCount;
        itemLength = Mathf.Min(itemLength, _dataCount);

        for (int i = 0; i < m_items.Count; i++)
        {
            UITableViewCell cell = m_items[i];
            cell.CellIndex = _startIndex + i;
            m_pTableCellWithShowedDelegate(cell, cell.CellIndex, false);
        }        

        _UpdateView(false);
    }

    /// <summary>
    /// 更新行
    /// </summary>
    /// <param name="rowIndex"></param>
    public void UpdateRow(int rowIndex)
    {
        if (useLoopItems)
        {
            _startIndex = Mathf.Max(0, Mathf.Min(_startIndex / ConstraintCount, DataUnitCount - _viewItemCount - CacheUnitCount)) * ConstraintCount;
        }
        else
        {
            _startIndex = 0;
        }

        var index = _startIndex + (rowIndex - 1);

        if (index < 0 || index >= m_items.Count)
        {
            DebugManager.LogWarning("TableView UpdateRow Warning:无效的行.");
        }
        else
        {
            m_pTableCellWithShowedDelegate(m_items[index], index, false);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public void UpdateView(bool forceUpdate = true, int posIndex = 0)
    {
        this._UpdateView(forceUpdate);        
    }

    /// <summary>
    /// 更新视图
    /// </summary>
    private void _UpdateView(bool forceUpdate)
    {
        if (this._itemSkin == null)
        {
            DebugManager.LogError("TableView UpdateView Error:itemSkin is null.");

            return;
        }

        if (useLoopItems)
        {
            _startIndex = Mathf.Max(0, Mathf.Min(_startIndex / ConstraintCount, DataUnitCount - _viewItemCount - CacheUnitCount)) * ConstraintCount;

            var frontSpace = _startIndex / ConstraintCount * _itemSpace;
            var behindSpace = Mathf.Max(0, _itemSpace * (DataUnitCount - CacheUnitCount) - frontSpace - (_itemSpace * _viewItemCount));

            if (_isVertical)
                _LayoutGroup.padding = new RectOffset(_oldPadding.left, _oldPadding.right, frontSpace, behindSpace);
            else
                _LayoutGroup.padding = new RectOffset(frontSpace, behindSpace, _oldPadding.top, _oldPadding.bottom);
        }
        else
            _startIndex = 0;

        int itemLength = useLoopItems ? _viewItemCount * ConstraintCount + CacheCount : _dataCount;
        itemLength = Mathf.Min(itemLength, _dataCount);

        int endIndex = _startIndex + itemLength - 1;
        endIndex = Mathf.Min(endIndex, _dataCount);

        if (m_items.Count == 0)
        {
            //add new

            for (int index = _startIndex; index <= endIndex; index++)
            {
                if (m_items.Count < _dataCount)
                {
                    GameObject skin = GameObject.Instantiate(this._itemSkin);

                    UITableViewCell cell = skin.AddComponent<UITableViewCell>();
                    cell.name = this._itemSkin.name + m_items.Count;
                    cell.transform.SetParent(this._content, false);
                    cell.CellIndex = index;                    
                    m_pTableCellWithShowedDelegate(cell, index, true);
                    m_items.Add(cell);
                }
            }

            return;
        }

        //forceUpdate

        if (forceUpdate == true)
        {
            if (m_items.Count < itemLength)
            {
                for (int index = m_items.Count; index < itemLength; index++)
                {
                    GameObject skin = GameObject.Instantiate(this._itemSkin);

                    UITableViewCell cell = skin.AddComponent<UITableViewCell>();
                    cell.transform.SetParent(this._content, false);
                    cell.CellIndex = index;
                    m_pTableCellWithShowedDelegate(cell, index, true);
                    m_items.Add(cell);

                    cell.transform.SetAsLastSibling();
                }
            }
            else if (m_items.Count > itemLength)
            {
                for(int index = m_items.Count - 1; index >= itemLength; index--)
                {
                    m_items[index].Dispose();
                    Destroy(m_items[index].gameObject);

                    m_items.RemoveAt(index);
                }
            }

            int sindex = 0;

            for(int index = _startIndex; index <= endIndex; index++)
            {
                UITableViewCell cell = m_items[sindex];
                cell.name = this._itemSkin.name + sindex;
                cell.CellIndex = index;

                m_pTableCellWithShowedDelegate(cell, cell.CellIndex, false);

                sindex++;
            }

            return;
        }

        //update

        if (m_items[0].CellIndex < _startIndex)
        {
            for (int i = 0; i < m_items.Count; i++)
            {
                UITableViewCell cell = m_items[0];

                if (cell.CellIndex < _startIndex)
                {
                    m_items.RemoveAt(0);                    

                    cell.CellIndex = m_items[m_items.Count - 1].CellIndex + 1; 
                    cell.transform.SetAsLastSibling();

                    m_pTableCellWithShowedDelegate(cell, cell.CellIndex, false);

                    m_items.Add(cell);
                }
                else
                {
                    break;
                }
            }
        }
        else if(m_items[m_items.Count - 1].CellIndex > endIndex)
        {
            for (int i = m_items.Count - 1; i >= 0; i--)
            {
                UITableViewCell cell = m_items[m_items.Count - 1];

                if (cell.CellIndex > endIndex)
                {
                    m_items.RemoveAt(m_items.Count - 1);

                    cell.CellIndex = m_items[0].CellIndex - 1;
                    cell.transform.SetAsFirstSibling();

                    m_pTableCellWithShowedDelegate(cell, cell.CellIndex, false);

                    m_items.Insert(0, cell);
                }
                else
                {
                    break;
                }
            }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public void Clear()
    {
        this._dataCount = 0;

        _LayoutGroup.padding = new RectOffset(_oldPadding.left, _oldPadding.right, _oldPadding.top, _oldPadding.bottom);

        for (int i = 0; i < m_items.Count; i++)
        {
            m_items[i].Dispose();

            Destroy(m_items[i].gameObject);

            m_items[i] = null;
        }

        m_items.Clear();
    }

    /// <summary>
    /// 
    /// </summary>
    private void OnDestroy()
    {
        this.Clear();

        if (this._itemSkin != null)
        {
            this._itemSkin = null;
        }

        m_pTableCellWithShowedDelegate = null;
        m_newLoadingDelegate = null;
    }

    private TableCellWithShowedDelegate m_pTableCellWithShowedDelegate = null;
    public delegate void TableCellWithShowedDelegate(UITableViewCell cell, int index, bool isNew);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="del"></param>
    public void setTableCellWithShowedDelegate(TableCellWithShowedDelegate del)
    {
        m_pTableCellWithShowedDelegate = del;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public TableCellWithShowedDelegate getTableCellWithShowedDelegate()
    {
        return m_pTableCellWithShowedDelegate;
    }

    private NewLoadingDelegate m_newLoadingDelegate = null;
    public delegate void NewLoadingDelegate(bool isTop);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="del"></param>
    public void setNewLoadingDelegate(NewLoadingDelegate del)
    {
        m_newLoadingDelegate = del;
    }

    /// <summary>
    /// 
    /// </summary>
    public void SetNewLoadingState()
    {
        this._newLoadingState = false;
    }

    /// <summary>
    /// 
    /// </summary>
    public ScrollRect GetScrollRect
    {
        get
        {
            return this._scrollRect;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public float verticalPos
    {
        get { return _scrollRect.verticalNormalizedPosition; }
        set { _scrollRect.verticalNormalizedPosition = value; }
    }

    /// <summary>
    /// 
    /// </summary>
    public float horizonPos
    {
        get { return _scrollRect.horizontalNormalizedPosition; }
        set { _scrollRect.horizontalNormalizedPosition = value; }
    }

    //内容长度
    private float ContentSpace
    {
        get
        {
            return _isVertical ? _content.sizeDelta.y : _content.sizeDelta.x;
        }
    }
    //可见区域长度
    private float ViewSpace
    {
        get
        {
            if (_tranScrollRect.anchorMax.y == 1 && _tranScrollRect.anchorMin.y == 0)
                return _isVertical ? (AppConst.DesignHeight - Mathf.Abs(_tranScrollRect.offsetMax.y) - Mathf.Abs(_tranScrollRect.offsetMin.y)) : (AppConst.DesignWidth - Mathf.Abs(_tranScrollRect.offsetMax.x) - Mathf.Abs(_tranScrollRect.offsetMin.x));
            else
                return _isVertical ? _tranScrollRect.sizeDelta.y : _tranScrollRect.sizeDelta.x;
        }
    }
    //约束常量（固定的行（列）数）
    private int ConstraintCount
    {
        get
        {
            return _LayoutGroup == null ? 1 : ((_LayoutGroup is GridLayoutGroup) ? (_LayoutGroup as GridLayoutGroup).constraintCount : 1);
        }
    }
    //数据量个数
    private int DataCount
    {
        get
        {
            return _dataCount;
        }
    }
    //缓存数量
    private int CacheCount
    {
        get
        {
            return ConstraintCount + DataCount % ConstraintCount;
        }
    }
    //缓存单元的行（列）数
    private int CacheUnitCount
    {
        get
        {
            return _LayoutGroup == null ? 1 : Mathf.CeilToInt((float)CacheCount / ConstraintCount);
        }
    }
    //数据单元的行（列）数
    private int DataUnitCount
    {
        get
        {
            return _LayoutGroup == null ? DataCount : Mathf.CeilToInt((float)DataCount / ConstraintCount);
        }
    }
}
