using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Pathfinding : MonoBehaviour
{
    public Tilemap tilemap;
    public Transform character;
    public Color pathColor = Color.red;
    private Dictionary<Vector3Int, Node> _nodes = new Dictionary<Vector3Int, Node>();
    
    private Camera _camera;
    private void Start()
    {
        _camera = Camera.main;
        InitializeNodes();
    }
    private void Update()
    {
        Vector3 mouseWorldPos = _camera.ScreenToWorldPoint(Input.mousePosition);
        Vector3Int cellPos = tilemap.WorldToCell(mouseWorldPos);

        if (tilemap.HasTile(cellPos))
        {
            Vector3Int characterCellPos = tilemap.WorldToCell(character.position);
            List<Vector3> path = FindPath(characterCellPos, cellPos);
            if (path != null)
            {
                HighlightPath(path);
            }
        }
    }
    private void InitializeNodes()
    {
        BoundsInt bounds = tilemap.cellBounds;
        TileBase[] allTiles = tilemap.GetTilesBlock(bounds);

        for (int x = 0; x < bounds.size.x; x++)
        {
            for (int y = 0; y < bounds.size.y; y++)
            {
                Vector3Int cellPosition = new Vector3Int(bounds.x + x, bounds.y + y, 0);
                if (tilemap.HasTile(cellPosition))
                {
                    _nodes[cellPosition] = new Node(cellPosition);
                }
            }
        }
    }

    public List<Vector3> FindPath(Vector3Int start, Vector3Int goal)
    {
        if (!_nodes.ContainsKey(start) || !_nodes.ContainsKey(goal))
        {
            return null;
        }

        List<Node> openSet = new List<Node>();
        HashSet<Node> closedSet = new HashSet<Node>();

        Node startNode = _nodes[start];
        Node goalNode = _nodes[goal];

        openSet.Add(startNode);

        while (openSet.Count > 0)
        {
            Node currentNode = openSet[0];
            for (int i = 1; i < openSet.Count; i++)
            {
                if (openSet[i].FCost < currentNode.FCost || (openSet[i].FCost == currentNode.FCost && openSet[i].HCost < currentNode.HCost))
                {
                    currentNode = openSet[i];
                }
            }

            openSet.Remove(currentNode);
            closedSet.Add(currentNode);

            if (currentNode == goalNode)
            {
                return RetracePath(startNode, goalNode);
            }

            foreach (Node neighbor in GetNeighbors(currentNode))
            {
                if (closedSet.Contains(neighbor))
                {
                    continue;
                }

                int newCostToNeighbor = currentNode.GCost + GetDistance(currentNode, neighbor);
                if (newCostToNeighbor < neighbor.GCost || !openSet.Contains(neighbor))
                {
                    neighbor.GCost = newCostToNeighbor;
                    neighbor.HCost = GetDistance(neighbor, goalNode);
                    neighbor.Parent = currentNode;

                    if (!openSet.Contains(neighbor))
                    {
                        openSet.Add(neighbor);
                    }
                }
            }
        }

        return null;
    }

    private bool IsTileAnObstacle(Vector3Int cellPos)
    {
        // Check if the tile has a collider from the obstacle layer
        Collider2D obstacleCollider = Physics2D.OverlapPoint(tilemap.GetCellCenterWorld(cellPos), obstacleLayer);
        
        // Check if the tile has any object with a collider, assuming characters have colliders
        Collider2D characterCollider = Physics2D.OverlapPoint(tilemap.GetCellCenterWorld(cellPos));
        
        // If there's an obstacle or a character, consider the tile an obstacle
        return obstacleCollider != null || (characterCollider != null && characterCollider.transform != character);
    }

    private List<Vector3> RetracePath(Node startNode, Node endNode)
    {
        List<Node> path = new List<Node>();
        Node currentNode = endNode;

        while (currentNode != startNode)
        {
            path.Add(currentNode);
            currentNode = currentNode.Parent;
        }
        path.Reverse();

        List<Vector3> waypoints = new List<Vector3>();
        foreach (Node node in path)
        {
            waypoints.Add(tilemap.GetCellCenterWorld(node.Position));
        }

        return waypoints;
    }



    private List<Node> GetNeighbors(Node node)
    {
        List<Node> neighbors = new List<Node>();

        Vector3Int[] directions = {
            new Vector3Int(1, 0, 0), new Vector3Int(-1, 0, 0),
            new Vector3Int(0, 1, 0), new Vector3Int(0, -1, 0)
        };

        foreach (Vector3Int direction in directions)
        {
            Vector3Int neighborPos = node.Position + direction;
            if (_nodes.ContainsKey(neighborPos))
            {
                neighbors.Add(_nodes[neighborPos]);
            }
        }

        return neighbors;
    }

    private int GetDistance(Node nodeA, Node nodeB)
    {
        int dstX = Mathf.Abs(nodeA.Position.x - nodeB.Position.x);
        int dstY = Mathf.Abs(nodeA.Position.y - nodeB.Position.y);
        return dstX + dstY;
    }

    private void HighlightPath(List<Vector3> path)
    {
        foreach (Node node in _nodes.Values)
        {
            node.ResetColor();
        }

        foreach (Vector3 point in path)
        {
            Vector3Int cellPos = tilemap.WorldToCell(point);
            if (_nodes.ContainsKey(cellPos))
            {
                _nodes[cellPos].SetColor(pathColor);
            }
        }
    }
}
