using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public abstract class GameState 
{
    protected GameManager _manager;
    public static Task conditionTask;
    private GameState() { }
    public GameState(GameManager manager) { _manager = manager; }
    public virtual void Start() {
        _manager.automataManager.ClearEdges();
    }
    public abstract void Update();
}
