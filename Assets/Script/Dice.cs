using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class Dice : MonoBehaviour
{
    // Start is called before the first frame update
    int _upperFace = 1;
    [SerializeField]
    AnimationCurve rollCurve;
    [SerializeField]
    float rollHeight;
    [SerializeField]
    float rollPeriod;
    public Vector3 originPosition;
    
    void Start()
    {
    }
    // Update is called once per frame
    void Update()
    {
        if (transform.position.magnitude > 20)
            transform.position = Vector3.up;
    }
    int upperFace {
        get { return _upperFace; }
        set { 
            if(_upperFace != value)
            {
                _upperFace = value; 
            }
        }
    }

    public int CheckUpperFace ()
    {
        float
            rDot = Vector3.Dot(Vector3.up, transform.right),
            gDot = Vector3.Dot(Vector3.up, transform.up),
            bDot = Vector3.Dot(Vector3.up, transform.forward),
            rDotAbs = Mathf.Abs(rDot),
            gDotAbs = Mathf.Abs(gDot),
            bDotAbs = Mathf.Abs(bDot)
            ;

        if (rDotAbs >= gDotAbs && rDotAbs >= bDotAbs)
        {
            upperFace = rDot > 0 ? 5 : 2;
        }
        else if (gDotAbs >= bDotAbs)
        {
            upperFace = gDot > 0 ? 3 : 4;
        }
        else
        {
            upperFace = bDot > 0 ? 1 : 6;
        }
        return upperFace;
    }
    public void setUpperFace(int faceNumber)
    {
        switch (faceNumber)
        {
            case 1:
                transform.forward = Vector3.up;
                break;
            case 2:
                transform.right = -Vector3.up; 
                break;
            case 3:
                transform.up = Vector3.up;
                break;
            case 4: 
                transform.up = -Vector3.up;
                break;
            case 5: 
                transform.right = Vector3.up;
                break;
            case 6: 
                transform.forward = -Vector3.up;
                break;
        }
    }
    public void ResetPosition()
    {
        transform.localPosition = originPosition;
    }

    internal void sinRotate(float speed)
    {
        this.GetComponent<Rigidbody>().AddTorque(
            new Vector3(
                Mathf.Sin(Time.time * Mathf.PI / 7 * 6),
                Mathf.Sin(Time.time * Mathf.PI / 12 * 6),
                Mathf.Sin(Time.time * Mathf.PI / 13 * 6)) * speed);
    }
    internal async Task Roll()
    {
        Vector3 startPostioin = Vector3.up;
        Vector3 endPosition = startPostioin + Vector3.up * rollHeight;
        float timeUse = 0;
        await Task.Yield();
        while (timeUse <= rollPeriod)
        {
            sinRotate(10);
            transform.position =
                Vector3.Lerp(startPostioin, endPosition, rollCurve.Evaluate(timeUse / rollPeriod));
            await Task.Yield();
            timeUse += Time.deltaTime;
        }
        await AnimationUtility.DelayAsync(.05f);
        GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
    }
}
