using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = nameof(TextureTable),menuName =nameof(TextureTable))]
public class TextureTable : ScriptableObject
{
    public Texture2D minus;
    public Texture2D plus;
    public Texture2D stop;
    public Texture2D snake;
    public Texture2D ladder;
}
