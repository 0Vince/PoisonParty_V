using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public enum CheckersSpecialMove
{
    None = 0,
    Promotion,
    BigEat
}
public class CheckersBoardValid : MonoBehaviour
{
    [Header("Art Stuff")]
    [SerializeField] private Material tileMaterialWhite;
    [SerializeField] private Material tileMaterialBlack;
    [SerializeField] private float yOffset = -0.1f;
    [SerializeField] private float tileSize = 6.0f;
    [SerializeField] private Vector3 boardCenter = Vector3.zero;
    [SerializeField] private float deathSize = 1.0f;
    [SerializeField] private float deathSpacing = 3.2f;
    [SerializeField] private float deathHeight = -3.0f;
    [SerializeField] private float dragOffset = 0.5f;
    [SerializeField] private GameObject victoryScreen;
    
    
    [Header("Prefabs & Materials")]
    [SerializeField] private GameObject[] prefabs;
    [SerializeField] private Material[] teamMaterials;


    private CheckersPiece[,] checkersPieces;
    private CheckersPiece currentlyDragging;
    private List<CheckersPiece> deadWhites = new List<CheckersPiece>();
    private List<CheckersPiece> deadBlacks = new List<CheckersPiece>();
    private List<Vector3Int> availableMoves = new List<Vector3Int>();
    private List<Vector2Int[]> moveList = new List<Vector2Int[]>(); 
    private CheckersSpecialMove specialMove;
    private const int TileCountX = 10;
    private const int TileCountY = 10;
    private GameObject[,] tiles;
    private Camera currentCamera;
    private Vector2Int currentHover;
    private Vector3 bounds;
    private bool isWhiteTurn;
    
    private void Awake()
    {
        isWhiteTurn = true;

        GenerateAllTiles(tileSize, TileCountX, TileCountY);

        SpawnAllPieces();
        PositionAllPieces();
    }
    
