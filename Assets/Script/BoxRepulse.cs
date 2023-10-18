using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxRepulse : MonoBehaviour
{

    private void Start()
    {
    }
    private void OnTriggerEnter(Collider other)
    {
        var rigid=other.GetComponent<Rigidbody>();
        if (rigid!=null)
        {
            Vector3 vec=Vector3.Project(rigid.velocity,this.transform.right);
            if (Vector3.Angle(vec, this.transform.right) > 90)
            {
                rigid.velocity-=vec*2;
            }
            rigid.AddForce(this.transform.right*500);
        }
    }

}
