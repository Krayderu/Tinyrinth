using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

#if UNITY_EDITOR
public class GridGeneratorWindow : EditorWindow
{
    int rows = 5; // Nombre de lignes de la grille
    int columns = 5; // Nombre de colonnes de la grille
    float cellSize = 1.0f; // Taille de chaque cellule de la grille
    float cellSpacing = 0.2f; // Espacement entre les cellules de la grille
    Texture2D cellIcon; // Icône pour chaque cellule de la grille

    [MenuItem("Tools/Grid Generator")]
    static void Init()
    {
        GridGeneratorWindow window = (GridGeneratorWindow)EditorWindow.GetWindow(typeof(GridGeneratorWindow));
        window.Show();
    }

    void OnGUI()
    {
        GUILayout.Label("Grid Settings", EditorStyles.boldLabel);

        rows = EditorGUILayout.IntField("Rows", rows);
        columns = EditorGUILayout.IntField("Columns", columns);
        cellSize = EditorGUILayout.FloatField("Cell Size", cellSize);
        cellSpacing = EditorGUILayout.FloatField("Cell Spacing", cellSpacing);
        cellIcon = (Texture2D)EditorGUILayout.ObjectField("Cell Icon", cellIcon, typeof(Texture2D), false);

        if (GUILayout.Button("Generate Grid"))
        {
            GenerateGrid();
        }
    }

    void GenerateGrid()
    {
        GameObject gridObject = new GameObject("Grid");
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

                // Ajouter une icône personnalisée
                if (cellIcon != null)
                {
                    Icon icon = cell.AddComponent<Icon>();
                    icon.icon = cellIcon;
                }

                currentPosition += new Vector3(cellSize + cellSpacing, 0, 0);
            }

            currentPosition = startPosition;
            currentPosition += new Vector3(0, 0, (cellSize + cellSpacing) * (row + 1));
        }
    }
}

public class Icon : MonoBehaviour
{
    public Texture2D icon;

    void OnValidate()
    {
        if (icon != null)
        {
            EditorGUIUtility.SetIconForObject(gameObject, icon);
        }
    }
}
#endif
