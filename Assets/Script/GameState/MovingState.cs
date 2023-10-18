using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Scripting.APIUpdating;
using UnityEngine.UIElements;

public class MovingState : GameState
{
    private Dice _dice;
    int moveCnt;
    public MovingState(GameManager manager):base(manager)
    {
        
    }
    public override void Start()
    {
        base.Start();
        _dice = _manager.dice;
        moveCnt = _dice.CheckUpperFace();
        _manager.automataManager.recieveStr($"{moveCnt},\r\n\t\t");
        conditionTask = StartMoving();
    }
    public override void Update(){ _manager.CheckBoardRaycast(); }

    async Task StartMoving()
    {
        await FirstMove();
        _manager.automataManager.recieveStr(",");
        if (_manager.currentPawn.currentPosition == 99)
        {
            _manager.setNextState(new CompleteState(_manager));
            _manager.automataManager.recieveStr("\r\n\tendgame\r\n]");
#if !UNITY_EDITOR
            _manager.sendToJson();
#endif
        }
        else
        {
            _manager.MoveToNextTurn();
            _manager.setNextState(new WaitForRollState(_manager));
        }
    }
    async Task FirstMove()
    {
        await GameUtility.Move(_manager,_manager.currentPawn, moveCnt, stepByStep: true);
        _manager.automataManager.recieveStr(",\r\n\t\tactions:[");
        await checkEvent(true);
        _manager.automataManager.recieveStr("]\r\n\t}");
    }
    async Task MoveForward()
    {
        _manager.currentPawn.collectRollActivity(moveCnt);
        _manager.currentPawn.collectMoveActivity(
            _manager.currentPawn.currentPosition, 
            _manager.currentPawn.currentPosition+moveCnt,
            GameActionType.None);
        await GameUtility.Move(_manager,_manager.currentPawn, moveCnt, stepByStep: true);
        await checkEvent();
    }
    async Task MoveBackword()
    {
        _manager.currentPawn.collectMoveActivity(
            _manager.currentPawn.currentPosition,
            _manager.currentPawn.currentPosition - moveCnt,
            GameActionType.Event);
        await GameUtility.Move(_manager,_manager.currentPawn, -moveCnt, stepByStep: true);
        await checkEvent();
    }
    async Task MoveUpTo()
    {
        _manager.currentPawn.collectMoveActivity(
            _manager.currentPawn.currentPosition,
            _manager.currentPawn.currentPosition + moveCnt,
            GameActionType.Ladder);
        await GameUtility.Move(_manager, _manager.currentPawn, moveCnt, stepByStep: false);
        await checkEvent();
    }
    async Task MoveDownTo()
    {
        _manager.currentPawn.collectMoveActivity(
            _manager.currentPawn.currentPosition,
            _manager.currentPawn.currentPosition - moveCnt,
            GameActionType.Snake);
        await GameUtility.Move(_manager, _manager.currentPawn, -moveCnt, stepByStep: false);
        await checkEvent();
    }
    async Task checkEvent(bool startTag = false)
    {
        var evt = _manager.actions.Find(evt => (evt.position == _manager.currentPawn.currentPosition));
        if (evt != null)
        {
            if (!startTag)
                _manager.automataManager.recieveStr("\r\n\t\t\t},");
            _manager.automataManager.recieveStr("\r\n\t\t\t{");
            switch (evt.type)
            {
                case GameActionType.Event:
                    _manager.automataManager.recieveStr($"\r\n\t\t\t\tevent :{{ player {_manager.currentTurn} ");
                    switch (evt.eventType)
                    {
                        case GameEventType.Forward:
                            moveCnt = evt.distance;
                            _manager.automataManager.recieveStr($"forward {moveCnt}}},\r\n\t\t\t\t");
                            _manager.showGoodParticleAt(_manager.currentPawn);
                            await AnimationUtility.DelayAsync(GameManager.PARTICLE_DELAY);
                            await MoveForward();
                            break;
                        case GameEventType.Backword:
                            moveCnt = evt.distance;
                            _manager.automataManager.recieveStr($"backward {moveCnt}}},\r\n\t\t\t\t");
                            _manager.showBadParticleAt(_manager.currentPawn);
                            await AnimationUtility.DelayAsync(GameManager.PARTICLE_DELAY);
                            await MoveBackword();
                            break;
                        case GameEventType.Stop:
                            _manager.automataManager.recieveStr($"stop {evt.distance}}}");
                            _manager.IncreaseSkipTurn(_manager.currentTurn, evt.distance);
                            _manager.showBadParticleAt(_manager.currentPawn);
                            await AnimationUtility.DelayAsync(GameManager.PARTICLE_DELAY);
                            break;
                    }
                    break;
                case GameActionType.Snake:
                    moveCnt = evt.distance;
                    _manager.automataManager.recieveStr($"\r\n\t\t\t\tsnake ,\r\n\t\t\t\t");
                    _manager.showBadParticleAt(_manager.currentPawn);
                    await AnimationUtility.DelayAsync(GameManager.PARTICLE_DELAY);
                    await MoveDownTo();
                    break;
                case GameActionType.Ladder:
                    moveCnt = evt.distance;
                    _manager.automataManager.recieveStr("\r\n\t\t\t\tladder ,\r\n\t\t\t\t");
                    _manager.showGoodParticleAt(_manager.currentPawn);
                    await AnimationUtility.DelayAsync(GameManager.PARTICLE_DELAY);
                    await MoveUpTo();
                    break;
            }
            if (startTag)
                _manager.automataManager.recieveStr("\r\n\t\t\t}\r\n\t\t");
        }
    }
}
