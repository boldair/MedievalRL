using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Pathfinding : MonoBehaviour
{
    public Tilemap tilemap;
    private Transform _character;
    [SerializeField] private Color pathColor = Color.red;
    private Dictionary<Vector3Int, Node> _nodes = new Dictionary<Vector3Int, Node>();
    private List<Node> _path;
    private Camera _camera;
    private void Start()
    {
        _camera = Camera.main;
        _character = this.transform;
        InitializeNodes();
    }
    private void Update()
    {
        Vector3 mouseWorldPos = _camera.ScreenToWorldPoint(Input.mousePosition);
        Vector3Int cellPos = tilemap.WorldToCell(mouseWorldPos);

        if (tilemap.HasTile(cellPos))
        {
            Vector3Int characterCellPos = tilemap.WorldToCell(_character.position);
            _path = FindPath(characterCellPos, cellPos);
            if (_path != null)
            {
                HighlightPath(_path);
            }
        }
    }
    private void InitializeNodes()
    {
        BoundsInt bounds = tilemap.cellBounds;

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

    private List<Node> FindPath(Vector3Int start, Vector3Int goal)
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
