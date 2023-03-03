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
    CustomGrid cGrid;

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

        if (Input.GetKey(KeyCode.R) && !isRotating){
            // rotation asjadj euler machin
            StartCoroutine(SpinAnimation());
            currentPrefab.rotation = (currentPrefab.rotation + 1) % 4;
        }

        if (Input.GetMouseButtonDown(0)){
            Vector3 click = mousePos;
            Vector3 snapPos = grid.SnapToGrid(click);
            //changer de tile pour current prefab
            //Debug.Log(grid.SnapToGrid(click));

            Vector3Int gridCellPos  = grid.GetGridCellPosition(snapPos);

            //Debug.Log(gridCellPos);
            // Vérifier qu'on peut construire
            if (!grid.IsPlaceable(gridCellPos)) return;

            //var newTile = Instantiate(currentPrefab, snapPos, currentPrefab.transform.rotation);


            // 1. trouver quelle colonne / ligne : row/columnIndex
            // Find shift direction
            Utils.Direction direction;
            int index;
            if (gridCellPos.x >= grid.rows){
                direction = Utils.Direction.Down;
                // shift column
                index = gridCellPos.z;
                grid.ShiftColumn(index, currentTile, direction);

            } else if (gridCellPos.x < 0){
                direction = Utils.Direction.Up;
                // shift column
                index = gridCellPos.z;
                grid.ShiftColumn(index, currentTile, direction);
            }

            if (gridCellPos.z >= grid.columns){
                direction = Utils.Direction.Right;
                // shift row
                index = gridCellPos.x;
                grid.ShiftRow(index, currentTile, direction);

            } else if (gridCellPos.z < 0){
                direction = Utils.Direction.Left;
                // shift row
                index = gridCellPos.x;
                grid.ShiftRow(index, currentTile, direction);
            }
            //Debug.Log(gridCellPos);

            currentPrefab = Instantiate(pickPrefab());

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
        return prefabs[Random.Range(0, prefabs.Length)];
    }

    private bool isRotating = false;

    private IEnumerator SpinAnimation()
    {
        isRotating = true;
        float duration = .1f;
        float elapsedTime = 0.0f;
        Quaternion startRotation = currentPrefab.transform.rotation;
        Quaternion endRotation = startRotation * Quaternion.Euler(0, 90, 0);

        while (elapsedTime < duration)
        {
            currentPrefab.transform.rotation = Quaternion.Slerp(startRotation, endRotation, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        currentPrefab.transform.rotation = endRotation;
        isRotating = false;
    }
}