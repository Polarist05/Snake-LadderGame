using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MonoNode : MonoBehaviour
{
    internal SpriteRenderer bgRenderer;   
    internal TextMeshPro tmp;
    public List<MonoEdge> edges;
    // Start is called before the first frame update
    private void Awake()
    {
        bgRenderer = this.transform.GetComponentInChildren<SpriteRenderer>();
        tmp = this.transform.GetComponentInChildren<TextMeshPro>();
    }
    public MonoEdge findAcceptEdge(string str)
    {
        for (int i = 0; i < edges.Count; i++)
        {
            
            if (edges[i].isAccept(str))
            {
                return edges[i];
            }
            else
            {
                /*Debug.Log("NOT FIND");   
                Debug.Log($"{nameof(str)} :{str}");
                Debug.Log($"accept string :{edges[i].acceptString}");
                Debug.Log($"pattern :{edges[i].regexPattern}" );*/
            }
        }
        return null;
    }
}
