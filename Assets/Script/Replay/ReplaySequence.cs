using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class ReplaySequence 
{
    public int pawnIndex;
    public ReplaySequence next = null, previous = null;
    public string automataStr;
    public ReplaySequence(int pawnIndex, string automataStr)
    {
        this.pawnIndex = pawnIndex;
        this.automataStr = automataStr;
    }

    public virtual async Task Invoke(GameManager manager) {
        manager.automataManager.recieveStr(automataStr);
    }
    public virtual async Task  MoveToNext(GameManager manager) {
        if(next!=null)
            await next.Invoke(manager);
    }
    public virtual async Task MoveToPrevious(GameManager manager) {
        if (previous != null)
            await previous.Invoke(manager); 
    }
    public void addNextSequence(ReplaySequence sequence)
    {
        sequence.previous = this;
        next = sequence;
    }
}
