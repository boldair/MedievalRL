using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Node
{
    public Vector3Int Position;
    public bool Walkable;
    public int GCost;
    public int HCost;
    public Node Parent;
    private Color originalColor;
    private Tilemap tilemap;
    public int FCost => GCost + HCost;
    
    public Node(Vector3Int position, bool walkable)
    {
        this.Position = position;
        this.Walkable = walkable;
        this.GCost = 0;
        this.HCost = 0;
        this.Parent = null;
        this.tilemap = GameObject.FindObjectOfType<Tilemap>();
        this.originalColor = tilemap.GetColor(position);
    }
    public void SetColor(Color color)
    {
        tilemap.SetTileFlags(Position, TileFlags.None);
        tilemap.SetColor(Position, color);
    }

    public void ResetColor()
    {
        tilemap.SetColor(Position, originalColor);
    }
    public Vector2 GetCenter()
    {
        Vector2 worldPosition = tilemap.CellToWorld(Position);
        Vector2 tileSize = tilemap.cellSize;
        Vector2 offset = new Vector2(tileSize.x / 2, tileSize.y / 2);
        return worldPosition + offset;
    }
}
