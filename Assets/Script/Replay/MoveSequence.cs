using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class MoveSequence : ReplaySequence
{
    int startPosition;
    int endPosition;
    bool stepByStepFlag,eventFlag;
    public MoveSequence(int pawnIndex,string automataStr,int startPosition,int endPosition,bool stepByStepFlag,bool eventFlag) : base(pawnIndex,automataStr)
    {
        this.startPosition = startPosition;
        this.endPosition = endPosition;
        this.stepByStepFlag = stepByStepFlag;
        this.eventFlag = eventFlag;
    }
    public override async Task Invoke(GameManager manager)
    {
        await base.Invoke(manager);
        if (eventFlag)
        {
            if (endPosition < startPosition)
                manager.showBadParticleAt(manager.currentPawn);
            else
                manager.showGoodParticleAt(manager.currentPawn);
        }
        await AnimationUtility.DelayAsync(GameManager.PARTICLE_DELAY);
        await GameUtility.MoveTo(manager, manager.pawns[pawnIndex], endPosition, stepByStep: stepByStepFlag);
    }
    public override async Task MoveToNext(GameManager manager)
    {
        await GameUtility.MoveTo(manager, manager.pawns[pawnIndex], endPosition, isAnimation: false);
        await base.MoveToNext(manager);
    }
    public override async Task MoveToPrevious(GameManager manager)
    {
        await GameUtility.MoveTo(manager, manager.pawns[pawnIndex], startPosition, isAnimation: false);
        await base.MoveToPrevious(manager);
    }
}
