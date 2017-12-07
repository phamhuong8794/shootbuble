using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Policy;
using Lean;
using UnityEngine;
using Random = UnityEngine.Random;

public enum BallType
{
    None = 0,
    // normal bubbles
    Color1 = 1,
    Color2 = 2,
    Color3 = 3,
    Color4 = 4,
    Color5 = 5,
    Color6 = 6,
    Color7 = 7,
    Color8 = 8,
    Color9 = 9
}

public enum RemoveType
{
    ChainRemoval,
    UnlinkRemoval
}

public class GameController : MonoBehaviour
{
    public static GameController instance;

    public Vector3 screen;
    public float scaleWidth;
    public float minTop;

    [Header("Ball Properties")]
    public GameObject ballPrefab;
    public GameObject compressor;
    public BallGrid grid;
    public Sprite[] ballsImage = new Sprite[9];
    public float margin;
    public float radius;
    public int maxBall;

    [Header("Shooter action")]
    public BallScript bulletBall;
    public BallScript nextBall;
    public bool isFire;
    public float shootingAngle;
    public float dx, dy;
    public float speed;
    public Finger f;
    public float limitAngle = 10;
    public BoundRect boundRect;
    public float realWidth, realHeight;

    void Awake()
    {
        instance = this;
        grid = new BallGrid();
        screen = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, 0f));
        scaleWidth = screen.x / Screen.width;
        isFire = false;
        speed = 20f;
        maxBall = 3;
    }

    void Start()
    {
        Vector3 ballSize = ballPrefab.GetComponent<SpriteRenderer>().bounds.size;
        radius = ballSize.x / 2 + margin;
        realWidth = G.cols * radius * 2;
        realHeight = calculatorRealHeight();
        minTop = screen.y - compressor.GetComponent<SpriteRenderer>().bounds.size.y;
        boundRect = new BoundRect(-realWidth / 2f - radius / 2, -screen.y / 5, realWidth, realHeight);
        initBallMatrix(G.rows, G.cols);
        LoadShooterBubble();
    }

    public float calculatorRealHeight()
    {
        Vector3 compressorPos = compressor.transform.position;
        return compressorPos.y + screen.y / 5;
    }

    // Update is called once per frame
    void Update()
    {
        if (isFire)
        {
            Vector3 position = bulletBall.transform.position;
            ParkingStateInfo info = isFinalParkingIndex(speed, position.x, position.y, dx, dy);

            dx = info.dx;
            dy = info.dy;
            if (info.final)
            {
                ParkBubble(new Vector3(info.x, info.y));
            }
            else
            {
                bulletBall.transform.position = new Vector3(info.x, info.y, position.z);
            }
            return;
        }
        if (Input.GetMouseButtonUp(0))
        {
            f.x = Input.mousePosition.x;
            f.y = Input.mousePosition.y;
            handleTouchMove(f);
            handleTouchEnd();
        }

    }
    private void ParkBubble(Vector3 currentPosition)
    {
        Index index = Misc.positionToIndex(currentPosition, boundRect);
        Vector3 position = Misc.indexToPosition(index, boundRect);

        bulletBall.transform.position = position;
        grid.set(index, bulletBall);

        OnVibrate(index);
        OnChain(index);

        OnUnlink(index);
        LoadShooterBubble();

        //		_newRoot++;
        //		if (_newRoot == timeBeforeNewRoot - 1)
        //		{
        //			// shake compressor
        //			iTween.ShakePosition(compressor, iTween.Hash("x", 5.0f, "y", 5.0f, "time", 1.0f, "looptype", "loop"));
        //		}
        //		else if (_newRoot > timeBeforeNewRoot)
        //		{
        //			_newRoot = 0;
        //
        //			AudioManager.Instance.Play(newRootSoloClip);
        //			
        //			iTween.Stop(compressor);			
        //
        //			float delta = Mathf.Sqrt(3.0f) * G.radius;
        //			compressor.transform.MoveBy(0, -delta, 0);
        //			_compressorLevel++;
        //			
        //			// recalculate the bubbles' positions
        //			_boundRect.top -= delta;
        //			_grid.Recalculate(_boundRect);
        //		}

        isFire = false;

    }

    private void LoadShooterBubble()
    {
        if (grid.count > 0)
        {
            // with random colors or from a specific sequence!
            if (nextBall == null)
            {
                nextBall = createBallWithPos(getNextPosition(), Random.Range(0, maxBall));
                // make it smaller
                nextBall.transform.localScale = new Vector3(0.6f, 0.6f, 1.0f);
            }

            bulletBall = createBallWithPos(getShooterPosition(), nextBall.id);

            Lean.LeanPool.Despawn(nextBall.transform);
            int t = Random.Range(0, maxBall);
            nextBall = createBallWithPos(getNextPosition(), t);
            // make it smaller
            nextBall.transform.localScale = new Vector3(0.6f, 0.6f, 1.0f);
        }

        isFire = false;
    }

    private void OnVibrate(Index index)
    {
        List<Index> vibrates = GetVibrate(index);
        vibrateBubbles(vibrates, index);

    }

    private void OnChain(Index index)
    {
        List<Index> chains = GetChain(index);
        if (chains.Count >= 3)
        {
            RemoveBubbles(chains, RemoveType.ChainRemoval);

            //            AudioManager.Instance.Play(destroyGroupClip);
        }
        //        else
        //        {
        //            AudioManager.Instance.Play(stickClip);
        //        }
    }

    private void vibrateBubbles(List<Index> bubbles, Index root)
    {
        float streng = 0.5f;
        Vector3 rootPosition = grid.get(root).transform.position;
        for (int i = 0; i < bubbles.Count; i++)
        {
            var b = grid.get(bubbles[i]);
            if (b == null) continue;
            Vector3 pos = b.transform.position;
            VibrateEffect vibrate = b.gameObject.AddComponent<VibrateEffect>();
            vibrate.init(streng,
                new Vector2(pos.x - rootPosition.x, pos.y - rootPosition.y));
        }
    }

    private void RemoveBubbles(List<Index> bubbles, RemoveType type)
    {
        for (int i = 0; i < bubbles.Count; i++)
        {
            var b = grid.get(bubbles[i]);

            if (type == RemoveType.UnlinkRemoval)
            {
                FallEffect fall = b.gameObject.AddComponent<FallEffect>();
                fall.gravity = 1500.0f;
                fall.initialAngle = 0;
                fall.initialVelocity = 0;
                fall.lowerLimit = boundRect.bottom;
            }
            else
            {
                FallEffect fall = b.gameObject.AddComponent<FallEffect>();
                fall.gravity = 1500.0f;

                if (i == 0)
                    fall.initialAngle = 15.0f;
                else if (i == bubbles.Count - 1)
                    fall.initialAngle = 180.0f - 15.0f;
                else
                    fall.initialAngle = Random.Range(15.0f, 175.0f);

                fall.initialVelocity = Random.Range(100.0f, 200.0f);
                fall.lowerLimit = boundRect.bottom;
            }

            grid.remove(bubbles[i]);
        }
    }

    private List<Index> GetVibrate(Index index)
    {
        List<Index> vibrateList = new List<Index>();
        Index[] indexs = Misc.GetNeighbours(index);
        addNeighbours(vibrateList, index);
        for (int i = 0; i < indexs.Length; i++)
        {
            addNeighbours(vibrateList, indexs[i]);
        }

        return vibrateList;
    }

    private void addNeighbours(List<Index> list, Index idx)
    {
        Index[] indexs = Misc.GetNeighbours(idx);
        for (int i = 0; i < indexs.Length; i++)
        {
            if (checkOverlap(list, indexs[i])) list.Add(indexs[i]);
        }
    }

    private bool checkOverlap(List<Index> list, Index index)
    {
        for (int i = 0; i < list.Count; i++)
        {
            if (list[i].col == index.col && list[i].row == index.row) return false;
        }
        return true;
    }

    private List<Index> GetChain(Index startIndex)
    {
        BallType shooterType = bulletBall.type;
        List<Index> chainList = new List<Index>();

        bool[,] visited = new bool[G.rows, G.cols];
        ClearVisitedList(visited);

        List<Index> dfsList = new List<Index>();
        dfsList.Add(startIndex);
        visited[startIndex.row, startIndex.col] = true;

        while (dfsList.Count != 0)
        {
            // pop the first entry
            Index current = dfsList[0];
            dfsList.RemoveAt(0);

            // add this to the final chain list!
            chainList.Add(current);

            Index[] neighbours = Misc.GetNeighbours(current);

            foreach (var next in neighbours)
            {
                if (InNewChain(visited, next, shooterType))
                {
                    dfsList.Add(next);
                    visited[next.row, next.col] = true;
                }
            }
        }

        return chainList;
    }

    private void ClearVisitedList(bool[,] visited)
    {
        for (int i = 0; i < G.rows; i++)
        {
            for (int j = 0; j < G.cols; j++)
            {
                visited[i, j] = false;
            }
        }
    }

    private bool InNewChain(bool[,] visited, Index next, BallType type)
    {
        if (IsIndexValid(next) &&
            visited[next.row, next.col] == false &&
            grid.get(next) != null)
        {
            BallScript bubble = grid.get(next);

            if (type != BallType.None && bubble.type == type)
            {
                return true;
            }
        }

        return false;
    }

    private bool IsIndexValid(Index index)
    {
        return index.row >= 0 && index.row < G.rows &&
               index.col >= 0 && index.col < G.cols;
    }

    private void OnUnlink(Index index)
    {
        List<Index> unlinked = GetUnlinked();
        RemoveBubbles(unlinked, RemoveType.UnlinkRemoval);
    }

    private List<Index> GetUnlinked()
    {
        List<Index> dfsList = new List<Index>();

        bool[,] visited = new bool[G.rows, G.cols];
        ClearVisitedList(visited);

        for (int i = 0; i < G.cols; i++)
        {
            if (grid.get(0, i) != null)
            {
                dfsList.Add(new Index(0, i));
                visited[0, i] = true;
            }
        }

        while (dfsList.Count != 0)
        {
            Index current = dfsList[0];
            dfsList.RemoveAt(0);

            Index[] neighbours = Misc.GetNeighbours(current);

            foreach (var next in neighbours)
            {
                if (IsIndexValid(next) &&
                    visited[next.row, next.col] == false &&
                    grid.get(next) != null)
                {
                    dfsList.Add(next);
                    visited[next.row, next.col] = true;
                }
            }
        }

        // final processing! those un-visited bubbles are unlinked ones.
        List<Index> unlinked = new List<Index>();
        for (int i = 0; i < G.rows; i++)
        {
            for (int j = 0; j < G.cols; j++)
            {
                if (grid.get(i, j) != null && visited[i, j] == false)
                {
                    unlinked.Add(new Index(i, j));
                }
            }
        }

        return unlinked;
    }

    public void startFire()
    {
        isFire = true;
        dx = Mathf.Cos(shootingAngle * Mathf.Deg2Rad);
        dy = Mathf.Sin(shootingAngle * Mathf.Deg2Rad);
    }

    public void handleTouchMove(Finger f)
    {
        Vector3 touchPosition = Camera.main.ScreenToWorldPoint(new Vector3(f.x, f.y));
        Vector3 midPosition = getShooterPosition();

        if (touchPosition.y >= midPosition.y)
        {
            shootingAngle = Mathf.Rad2Deg * Mathf.Atan2(touchPosition.y - midPosition.y, touchPosition.x - midPosition.x);
            shootingAngle = Mathf.Clamp(shootingAngle, 0.0f + limitAngle, 180.0f - limitAngle);

            //            cannon.transform.rotation = Quaternion.Euler(0, 0, shootingAngle - 90.0f);
        }
    }

    public ParkingStateInfo isFinalParkingIndex(float speed, float x, float y, float dx, float dy)
    {
        bool shouldPark = false;
        float preX = x;
        float preY = y;
        x += dx * speed;
        y += dy * speed;
        if (x < boundRect.left + radius)
        {
            x = boundRect.left + radius;
            dx *= -1;
        }
        else if (x > boundRect.right)
        {
            x = boundRect.right;
            dx *= -1;
        }
        if (y > boundRect.top - radius)
        {
            y = boundRect.top - radius;
            shouldPark = true;
        }

        if (IsCollidingOthers(new Vector3(x, y)))
        {
            shouldPark = true;
        }
        if (shouldPark)
        {
            Index parkingIndex = Misc.positionToIndex(new Vector3(x, y), boundRect);
            if (parkingIndex.row == G.rows) G.rows += 1;
            // we go backtrack to find the first non-colliding point!
            while (!(Mathf.Approximately(x, preX) && Mathf.Approximately(y, preY)))
            {
                // let's try 2 points a step!
                x -= dx * 2.0f;
                y -= dy * 2.0f;

                if (!IsCollidingOthers(new Vector3(x, y)))
                {
                    break;
                }
            }

            parkingIndex = Misc.positionToIndex(new Vector3(x, y), boundRect);
            Vector3 tempPosition = Misc.indexToPosition(parkingIndex, boundRect);
            x = tempPosition.x;
            y = tempPosition.y;
        }
        return new ParkingStateInfo(shouldPark, x, y, dx, dy);
    }

    public bool IsCollidingOthers(Vector3 bulletBall)
    {
        BallScript t;
        for (int i = 0; i < G.rows; i++)
        {
            for (int j = 0; j < G.cols; j++)
            {
                t = grid.get(i, j);
                if (t != null && checkColliding(bulletBall, t.transform.position, radius)) return true;
            }
        }
        return false;
    }

    public bool checkColliding(Vector3 ball1, Vector3 ball2, float radius)
    {
        float t1 = ball1.x - ball2.x;
        float t2 = ball1.y - ball2.y;
        float r = (radius - margin * 3) * 2;
        return r * r >= t1 * t1 + t2 * t2;
    }

    public void handleTouchEnd()
    {
        startFire();
    }

    //tao ma tran bong
    public void initBallMatrix(int rows, int cols)
    {

        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < cols; j++)
            {
                grid.set(i, j, createBallWithIndex(new Index(i, j), Random.Range(0, maxBall)));
            }
        }
    }

    private Vector3 getNextPosition()
    {
        Vector3 shooterPosition = getShooterPosition();
        return new Vector3(shooterPosition.x - screen.x / 2, shooterPosition.y);
    }
    public Vector3 getShooterPosition()
    {
        return new Vector3(0, -screen.y + 100f);
    }

    public BallScript createBallWithPos(Vector3 pos, int id)
    {
        var newBall = Lean.LeanPool.Spawn(GameController.instance.ballPrefab).GetComponent<BallScript>();
        newBall.init(id, -1, -1, pos);
        return newBall;
    }

    private BallScript createBallWithIndex(Index index, int ballType)
    {
        var newBall = Lean.LeanPool.Spawn(ballPrefab, Misc.indexToPosition(index, boundRect),
            gameObject.transform.rotation, gameObject.transform).GetComponent<BallScript>();
        newBall.init(ballType, index.row, index.col, Misc.indexToPosition(index, boundRect));
        return newBall;
    }
}
