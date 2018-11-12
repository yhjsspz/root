
using UnityEngine.UI;

using LuaInterface;
using UnityEngine.EventSystems;

public class ButtonEx : Button
{
    private LuaFunction _onDownCallback;
    private LuaFunction _onUpCallback;

    protected override void OnDestroy()
    {
        base.OnDestroy();

        if (this._onDownCallback != null)
        {
            this._onDownCallback.Dispose();
            this._onDownCallback = null;
        }

        if (this._onUpCallback != null)
        {
            this._onUpCallback.Dispose();
            this._onUpCallback = null;
        }
    }

    public void SetCallback(LuaFunction onDownCallback, LuaFunction onUpCallback)
    {
        this._onDownCallback = onDownCallback;
        this._onUpCallback = onUpCallback;
    }

    public override void OnPointerDown(PointerEventData eventData)
    {
        base.OnPointerDown(eventData);

        if (this._onDownCallback != null)
            this._onDownCallback.Call(this.gameObject, eventData);
    }

    public override void OnPointerUp(PointerEventData eventData)
    {
        base.OnPointerUp(eventData);

        if (this._onUpCallback != null)
            this._onUpCallback.Call(this.gameObject, eventData);
    }
}
