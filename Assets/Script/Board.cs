using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour
{
    public int getIndex(Vector3 position)
    {
        var delta = position - transform.position;
        if (Mathf.Abs(delta.x) < 5 && Mathf.Abs(delta.z) < 5)
        {
            int z = (int)(delta.z + 5f);
            int x = (int)(delta.x + 5f);
            return (z * 10) + ((z % 2 == 0) ? x : (9 - x));
        }
        else
            return -1;
    }
    public Vector3 getPositionAt(int index) => getPositionAt(row: index / 10, column: index % 10);
    public Vector3 getPositionAt(int row, int column)
    {
        return ((row & 1) == 0) ?
            new Vector3(column - 4.5f, 0, row - 4.5f) :
            new Vector3(-(column - 4.5f), 0, row - 4.5f);
    }
}
