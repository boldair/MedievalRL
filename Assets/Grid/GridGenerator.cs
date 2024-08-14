using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Tilemaps;
using Random = UnityEngine.Random;

namespace Grid
{
    public class GridGenerator : MonoBehaviour
    {
    
        public Tilemap tilemap;

        public int sizeGrid = 0;
        private Dictionary<Vector3Int, Node> grid;
        // Start is called before the first frame update
        void Start()
        {
            GenerateGrid();
        }

        private void GenerateGrid()
        {
            grid = new Dictionary<Vector3Int, Node>();

            // Define the bounds of the Tilemap
            BoundsInt bounds = tilemap.cellBounds;

            // Iterate through each cell position in the Tilemap
            foreach (Vector3Int position in bounds.allPositionsWithin)
            {
                // Check if the Tilemap has a tile at this position
                if (tilemap.HasTile(position))
                {
                    // Get the tile at the current cell position
                    TileBase tile = tilemap.GetTile(position);

                    // Create a new Node for the A* algorithm
                    Node node = new Node(position)
                    {
                        Walkable = IsTileWalkable(tile),
                        GCost = int.MaxValue,
                        HCost = 0,
                        Parent = null
                    };
                    // Add the node to the grid
                    grid[position] = node;
                    Vector3 worldPosition = tilemap.CellToWorld(position) + tilemap.cellSize / 2;
                }
                
            }

            sizeGrid = grid.Count;
        }

        private bool IsTileWalkable(TileBase tile)
        {
            

            // Add other logic to determine if a tile is walkable based on other criteria
            // For example, based on tile type or specific tile properties

            return true;
        }

        
    }
}
