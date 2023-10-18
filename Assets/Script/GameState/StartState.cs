using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartState : GameState
{
    public StartState(GameManager manager):base(manager)
    {
        
    }

    public override void Start()
    {
        _manager.ResetGame();
        _manager.GenerateRandomActions();
        _manager.automataManager.resetAutomata(_manager); 
        _manager.automataManager.recieveStr("walks:[");
    }

    public override void Update()
    {
        _manager.setNextState(new WaitForRollState(_manager));
        
    }
}
