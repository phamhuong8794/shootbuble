using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Misc : MonoBehaviour {

    public static Vector3 indexToPosition(Index idx, BoundRect bounds)
    {
        float x;
        float y;

        if (idx.row % 2 == 0)
            x = GameController.instance.radius + 2 * GameController.instance.radius * idx.col;
        else
            x = 2 * GameController.instance.radius + 2 * GameController.instance.radius * idx.col;

        y = bounds.height - GameController.instance.radius - Mathf.Sqrt(3) * GameController.instance.radius * idx.row;

        return new Vector3(x + bounds.left, y + bounds.bottom, 0);
    }

    public static Index positionToIndex(Vector3 position, BoundRect bounds)
    {
        float x = position.x - bounds.left;
        float y = position.y - bounds.bottom;

        int row;
        int col;

        row = Mathf.FloorToInt((bounds.height - GameController.instance.radius - y + Mathf.Sqrt(3) / 2 * GameController.instance.radius) / (Mathf.Sqrt(3) * GameController.instance.radius));

        if (row % 2 == 0)
        {
            if (x < 0)
                x = 0;

            col = Mathf.FloorToInt(x / (2 * GameController.instance.radius));
        }
        else
        {
            if (x < GameController.instance.radius)
                x = GameController.instance.radius;

            col = Mathf.FloorToInt((x - GameController.instance.radius) / (2 * GameController.instance.radius));

            // NOTE: we need to check whether col is numberColumns - 1,
            // because in this case, half of the actual grid if out of the screen.
            // The actual reason behind this is that when our shooter's position is the
            // same as right bound, the above calculation would give us numberColumns-1,
            // since we round things up a little bit, which means:
            //      x position      x index
            //          0               0
            //          2R              1
            //          19R             cols-1
            if (col >= G.cols - 1)
                col = G.cols - 2;
        }

        return new Index(row, col);
    }

    public static Index[] GetNeighbours(Index current)
    {
        int r = current.row;
        int c = current.col;

        // neighbouring indices are different according to their row index.
        //   for even row:
        //       (r-1, c-1) (r-1, c)
        //     (r, c-1) (r, c) (r, c+1)
        //       (r+1, c-1) (r+1, c)
        //
        //   for odd row:
        //       (r-1, c) (r-1, c+1)
        //     (r, c-1) (r, c) (r, c+1)
        //       (r+1, c) (r+1, c+1)
        if (r % 2 == 0)
        {
            // even
            return new Index[] {
                new Index(r - 1, c - 1),    new Index(r - 1, c),
                new Index(r, c - 1),        new Index(r, c + 1),
                new Index(r + 1, c -1),     new Index(r + 1, c)
            };
        }
        else
        {
            // odd
            return new Index[] {
                new Index(r - 1, c),        new Index(r - 1, c + 1),
                new Index(r, c - 1),        new Index(r, c + 1),
                new Index(r + 1, c),        new Index(r + 1, c + 1)
            };
        }
    }
}
