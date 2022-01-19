using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PMovement : Movement
{
    public static PMovement Player { private set; get; }

    void Awake()
    {
        if(Player != null && Player != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Player = this;
        }
    }

    protected override void Initialize()
    {
        base.Initialize();
    }

    public override void GetMomentum()
    {
        float vert = Input.GetAxis("Vertical");
        float horz = Input.GetAxis("Horizontal");
        float rot = 0f;
        float jump = 0f;

        if(Input.GetKey(KeyCode.Q))
        {
            rot = -1f;
        }
        else if (Input.GetKey(KeyCode.E))
        {
            rot = 1f;
        }

        if (Input.GetKey(KeyCode.Space))
        {
            jump = 1f;
        }
        else if (Input.GetKey(KeyCode.Z))
        {
            jump = -1f;
        }

        transform.Rotate(new Vector3(0f, rot, 0f));
        momentum = transform.forward * vert + transform.right * horz + transform.up * jump;
    }
}
