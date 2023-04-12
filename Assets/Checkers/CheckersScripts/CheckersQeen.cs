using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckersQeen : CheckersPiece
{
    private Vector3Int direction;
    private int tmpX;
    private int tmpY;

    public override List<Vector3Int> GetAvailableMoves(ref CheckersPiece[,] board, int tileCountX, int tileCountY)
    {
        List<Vector3Int> r = new List<Vector3Int>();

        for (int i = 0; i < 4; i++)
        {
            tmpX = currentX;
            tmpY = currentY;

            direction = i switch
            {
                0 => new Vector3Int(1, 1, 0),
                1 => new Vector3Int(1, -1, 0),
                2 => new Vector3Int(-1, 1, 0),
                3 => new Vector3Int(-1, -1, 0),
                _ => new Vector3Int(1, 1, 0)
            };

            while ((tmpX + direction.x) > -1 && (tmpX + direction.x) < tileCountX
                                             && (tmpY + direction.y) > -1 && (tmpY + direction.y) < tileCountY
                                             && (board[tmpX + direction.x, tmpY + direction.y] == null))
            {
                tmpX += direction.x;
                tmpY += direction.y;
                r.Add(new Vector3Int(tmpX, tmpY, 0));
            }

            if ((tmpX + 2 * direction.x) > -1 && (tmpX + 2 * direction.x) < tileCountX
                                          && (tmpY +  2 * direction.y) > -1 && (tmpY + 2 * direction.y) < tileCountY
                                          && board[tmpX + direction.x, tmpY + direction.y] != null
                                          && board[tmpX + direction.x, tmpY + direction.y].team != this.team)
            {
                r.Add(new Vector3Int(tmpX + 2 * direction.x, tmpY + 2 * direction.y, 1));
            }
        }
        return r;
    }
}
