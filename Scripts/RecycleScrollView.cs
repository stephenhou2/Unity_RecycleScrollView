using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public delegate GameObject RecyclePopFunc(string path);
public delegate void RecyclePushFunc(string itemPath,GameObject item);

public class RecycleScrollView : ScrollRect
{
    //[SerializeField]
    public Transform mPoolNode;

    private ScrollSubstitute mHeadSubstitute; // 头部
    private ScrollSubstitute mTailSubstitute; //尾部

    private Dictionary<string, RecycleScrollItemPool> mItemPools;
    private Dictionary<string, RecycleRectTransRecord> mItemRectPool;

    private Rect mViewport_to_content_rect;

    private IRecycleScrollLoader mLoader;
    private IRecycleScrollHandle mHandle;


    public void InitializeRecycleScrollView(IRecycleScrollLoader loader,IRecycleScrollHandle handle)
    {
        mLoader = loader;
        mHandle = handle;

        mItemPools = new Dictionary<string, RecycleScrollItemPool>();
        mItemRectPool = new Dictionary<string, RecycleRectTransRecord>();
    }

    /// <summary>
    /// 获取可见区域的范围相对与content的rect
    /// </summary>
    /// <returns></returns>
    private Rect CalcContentVisibleRect()
    {
        Vector3 inverse_pos = -content.localPosition;
        float x_min = inverse_pos.x  - viewport.pivot.x * viewport.rect.width;
        float y_min = inverse_pos.y - viewport.pivot.y * viewport.rect.height;

        return new Rect(new Vector2(x_min,y_min), viewport.rect.size);
    }

    public void PushItem(string itemPath,GameObject item)
    {
        if (item == null)
        {
            Debug.LogWarning("Try to push a null item!");
            return;
        }

        item.transform.SetParent(mPoolNode);

        RecycleScrollItemPool itemPool;
        if(!mItemPools.TryGetValue(itemPath,out itemPool))
        {
            itemPool = new RecycleScrollItemPool(itemPath);
            mItemPools.Add(itemPath, itemPool);
        }

        itemPool.PushItem(item);
    }

    private GameObject PopOrLoadItem(string itemPath)
    {
        GameObject item = null;
        RecycleScrollItemPool itemPool;
        if (mItemPools.TryGetValue(itemPath, out itemPool))
        {
            item = itemPool.PopItem();
        }

        if(item == null)
        {
            item = mLoader.LoadRecycleItem(itemPath);
        }

        return item;
    }

    private RecycleRectTransRecord GetRectTrans(string path)
    {
        RecycleRectTransRecord rt = null;
        if(mItemRectPool.TryGetValue(path,out rt))
        {
            return rt;
        }

        GameObject item = mLoader.LoadRecycleItem(path);
        if(item != null)
        {
            rt = RecycleScrollHelper.GetRecycleItemRectTransformCopy(item.transform);          
            PushItem(path,item);
            mItemRectPool.Add(path, rt);
            return rt;
        }

        return null;
    }

    public int AddNewItem(string path)
    {
        int tailEntityId = 0;
        if (mTailSubstitute != null)
        {
            tailEntityId = mTailSubstitute.GetEntityId()+1;
        }

        GameObject subsGo = new GameObject();
        subsGo.AddComponent<RectTransform>();
        subsGo.transform.SetParent(content);
        ScrollSubstitute substitute = subsGo.AddComponent<ScrollSubstitute>();
        RecycleRectTransRecord rt = GetRectTrans(path);
        substitute.InitializeScrollSubstitute(tailEntityId, rt,PopOrLoadItem, PushItem,path, mHandle);
        if (mHeadSubstitute == null)
        {
            mHeadSubstitute = substitute;
        }
        if(mTailSubstitute != null)
        {
            mTailSubstitute.SetNext(substitute);
            substitute.SetLast(mTailSubstitute);
        }
        mTailSubstitute = substitute;
        return tailEntityId;
    }

    public void RemoveItem(int entityId)
    {
        if(mHeadSubstitute == null)
        {
            Debug.LogError("RemoveItem Failed,no entity in scroll view");
            return;
        }

        ScrollSubstitute current = mHeadSubstitute;
        while(current != null)
        {
            var last = current.GetLast();
            var next = current.GetNext();
            if(current.GetEntityId() == entityId)
            {
                if(last != null)
                {
                    last.SetNext(next);
                }

                if(next != null)
                {
                    next.SetLast(last);
                }
                break;
            }

            current = next;
        }
    }

    protected override void LateUpdate()
    {
        base.LateUpdate();

        if (mHeadSubstitute == null) 
            return;

        mViewport_to_content_rect = CalcContentVisibleRect();

        ScrollSubstitute toUpdate = mHeadSubstitute;
        while(toUpdate != null)
        {
            toUpdate.UpdateScrollSubstitute(content.localScale.x,content.localScale.y,mViewport_to_content_rect);
            toUpdate = toUpdate.GetNext();
        }
    }
}
