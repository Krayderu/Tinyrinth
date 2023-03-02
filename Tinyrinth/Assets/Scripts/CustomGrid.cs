using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomGrid : MonoBehaviour
{

    public PassageTile[,] cells;
    private float cellSpacing = 0f;
    private float cellSize = 0f;

    public void InitializeGridData(int rows, int columns, float size, float spacing)
    {
        cells = new PassageTile[rows, columns];
        cellSize = size;
        cellSpacing = spacing;
    }
    public PassageTile getTile(int x,int y)
    {
        //x = row y = column
        if (cells[x, y] is PassageTile)
        {
            return cells[x,y];
        }
        else
        {
            return null;
        }
    }

    public bool IsWithinBounds(int row, int col)
    {
        return (row >= 0 && row < cells.GetLength(0)) && (col >= 0 && col < cells.GetLength(1));
    }

    public bool IsPlaceable(Vector3Int position, PassageTile data)
    {
        int x = position.x;
        int y = position.z;

        // Check if the tile is on the border of the grid but not on a corner
        if (!((x == 0 && y > 0 && y < cells.GetLength(1) - 1) || // Left border
              (x == cells.GetLength(0) - 1 && y > 0 && y < cells.GetLength(1) - 1) || // Right border
              (y == 0 && x > 0 && x < cells.GetLength(0) - 1) || // Bottom border
              (y == cells.GetLength(1) - 1 && x > 0 && x < cells.GetLength(0) - 1))) // Top border
        {
            return false;
        }

        // Check if the tile overlaps with any other tiles
        if (getTile(x, y) != null)
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    public bool IsTileConnected(Vector3Int cellpos, PassageTile data, int rotation)
    {
        // check that the tile can connect to another tile
        bool canConnect = false;

        //liste des cases voisines
        Vector3Int[] neighbors = new Vector3Int[]
        {
            new Vector3Int(0,0,1), //Up
            new Vector3Int(1,0,0), //Left
            new Vector3Int(0,0,-1), //Down
            new Vector3Int(-1,0,0), //Right
        };
        // liste des connections
        int[] connectionIndices = new int[]
        {
            2,//Up -> Down
            1,//Left -> Right
            0,//Down -> Up
            3,//Right -> Left
        };

        for (int i = 0; i < neighbors.Length; i++)
        {
            Vector3Int neighborPos = cellpos + neighbors[i];
            int neighborX = neighborPos.x;
            int neighborY = neighborPos.z;
            PassageTile neighborTile = getTile(neighborX,neighborY);

            if (neighborTile == null)
            {
                continue;
            }

            bool[] neighborConnections = neighborTile.sockets.ToArray();

            int neighborIndex = (i + neighborTile.rotation) % 4;
            int selfIndex = (i + rotation) % 4;
            if(neighborConnections[connectionIndices[neighborIndex]] && data.sockets[selfIndex])
            {
                canConnect = true;
                break;
            }
        }
        if (!canConnect) return false;

        return true;
    }

    public Vector3Int GetGridCellPosition(Vector3 position)
    {
        float gridSize = 1f;
        int x = Mathf.RoundToInt(position.x / gridSize);
        int y = Mathf.RoundToInt(position.z / gridSize);

        // Get the closest cell position on the grid
        Vector3Int cellPos = new Vector3Int(x, 0, y);

        return cellPos;
    }


    public Vector3 SnapToGrid(Vector3 position)
    {
        float gridSize = 1f;
        int x = Mathf.RoundToInt(position.x / gridSize);
        int y = Mathf.RoundToInt(position.z / gridSize);

        // Get the closest cell position on the grid
        Vector3Int cellPos = new Vector3Int(x, 0, y);

        // Get the world position of the center of the closest cell on the grid
        //Vector3 gridPos = cells[cellPos.x, cellPos.z].transform.position;
        float gridX = x * (cellSize + cellSpacing);
        float gridY = y * (cellSize + cellSpacing);
        Vector3 gridPos = new Vector3(gridX, 0, gridY);

        // Adjust the y-coordinate of the snapped position to match the original position
        gridPos.y = position.y;

        return gridPos;
    }

    #region ArrayOperations
    public enum Direction
    {
        Left,
        Right,
        Up,
        Down
    }

    public ShiftRowForReal(int rowIndex, PassageTile replaceValue, Direction direction){
        var outTile = ShiftRow(rowIndex, replaceValue, direction);
        // move the tiles
        // move the player
    }

    public PassageTile ShiftRow(int rowIndex, PassageTile replaceValue, Direction direction)
    {
        PassageTile lastValue = cells[rowIndex, direction == Direction.Left ? 0 : cells.GetLength(1) - 1];

        if (direction == Direction.Left)
        {
            for (int j = cells.GetLength(1) - 1; j > 0; j--)
            {
                cells[rowIndex, j] = cells[rowIndex, j - 1];
            }
            cells[rowIndex, 0] = replaceValue;
        }
        else if (direction == Direction.Right)
        {
            for (int j = 0; j < cells.GetLength(1) - 1; j++)
            {
                cells[rowIndex, j] = cells[rowIndex, j + 1];
            }
            cells[rowIndex, cells.GetLength(1) - 1] = replaceValue;
        }
        return lastValue;
    }

    public PassageTile ShiftColumn(int columnIndex, PassageTile replaceValue, Direction direction)
    {
        PassageTile lastValue = cells[direction == Direction.Up ? 0 : cells.GetLength(0) - 1, columnIndex];

        if (direction == Direction.Up)
        {
            for (int i = cells.GetLength(0) - 1; i > 0; i--)
            {
                cells[i, columnIndex] = cells[i - 1, columnIndex];
            }
            cells[0, columnIndex] = replaceValue;
        }
        else if (direction == Direction.Down)
        {
            for (int i = 0; i < cells.GetLength(0) - 1; i++)
            {
                cells[i, columnIndex] = cells[i + 1, columnIndex];
            }
            cells[cells.GetLength(0) - 1, columnIndex] = replaceValue;
        }
        return lastValue;
    }
    #endregion
}

