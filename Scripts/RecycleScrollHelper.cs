using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class RecycleScrollHelper
{
    public static RecycleRectTransRecord GetRecycleItemRectTransformCopy(Transform ori)
    {
        RecycleRectTransRecord record = new RecycleRectTransRecord();
        record.localPosition = ori.localPosition;
        record.localScale = ori.localScale;
        record.localRotation = ori.localRotation;
        RectTransform rt = ori as RectTransform;
        if(rt != null)
        {
            record.pivot = rt.pivot;
            record.sizeDelta = rt.sizeDelta;
            record.anchoredPosition = rt.anchoredPosition;
        }
        return record;
    }
}
