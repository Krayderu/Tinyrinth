using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class NewGridGenerator : MonoBehaviour
{
//    public GameObject[] prefabs; // Liste de prefabs à assigner aléatoirement aux cellules
//    public List<LimitedPrefab> prefabsWithLimit; //Liste de prefab avec limites
//    private List<GameObject> instantiatedPrefabs = new List<GameObject>(); // Liste des prefabs instanciés

//    private Dictionary<PrefabType, int> prefabCounts = new Dictionary<PrefabType, int>(); // Compteur du nombre d'instances de chaque type de prefab déjà présent dans la grille


//    void Start()
//    {
//        // Initialiser le compteur pour chaque type de prefab avec une limite
//        foreach (LimitedPrefab limitedPrefab in prefabsWithLimit)
//        {
//            prefabCounts[GetPrefabType(limitedPrefab.prefab)] = 0;
//        }

//        GenerateGrid();
//    }

//    public void InitializeGrid()
//    {
//        Start();
//    }

//    void GenerateGrid()
//    {
//        // Trouver la grille
//        GameObject gridObject = GameObject.Find("Grid");
//        if (gridObject == null)
//        {
//            Debug.LogError("La grille n'a pas été trouvée.");
//            return;
//        }

//        // Supprimer les anciens prefabs instanciés
//        foreach (GameObject prefab in instantiatedPrefabs)
//        {
//            DestroyImmediate(prefab);
//        }
//        instantiatedPrefabs.Clear();

//        // Parcourir toutes les cellules de la grille
//        foreach (Transform cellTransform in gridObject.transform)
//        {
//            int row, column;
//            if (ParseCellName(cellTransform.name, out row, out column))
//            {
//                // Vérifier si la cellule est intérieure à la grille
//                if (row > 0 && row < 8 && column > 0 && column < 8)
//                {
//                    // Assigner un prefab aléatoire à la cellule
//                    GameObject prefab = GetRandomPrefab();
//                    if (prefab != null)
//                    {
//                        prefabCounts[GetPrefabType(prefab)]++;
//                        GameObject instance = Instantiate(prefab, cellTransform.position, GetRandomRotation()) as GameObject;
//                        instance.transform.SetParent(cellTransform);
//                        instantiatedPrefabs.Add(instance);
//                    }
//                }
//            }
//        }
//    }

//    GameObject GetRandomPrefab()
//    {
//        List<GameObject> allowedPrefabs = new List<GameObject>(prefabs);
//        foreach (LimitedPrefab limitedPrefab in prefabsWithLimit)
//        {
//            if (prefabCounts[GetPrefabType(limitedPrefab.prefab)] >= limitedPrefab.maxCount)
//            {
//                allowedPrefabs.RemoveAll(prefab => GetPrefabType(prefab) == GetPrefabType(limitedPrefab.prefab));
//            }
//        }
//        return allowedPrefabs.Count > 0 ? allowedPrefabs[Random.Range(0, allowedPrefabs.Count)] : null;
//    }

//    Quaternion GetRandomRotation()
//    {
//        int angle = Random.Range(0, 4) * 90;
//        return Quaternion.Euler(0f, 0f, angle);
//    }
//    PrefabType GetPrefabType(GameObject prefab)
//    {
//        PrefabTypeAttribute attribute = prefab.GetComponent<PrefabTypeAttribute>();
//        return attribute != null ? attribute.type : PrefabType.None;
//    }

//    bool ParseCellName(string name, out int row, out int column)
//    {
//        string[] parts = name.Split(',');
//        if (parts.Length == 2 && int.TryParse(parts[0], out row) && int.TryParse(parts[1], out column))
//        {
//            return true;
//        }
//        else
//        {
//            row = 0;
//            column = 0;
//            return false;
//        }
//    }
}

//#if UNITY_EDITOR
//[CustomEditor(typeof(GridGenerator))]
//public class GridGeneratorEditor : Editor
//{
//    public override void OnInspectorGUI()
//    {
//        DrawDefaultInspector();

//        GridGenerator gridGenerator = (GridGenerator)target;

//        // Bouton pour assigner des prefabs aléatoires
//        if (GUILayout.Button("Assign Random Prefabs"))
//        {
//            gridGenerator.InitializeGrid();
//            EditorUtility.SetDirty(gridGenerator);
//        }
//    }
//}
//#endif