using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;

public class BallScript : CellInfo
{
    public EffectSpecial effect;
    public int effectId;
    public int id;
    public int spriteId;
    public BallType type;

    void Awake()
    {
        Debug.Log("fuck");
    }

    void OnEnable()
    {
        FallEffect fall = gameObject.GetComponent<FallEffect>();
        if (fall != null)
        {
            Destroy(gameObject.GetComponent<FallEffect>());
        }
    }

    public void init(int id, int row, int col, Vector3 pos)
    {
        this.row = row;
        this.col = col;
        this.gameObject.transform.position = pos;
        this.id = id;
        setState(id);
        type = getBallType(id);
    }

    private BallType getBallType(int id)
    {
        switch (id)
        {
            case 0:
                return BallType.Color1;
            case 1:
                return BallType.Color2;
            case 2:
                return BallType.Color3;
            case 3:
                return BallType.Color4;
            case 4:
                return BallType.Color5;
            case 5:
                return BallType.Color1;
            case 6:
                return BallType.Color1;
            case 7:
                return BallType.Color1;
            case 8:
                return BallType.Color1;
            case 9:
                return BallType.Color1;
            default:
                return BallType.None;
        }
    }

    public void setState(int id)
    {
        SpriteRenderer render = GetComponent<SpriteRenderer>();
        render.sprite = GameController.instance.ballsImage[id];
    }

    public void destroyBall()
    {
        //show animation destroy and set ball is no used - non "destroy"
    }

    public void vibrate(float streng, Vector2 direction)
    {
        //show andimation vibrate, set state of ball 
        for (int k = 0; k < 3; k++) 
        {
            for (int i = 0; i < 5; i++)
            {
                gameObject.transform.Translate(new Vector3(streng, 0f, 0f));
            }
            streng *= -0.8f;
        }
    }

}
