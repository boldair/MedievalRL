using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Pathfinding : MonoBehaviour
{
    public Tilemap _tilemap;
    private Transform _character;
    [SerializeField] private Color pathColor = Color.red;
    [SerializeField] private int maxWalkingLength = 12;
    private Dictionary<Vector3Int, Node> _nodes = new Dictionary<Vector3Int, Node>();
    public List<Node> Path { get;  private set; }
    [SerializeField] private LayerMask obstacleLayer;
    private Camera _camera;
    private void Start()
    {
        _camera = Camera.main;
        _character = this.transform;
        _tilemap = GameObject.Find("GroundTM").GetComponent<Tilemap>();
        InitializeNodes();
    }
    private void Update()
    {
        Vector3 mouseWorldPos = _camera.ScreenToWorldPoint(Input.mousePosition);
        Vector3Int cellPos = _tilemap.WorldToCell(mouseWorldPos);
        
        if (_tilemap.HasTile(cellPos))
        {
            Path = FindPathToLastValidTile(cellPos);
            if (Path != null)
            {
                HighlightPath(Path);
            }
        }
    }
    private void InitializeNodes()
    {
        BoundsInt bounds = _tilemap.cellBounds;

        for (int x = 0; x < bounds.size.x; x++)
        {
            for (int y = 0; y < bounds.size.y; y++)
            {
                Vector3Int cellPosition = new Vector3Int(bounds.x + x, bounds.y + y, 0);
                if (_tilemap.HasTile(cellPosition))
                {
                    bool walkable = !IsTileAnObstacle(cellPosition);
                    _nodes[cellPosition] = new Node(cellPosition, walkable);
                }
            }
        }
    }

    public List<Node> FindPathToLastValidTile(Vector3Int goal)
    {
        Vector3Int start = _tilemap.WorldToCell(_character.position);
        if (!_nodes.ContainsKey(start) || !_nodes.ContainsKey(goal))
        {
            return null;
        }

        List<Node> openSet = new List<Node>();
        HashSet<Node> closedSet = new HashSet<Node>();

        Node startNode = _nodes[start];
        Node goalNode = _nodes[goal];

        openSet.Add(startNode);

        Node lastValidNode = startNode;

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

            if (!currentNode.Walkable || GetDistance(startNode, currentNode) > maxWalkingLength)
            {
                // If we've hit an obstacle or exceeded max distance, return the path to the last valid node
                return RetracePath(startNode, lastValidNode);
            }

            lastValidNode = currentNode;

            if (currentNode == goalNode)
            {
                return RetracePath(startNode, goalNode);
            }

            foreach (Node neighbor in GetNeighbors(currentNode))
            {
                if (!neighbor.Walkable || closedSet.Contains(neighbor))
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

        // If we exhaust the openSet without finding a path, return null
        return null;
    }

    private bool IsTileAnObstacle(Vector3Int cellPos)
    {
        Tilemap obstaclesTilemap = GameObject.Find("ObstacleTM").GetComponent<Tilemap>();
        return obstaclesTilemap.HasTile(cellPos);
    }

    private List<Node> RetracePath(Node startNode, Node endNode)
    {
        List<Node> pathNodes = new List<Node>();
        Node currentNode = endNode;

        while (currentNode != startNode)
        {
            pathNodes.Add(currentNode);
            currentNode = currentNode.Parent;
        }
        pathNodes.Reverse();

        List<Node> waypoints = new List<Node>();
        foreach (Node node in pathNodes)
        {
            waypoints.Add(node);
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

    private void HighlightPath(List<Node> pathToHighlight)
    {
        foreach (Node node in _nodes.Values)
        {
            node.ResetColor();
        }

        foreach (Node node in pathToHighlight)
        {
            node.SetColor(pathColor);
        }
    }
}
