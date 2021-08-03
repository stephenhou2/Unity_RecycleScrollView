using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TestLoader : IRecycleScrollLoader
{
    public GameObject mTestGo;
    public GameObject LoadRecycleItem(string path)
    {
        return GameObject.Instantiate(mTestGo);
    }
}

public class TestHandle : IRecycleScrollHandle
{
    public void OnItemEnter(int entityId,GameObject item)
    {
        Debug.LogFormat("Item Enter++entityId:{0}",entityId);
    }

    public void OnItemExit(int entityId,GameObject item)
    {
        Debug.LogFormat("Item Exit---entityId:{0}", entityId);
    }
}


public class RecycleTest : MonoBehaviour
{
    public GameObject TestGo;
    public RecycleScrollView view;
    public TestLoader loader;
    public TestHandle handle;

    void Start()
    {
        loader = new TestLoader();
        loader.mTestGo = TestGo;
        handle = new TestHandle();

        view.InitializeRecycleScrollView(loader, handle);
    }

    [ContextMenu("Test")]
    public void Test()
    {
        view.AddNewItem("1");
    }


}
