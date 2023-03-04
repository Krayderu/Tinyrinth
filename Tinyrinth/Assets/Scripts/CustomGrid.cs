using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class CustomGrid : MonoBehaviour
{

    public PassageTile[,] cells;
    private float cellSpacing = 0f;
    private float cellSize = 0f;
    public float gridSize = 1f;
    public int rows;
    public int columns;
    public bool isMoving = false;
    bool isSpinning = false;
    private PlayerController player;

    public void Start(){
        player = FindObjectOfType<PlayerController>();
    }

    public void InitializeGridData(int nRows, int nColumns, float size, float spacing)
    {
        rows = nRows;
        columns = nColumns;
        cells = new PassageTile[nRows, nColumns];
        cellSize = size;
        cellSpacing = spacing;
    }

    public void ShiftRow(int rowIndex, PassageTile replaceValue, Utils.Direction direction){
        List<PassageTile> totalRow = GetRow(rowIndex);
        // Shift the tile
        PassageTile outTile = ShiftDataRow(rowIndex, replaceValue, direction);

        // Shift visually
        totalRow.Add(replaceValue); // add the new tile so it gets moved as well
        // move the tiles
        foreach(PassageTile tile in totalRow)
        {
            StartCoroutine(ShiftAnimation(tile, direction, outTile));
        }
        
        // if the player is on one of the shifted tiles
        Vector3Int playerPos = GetGridCellPosition(player.gameObject.transform.position);
        // move the player
        if (playerPos.x == rowIndex){
            StartCoroutine(ShiftAnimation(player, direction, outTile));
        }

    }

    public void ShiftColumn(int columnIndex, PassageTile replaceValue, Utils.Direction direction){
        List<PassageTile> totalColumn = GetColumn(columnIndex);
        // Shift the tile
        PassageTile outTile = ShiftDataColumn(columnIndex, replaceValue, direction);

        // Shift visually
        totalColumn.Add(replaceValue);
        // move the tiles
        foreach(PassageTile tile in totalColumn)
        {
            StartCoroutine(ShiftAnimation(tile, direction, outTile));
        }

        // if the player is on one of the shifted tiles
        Vector3Int playerPos = GetGridCellPosition(player.gameObject.transform.position);
        // move the player
        if (playerPos.z == columnIndex){
            StartCoroutine(ShiftAnimation(player, direction, outTile));
        }
    }

    private void EnableAllLights(bool value){
        foreach(var tile in GetAllTiles()){
            tile.EnableLights(value);
        }
    }

    public void LightThePath(PassageTile originTile){
        EnableAllLights(false);
        // Flood-fill: Queue-based recursive implementation
        // 1. Set Q to the empty queue
        Queue<PassageTile> connectedTiles = new Queue<PassageTile>();
        
        // 2. Add node to the end of Q.
        connectedTiles.Enqueue(originTile); //GetConnectedTiles(origin, originTile);

        // 3. While Q is not empty:
        while (connectedTiles.Count > 0){
            // 4.   Set n equal to the first element of Q.
            // 5.   Remove first element from Q.
            PassageTile tile = connectedTiles.Dequeue();
            
            if (!tile.lit){
                // Light the tile
                tile.EnableLights(true);
                // Add all the tiles that are connected to this one to the queue
                var connectedNeighbors = GetConnectedTiles(tile);
                foreach (PassageTile connectedTile in connectedNeighbors){
                    if (!connectedTile.lit){
                        connectedTiles.Enqueue(connectedTile);
                    }
                }
            }
            // Continue looping until Q is exhausted.
        }
    }

    public List<PassageTile> GetConnectedTiles(PassageTile tile)
    {
        // check that the tile can connect to another tile
        List<PassageTile> connections = new List<PassageTile>();
        Vector3Int cellPos = GetGridCellPosition(tile.gameObject.transform.position);

        //liste des cases voisines
        Vector3Int[] neighbors = new Vector3Int[]
        {
            new Vector3Int(1,0,0), //Up
            new Vector3Int(0,0,-1), //Right
            new Vector3Int(-1,0,0), //Down
            new Vector3Int(0,0,1), //Left
        };
        // liste des connections
        int[] connectionIndices = new int[]
        {
            2,//Up -> Down
            3,//Right -> Left
            0,//Down -> Up
            1,//Left -> Right
        };

        for (int i = 0; i < neighbors.Length; i++)
        {
            Vector3Int neighborPos = cellPos + neighbors[i];
            PassageTile neighborTile = GetTile(neighborPos);

            if (neighborTile == null) continue;

            bool[] neighborSockets = neighborTile.sockets.ToArray();

            int neighborIndex = connectionIndices[(i - neighborTile.rotation + 4) % 4];
            int selfIndex = (i - tile.rotation + 4) % 4;

            if(neighborSockets[neighborIndex] && tile.sockets[selfIndex])
            {
                connections.Add(neighborTile);
            }
        }

        return connections;
    }

    #region Utils
    public bool IsWithinBounds(Vector3Int pos)
    {
       return (pos.x >= 0 && pos.x < cells.GetLength(0)) && (pos.z >= 0 && pos.z < cells.GetLength(1));
    }

    public bool IsPlaceable(Vector3Int position)
    {
        int x = position.x;
        int y = position.z;

        return (
            (((x == rows || x == -1) && !(y <= -1 || y >= columns)) ||
            ((y == columns || y == -1) && !(x <= -1 || x >= rows)))
        );
    }

    public Vector3Int GetGridCellPosition(Vector3 position)
    {
        int x = Mathf.RoundToInt(position.x / gridSize);
        int y = Mathf.RoundToInt(position.z / gridSize);

        // Get the closest cell position on the grid
        Vector3Int cellPos = new Vector3Int(x, 0, y);

        return cellPos;
    }

    // public Vector3 CellToWorld(Vector3Int pos){
    //     return new Vector3(pos.x * (cellSize + cellSpacing), 0, pos.z * (cellSize + cellSpacing));
    // }


    public Vector3 SnapToGrid(Vector3 position)
    {
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
        //gridPos.y = position.y;

        return gridPos;
    }
    #endregion

    #region ArrayOperations

    public PassageTile GetTile(Vector3Int pos)
    {
        PassageTile tile;
        //x = row z = column
        try {
            tile = cells[pos.x, pos.z];
        } catch(Exception ex)
        {
            tile = null;
        }
        return tile;
    }


    public List<PassageTile> GetAllTiles(){
        List<PassageTile> allTiles = new List<PassageTile>();
        for (int row = 0; row < rows; row++){
            for (int column = 0; column < columns; column++){
                allTiles.Add(cells[row, column]);
            }
        }
        return allTiles;
    }

    private List<PassageTile> GetRow(int rowIndex){
        List<PassageTile> totalRow = new List<PassageTile>();
        for (int i = 0; i < columns; i++){
            totalRow.Add(cells[rowIndex, i]);
        }
        return totalRow;
    }

    private List<PassageTile> GetColumn(int columnIndex){
        List<PassageTile> totalColumn = new List<PassageTile>();
        for (int i = 0; i < rows; i++){
            totalColumn.Add(cells[i, columnIndex]);
        }
        return totalColumn;
    }

    private PassageTile ShiftDataRow(int rowIndex, PassageTile replaceValue, Utils.Direction direction)
    {
        PassageTile lastValue = cells[rowIndex, direction == Utils.Direction.Left ? cells.GetLength(1) - 1 : 0];

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

    private PassageTile ShiftDataColumn(int columnIndex, PassageTile replaceValue, Utils.Direction direction)
    {
        PassageTile lastValue = cells[direction == Utils.Direction.Up ? cells.GetLength(0) - 1 : 0, columnIndex];

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

    IEnumerator ShiftAnimation(MonoBehaviour obj, Utils.Direction direction, PassageTile outTile)
    {

        isMoving = true;
        float moveDistance = cellSize + cellSpacing;
        float vDistance = 0.5f;
        float moveDuration = 0.5f;

        // move the row up by a margin
        float t = 0.0f;
        Vector3 startPosition = obj.transform.position;
        Vector3 endPosition = startPosition + Vector3.up * vDistance;
        while (t < moveDuration /3)
        {
            t += Time.deltaTime;
            obj.transform.position = Vector3.Lerp(startPosition, endPosition, t / (moveDuration / 3));
            yield return null;
        }
        // move direction
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
        startPosition = obj.transform.position;
        endPosition = startPosition + (directionVector * moveDistance);
        while (t < moveDuration /3)
        {
            t += Time.deltaTime;
            obj.transform.position = Vector3.Lerp(startPosition, endPosition, t / (moveDuration / 3));
            yield return null;
        }

        //move tile back down
        t = 0f;

        startPosition = obj.transform.position;
        endPosition = startPosition + Vector3.down * vDistance;
        while (t < moveDuration /3)
        {
            t += Time.deltaTime;
            obj.transform.position = Vector3.Lerp(startPosition, endPosition, t / (moveDuration / 3));
            yield return null;
        }
        isMoving = false;
        if (!isSpinning)
        {
            StartCoroutine(DisappearAnim(outTile));
        }
    }

    IEnumerator DisappearAnim(PassageTile outTile)
    {
        isSpinning = true;
        float animDuration = .3f;
        Vector3 scale = Vector3.one;
        float rotation = 0f;
        float rotationSpeed = 0f;
        float rotationAccel = 1f;
        //spin real fast
        var t = 0f;

        while (t < animDuration)
        {
            t += Time.deltaTime;
            rotation += rotationSpeed;
            rotationSpeed += rotationAccel;
            scale = Vector3.Lerp(Vector3.one, Vector3.zero, t / animDuration);
            outTile.transform.Rotate(new Vector3(0, rotation, 0));
            outTile.transform.localScale = scale;
            yield return null;
        }
        
        Destroy(outTile.gameObject);

        Vector3Int playerPos = GetGridCellPosition(player.gameObject.transform.position);
        LightThePath(GetTile(playerPos));

        isSpinning = false;
    }
    #endregion
}

