using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class GridGenerator : MonoBehaviour
{
    public PassageTile[] prefabs; // Liste de prefabs � assigner al�atoirement aux cellules
    private List<PassageTile> instantiatedPrefabs = new List<PassageTile>(); // Liste des prefabs instanci�s

    void Start()
    {
        GenerateGrid();
    }

    public void InitializeGrid()
    {
        Start();
    }

    void GenerateGrid()
    {
        // Trouver la grille
        GameObject gridObject = GameObject.Find("Grid");
        if (gridObject == null)
        {
            Debug.LogError("La grille n'a pas �t� trouv�e.");
            return;
        }

        CustomGrid customGridObject = gridObject.GetComponent<CustomGrid>();


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
                // V�rifier si la cellule est int�rieure � la grille
                if (row > 0 && row < 8 && column > 0 && column < 8)
                {
                    // Assigner un prefab al�atoire � la cellule
                    PassageTile prefab = prefabs[Random.Range(0, prefabs.Length)];
                    PassageTile instance = Instantiate(prefab, cellTransform.position, Quaternion.identity);
                    instance.transform.rotation = Quaternion.Euler(0, Random.Range(0, 4) * 90, 0); // Rotation al�atoire en incr�ment de 90 degr�s
                    instantiatedPrefabs.Add(instance);

                    customGridObject.cells[row,column] = instance;
                }
            }
        }

        Debug.Log(customGridObject.cells);
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

#if UNITY_EDITOR
[CustomEditor(typeof(GridGenerator))]
public class GridGeneratorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        GridGenerator gridGenerator = (GridGenerator)target;

        // Bouton pour assigner des prefabs al�atoires
        if (GUILayout.Button("Assign Random Prefabs"))
        {
            gridGenerator.InitializeGrid();
            EditorUtility.SetDirty(gridGenerator);
        }
    }
}
#endif
