using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridGenerator : MonoBehaviour
{
    // Generation de grille custom
    
    public int rows = 5; // Nombre de lignes de la grille
    public int columns = 5; // Nombre de colonnes de la grille
    float cellSize = 1.0f; // Taille de chaque cellule de la grille
    public float cellSpacing = 0.2f; // Espacement entre les cellules de la grille
    public PassageTile[] prefabs; // Liste de prefabs a assigner aleatoirement aux cellules


    void Awake()
    {
        GenerateGrid();
    }

    void GenerateGrid()
    {
        GameObject gridObject = new GameObject("Grid");
        CustomGrid grid = gridObject.AddComponent<CustomGrid>();

        grid.InitializeGridData(rows, columns, cellSize, cellSpacing);

        // Generate tiles
        for (int row = 0; row < rows; row++)
        {
            for (int column = 0; column < columns; column++)
            {
                Vector3 position = new Vector3(row*(cellSize+cellSpacing), 0, column*(cellSize+cellSpacing));
                // Random rotation in increments of 90 degrees
                int rotation = Random.Range(0, 4);

                // Instantiate random prefab tile with given position and rotation
                PassageTile prefab = prefabs[Random.Range(0, prefabs.Length)];
                PassageTile instance = Instantiate(prefab);

                instance.rotation = rotation; // set rotation data
                instance.transform.rotation = Quaternion.Euler(0, rotation * 90, 0); // set visual rotation

                instance.transform.position = position; // set position
                instance.transform.parent = grid.transform; // set as child of Grid
                instance.EnableLights(false);

                grid.cells[row, column] = instance; // add to cell list
            }
        }

        var player = FindObjectOfType<PlayerController>();
        Vector3Int playerPos = grid.GetGridCellPosition(player.gameObject.transform.position);
        grid.LightThePath(grid.GetTile(playerPos));
    }
}
