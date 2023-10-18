using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class ColorModel
{
    public string Name;
    public Material material;
}
[CreateAssetMenu(fileName =nameof(PawnColorTable),menuName =nameof(PawnColorTable))]
public class PawnColorTable : ScriptableObject
{
    public List<ColorModel> datas;
    public Material getMaterial(int index)
    {
        return datas[index].material;
    }
}
