using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class RollSequence : ReplaySequence
{
    int upperFace;
    public RollSequence(int pawnIndex,string automataStr,int upperFace) : base(pawnIndex,automataStr)
    {
        this.upperFace = upperFace;
    }
    public override async Task Invoke(GameManager manager)
    {
        manager.currentTurn = pawnIndex;
        await base.Invoke(manager);
        manager.soundManager.Play(SoundEffectType.roll);
        await manager.dice.Roll();
        manager.dice.setUpperFace(upperFace);
    }
}
