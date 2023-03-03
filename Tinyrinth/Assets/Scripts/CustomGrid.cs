using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomGrid : MonoBehaviour
{

    public PassageTile[,] cells;
    private float cellSpacing = 0f;
    private float cellSize = 0f;
    public int rows;
    public int columns;

    public void InitializeGridData(int nRows, int nColumns, float size, float spacing)
    {
        rows = nRows;
        columns = nColumns;
        cells = new PassageTile[nRows, nColumns];
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

    //public bool IsWithinBounds(int row, int col)
    //{
    //    return (row >= 0 && row < cells.GetLength(0)) && (col >= 0 && col < cells.GetLength(1));
    //}

    public bool IsPlaceable(Vector3Int position)
    {
        int x = position.x;
        int y = position.z;

        return (
            (((x == rows || x == -1) && !(y <= -1 || y >= columns)) ||
            ((y == columns || y == -1) && !(x <= -1 || x >= rows)))
        );
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

    public void ShiftRow(int rowIndex, PassageTile replaceValue, Utils.Direction direction){
        List<PassageTile> totalRow = new List<PassageTile>(){replaceValue}
        for (i = 0; i < rows; i++){
            totalRow.Add(cells[rowIndex, i]);
        }
        PassageTile outTile = ShiftDataRow(rowIndex, replaceValue, direction);
        // move the tiles
        // move the player
    }

    public void ShiftColumn(int columnIndex, PassageTile replaceValue, Utils.Direction direction){
        List<PassageTile> totalRow = new List<PassageTile>(){replaceValue}
        for (i = 0; i < columns; i++){
            totalRow.Add(cells[i, columnIndex]);
        }
        PassageTile outTile = ShiftDataColumn(columnIndex, replaceValue, direction);
        // move the tiles
        // move the player
    }

    public PassageTile ShiftDataRow(int rowIndex, PassageTile replaceValue, Utils.Direction direction)
    {
        PassageTile lastValue = cells[rowIndex, direction == Utils.Direction.Left ? 0 : cells.GetLength(1) - 1];

        if (direction == Utils.Direction.Left)
        {
            for (int j = cells.GetLength(1) - 1; j > 0; j--)
            {
                cells[rowIndex, j] = cells[rowIndex, j - 1];
            }
            cells[rowIndex, 0] = replaceValue;
        }
        else if (direction == Utils.Direction.Right)
        {
            for (int j = 0; j < cells.GetLength(1) - 1; j++)
            {
                cells[rowIndex, j] = cells[rowIndex, j + 1];
            }
            cells[rowIndex, cells.GetLength(1) - 1] = replaceValue;
        }
        return lastValue;
    }

    public PassageTile ShiftDataColumn(int columnIndex, PassageTile replaceValue, Utils.Direction direction)
    {
        PassageTile lastValue = cells[direction == Utils.Direction.Up ? 0 : cells.GetLength(0) - 1, columnIndex];

        if (direction == Utils.Direction.Up)
        {
            for (int i = cells.GetLength(0) - 1; i > 0; i--)
            {
                cells[i, columnIndex] = cells[i - 1, columnIndex];
            }
            cells[0, columnIndex] = replaceValue;
        }
        else if (direction == Utils.Direction.Down)
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

    #region Animations

    private List<GameObject> TilesInRow = new List<GameObject>();
    private List<PassageTile> TilesInColumn = new List<PassageTile>();

    IEnumerator ShiftRowAnimation(Utils.Direction direction)
    {
        float moveDistance;
        //row = [here, not there]
        // move the row up by a margin


        //maybe anticipation by moving the row a bit in the opposite direction
        //move all tiles by 1 in the direction of the shift
        //move row back to y = 0
        yield return null;
    }

    void DisappearAnim()
    {
        //spin real fast
        //Spin faster
        //shrink
        //particle
    }
    #endregion
}

