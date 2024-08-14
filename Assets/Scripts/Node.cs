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
    public float Debug;
    private Color originalColor;
    private Tilemap tilemap;
    public int FCost => GCost + HCost;
    
    public Node(Vector3Int position)
    {
        this.Position = position;
        this.GCost = 0;
        this.HCost = 0;
        this.Parent = null;
        this.tilemap = GameObject.FindObjectOfType<Tilemap>();
        this.originalColor = tilemap.GetColor(position);
    }
    public void SetColor(Color color)
    {
        tilemap.SetColor(Position, color);
    }

    public void ResetColor()
    {
        tilemap.SetColor(Position, originalColor);
    }
}
