using UnityEngine;
using UnityEngine.UI;

public struct TileData
{
    public int x, y;
    public TileType type;
}

public enum TileType
{
    empty,
    player,
    ai
}

public class Tile : MonoBehaviour
{
    private GameManager gm;
    private SpriteRenderer sr;
    [SerializeField]
    private Sprite emptySprite, playerSprite, aISprite, playerWinSprite, aiWinSprite;
    public TileData data;
    [SerializeField]
    private GameObject textGameObject;
    [SerializeField]
    private Text text;

    public bool IsUseable { get; set; }
    
    public void Setup(GameManager gm, TileType type, int x, int y)
    {
        this.gm = gm;
        data.type = type;
        data.x = x;
        data.y = y;
        IsUseable = true;
        sr = GetComponent<SpriteRenderer>();
    }

    private void OnMouseUp()
    {
        if (gm.IsPlayerTurn() && IsUseable)
        {
            gm.PlaceTileInColumn(data.x, TileType.player);
            //Debug.Log("Released in Column: " + (x+1) + ", x: " + x + "; y:" + y);
        }
    }

    public void PlayerGlow()
    {
        //Debug.Log(data.x + ", " + data.y + ", " + data.type);
        //textGameObject.SetActive(true);
        //text.text = "Winning tile";
        sr.sprite = playerWinSprite;
    }

    public void AIGlow()
    {
        //Debug.Log(data.x + ", " + data.y + ", " + data.type);
        //textGameObject.SetActive(true);
        //text.text = "AI Winning Tile";
        sr.sprite = aiWinSprite;
    }

    public void ResetTile()
    {
        data.type = TileType.empty;
        sr.sprite = emptySprite;
        IsUseable = true;
    }

    public void SetTextActiveTo(bool b)
    {
        textGameObject.SetActive(b);
    }

    public void TurnIntoPlayerTile()
    {
        data.type = TileType.player;
        sr.sprite = playerSprite;
    }

    public void TurnIntoAITile()
    {
        data.type = TileType.ai;
        sr.sprite = aISprite;
    }

    public void TransformTile(TileType type)
    {
        data.type = type;
        if (data.type == TileType.ai)
            sr.sprite = aISprite;
        else if (data.type == TileType.player)
            sr.sprite = playerSprite;
    }
    
    public int GetTypeAsInt()
    {
        int value = 0;
        switch (data.type)
        {
            case TileType.empty:
                value = 0;
                break;
            case TileType.player:
                value = 1;
                break;
            case TileType.ai:
                value = 2;
                break;
        }
        return value;
    }

    public void SetType(int value)
    {
        switch (value)
        {
            case 0:
                data.type = TileType.empty;
                break;
            case 1:
                data.type = TileType.player;
                break;
            case 2:
                data.type = TileType.ai;
                break;
        }
    }

    public void CopyValuesFrom(Tile tile)
    {
        gm = tile.gm;
        data.type = tile.data.type;
        data.x = tile.data.x;
        data.y = tile.data.y;
    }

    public void Introduce()
    {
        Debug.Log("Tile at cords: x=" + data.x + "; y=" + data.y + " is of type: " + ToString());
        //textMesh.text = x + "; " + y;
    }

    public void ClearDebugText()
    {
        text.text = "";
    }

    public void SetDebugText(int value)
    {
        text.text = value.ToString();
    }

    public new string ToString()
    {
        string name = "";
        switch (data.type)
        {
            case TileType.empty:
                name = "Empty";
                break;
            case TileType.player:
                name = "Player";
                break;
            case TileType.ai:
                name = "AI";
                break;
        }
        return name;
    }

    public string ToStringNumFormat()
    {
        string name = "";
        switch (data.type)
        {
            case TileType.empty:
                name = "0";
                break;
            case TileType.player:
                name = "1";
                break;
            case TileType.ai:
                name = "2";
                break;
        }
        return name;
    }
}
