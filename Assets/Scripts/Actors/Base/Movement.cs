using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    [Header("Base Movement Stats")]
    public float speed;
    public Vector3 tether; //Aka: Spawn Location
    protected Vector3 momentum;

    // Start is called before the first frame update
    void Start()
    {
        Initialize();
    }

    // Update is called once per frame
    void Update()
    {
        GameLoop();
    }

    public void SetSpawn(Vector3 location)
    {
        tether = location;
        transform.position = location;
    }

    protected virtual void Initialize()
    {

    }

    protected virtual void GameLoop()
    {
        GetMomentum();

        transform.position += momentum * speed * Time.deltaTime;
    }

    public virtual void GetMomentum()
    {
        momentum.x = 0f;
        momentum.z = 0f;
    }
}
