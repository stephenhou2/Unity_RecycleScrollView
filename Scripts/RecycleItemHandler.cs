using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecycleItemHandler : IRecycleScrollHandle
{
    private string mItemEnterEvent = string.Empty;
    private string mItemExitEvent = string.Empty;
    private string mItemDestroyEvent = string.Empty;

    public RecycleItemHandler(string itemEnterEvent,string itemExitEvent,string itemDestroyEvent)
    {
        mItemEnterEvent = itemEnterEvent;
        mItemExitEvent = itemExitEvent;
        mItemDestroyEvent = itemDestroyEvent;
    }

    public void OnItemEnter(int entityId, GameObject item)
    {
        if(!string.IsNullOrEmpty(mItemEnterEvent))
        {
            LuaFunSet.FireEvent(mItemEnterEvent, new object[] { entityId, item });
        }
    }

    public void OnItemExit(int entityId, GameObject item)
    {
        if (!string.IsNullOrEmpty(mItemExitEvent))
        {
            LuaFunSet.FireEvent(mItemExitEvent, new object[] { entityId, item });
        }
    }

    public void OnItemDestroy(int entityId,GameObject item)
    {
        if(!string.IsNullOrEmpty(mItemDestroyEvent))
        {
            LuaFunSet.FireEvent(mItemDestroyEvent, new object[] { entityId, item });
        }
    }
}
