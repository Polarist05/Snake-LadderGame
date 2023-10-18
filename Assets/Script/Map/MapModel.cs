using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class MapModel 
{
    public string Name;
    public Material material;
    public List<int> eventPostion;
    public List<GameAction> nonEventAction;
}
