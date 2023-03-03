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
    public bool isMoving = false;

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
        List<PassageTile> totalRow = new List<PassageTile>{ replaceValue };
        for (int i = 0; i < rows; i++){
            totalRow.Add(cells[i, rowIndex]);
        }
        PassageTile outTile = ShiftDataRow(rowIndex, replaceValue, direction);

        // move the tiles
        foreach(PassageTile tile in totalRow)
        {
            StartCoroutine(ShiftRowAnimation(tile, direction, outTile));
        }
        // move the player
    }

    public void ShiftColumn(int columnIndex, PassageTile replaceValue, Utils.Direction direction){
        List<PassageTile> totalRow = new List<PassageTile>() { replaceValue };
        for (int i = 0; i < columns; i++){
            totalRow.Add(cells[columnIndex, i]);
        }
        PassageTile outTile = ShiftDataColumn(columnIndex, replaceValue, direction);
        
        // move the tiles
        foreach(PassageTile tile in totalRow)
        {
            StartCoroutine(ShiftRowAnimation(tile, direction, outTile));
        }
        // move the player
    }

    public PassageTile ShiftDataRow(int rowIndex, PassageTile replaceValue, Utils.Direction direction)
    {
        PassageTile lastValue = cells[direction == Utils.Direction.Left ? cells.GetLength(1) - 1 : 0, rowIndex];

        if (direction == Utils.Direction.Left)
        {
            for (int j = cells.GetLength(1) - 1; j > 0; j--)
            {
                cells[j, rowIndex] = cells[j - 1, rowIndex];
            }
            cells[0, rowIndex] = replaceValue;
        }
        else if (direction == Utils.Direction.Right)
        {
            for (int j = 0; j < cells.GetLength(1) - 1; j++)
            {
                cells[j, rowIndex] = cells[j + 1, rowIndex];
            }
            cells[cells.GetLength(1) - 1, rowIndex] = replaceValue;
        }
        return lastValue;
    }

    public PassageTile ShiftDataColumn(int columnIndex, PassageTile replaceValue, Utils.Direction direction)
    {
        PassageTile lastValue = cells[columnIndex, direction == Utils.Direction.Up ? cells.GetLength(0) - 1 : 0];

        if (direction == Utils.Direction.Up)
        {
            for (int i = cells.GetLength(0) - 1; i > 0; i--)
            {
                cells[columnIndex, i] = cells[columnIndex, i - 1];
            }
            cells[columnIndex, 0] = replaceValue;
        }
        else if (direction == Utils.Direction.Down)
        {
            for (int i = 0; i < cells.GetLength(0) - 1; i++)
            {
                cells[columnIndex, i] = cells[columnIndex, i + 1];
            }
            cells[columnIndex, cells.GetLength(0) - 1] = replaceValue;
        }
        return lastValue;
    }
    #endregion

    #region Animations

    private List<GameObject> TilesInRow = new List<GameObject>();
    private List<PassageTile> TilesInColumn = new List<PassageTile>();

    IEnumerator ShiftRowAnimation(PassageTile tile, Utils.Direction direction, PassageTile outTile)
    {

        isMoving = true;
        float moveDistance = cellSize + cellSpacing;
        float vDistance = 0.5f;
        float moveDuration = 0.5f;

        // move the row up by a margin
        float t = 0.0f;
        Vector3 startPosition = tile.transform.position;
        Vector3 endPosition = startPosition + Vector3.up * vDistance;
        while (t < moveDuration /3)
        {
            t += Time.deltaTime;
            tile.transform.position = Vector3.Lerp(startPosition, endPosition, t / (moveDuration / 3));
            yield return null;
        }
        // move tile direction
        Vector3 directionVector = new Vector3(0, 0, 0);
        if(direction == Utils.Direction.Left)
        {
            directionVector = new Vector3(0,0,1);
        }
        if (direction == Utils.Direction.Right)
        {
            directionVector = new Vector3(0, 0, -1);
        }
        if (direction == Utils.Direction.Up)
        {
            directionVector = new Vector3(1, 0, 0);

        }
        if (direction == Utils.Direction.Down)
        {
            directionVector = new Vector3(-1, 0, 0);
        }

        t = 0f;
        startPosition = tile.transform.position;
        endPosition = startPosition + (directionVector * moveDistance);
        while (t < moveDuration /3)
        {
            t += Time.deltaTime;
            tile.transform.position = Vector3.Lerp(startPosition, endPosition, t / (moveDuration / 3));
            yield return null;
        }

        //move tile back down
        t = 0f;

        startPosition = tile.transform.position;
        endPosition = startPosition + Vector3.down * vDistance;
        while (t < moveDuration /3)
        {
            t += Time.deltaTime;
            tile.transform.position = Vector3.Lerp(startPosition, endPosition, t / (moveDuration / 3));
            yield return null;
        }
        isMoving = false;

    }

    void DisappearAnim()
    {
        //spin real fast
        //Spin faster
        //shrink
        //particle
        //Destroy(outTile);
    }
    #endregion
}

