using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallController : MonoBehaviour
{

    public static BallController instance;
    public Vector3 rootShooter;

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        rootShooter = new Vector3();
    }

    
}
