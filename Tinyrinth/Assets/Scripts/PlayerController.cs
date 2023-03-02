using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private PassageTile currentTile;
    private PassageTile startingTile;
    private CustomGrid grid;
    private PassageTile[] prefabs;
    private PassageTile currentPrefab;
    public Vector2 gridPos = new Vector2(0,0);

    private void Start()
    {
        //Setting the playerPos in the Grid
        grid = FindObjectOfType<CustomGrid>();
        //grid = findGrid.GetComponent<CustomGrid>();
        PassageTile startingTile = grid.cells[0,0];
        currentTile = startingTile;
        GridGenerator generator = FindObjectOfType<GridGenerator>();
        prefabs = generator.prefabs;
        var chosenPrefab = pickPrefab();
        Debug.Log(chosenPrefab);
        currentPrefab = Instantiate(chosenPrefab);
    }
    private void Update()
    {
        var mousePos = GetMouseWorldPosition();
        currentPrefab.transform.position = grid.SnapToGrid(mousePos);
        
        //Player Movement
        if (Input.GetKey(KeyCode.W))
        {

        }

        if (Input.GetKey(KeyCode.R)){
            // rotation asjadj euler machin
            currentPrefab.rotation = (currentPrefab.rotation + 1) % 4;
        }

        if (Input.GetMouseButtonDown(0)){
            Vector3 click = mousePos;
            Debug.Log(grid.SnapToGrid(click));
            // 1. trouver quelle colonne / ligne
            // 2. shift colonne/ligne
        }

    }

    public Vector3 GetMouseWorldPosition()
    {
        // Get the position on the tilemap that the mouse is pointing to
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Plane plane = new Plane(new Vector3(0,1,0), new Vector3(0,0,0));
        if (plane.Raycast(ray, out float distance))
        {
            Vector3 hitPoint = ray.GetPoint(distance);
            return hitPoint;
        }
        else
        {
            return Vector3.zero;
        }
    }

    private PassageTile pickPrefab(){
        return prefabs[Random.Range(0, prefabs.Length-1)];
    }
}