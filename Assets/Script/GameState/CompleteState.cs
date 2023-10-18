using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class CompleteState : GameState
{
    public CompleteState(GameManager manager):base(manager) 
    {
        _manager.confettiParticle.Play();
        _manager.soundManager.Play(SoundEffectType.win);
        _ = initialAsync();
    }
    private async Task initialAsync()
    {
        await AnimationUtility.DelayAsync(3);
        _ = AnimationUtility.FadeIn(_manager.view.endGameElement, 1f);
        _manager.view.endGameElement.restartBtn.clicked += OnClick;
        _manager.view.endGameElement.label.text = $"{_manager.recieveData.pawnColor[_manager.currentTurn]} WIN".ToUpper();
    }
    public async void OnClick()
    {
        //_manager.view.endGameElement.restartBtn.clicked -= OnClick;
        //await AnimationUtility.FadeOut(_manager.view.endGameElement);
        #if !UNITY_EDITOR
        _manager.Summarize();    
        #endif
    }
    public override void Start()
    {
        base.Start();
    }
    public override void Update()
    {

    }

}
