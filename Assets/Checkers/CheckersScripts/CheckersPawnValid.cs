using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckersPawnValid : CheckersPiece
{
    public override List<Vector3Int> GetAvailableMoves(ref CheckersPiece[,] board, int tileCountX, int tileCountY)
    {
        List<Vector3Int> r = new List<Vector3Int>();

        int direction = (team == 0) ? 1 : -1;

        //One in diagonal
        if (currentX != 0)
            if (board[currentX - 1 , currentY + direction] == null)
                r.Add(new Vector3Int(currentX - 1, currentY + direction, 0));
        
        if (currentX != tileCountX - 1)
            if (board[currentX + 1, currentY + direction] == null)
                r.Add(new Vector3Int(currentX + 1, currentY + direction, 0));

        //Kill move
        if (currentX > 1)
            if (board[currentX - 1, currentY + direction] != null && board[currentX - 1, currentY + direction].team != this.team && board[currentX - 2, currentY + 2 * direction] == null)
            {
                r.Add(new Vector3Int(currentX - 2, currentY + 2 * direction, 1));
            }
            
        if (currentX < tileCountX - 2)
            if (board[currentX + 1, currentY + direction] != null && board[currentX + 1, currentY + direction].team != this.team && board[currentX + 2, currentY + 2 * direction] == null)
                r.Add(new Vector3Int(currentX + 2, currentY + 2 * direction, 1));

        return r;
    }

    public override CheckersSpecialMove GetSpecialMoves(ref CheckersPiece[,] board, ref List<Vector2Int[]> moveList, ref List<Vector3Int> availableMoves)
    {
        int direction = (team == 0) ? 1 : -1;

        //Promotion
        if ((team == 0 && currentY == 8) || (team == 1 && currentY == 1))
        {
            return CheckersSpecialMove.Promotion;
        }

        return CheckersSpecialMove.None;
    }
}
