using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class GridGenerator : MonoBehaviour
{
    // Generation de grille custom
    
    public int rows = 5; // Nombre de lignes de la grille
    public int columns = 5; // Nombre de colonnes de la grille
    float cellSize = 1.0f; // Taille de chaque cellule de la grille
    public float cellSpacing = 0.2f; // Espacement entre les cellules de la grille
    void GenerateGridWindow()
    {
        GameObject gridObject = new GameObject("Grid");
        gridObject.AddComponent<CustomGrid>();
        Undo.RegisterCreatedObjectUndo(gridObject, "Created Grid");

        Vector3 startPosition = Vector3.zero;
        Vector3 currentPosition = startPosition;

        for (int row = 0; row < rows; row++)
        {
            for (int column = 0; column < columns; column++)
            {
                GameObject cell = new GameObject(row.ToString() + "," + column.ToString());
                Undo.RegisterCreatedObjectUndo(cell, "Created Cell");

                cell.transform.position = currentPosition;
                cell.transform.parent = gridObject.transform;

                // Ajouter le composant Empty
                cell.AddComponent<MeshFilter>();
                cell.AddComponent<MeshRenderer>();

                currentPosition += new Vector3(cellSize + cellSpacing, 0, 0);
            }

            currentPosition = startPosition;
            currentPosition += new Vector3(0, 0, (cellSize + cellSpacing) * (row + 1));
        }
    }

    // Addition de prefabs sur la grille + data


    public PassageTile[] prefabs; // Liste de prefabs � assigner al�atoirement aux cellules
    private List<PassageTile> instantiatedPrefabs = new List<PassageTile>(); // Liste des prefabs instanci�s

    void Awake()
    {
        GenerateGridWindow();

        GenerateGrid();
        //Debug.Log("la grille contient " + rows * columns + " cases");
    }


    void GenerateGrid()
    {
        // Trouver la grille
        GameObject gridObject = GameObject.Find("Grid");
        if (gridObject == null)
        {
            Debug.LogError("La grille n'a pas ete trouvee.");
            return;
        }

        CustomGrid customGridObject = gridObject.GetComponent<CustomGrid>();

        if (customGridObject != null)
        {
            customGridObject.InitializeGridData(rows, columns, cellSize, cellSpacing);

            // Supprimer les anciens prefabs instanci�s
            foreach (PassageTile prefab in instantiatedPrefabs)
            {
                DestroyImmediate(prefab);
            }
            instantiatedPrefabs.Clear();

            // Parcourir toutes les cellules de la grille
            foreach (Transform cellTransform in gridObject.transform)
            {
                int row, column;
                if (ParseCellName(cellTransform.name, out row, out column))
                {
                    //// Verifier si la cellule est interieure a la grille
                    //if (row > 0 && row < rows && column > 0 && column < columns)
                    //{
                        // Assigner un prefab aleatoire a la cellule
                        PassageTile prefab = prefabs[Random.Range(0, prefabs.Length)];
                        PassageTile instance = Instantiate(prefab, cellTransform.position, Quaternion.identity);
                        instance.transform.rotation = Quaternion.Euler(0, Random.Range(0, 4) * 90, 0); // Rotation al�atoire en incr�ment de 90 degr�s
                        int currentRotation = (int)instance.transform.rotation.y / 90;
                        instance.rotation = currentRotation + 1;
                        instantiatedPrefabs.Add(instance);

                        customGridObject.cells[row, column] = instance;
                    //}
                }
            }

            Debug.Log(customGridObject.cells.Length) ;
        }
        else
        {
            Debug.LogError("Le composant CustomGrid n'a pas ete trouve sur l'objet Grid.");
        }
    }

    bool ParseCellName(string name, out int row, out int column)
    {
        string[] parts = name.Split(',');
        if (parts.Length == 2 && int.TryParse(parts[0], out row) && int.TryParse(parts[1], out column))
        {
            return true;
        }
        else
        {
            row = 0;
            column = 0;
            return false;
        }
    }
}

// #if UNITY_EDITOR
// [CustomEditor(typeof(GridGenerator))]
// public class GridGeneratorEditor : Editor
// {
//     public override void OnInspectorGUI()
//     {
//         DrawDefaultInspector();

//         GridGenerator gridGenerator = (GridGenerator)target;

//         // Bouton pour assigner des prefabs al�atoires
//         if (GUILayout.Button("Assign Random Prefabs"))
//         {
//             gridGenerator.InitializeGrid();
//             EditorUtility.SetDirty(gridGenerator);
//         }
//     }
// }
// #endif
