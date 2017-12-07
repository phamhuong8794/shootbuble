using UnityEngine;
using System.Collections;

public class MoveSample : MonoBehaviour
{	
	void Start(){
//		iTween.MoveBy(gameObject, iTween.Hash("x", 2, "easeType", "easeInOutExpo", "loopType", "pingPong", "delay", .1));
	    iTween.ShakePosition(gameObject, iTween.Hash("x", 0.08f, "time", 3f, "loopType", "loop"));
    }
}

