using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Lean;

public class BallGrid
{
	private BallScript[,] _grids;	
	private int _rows;
	private int _cols;
	
    public BallGrid()
    {
        _rows = 50;
		_cols = 10;
		
		_grids = new BallScript[_rows, _cols];
    }
	
	public BallScript get(Index index)
	{
		return _grids[index.row, index.col];
	}
	
	public BallScript get(int row, int col)
	{
		return _grids[row, col];
	}
	
	public void set(Index index, BallScript bubble)
	{
		_grids[index.row, index.col] = bubble;
//	    _grids[index.row, index.col] = Lean.LeanPool.Spawn(GameController.instance.ballPrefab,
//	        new Vector3(0f, 0f, 0f), GameController.instance.gameObject.transform.rotation,
//	        GameController.instance.gameObject.transform).GetComponent<BallScript>();
//	    _grids[index.row, index.col].init(bubble.id, bubble.row, bubble.col, bubble.transform.position);
    }
	
	public void set(int row, int col, BallScript bubble)
	{
		_grids[row, col] = bubble;
	}
	
	public void remove(Index index)
	{
        _grids[index.row, index.col] = null;
	}
	
//	public void Recalculate(BoundRect rect)
//	{
//		for (int i = 0; i < _rows; i++)
//		{
//			for (int j = 0 ; j < _cols; j++)
//			{
//				var one = _grids[i, j];
//				if (one != null)
//				{
//					one.transform.position = Misc.indexToPosition(rect, new Index(i, j));
//				}
//			}
//		}
//	}
	
//	public List<Bubble.Type> GetAllUniqueTypes()
//	{
//		List<Bubble.Type> all = new List<Bubble.Type>();
//		
//		for (int i = 0; i < G.rows; i++)
//		{
//			for (int j = 0; j < G.cols; j++)
//			{
//				var one = _grids[i, j];
//				
//				if (one != null)
//				{
//					Bubble.Type type = one.type;
//					
//					if (!all.Contains(type))
//					{
//						all.Add(type);
//					}
//				}
//			}
//		}
//		
//		return all;
//	}
	
//	public void reset()
//	{
//		for (int i = 0; i < GameController.instance.rows; i++)
//		{
//			for (int j = 0; j < GameController.instance.cols; j++)
//			{
//				var one = _grids[i, j];
//				if (one != null)
//				{
//					Destroy(one.gameObject);
//					_grids[i, j] = null;
//				}
//			}
//		}
//	}
	
	public int count
	{
		get
		{
			int sum = 0;
			for (int i = 0; i < _rows; i++)
			{
				for (int j = 0; j < _cols; j++)
				{
					if (_grids[i, j] != null)
					{
						sum++;
					}
				}
			}
			return sum;
		}
	}
	
	public int maxRow
	{
		get
		{
			int max = -1;
			for (int i = 0; i < _rows; i++)
			{
				for (int j = 0; j < _cols; j++)
				{
					if (_grids[i, j] != null)
					{
						if (i > max)
						{
							max = i;
						}
					}
				}
			}
			return max;
		}
	}
}
