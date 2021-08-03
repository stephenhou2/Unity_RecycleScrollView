using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecycleScrollItemPool 
{
    private string mKey;
    private List<GameObject> mRecycleItems;

    public RecycleScrollItemPool(string key)
    {
        mKey = key;
        mRecycleItems = new List<GameObject>();
    }

    public int GetItemCount()
    {
        return mRecycleItems.Count;
    }

    public void PushItem(GameObject item)
    {
        if (item == null)
            return;

        mRecycleItems.Add(item);
    }

    public GameObject PopItem()
    {
        int cnt = GetItemCount();
        if (cnt == 0)
            return null;

        var item = mRecycleItems[cnt - 1];
        mRecycleItems.RemoveAt(cnt - 1);
        return item;
    }
}
