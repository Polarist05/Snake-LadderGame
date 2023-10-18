using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class Pawn : MonoBehaviour
{
    public int currentPosition;
    public int skipTurnCount=0;
    #region
    public int totalDistance = 0;
    public int totalDecrement = 0;
    public int snakeCount = 0;
    public int eventCount = 0;
    public int sameDiceCount = 0;
    public int lastDiceFace = 0;
    #endregion
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public async Task Move(  Vector3 endPosition, float duration,AnimationCurve animation,bool isAnimation = true)
    {
        if (isAnimation)
        {
            float timeUse = 0;
            await Task.Yield();
            while (timeUse <= duration)
            {
                transform.position =
                    Vector3.Lerp(transform.position, endPosition, animation.Evaluate(timeUse / duration));
                await Task.Yield();
                timeUse += Time.deltaTime;
            }
        }
        else
        {
            transform.position=endPosition;
        }
    }

    public void collectMoveActivity(int start, int end,GameActionType type =GameActionType.None)
    {
        var distance = end - start;
        totalDistance += Math.Abs(distance);
        if (distance < 0) totalDecrement += Math.Abs(distance) ;
        if (type == GameActionType.Snake) snakeCount++;
        if (type == GameActionType.Event) eventCount++;
    }
    public void collectRollActivity(int diceFace)
    {
        if (lastDiceFace == diceFace)
            sameDiceCount++;
        else
            sameDiceCount = 0;
        lastDiceFace = diceFace;
    }

    public void SetMaterial(Material material)
    {
        this.GetComponentInChildren<MeshRenderer>().material = material;
    }
    public Material GetMaterial() 
    {
        return this.GetComponentInChildren<MeshRenderer>().material;
    }
}
