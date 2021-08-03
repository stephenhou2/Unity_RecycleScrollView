using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IRecycleScrollHandle 
{
    void OnItemEnter(int entityId,GameObject item);

    void OnItemExit(int entityId,GameObject item);
}
