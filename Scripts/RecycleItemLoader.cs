using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecycleItemLoader : IRecycleScrollLoader
{
    public GameObject LoadRecycleItem(string path)
    {
           return ObjectBase.InstantiatePrefab(path, "LoadRecycleItem");
    }
}
