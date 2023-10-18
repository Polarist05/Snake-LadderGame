using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;

[Serializable]
public class EventModel 
{
    public GameEventType type;
    [Range(1,6)]
    public int distance;
}
