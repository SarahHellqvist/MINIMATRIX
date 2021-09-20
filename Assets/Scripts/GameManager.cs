using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField]
    private Tile tilePrefab;
    [SerializeField]
    private int columns, rows;
    [SerializeField]
    private int tilesToWin = 4;

    [Header("AI Settings"), SerializeField,
        Range(1, 10), Tooltip("Difficulty setting")]
    private int depth = 3;
    [SerializeField]
    private bool aIGoesFirst;

    private int turn;

    private Tile[,] board;

    void Awake()
    { 
        GenerateBoard();
    }

    private void GenerateBoard()
    {
        board = new Tile[columns, rows];
        Vector3 pos;
        Tile tile;
        for (int x = 0; x < columns; x++)
        {
            for (int y = 0; y < rows; y++)
            {
                pos = new Vector3((float)((x - columns / 2) * 1.2), (float)((y - rows / 2) * 1.2), 0);
                tile = Instantiate(tilePrefab);
                tile.Setup(this, TileType.empty, x, y);
                tile.transform.position = pos;
                board[x, y] = tile;
            }
        }
        StartGame();
    }

    private void StartGame()
    {
        if (aIGoesFirst)
        {
            turn++;
            AITurn();
        }
    }
    
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
            GetBoardWithAFewChanges();
        if (Input.GetKeyDown(KeyCode.F))
            GetBoardDataWithAFewChanges();
        if (Input.GetKeyDown(KeyCode.R))
            DoQuickWinCheck();
    }

    void DoQuickWinCheck() 
    {
        TileData data = new TileData();
        data.x = 5;
        data.y = 0;
        PrintBoard(board);
        PrintBoard(GetPotentialBoard(GetBoardData(), data, TileType.player));
        if (CheckWin(GetPotentialBoard(GetBoardData(), data, TileType.player)))
            Debug.Log("Player Would Win");
        else
            Debug.Log("not win con");
    }

    public void PlaceTileInColumn(int col, TileType type)
    {
        if (board[col, rows - 1].data.type != TileType.empty)
        {
            Debug.Log("Pressed on a full row, turn still yours");
            return;
        }
        for (int y = 0; y < rows; y++)
        { //turns the first available tile in the given column to the type given
            if (board[col, y].data.type != TileType.empty)
                continue;
            board[col, y].TransformTile(type);
            break;
        }
        //PrintBoard(board);
        if (CheckWin(GetBoardData()))
        {
            GameOver();
            return;
        }
        turn++;
        if (!IsPlayerTurn())
            AITurn();
    }

    private void AITurn()
    {
        //int score = MiniMax(GetBoardData(), depth, int.MinValue, int.MaxValue, true);

        Dictionary<TileData, int> map = MiniMaxMap();
        TileData data = new TileData();
        int colValue = int.MinValue;
        foreach (TileData d in map.Keys)
        {
            Debug.Log(d.x + ", " + d.y + " value: " + map[d]);
            if (map[d] > colValue)
            {
                colValue = map[d];
                data = d;
            }
        }
        PlaceTileInColumn(data.x, TileType.ai);

        //Debug.Log(score);
        //int col = ChoseColumnBasedOnScore(score);
        //PlaceTileInColumn(col, TileType.ai);
        //PlaceTileInColumn(UnityEngine.Random.Range(0, possibleMoves.Count), TileType.ai);
        //Debug.Log(MiniMax(GetBoardData(), depth, int.MinValue, int.MaxValue, true));
    }

    private int MiniMax(TileData[,] boardParent, int depth, int alpha, int beta, bool maximizingPlayer)
    {
        //Debug.Log("Ran minimax");
        if (depth == 0 || CheckWin(boardParent))
        {
            return StaticEvaluationOfBoard(boardParent); //ska ge ett värde för board state, inte tile placement
        }

        List<TileData> possibleMoves = GetPossibleMovesInBoardState(boardParent);
        if (maximizingPlayer)
        {
            int maxEval = int.MinValue; //(int)-Mathf.Infinity
            foreach (TileData tile in possibleMoves)
            {
                TileData[,] boardChild = GetPotentialBoard(boardParent, tile, TileType.ai);
                int eval = MiniMax(boardChild, depth - 1, alpha, beta, false);
                maxEval = Mathf.Max(maxEval, eval);
                alpha = Mathf.Max(alpha, eval);
                if (beta <= alpha)
                    break;
            }
            return maxEval;
        }
        else //!maximizingPlayer
        {
            int minEval = int.MaxValue; //(int)Mathf.Infinity
            foreach (TileData tile in possibleMoves)
            {
                TileData[,] boardChild = GetPotentialBoard(boardParent, tile, TileType.player);
                int eval = MiniMax(boardChild, depth - 1, alpha, beta, true);
                minEval = Mathf.Min(minEval, eval);
                beta = Mathf.Min(beta, eval);
                if (beta <= alpha)
                    break;
            }
            return minEval;
        }
    }
    
    private Dictionary<TileData, int> MiniMaxMap()
    { 
        Dictionary<TileData, int> map = new Dictionary<TileData, int>();
        
        List<TileData> possibleMoves = GetPossibleMovesInBoardState(GetBoardData());
        foreach (TileData tile in possibleMoves)
        {
            TileData[,] boardChild = GetPotentialBoard(GetBoardData(), tile, TileType.ai);
            int eval = MiniMax(boardChild, depth - 1, int.MinValue, int.MaxValue, false);
            map.Add(tile, eval);
        }
        return map;
    }

    private int StaticEvaluationOfBoard(TileData[,] board)
    {
        int score = 0;
        List<TileData> possibleMoves = GetPossibleMovesInBoardState(board);
        foreach (TileData tile in possibleMoves)
        {
            if (IsPlayerTurn())
            {
                if (CheckWin(GetPotentialBoard(board, tile, TileType.player)))
                    score += 1000;
            }
            else
            {
                if (CheckWin(GetPotentialBoard(board, tile, TileType.ai)))
                    score -= 1000;
            }

            //else
            //{
            //    if (depth % 2 != 0)
            //        score += 1;
            //    else
            //        score -= 1;
            //}
        }
        //if (CheckWin(board) && IsPlayerTurn())
        //    score += 1000;
        //else if (CheckWin(board) && !IsPlayerTurn())
        //    score -= 1000;

        //mer value som mitten column osv

        return score;
    }
    
    private int StaticMapEvaluationOfBoard(TileData[,] board)
    {
        int score = 0;
        List<TileData> possibleMoves = GetPossibleMovesInBoardState(board);
        foreach (TileData tile in possibleMoves)
        {
            if (IsPlayerTurn())
            {
                if (CheckWin(GetPotentialBoard(board, tile, TileType.player)))
                    score += 1000;
                else
                    score += 1;
            }
            else
            {
                if (CheckWin(GetPotentialBoard(board, tile, TileType.ai)))
                    score -= 1000;
                else score -= 1;
            }
        }
        return score;
    }

    private int ChoseColumnBasedOnScore(int score)
    {
        int col = 3;

        return col;
    }

    private List<TileData> GetPossibleMovesInBoardState(TileData[,] board)
    {
        List<TileData> possibleMoves = new List<TileData>();
        for (int x = 0; x < columns; x++)
        {
            for (int y = 0; y < rows; y++)
            {
                if (board[x, y].type == TileType.empty) 
                {
                    possibleMoves.Add(board[x, y]);
                    break;
                }
            }
        }
        return possibleMoves;
    }

    /*private List<Tile[,]> SimulatePossibleBoardStates(Tile[,] boardParent, TileType tileType) //list of possible board states based on possible moves
    {
        List<Tile[,]> b = new List<Tile[,]>();
        List<Tile> possibleMoves = GetPossibleMovesInBoardState(boardParent);
        foreach (Tile tile in possibleMoves)
        {
            b.Add(GetPotentialBoard(boardParent, tile, tileType));
        }
        return b;
    }*/

    private TileData[,] GetPotentialBoard(TileData[,] boardParent, TileData tile, TileType tileType)
    {
        TileData[,] clone = GetDeepCopyOfBoard(boardParent);
        clone[tile.x, tile.y] = tile;
        clone[tile.x, tile.y].type = tileType;
        return clone;
    }

    /*private TileData[,] GetCopyOfBoard(TileData[,] b)
    {
        TileData[,] copy = new TileData[columns, rows];
        TileData tileData;
        for (int x = 0; x < columns; x++)
        {
            for (int y = 0; y < rows; y++)
            {
                tileData = new TileData();
                tileData.x = b[x, y].x;
                tileData.y = b[x, y].y;
                tileData.type = b[x, y].type;
            }
        }
        return copy;
    }*/

    private TileData[,] GetDeepCopyOfBoard(TileData[,] b) //return a deep copy of the board given
    {
        return b.Clone() as TileData[,];
    }

    /*private int[,] GetIntCopyOfBoard(int[,] b)
    {
        int[,] copy = b.Clone() as int[,];
        //int[,] copy = (int[,])b.Clone();

        return copy;
    }*/

    private TileData[,] GetBoardData() //return the actual board state in TileData[,] form
    {
        TileData[,] dataBoard = new TileData[columns, rows];
        TileData data;
        for (int x = 0; x < columns; x++)
        {
            for (int y = 0; y < rows; y++)
            {
                data = new TileData();
                data.x = board[x, y].data.x;
                data.y = board[x, y].data.y;
                data.type = board[x, y].data.type;
                dataBoard[x, y] = data;
            }
        }
        return dataBoard;
    }

    /*private int[,] GetBoardAsInt(Tile[,] board) //returns int version of the board https://docs.microsoft.com/en-us/dotnet/api/system.array.clone?view=net-5.0
    {
        int[,] newBoard = new int[columns, rows];
        for (int x = 0; x < columns; x++)
        {
            for (int y = 0; y < rows; y++)
            {
                newBoard[x, y] = board[x, y].GetTypeAsInt();
            }
        }
        return newBoard;
    }*/

    private bool CheckWin(TileData[,] b)
    {
        foreach (TileData tile in b)
        {
            if (tile.type != TileType.empty)
            {
                if (CheckWin(b, tile))
                    return true;
            }
        }
        return false;
    }

    private bool CheckWin(TileData[,] b, TileData tile)
    {
        int tilesInARow = 0;
        for (int x = -(tilesToWin - 1); x < (tilesToWin - 1); x++)
        {
            if (CheckIfOutOfBounds(tile.x + x, tile.y))
            {
                if (b[tile.x + x, tile.y].type == tile.type)
                    tilesInARow++;
                else
                    tilesInARow = 0;
            }
            if (tilesInARow == tilesToWin)
                return true;
        }
        tilesInARow = 0;
        for (int y = -(tilesToWin - 1); y < (tilesToWin - 1); y++)
        {
            if (CheckIfOutOfBounds(tile.x, tile.y + y))
            {
                if (b[tile.x, tile.y + y].type == tile.type)
                    tilesInARow++;
                else
                    tilesInARow = 0;
            }
            if (tilesInARow == tilesToWin)
                return true;
        }
        tilesInARow = 0; 
        for (int yx = -(tilesToWin - 1); yx < (tilesToWin - 1); yx++)
        {
            if (CheckIfOutOfBounds(tile.x + yx, tile.y + yx))
            {
                if (b[tile.x + yx, tile.y + yx].type == tile.type)
                    tilesInARow++;
                else
                    tilesInARow = 0;
            }
            if (tilesInARow == tilesToWin)
                return true;
        }
        tilesInARow = 0;
        for (int xy = -(tilesToWin-1); xy < (tilesToWin - 1); xy++)
        {
            if (CheckIfOutOfBounds(tile.x - xy, tile.y + xy))
            {
                if (b[tile.x - xy, tile.y + xy].type == tile.type)
                    tilesInARow++;
                else
                    tilesInARow = 0;
            }
            if (tilesInARow == tilesToWin)
                return true;
        }
        return false;
    }

    private void GameOver()
    {
        foreach (Tile tile in board)
            tile.IsUseable = false;
        //Debug.Log(StaticEvaluationOfBoard(board));
        if (IsPlayerTurn())
            Debug.Log("Player Won!");
        else
            Debug.Log("AI Revolution is starting");
    }

    private bool CheckIfOutOfBounds(int x, int y) 
    {
        return !(x < 0 || x >= columns || y < 0 || y >= rows);
    }

    private void GetBoardWithAFewChanges()
    {
        Tile[,] clone = new Tile[columns, rows];
        Tile tile;
        //Tile[,] clone = board.Clone() as Tile[,];

        for (int x = 0; x < columns; x++)
        {
            for (int y = 0; y < rows; y++)
            {
                tile = new Tile();
                tile.CopyValuesFrom(board[x, y]);
                clone[x, y] = tile;
            }
        }

        clone[3, 3].data.type = TileType.player;

        PrintBoard(clone);
        PrintBoard(board);
    }

    private void GetBoardDataWithAFewChanges()
    {
        TileData[,] copy = GetDeepCopyOfBoard(GetBoardData());
        copy[3, 3].type = TileType.player;

        PrintBoard(copy);//This is done to see whether the copy changes the original
        PrintBoard(board);
    }

    private void PrintBoard(Tile[,] b)
    {
        Debug.Log("------Board------");
        for (int x = rows - 1; x > - 1; x--)
        {
            string wholeRow = "";
            for (int y = 0; y < columns; y++)
            {
                wholeRow += b[y, x].ToStringNumFormat() + ", ";
            }
            Debug.Log(wholeRow);
        }
    }

    private void PrintBoard(TileData[,] b)
    {
        Debug.Log("----BoardData----");
        for (int x = rows - 1; x > -1; x--)
        {
            string wholeRow = "";
            for (int y = 0; y < columns; y++)
            {
                wholeRow += b[y, x].type + ", ";
            }
            Debug.Log(wholeRow);
        }
    }

    public bool IsPlayerTurn()
    {
        return turn % 2 == 0;
    }
}