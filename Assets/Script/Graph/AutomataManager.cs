using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;

public class AutomataManager : MonoBehaviour
{
    public MonoNode startNode;
    public MonoNode endNode;
    public MonoNode startRollNode;
    public MonoNode endRollNode;
    public MonoNode lastMoveNode;
    public MonoNode moveNode;
    public MonoNode moveActionNode1;
    public MonoNode loopNode;
    public MonoNode stopNode;
    public TMPro.TextMeshPro allStringTMP;
    public int maxLine;
    public Color originalNodeColor;
    public Color hilightNodeColor;
    public Color startEndNodeColor;
    public Gradient hilightEdgeColor;
    
    MonoNode _currentNode;
    public MonoNode currentNode {
        get { return _currentNode; }
        set { 
            if(_currentNode != value)
            {
                if(_currentNode != null)
                {
                    if(_currentNode != startNode && _currentNode != endNode)
                        _currentNode.bgRenderer.color = originalNodeColor;
                    else
                        _currentNode.bgRenderer.color = startEndNodeColor;
                }
                if (value!=startNode && value != null)
                    value.bgRenderer.color = hilightNodeColor;
                _currentNode = value;
            }
        }
    }
    [SerializeField]
    string bufferStr="";
    [Multiline(600)]
    [SerializeField]
    private string _AllString="";
    public string AllString {
        get { return _AllString; }
        set { 
            _AllString = value;
            displayAllString();
        }
    }

    private void displayAllString()
    {
        var strs = AllString.Split("\r\n").ToList();
        var lineCnt = strs.Count;
        var outputStr = "";
        var maxLine = Math.Min(this.maxLine,lineCnt);
        for (int i = 0; i < maxLine; i++)
        {
            outputStr += strs[lineCnt - maxLine + i] + "\r\n";
        }
        allStringTMP.text = outputStr;
    }

    private List<MonoEdge> edges = new List<MonoEdge>();
    public List<MonoNode> nodes => edges.Select(edge => edge.end).ToList();

    private void Awake()
    {
        currentNode = startNode;
    }
    public void recieveStr(string str)
    {
        AllString += str;
        str = GameUtility.SimpliflyString(str);
        bufferStr = bufferStr + str;
        MonoEdge edge = currentNode.findAcceptEdge(bufferStr);
        AddEdge( ref edge);
    }
    public void ClearEdges()
    {
        for (int i = 0; i < edges.Count; i++)
        {
            var line = edges[i].line;
            line.startColor = Color.white;
            line.endColor = Color.white;
        }
        edges.Clear();
    }
    private void AddEdge( ref MonoEdge edge)
    {
        var nextColor = hilightEdgeColor.Evaluate(0);
        while (edge != null && bufferStr.Length != 0)
        {
            edges.Add(edge);
            currentNode = edge.end;
            bufferStr = Regex.Replace(bufferStr, edge.regexPattern, "");
            edge = currentNode.findAcceptEdge(bufferStr);
        }
        for (int i = 0; i < edges.Count; i++)
        {
            var line = edges[i].line;
            line.startColor = nextColor;
            nextColor = hilightEdgeColor.Evaluate((float)(i + 1) / edges.Count);
            line.endColor = nextColor;
        }
    }
    internal void resetAutomata(GameManager manager)
    {
        ClearEdges();
        currentNode = startNode;
        bufferStr = string.Empty;
        AllString = manager.GenerateSetupLanguage()+",\r\n";
    }

    public static string FromListToString(List<string> list)
    {
        if (list.Count == 0)
            return "[]";
        var listStr = list[0];
        for (int i = 1; i < list.Count; i++)
        {
            listStr= listStr +", "+list[i];
        }
        return $"[{listStr}]";
    }

}
