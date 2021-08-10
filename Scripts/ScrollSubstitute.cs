using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScrollSubstitute : MonoBehaviour
{
    private int mEntityId;
    private GameObject mItem;
    private string mItemPath;
    private IRecycleScrollHandle mHandle;
    private RecyclePopFunc mPopFunc;
    private RecyclePushFunc mPushFunc;

    private ScrollSubstitute mLastSubstitute;  // 前项
    private ScrollSubstitute mNextSubstitute; //后项

    private RectTransform mRectTrans;
    private Rect mItemRect;

    public bool ShowDebugLine = false;

    private void CopyRectTransform(RecycleRectTransRecord itemRt)
    {
        if (itemRt == null)
            return;

        this.transform.localScale = itemRt.localScale;
        this.transform.localRotation = itemRt.localRotation;

        RectTransform rt = this.transform as RectTransform;
        rt.pivot = itemRt.pivot;
        rt.sizeDelta = itemRt.sizeDelta;
        rt.localScale = itemRt.localScale;
        rt.localRotation = itemRt.localRotation;
        rt.anchoredPosition = itemRt.anchoredPosition;
        rt.localPosition = new Vector3(itemRt.localPosition.x,itemRt.localPosition.y,0);
    }

    public void InitializeScrollSubstitute(int entityId,RecycleRectTransRecord rt, RecyclePopFunc popFunc, RecyclePushFunc pushFunc,string itemPath, IRecycleScrollHandle handle)
    {
        mEntityId = entityId;
        mPopFunc = popFunc;
        mPushFunc = pushFunc;
        mHandle = handle;
        mItemPath = itemPath;
        CopyRectTransform(rt);
        mRectTrans = transform as RectTransform;
    }

    public int GetEntityId()
    {
        return mEntityId;
    }
    
    public ScrollSubstitute GetNext()
    {
        return mNextSubstitute;
    }

    public void SetNext(ScrollSubstitute next)
    {
        mNextSubstitute = next;
    }

    public ScrollSubstitute GetLast()
    {
        return mLastSubstitute;
    }

    public void SetLast(ScrollSubstitute last)
    {
        mLastSubstitute = last;
    }

    private bool CheckInView(float scaleX, float scaleY, Rect visibleContentRect)
    {
        float x_min = (mRectTrans.localPosition.x - mRectTrans.pivot.x * mRectTrans.rect.width ) * scaleX;
        float y_min = (mRectTrans.localPosition.y - mRectTrans.pivot.y * mRectTrans.rect.height) * scaleY;

        mItemRect = new Rect(new Vector2(x_min, y_min), new Vector2(mRectTrans.rect.width*scaleX, mRectTrans.rect.height*scaleY));
        return visibleContentRect.Overlaps(mItemRect);
    }

    public void PushItem()
    {
        if(mItem != null)
        {
            mPushFunc(mItemPath, mItem);
        }
    }

    public GameObject PopItem()
    {
        return mPopFunc(mItemPath); 
    }

    public void UpdateScrollSubstitute(Vector3 viewportScale, Vector3 contentScale, Rect visibleContentRect)
    {
        bool inView = CheckInView(viewportScale.x*contentScale.x, viewportScale.y*viewportScale.y, visibleContentRect);
        if (inView && mItem == null)
        {
            mItem = PopItem();

            if (mItem != null)
            {
                mItem.transform.SetParent(this.transform);
                mItem.transform.localPosition = Vector3.zero;
                mItem.transform.localScale = Vector3.one;
                mItem.transform.localRotation = Quaternion.identity;
            }

            if(mHandle != null)
            {
                mHandle.OnItemEnter(mEntityId,mItem);
            }
        }
        else if(!inView && mItem != null)
        {
            PushItem();
            if (mHandle != null)
            {
                mHandle.OnItemExit(mEntityId,mItem);
            }

            mItem = null;
        }
    }

    private void OnDrawGizmos()
    {
        if (!ShowDebugLine)
            return;

        Gizmos.color = Color.green;

        if (mItemRect != null)
        {
            Gizmos.DrawLine(new Vector2(mItemRect.xMin, mItemRect.yMin), new Vector2(mItemRect.xMax, mItemRect.yMin));
            Gizmos.DrawLine(new Vector2(mItemRect.xMax, mItemRect.yMin), new Vector2(mItemRect.xMax, mItemRect.yMax));
            Gizmos.DrawLine(new Vector2(mItemRect.xMax, mItemRect.yMax), new Vector2(mItemRect.xMin, mItemRect.yMax));
            Gizmos.DrawLine(new Vector2(mItemRect.xMin, mItemRect.yMax), new Vector2(mItemRect.xMin, mItemRect.yMin));
        }
    }

}