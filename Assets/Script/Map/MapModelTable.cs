using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = nameof(MapModelTable),menuName = nameof(MapModelTable))]
public class MapModelTable : ScriptableObject
{
    public List<MapModel> maps;
}
