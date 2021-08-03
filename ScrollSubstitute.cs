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

    private bool CheckInView(float contentScaleX, float contentScaleY, Rect visibleContentRect)
    {
        float x_min = mRectTrans.localPosition.x - mRectTrans.pivot.x * mRectTrans.rect.width * contentScaleX;
        float y_min = mRectTrans.localPosition.y - mRectTrans.pivot.y * mRectTrans.rect.height * contentScaleY;

        Rect substituteRect = new Rect(new Vector2(x_min, y_min), new Vector2(contentScaleX * mRectTrans.rect.width, contentScaleY * mRectTrans.rect.height));
        return visibleContentRect.Overlaps(substituteRect);
    }

    public void UpdateScrollSubstitute(float contentScaleX,float contentScaleY,Rect visibleContentRect)
    {
        bool inView = CheckInView(contentScaleX, contentScaleY, visibleContentRect);
        if (inView && mItem == null)
        {
            mItem = mPopFunc(mItemPath);

            if(mItem != null)
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
            mPushFunc(mItemPath, mItem);
            if (mHandle != null)
            {
                mHandle.OnItemExit(mEntityId,mItem);
            }

            mItem = null;
        }
    }
}
