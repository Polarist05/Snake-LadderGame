using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class WaitForRollState : GameState
{
    private Dice _dice;
    private Rigidbody _rigidBody;
    private float startTime;
    public WaitForRollState(GameManager gameManager) : base(gameManager) 
    { 
        
    }

    public override void Start()
    {
        _dice = _manager.dice;
        _rigidBody = _dice.GetComponent<Rigidbody>();
        _rigidBody.useGravity = false;
        _dice.ResetPosition();
        if (_manager.view.rollLabel != null)
            _manager.view.rollLabel.style.display = DisplayStyle.Flex;
        _manager.automataManager.recieveStr($"\r\n\tplayer {_manager.currentTurn} walk : {{ \r\n\t");
        startTime = Time.time;
    }

    public override void Update()
    {
        _manager.CheckBoardRaycast();
        _dice.sinRotate(_manager.diceSpeed);
        if (Input.GetMouseButtonDown(0)&&!GameManager.pauseFlag)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, maxDistance: 50, layerMask: _manager.diceMask))
            {
                _manager.setNextState(new RollingState(_manager));
                setRollOpacity(0);
                return;
            }
        }
        setRollOpacity(Mathf.Sin(((Time.time-startTime) / 4)%1 * Mathf.PI));
    }
    public void setRollOpacity(float opacity)
    {
        var spriteRenderer = _manager.diceBase.GetComponent<SpriteRenderer>();
        spriteRenderer.color = new Color(
            spriteRenderer.color.r,
            spriteRenderer.color.g,
            spriteRenderer.color.b,
            opacity * .8f
        );
        _manager.view.rollLabel.style.opacity = opacity;

    }   
}
