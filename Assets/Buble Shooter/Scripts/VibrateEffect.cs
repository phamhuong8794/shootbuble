using System.Collections;
using System.Collections.Generic;
using NUnit.Framework.Api;
using NUnit.Framework.Constraints;
using UnityEngine;
using UnityEngine.UI;

public class VibrateEffect : MonoBehaviour
{
    public float streng;
    public Vector2 direction;
    private Transform trans;
    private Vector3 origin;
    private int i, j;
    private bool onVibrate;
    private int LOOP = 1;
    private int MOVE = 10;

    void Awake()
    {
        onVibrate = false;
        this.trans = this.gameObject.transform;
        this.origin = trans.position;
    }

	// Use this for initialization
	void Start ()
	{
//	    this.streng = 0.75f;
//        this.direction = new Vector2(1f, 1f);
//	    this.i = 3;
//	    this.j = 5;
    }

    public void init(float streng, Vector2 direction)
    {
        if (onVibrate) return;
        this.origin = this.gameObject.transform.position;
        this.streng = streng;
        this.direction = setdirection(direction);
        this.i = LOOP;
        this.j = MOVE / 2;
        onVibrate = true;
    }

    public void readyDestroy()
    {
        onVibrate = false;
        this.i = 0;
        this.j = 0;
        this.gameObject.transform.position = origin;
    }

    private Vector2 setdirection(Vector2 direction)
    {
        if (Mathf.Approximately(0f, direction.y))
        {
            if(direction.x > 0) return new Vector2(1, 0);
            else return new Vector2(-1, 0);
        } else if (direction.x < 0)
        {
            if(direction.y > 0) return new Vector2(-1, 1);
            else return new Vector2(-1, -1);
        } else if (direction.x > 0)
        {
            if(direction.y > 0) return new Vector2(1, 1);
            return new Vector2(1, -1);
        }
        return new Vector2(0, 0);
    }

	// Update is called once per frame
	void Update () {
	    if (onVibrate)
	    {
	        Vector3 pos = trans.position;
	        pos += new Vector3(streng * direction.x, streng * direction.y, 0f);
	        this.gameObject.transform.position = pos;
	        if (i == 0 && j == 0)
	        {
	            Destroy(this);
	        }
	        if (i == 0)
	        {
	            if (j != MOVE / 2)
	            {
	                j--;
	            }
	            else
	            {
	                Destroy(this);
                }
	        }
	        else if (j != 0)
	        {
	            j--;
	        }
	        else
	        {
	            MOVE -= 2;
	            j = MOVE;
	            i--;
	            streng *= -1f;
	        }
        }	    
	}
}
