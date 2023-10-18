using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public enum GameActionType
{
    None = 0,
    Event = 1,
    Snake = 2,
    Ladder = 3
}
public enum GameEventType
{
    Forward = 0,
    Backword = 1,
    Stop = 2,
}
[Serializable]
public class GameAction
{
    public int position;
    public GameActionType type;
    public GameEventType eventType;
    [Range(1,99)]
    public int distance;
    public GameAction(EventModel evt,int position)
    {
        this.position= position;
        type = GameActionType.Event;
        eventType = evt.type;
        distance = evt.distance;
    }
    public GameAction(){}
}
