using Cinemachine.Utility;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class RollingState : GameState
{
    bool isHold = true;

    private Dice _dice;
    private Rigidbody _rigidBody;
    public RollingState(GameManager manager):base(manager)
    {
        
    }

    public override void Start()
    {
        base.Start();
        _dice = _manager.dice;
        _rigidBody = _dice.GetComponent<Rigidbody>();
        _manager.automataManager.recieveStr($"\troll ");
    }

    public override void Update()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Plane plane = new Plane(
            inNormal: Vector3.up,
            inPoint: Vector3.Project(_dice.originPosition-Camera.main.transform.position ,Vector3.up)
        ) ;
        float distance;
        plane.Raycast(ray,out distance);
        if (isHold)
        {
            var endPos = Camera.main.transform.position + ray.GetPoint(distance: distance);
            var trartPosition = _dice.transform.position;
            var force = endPos - trartPosition;
            
            _rigidBody.AddForce(force * _manager.diceSpeed);
            _rigidBody.AddTorque(force* _manager.diceSpeed);
            _dice.sinRotate(_manager.diceSpeed);
        }
        else if(_rigidBody.velocity.sqrMagnitude<_manager.threashold&&
            _rigidBody.angularVelocity.sqrMagnitude < _manager.threashold)
        {
            _manager.setNextState(new MovingState(_manager));
        }
        if (Input.GetMouseButtonUp(0))
        {
            _rigidBody.useGravity = true;
            isHold = false;
            runAudio();
        }
    }
    private async void runAudio()
    {
        await AnimationUtility.DelayAsync(.5f);
        _manager.soundManager.Play(SoundEffectType.roll);
    }
}
