using UnityEngine;
using System.Collections.Generic;

public enum CheckersPieceType
{
    None = 0,
    Pawn = 1,
    Queen = 2
}
public class CheckersPiece : MonoBehaviour
{
    private CheckersPieceType type;
    public int team;
    public int currentX;
    public int currentY;
    private Vector3 desiredPosition;
    private Vector3 desiredScale = new Vector3((float)0.046, (float)0.006, (float)0.046);
    
    public CheckersPieceType Type { get; set; } 

    private void Update()
    {
        transform.position = Vector3.Lerp(transform.position, desiredPosition, Time.deltaTime * 10);
        transform.localScale = Vector3.Lerp(transform.localScale, desiredScale, Time.deltaTime * 10);
    }

    public virtual List<Vector3Int> GetAvailableMoves(ref CheckersPiece[,] board, int tileCountX, int tileCountY)
    {
        List<Vector3Int> r = new List<Vector3Int>();

        return r;
    }

    public virtual CheckersSpecialMove GetSpecialMoves(ref CheckersPiece[,] board, ref List<Vector2Int[]> moveList, ref List<Vector3Int> availableMoves)
    {
        return CheckersSpecialMove.None;
    }

    public  void SetPosition(Vector3 position, bool force = false)
    {
        if (this.Type == CheckersPieceType.Pawn)
            desiredPosition = position + new Vector3(0.0f, 0.5f, 0.0f);
        else 
            desiredPosition = position;

        if (force)
            transform.position = desiredPosition;
    }

    public virtual void SetScale(Vector3 scale, bool force = false)
    {
        if (force)
            transform.localScale = desiredScale;
    }

}
