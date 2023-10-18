using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;

public class MonoEdge : MonoBehaviour
{
    public MonoNode start;
    public MonoNode end;
    internal TextMeshPro textMeshPro;
    internal LineRenderer line;
    
    [Multiline(5)]
    [SerializeField]
    private string _acceptString;
    private string __acceptString;
    private string simpliflyAcceptString;
    private string _regexPattern;
    public string acceptString
    { 
        get { updateString(); return simpliflyAcceptString; }
    }
    public string regexPattern
    {
        get { updateString();return _regexPattern; }
    }
    private void updateString()
    {
        if (_acceptString != __acceptString)
        {
            __acceptString = _acceptString;
            simpliflyAcceptString = GameUtility.SimpliflyString(_acceptString);

            _regexPattern = "^(" + simpliflyAcceptString+")";
        }
    }
    private void Start()
    {
        textMeshPro = GetComponentInChildren<TextMeshPro>();
        line = GetComponentInChildren<LineRenderer>();
    }
    public bool isAccept(string str)
    {
        return Regex.IsMatch(str,regexPattern+".*") ;
    }
    [ContextMenu(nameof(addToNode))]
    private void addToNode()
    {
        if (start !=null&& !start.edges.Contains(this))
        {
            start.edges.Add(this);
        }
    }
}
