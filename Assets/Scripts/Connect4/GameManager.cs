using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [SerializeField]
    private Tile tilePrefab;
    [SerializeField]
    private int columns, rows;
    [SerializeField]
    private int tilesToWin = 4;
    [SerializeField]
    private GameObject GameOverPanel;
    [SerializeField]
    private Text WinnerText;

    [Header("AI Settings"), SerializeField,
        Range(1, 8), Tooltip("Difficulty setting")]
    private int depth = 4;
    [SerializeField, Range(0.0f, 1.0f), Tooltip("The time it takes for the AI to place the tile after calculations")]
    private float tilePlacementDelay = 0.7f;
    [SerializeField]
    private bool aIGoesFirst;

    private int turn;

    private Tile[,] board;
    private List<Tile> tilePlaceOrder;

    [Header("Debugging")]
    [SerializeField]
    private DebugData debugData;
    private int iterations;
    private float time;
    private float calculationTime;

    void Awake()
    {
        tilePlaceOrder = new List<Tile>();
        GameOverPanel.SetActive(false);
        GenerateBoard();
        if (debugData == null)
            debugData = GetComponent<DebugData>();
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

    public void PlaceTileInColumn(int col, TileType type)
    {
        if (board[col, rows - 1].data.type != TileType.empty)
        {
            Debug.Log("Pressed on a full row, turn still yours");
            return;
        }

        if (type == TileType.player)
            Sound.Instance.AudioClips.PlayPlayerSound();
        else if (type == TileType.ai)
            Sound.Instance.AudioClips.PlayAISound();

        for (int y = 0; y < rows; y++)
        { //turns the first available tile in the given column to the type given
            if (board[col, y].data.type != TileType.empty)
                continue;
            board[col, y].TransformTile(type);
            tilePlaceOrder.Add(board[col, y]);
            break;
        }
        //PrintBoard(board);
        if (CheckWin(GetBoardData(), tilesToWin))
        {
            GameOver();
            return;
        }
        turn++;
        if (!IsPlayerTurn())
            StartCoroutine(AITurnDelay());
    }

    IEnumerator AITurnDelay()
    {
        yield return new WaitForSeconds(0.01f);
        AITurn();
    }

    private void AITurn()
    {
        iterations = 0;

        ClearDebugValues();

        time = Time.realtimeSinceStartup;
        Dictionary<TileData, int> map = MiniMaxMap();
        calculationTime = Time.realtimeSinceStartup - time;

        UpdateDebugData(calculationTime, iterations);

        DebugTileValues(map);

        TileData data = new TileData();
        int colValue = int.MinValue;

        //om flera v�rden �r samma s� l�gg centralt

        int col = DealWithEqualValueCase(map);
        if (col != -1)
        {
            StartCoroutine(AITilePlaceDelay(col, tilePlacementDelay));
            return;
        }

        foreach (TileData d in map.Keys)
        {
            //Debug.Log(d.x + ", " + d.y + " value: " + map[d]);
            if (map[d] > colValue)
            {
                colValue = map[d];
                data = d;
            }
        }
        data = PrioritizeCentralPosition(map);

        StartCoroutine(AITilePlaceDelay(data.x, tilePlacementDelay));
    }

    IEnumerator AITilePlaceDelay(int col, float time)
    {
        yield return new WaitForSeconds(time);
        PlaceTileInColumn(col, TileType.ai);
    }

    private void ClearDebugValues()
    {
        foreach (Tile tile in board)
        {
            tile.ClearDebugText();
        }
    }

    private void DebugTileValues(Dictionary<TileData, int> map)
    {
        List<TileData> tileData = new List<TileData>(map.Keys);
        foreach (TileData td in tileData)
        {
            board[td.x, td.y].SetDebugText(map[td]);
        }

    }

    private int MiniMax(TileData[,] boardParent, int depth, int alpha, int beta, bool maximizingPlayer)
    {
        iterations++;
        //Debug.Log("Ran minimax");
        if (depth == 0 || CheckWin(boardParent, tilesToWin))
        {
            return StaticEvaluationOfBoard(boardParent, maximizingPlayer);
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

    private int StaticEvaluationOfBoard(TileData[,] b, bool maximizingPlayer)
    {
        int score = 0;
        List<TileData> possibleMoves = GetPossibleMovesInBoardState(b);
        foreach (TileData tile in possibleMoves)
        {
            if (!maximizingPlayer)
            {
                if (CheckWin(GetPotentialBoard(b, tile, TileType.player), tilesToWin))
                    score += 1000;
                if (CheckWin(GetPotentialBoard(b, tile, TileType.player), tilesToWin - 1)) //3 i rad �r inte lika v�rdefullt om det inte finns en tillg�nglig fj�rde plats
                    score += 100;
            }
            else
            {
                if (CheckWin(GetPotentialBoard(b, tile, TileType.ai), tilesToWin))
                    score -= 1000;
                if (CheckWin(GetPotentialBoard(b, tile, TileType.player), tilesToWin - 1))
                    score -= 100;
            }
        }
        return score;
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

    private TileData[,] GetPotentialBoard(TileData[,] boardParent, TileData tile, TileType tileType)
    {
        TileData[,] clone = GetDeepCopyOfBoard(boardParent);
        clone[tile.x, tile.y] = tile;
        clone[tile.x, tile.y].type = tileType;
        return clone;
    }

    private TileData[,] GetDeepCopyOfBoard(TileData[,] b) //return a deep copy of the board given https://docs.microsoft.com/en-us/dotnet/api/system.array.clone?view=net-5.0
    {
        return b.Clone() as TileData[,];
    }

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

    private bool CheckWin(TileData[,] b, int inARow)
    {
        foreach (TileData tile in b)
        {
            if (tile.type != TileType.empty)
            {
                if (CheckWin(b, tile, inARow))
                    return true;
            }
        }
        return false;
    }

    private bool CheckWin(TileData[,] b, TileData tile, int inARow)
    {
        int tilesInARow = 0;
        for (int x = -(inARow - 1); x < (inARow - 1); x++)
        {
            if (CheckIfOutOfBounds(tile.x + x, tile.y))
            {
                if (b[tile.x + x, tile.y].type == tile.type)
                    tilesInARow++;
                else
                    tilesInARow = 0;
            }
            if (tilesInARow == inARow)
                return true;
        }
        tilesInARow = 0;
        for (int y = -(inARow - 1); y < (inARow - 1); y++)
        {
            if (CheckIfOutOfBounds(tile.x, tile.y + y))
            {
                if (b[tile.x, tile.y + y].type == tile.type)
                    tilesInARow++;
                else
                    tilesInARow = 0;
            }
            if (tilesInARow == inARow)
                return true;
        }
        tilesInARow = 0;
        for (int yx = -(inARow - 1); yx < (inARow - 1); yx++)
        {
            if (CheckIfOutOfBounds(tile.x + yx, tile.y + yx))
            {
                if (b[tile.x + yx, tile.y + yx].type == tile.type)
                    tilesInARow++;
                else
                    tilesInARow = 0;
            }
            if (tilesInARow == inARow)
                return true;
        }
        tilesInARow = 0;
        for (int xy = -(inARow - 1); xy < (inARow - 1); xy++)
        {
            if (CheckIfOutOfBounds(tile.x - xy, tile.y + xy))
            {
                if (b[tile.x - xy, tile.y + xy].type == tile.type)
                    tilesInARow++;
                else
                    tilesInARow = 0;
            }
            if (tilesInARow == inARow)
                return true;
        }
        return false;
    }

    private int DealWithEqualValueCase(Dictionary<TileData, int> map)
    {
        int value = 0;
        bool sameValue = true;
        bool firstValue = true;
        foreach (TileData d in map.Keys)
        {
            if (firstValue)
            {
                value = map[d];
                firstValue = false;
                continue;
            }
            else
            {
                if (value == map[d])
                    continue;
            }
            sameValue = false;
        }
        if (sameValue)
        {
            Debug.Log("All Values are the same");
            if (EarlyGame())
                return 3;
            //else
                //return Random.Range(0, map.Count);
        }
        return -1;
    }

    private TileData PrioritizeCentralPosition(Dictionary<TileData, int> map)
    {
        TileData data = new TileData();
        List<TileData> dataList = new List<TileData>();
        int largestNum = int.MinValue;
        foreach (TileData d in map.Keys) //hitta st�rst
        {
            if (map[d] > largestNum)
            {
                largestNum = map[d];
                data = d;
            }
        }
        foreach (TileData d in map.Keys)//add all equal values
        {
            if (map[data] == map[d])
                dataList.Add(d);
        }
        //Debug.Log(dataList.Count + " positions have the same value");
        foreach (TileData d in dataList)//pick the most central one
        {
            if (d.x == 3)
            {
                data = d;
                break;
            }
            else if (d.x == 2 || d.x == 4)
            {
                data = d;
                continue;
            }
            else if (d.x == 1 || d.x == 5)
            {
                data = d;
                continue;
            }
            else
            {
                data = d;
                continue;
            }
        }

        return data;
    }

    private void GameOver()
    {
        foreach (Tile tile in board)
            tile.IsUseable = false;

        HighlightLastPlacedTile(IsPlayerTurn());

        //Debug.Log(StaticEvaluationOfBoard(board));
        if (IsPlayerTurn())
        {
            Debug.Log("Player Won!");
            Sound.Instance.AudioClips.PlayPlayerWinSound();
        }
        else
        {
            Debug.Log("AI Revolution is starting");
            Sound.Instance.AudioClips.PlayAIWinSound();
        }

        OpenGameOverPanel(IsPlayerTurn());
    }

    private void HighlightLastPlacedTile(bool isPlayer)
    {
        if (isPlayer)
            tilePlaceOrder.Last().PlayerGlow();
        else
            tilePlaceOrder.Last().AIGlow();
    }

    private void OpenGameOverPanel(bool playerWon)
    {
        if (GameOverPanel != null)
        {
            GameOverPanel.SetActive(true);

            if(playerWon == true)
            {
                WinnerText.text = "You Won!";
            }
            else
            {
                WinnerText.text = "AI Won!";
            }

        }
    }


    public void Restart()
    {
        Sound.Instance.AudioClips.PlayRestartSound();
        tilePlaceOrder.Clear();
        ClearDebugValues();
        UpdateDebugData(0.0f, 0);
        foreach (Tile tile in board)
        {
            turn = 0;
            tile.ResetTile();
        }
    }

    private bool CheckIfOutOfBounds(int x, int y)
    {
        return !(x < 0 || x >= columns || y < 0 || y >= rows);
    }

    private void PrintBoard(Tile[,] b)
    {
        Debug.Log("------Board------");
        for (int x = rows - 1; x > -1; x--)
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

    private void SetTileTextActiveTo(bool b)
    {
        foreach (Tile tile in board)
        {
            tile.SetTextActiveTo(b);
        }
    }

    public void ActivateDebugMode(bool b)
    {
        //Debug.Log("Debug = " + b);
        SetTileTextActiveTo(b);
        debugData.SetActiveTo(b);
    }

    private void UpdateDebugData(float calculationTime, int iterations)
    {
        debugData.UpdateData(calculationTime, iterations);
    }

    public void UpdateDifficulty(float f)
    {
        //Debug.Log("Difficulty = " + f);
        switch (f)
        {
            case 1:
                depth = 4;
                break;
            case 2:
                depth = 6;
                break;
            case 3:
                depth = 8;
                break;
        }
    }

    private bool EarlyGame()
    {
        return turn < 6;
    }

    public bool IsPlayerTurn()
    {
        return turn % 2 == 0;
    }
}