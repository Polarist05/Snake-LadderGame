using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class StopSequence : ReplaySequence
{
    public StopSequence(int pawnIndex,string automataStr) : base(pawnIndex,automataStr)
    {
    }
    public override async Task Invoke(GameManager manager)
    {
        await base.Invoke(manager);
        manager.showBadParticleAt(manager.currentPawn);
        await AnimationUtility.DelayAsync(GameManager.PARTICLE_DELAY);
    }
}