    private void Update()
   {
        if (!currentCamera)
        {
            currentCamera = Camera.main;
            return;
        }
        RaycastHit info;
        Ray ray = currentCamera.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out info, 300, LayerMask.GetMask("Tile", "Hover", "Highlight")))
        {
            //Get the indexes of tile we hit
            Vector2Int hitPosition = LookupTileIndex(info.transform.gameObject);

            //If we are hovering any tile after not hovering any tile
            if (currentHover == -Vector2Int.one)
            {
                currentHover = hitPosition;
                tiles[hitPosition.x, hitPosition.y].layer = LayerMask.NameToLayer("Hover");
            }
            //if we were already hovering a tile, change previous
            if (currentHover != hitPosition)
            {
                tiles[currentHover.x, currentHover.y].layer = (ContainsValidMoves(ref availableMoves, currentHover).Item1) ? (LayerMask.NameToLayer("Highlight")) : LayerMask.NameToLayer("Tile");
                currentHover = hitPosition;
                tiles[currentHover.x, hitPosition.y].layer = LayerMask.NameToLayer("Hover");
            }

            //If we press down on the mouse
            if (Input.GetMouseButtonDown(0))
            {
                if (checkersPieces[hitPosition.x, hitPosition.y] != null)
                {
                    //Is it our turn?
                    if ((checkersPieces[hitPosition.x, hitPosition.y].team == 0 && isWhiteTurn) || (checkersPieces[hitPosition.x, hitPosition.y].team ==1 && !isWhiteTurn))
                    {
                        currentlyDragging = checkersPieces[hitPosition.x, hitPosition.y];

                        //Get a list of where i can go, highlight the tiles as well
                        availableMoves = currentlyDragging.GetAvailableMoves(ref checkersPieces, TileCountX, TileCountY);
                        //Get a lit of special moves
                        specialMove = currentlyDragging.GetSpecialMoves(ref checkersPieces, ref moveList, ref availableMoves);
                        
                    }
                }
            }

            //If we are releasing the mouse button
            if (currentlyDragging != null && Input.GetMouseButtonUp(0))
            {
                Vector2Int previousPosition = new Vector2Int(currentlyDragging.currentX, currentlyDragging.currentY);

                bool validMove = MoveTo(currentlyDragging, hitPosition.x, hitPosition.y);
                if (!validMove)
                    currentlyDragging.SetPosition(GetTileCenter(previousPosition.x, previousPosition.y));

                currentlyDragging = null;
            }
        }
        else
        {
            if(currentHover != -Vector2Int.one)
            { 
                currentHover = -Vector2Int.one;
            }

            if (currentlyDragging && Input.GetMouseButtonUp(0))
            {
                currentlyDragging.SetPosition(GetTileCenter(currentlyDragging.currentX, currentlyDragging.currentY));
                currentlyDragging = null;
            }
        }

        if (currentlyDragging)
        {
            Plane horizontalPlane = new Plane(Vector3.up, Vector3.up * yOffset);
            float distance = 0.0f;
            if (horizontalPlane.Raycast(ray, out distance))
                currentlyDragging.SetPosition(ray.GetPoint(distance) + Vector3.up * dragOffset);
        }
   }
    
    private void GenerateAllTiles(float tileSize, int tileCountX, int tileCountY)
    {

        yOffset += transform.position.y;
        bounds = new Vector3((tileCountX / 2) * tileSize, 0, (tileCountY / 2) * tileSize) + boardCenter;

        tiles = new GameObject[tileCountX, tileCountY];

        for (int x = 0; x < tileCountX; x++)
        for (int y = 0; y < tileCountY; y++)
            tiles[x, y] = GenerateSingleTile(tileSize, x, y);
    }

    
    private GameObject GenerateSingleTile(float tileSize, int x, int y)
    {
        GameObject tileObject = new GameObject(string.Format("X:{0}, Y:{1}", x, y));
        tileObject.transform.parent = transform;

        Mesh mesh = new Mesh();
        tileObject.AddComponent<MeshFilter>().mesh = mesh;
        if ((x % 2 == 0 && y % 2 == 0) || (x % 2 == 1 && y % 2 == 1))
            tileObject.AddComponent<MeshRenderer>().material = tileMaterialBlack;
        if ((x % 2 == 1 && y % 2 == 0) || (x % 2 == 0 && y % 2 == 1))
            tileObject.AddComponent<MeshRenderer>().material = tileMaterialWhite;

        Vector3[] vertices = new Vector3[4];
        vertices[0] = new Vector3(x * tileSize, yOffset, y * tileSize) - bounds;
        vertices[1] = new Vector3(x * tileSize, yOffset, (y+1) * tileSize) - bounds;
        vertices[2] = new Vector3((x+1) * tileSize, yOffset, y * tileSize) - bounds;
        vertices[3] = new Vector3((x+1) * tileSize, yOffset, (y+1) * tileSize) - bounds;
        int[] tris = new int[] { 0, 1, 2, 1, 3, 2};

        mesh.vertices = vertices;
        mesh.triangles = tris;
        mesh.RecalculateNormals();

        tileObject.layer = LayerMask.NameToLayer("Tile");
        tileObject.AddComponent<BoxCollider>();

        return tileObject;
    }
    
    //Spawning of the pieces
    private void SpawnAllPieces()
    {
        checkersPieces = new CheckersPiece[TileCountX, TileCountY];

        int whiteTeam = 0;
        int blackTeam = 1;

        //White team
        for (int i = 0; i < TileCountX; i+=2)
            checkersPieces[i, 0] = SpawnSinglePiece(CheckersPieceType.Pawn, whiteTeam);
        for (int i = 0; i < TileCountX; i+=2)
            checkersPieces[i+1, 1] = SpawnSinglePiece(CheckersPieceType.Pawn, whiteTeam);
        for (int i = 0; i < TileCountX; i+=2)
            checkersPieces[i, 2] = SpawnSinglePiece(CheckersPieceType.Pawn, whiteTeam);
        for (int i = 0; i < TileCountX; i+=2)
            checkersPieces[i+1, 3] = SpawnSinglePiece(CheckersPieceType.Pawn, whiteTeam);

        //Black team
        /*for (int i = 0; i < TileCountX; i+=2)
            checkersPieces[i+1, 9] = SpawnSinglePiece(CheckersPieceType.Pawn, blackTeam);
        for (int i = 0; i < TileCountX; i+=2)
            checkersPieces[i, 8] = SpawnSinglePiece(CheckersPieceType.Pawn, blackTeam);
        for (int i = 0; i < TileCountX; i+=2)
            checkersPieces[i+1, 7] = SpawnSinglePiece(CheckersPieceType.Pawn, blackTeam);*/
        for (int i = 0; i < TileCountX; i+=2)
            checkersPieces[i, 6] = SpawnSinglePiece(CheckersPieceType.Pawn, blackTeam);
    }

    private CheckersPiece SpawnSinglePiece(CheckersPieceType type, int team)
    {
        CheckersPiece dp = Instantiate(prefabs[(int)type - 1], transform).GetComponent<CheckersPiece>();

        dp.Type = type;
        dp.team = team;
        dp.GetComponent<MeshRenderer>().material = teamMaterials[team];

        return dp;
    }

    //Positioning
    private void PositionAllPieces()
    {
            for (int x = 0; x < TileCountX; x++)
                for (int y = 0; y < TileCountY; y++)
                    if (checkersPieces[x, y] != null)
                        PositionSinglePiece(x, y, true);
    }

    private void PositionSinglePiece(int x, int y, bool force = false)
    {
        checkersPieces[x, y].currentX = x;
        checkersPieces[x, y].currentY = y;
        checkersPieces[x, y].SetPosition(GetTileCenter(x, y), force);
        checkersPieces[x, y].SetScale(new Vector3(0.1f, 0.1f, 0.1f), true);
    }
    
    private Vector3 GetTileCenter(int x, int y)
    {
        return new Vector3(x * tileSize, yOffset, y * tileSize) - bounds + new Vector3(tileSize / 2, 0, tileSize / 2);
    }

    //Checkmate

    private void DisplayVictory(int winningTeam)
    {
        Debug.Log("WinningTeam win");
        //What to do if we win
    }

    //Special Moves
    private void ProcessSpecialMove()
    {
        if (specialMove == CheckersSpecialMove.Promotion)
        {
            Vector2Int[] lastMove = moveList[^1];
            CheckersPiece targetPawn = checkersPieces[lastMove[1].x, lastMove[1].y];

            //White team
            if (targetPawn.team == 0 && lastMove[1].y == 9)
            {
                //Create a new piece
                CheckersPiece newQueen = SpawnSinglePiece(CheckersPieceType.Queen, 0);
                newQueen.transform.position = checkersPieces[lastMove[1].x, lastMove[1].y].transform.position;
                Destroy(checkersPieces[lastMove[1].x, lastMove[1].y].gameObject);
                checkersPieces[lastMove[1].x, lastMove[1].y] = newQueen;
                PositionSinglePiece(lastMove[1].x, lastMove[1].y);
                
            } 

            //Black team
            if (targetPawn.team == 1 && lastMove[1].y == 0)
            {
                //Create a new piece
                CheckersPiece newQueen = SpawnSinglePiece(CheckersPieceType.Queen, 1);
                newQueen.transform.position = checkersPieces[lastMove[1].x, lastMove[1].y].transform.position;
                Destroy(checkersPieces[lastMove[1].x, lastMove[1].y].gameObject);
                checkersPieces[lastMove[1].x, lastMove[1].y] = newQueen;
                PositionSinglePiece(lastMove[1].x, lastMove[1].y);
            }
        }
    }
    //Operations
    private (bool, bool) ContainsValidMoves(ref List<Vector3Int> moves, Vector2Int pos){
        for (int i = 0; i < moves.Count; i++)
            if (moves[i].x == pos.x && moves[i].y == pos.y)
            {
                if (moves[i].z == 1)
                    return (true, true);
                return (true, false);
            }
        
        return (false, false);
    }

    private Vector2Int LookupTileIndex(GameObject hitInfo){
        for (int x = 0; x < TileCountX; x++)
            for (int y = 0; y < TileCountY; y++)
                if (tiles[x,y] == hitInfo)
                    return new Vector2Int(x, y);

        return -Vector2Int.one; //Invalid -> -1, -1
    }

     private void KillPiece(int xToKill, int yToKill)
     {
         CheckersPiece ocp = checkersPieces[xToKill, yToKill];

         if (ocp.team == 0)
         {
             deadWhites.Add(ocp);
             ocp.SetScale(Vector3.one * deathSize);
             ocp.SetPosition(
                 new Vector3(11f * tileSize, deathHeight, -1 * tileSize)
                 - bounds
                 + new Vector3(tileSize / 2, 0, tileSize / 2)
                 + Vector3.forward * (deathSpacing * deadWhites.Count));

         }
         else
         {
             deadBlacks.Add(ocp);
             ocp.SetScale((Vector3.one) * deathSize);
             ocp.SetPosition(
                 new Vector3(-1.8f * tileSize, deathHeight, 10 * tileSize)
                 - bounds
                 + new Vector3(tileSize / 2, 0, tileSize / 2)
                 + Vector3.back * (deathSpacing * deadBlacks.Count));

         }

         checkersPieces[xToKill, yToKill] = null;
     }

    private bool MoveTo(CheckersPiece cp, int x, int y)
    {
        (bool, bool) moveToDo = ContainsValidMoves(ref availableMoves, new Vector2Int(x, y));
        
        if (!moveToDo.Item1)//If there is a invalid move
            return false;
        
        if (moveToDo.Item2 && cp.Type == CheckersPieceType.Pawn)//If it is a kill move and a pawn
            KillPiece((cp.currentX + x) / 2, (cp.currentY + y) / 2);

        if (moveToDo.Item2 && cp.Type == CheckersPieceType.Queen)//If it is a kill move and a queen
        {
            Vector2Int direction = new Vector2Int(x - cp.currentX, y - cp.currentY);

            switch (direction.x)
            {
                case < 0 : direction.x = 1; break;
                case > 0 : direction.x = -1; break;
                default: throw new InvalidDataException("MoveToIncorrect");
            }

            switch (direction.y)
            {
                case < 0 : direction.y = 1; break;
                case > 0 : direction.y = -1; break;
                default: throw new InvalidDataException("MotToincorrect");
            }
            
            KillPiece(x + direction.x, y + direction.y);
        }

        Vector2Int previousPosition = new Vector2Int(cp.currentX, cp.currentY);

        checkersPieces[x, y] = cp;
        checkersPieces[previousPosition.x, previousPosition.y] = null;

        PositionSinglePiece(x,y);
        
        isWhiteTurn = !isWhiteTurn;
        moveList.Add(new Vector2Int[] { previousPosition, new Vector2Int(x, y)});

        ProcessSpecialMove();

        if (CheckForLeftPawn())
            DisplayVictory(cp.team);



        return true;
    }

    private bool CheckForLeftPawn()
    {
        return deadWhites.Count == 20 || deadBlacks.Count == 20;
    }
}
