using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class ReplayState : GameState
{
    public string automataString;
    ReplaySequence _startSequence, _currentSequence;
    public ReplayState(GameManager manager,string automataString) : base(manager)
    {
        this.automataString = automataString;
    }
    public override void Start()
    {
        void Add(ReplaySequence sequence)
        {
            if (sequence != null)
            {
                if (_currentSequence == null)
                    _startSequence = sequence;
                else
                    _currentSequence.addNextSequence(sequence);
                _currentSequence = sequence;
            }
        }
        List<int> positions = new List<int>(6);
        for (int i= 0;i < positions.Capacity;i++)
        {
            positions.Add(GameManager.START_POSITION);
        }
        var strings = automataString.Split("\r\n");
        string buffer = "";
        string simpBuffer = "";
        int currentPawnIndex=0;
        AutomataManager automata = _manager.automataManager;
        foreach (var str in strings)
        {
            AddBuffer(str+"\r\n");
            var nodes =automata.nodes;
            if (nodes.Contains(automata.startRollNode))
            {
                var match = Regex.Match(simpBuffer, @"player([0-5])walk.*roll([1-6])");
                currentPawnIndex = Int32.Parse(match.Groups[1].Value);
                var diceFace = Int32.Parse(match.Groups[2].Value);
                Add(new RollSequence(currentPawnIndex,buffer,diceFace));
                ClearBuffer();
            }
            else if (nodes.Contains(automata.moveNode))
            {
                var match = Regex.Match(simpBuffer, @"move_to([1-9][0-9]|[0-9])");
                var endPosition = Int32.Parse(match.Groups[1].Value);
                Add(new MoveSequence(currentPawnIndex,buffer, positions[currentPawnIndex], endPosition,true,false));
                positions[currentPawnIndex] = endPosition;
                ClearBuffer();
            }
            else if (nodes.Contains(automata.stopNode))
            {
                Add(new StopSequence(currentPawnIndex,buffer));
                ClearBuffer();
            }
            else if (nodes.Contains(automata.loopNode))
            {
                Match match=Regex.Match(simpBuffer, @"(snake|ladder|forward|backward).*move_to([1-9][0-9]|[0-9])");
                var endPosition = Int32.Parse(match.Groups[2].Value);
                switch (match.Groups[1].Value)
                {
                    case "snake":
                        Add(new MoveSequence(currentPawnIndex, buffer, positions[currentPawnIndex], endPosition, false, true));
                        break;
                    case "ladder":
                        Add(new MoveSequence(currentPawnIndex, buffer, positions[currentPawnIndex], endPosition, false, true));
                        break;
                    case "forward":
                        Add(new MoveSequence(currentPawnIndex, buffer, positions[currentPawnIndex], endPosition, true, true));
                        break;
                    case "backward":
                        Add(new MoveSequence(currentPawnIndex, buffer, positions[currentPawnIndex], endPosition, true, true));
                        break;
                }
                positions[currentPawnIndex]=endPosition;
                ClearBuffer();
            }
            else if (nodes.Contains(automata.endNode))
            {
                int endPosition = Int32.Parse(Regex.Match(simpBuffer, @"move_to([1-9][0-9]|[0-9])").Groups[1].Value);
                string type = Regex.Match(simpBuffer, @"(snake|ladder|forward|backward)").Value;
                if (type !=string.Empty) {
                    Add(new MoveSequence(currentPawnIndex, buffer, positions[currentPawnIndex], endPosition, true, true));
                }
                else
                {
                    Add(new MoveSequence(currentPawnIndex, buffer, positions[currentPawnIndex], endPosition, true, false));
                }
                positions[currentPawnIndex] = endPosition;
                ClearBuffer();
            }
        }
        void AddBuffer(string value)
            {
                string simp = GameUtility.SimpliflyString(value);
                buffer += value;
                simpBuffer += simp;
                _manager.automataManager.recieveStr(value);
            }
        void ClearBuffer()
        {
            buffer = string.Empty;
            simpBuffer = string.Empty;
            automata.ClearEdges();
        }
        _manager.automataManager.resetAutomata(_manager);
        _ = LoopRun();   
    }
    public async Task LoopRun()
    {
        _currentSequence=_startSequence;
        await _currentSequence.Invoke(_manager);
        while (_currentSequence!=null)
        {
            await _currentSequence.MoveToNext(_manager);
            _currentSequence = _currentSequence.next;

        }
        
        _manager.setNextState(new CompleteState(_manager));
    }
    public override void Update()
    {
        _manager.CheckBoardRaycast();
    }
}
