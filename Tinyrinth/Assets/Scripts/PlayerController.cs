using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    //variables for playerState

    private PassageTile currentTile;                //current position of the player
    private PassageTile startingTile;               //Starting position of the player's movement
    public Vector2 gridPos = new Vector2(0, 0);     //PlayerPos in datagrid

    //variables to call other scripts

    private CustomGrid grid;                        //grid element from CustomGrid Class
    private PassageTile[] prefabs;                  //An array of tile prefabs

    //Variables responsible for visual placement of Tiles

    /*public PassageTile[] pickedPrefabs; */            //An array of corresponding tile with hover material 
    private PassageTile currentPrefab;              //instanciated current game tile
    /*private PassageTile currentPickedPrefab; */       //instanciated current hover tile
    private PassageTile chosenPrefab;               //picked tile
    private int prefabIndex;                        //Get the index from the RandomRange in PickPrefab()

    //variable responsible for animation states
    private bool isRotating = false;                //Animation condition
    


    private void Start()
    {
        //Finding grid and setting the playerPos in the Grid
        grid = FindObjectOfType<CustomGrid>();
        PassageTile startingTile = grid.cells[0,0];
        currentTile = startingTile;

        //Get the prefabs array from the GridGenerator script, pick a random prefab, and instantiate it
        GridGenerator generator = FindObjectOfType<GridGenerator>();
        prefabs = generator.prefabs;
        chosenPrefab = PickPrefab();
        currentPrefab = Instantiate(chosenPrefab);

        //Instantiate HoverPrefab
        //currentPickedPrefab = Instantiate(HoverPrefab(prefabIndex));
        
    }
    private void Update()
    {
        var mousePos = GetMouseWorldPosition();
        Vector3 snapPos = grid.SnapToGrid(mousePos);
        currentPrefab.transform.position = snapPos;
        //currentPickedPrefab.transform.position = snapPos;

        
        //Player Movement
        if (Input.GetKey(KeyCode.W))
        {

        }

        if (Input.GetKeyUp(KeyCode.R) && !isRotating){
            // rotation asjadj euler machin
            StartCoroutine(SpinAnimation(currentPrefab));
            //StartCoroutine(SpinAnimation(currentPickedPrefab));
            //chosenPrefab.rotation = (chosenPrefab.rotation + 1) % 4;
            currentPrefab.rotation = (currentPrefab.rotation + 1) % 4;
            //currentPickedPrefab.rotation = (currentPickedPrefab.rotation + 1) % 4;
            //Debug.Log(chosenPrefab.rotation);
            //Debug.Log(currentPickedPrefab.rotation);
        }

        if (Input.GetMouseButtonUp(0)){
            //changer de tile pour current prefab
            //Debug.Log(grid.SnapToGrid(click));

            Vector3Int gridCellPos = grid.GetGridCellPosition(snapPos);

            //Debug.Log(gridCellPos);
            // Vérifier qu'on peut construire
            if (!grid.IsPlaceable(gridCellPos) && !grid.isMoving) return;


            // 1. trouver quelle colonne / ligne : row/columnIndex
            // Find shift direction
            Utils.Direction direction;
            if (!grid.isMoving)
            {
                int index;
                if (gridCellPos.x >= grid.rows)
                {
                    direction = Utils.Direction.Down;
                    // shift column
                    index = gridCellPos.z;
                    grid.ShiftColumn(index, currentPrefab, direction);

                }
                else if (gridCellPos.x < 0)
                {
                    direction = Utils.Direction.Up;
                    // shift column
                    index = gridCellPos.z;
                    grid.ShiftColumn(index, currentPrefab, direction);
                }

                if (gridCellPos.z >= grid.columns)
                {
                    direction = Utils.Direction.Right;
                    // shift row
                    index = gridCellPos.x;
                    grid.ShiftRow(index, currentPrefab, direction);

                }
                else if (gridCellPos.z < 0)
                {
                    direction = Utils.Direction.Left;
                    // shift row
                    index = gridCellPos.x;
                    grid.ShiftRow(index, currentPrefab, direction);
                }
                //Debug.Log(gridCellPos);

                //Debug.Log(currentPrefab);
                //Debug.Log(currentPickedPrefab);
                chosenPrefab = PickPrefab();
                currentPrefab = Instantiate(PickPrefab());
                //currentPickedPrefab = Instantiate(HoverPrefab(prefabIndex));
                //currentPickedPrefab.transform.position = snapPos;
                currentPrefab.transform.position = snapPos;
            }

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

    //Picks a random tile in prefabs array
    private PassageTile PickPrefab(){
        //prefabIndex = Random.Range(0, prefabs.Length);
        //PassageTile randomPrefab = prefabs[prefabIndex];
        //return randomPrefab;
        return prefabs[Random.Range(0, prefabs.Length)];
    }

    //Picks same tile in array HoverPrefabs as the tile to be instanciated
    //private PassageTile HoverPrefab(int prefabI)
    //{
    //    PassageTile pickedTile = pickedPrefabs[prefabI];
    //    return pickedTile;
    //}

    

    private IEnumerator SpinAnimation(PassageTile prefab)
    {
        isRotating = true;
        float duration = .1f;
        float elapsedTime = 0.0f;
        Quaternion startRotation = prefab.transform.rotation;
        Quaternion endRotation = startRotation * Quaternion.Euler(0, 90, 0);

        while (elapsedTime < duration)
        {
            prefab.transform.rotation = Quaternion.Slerp(startRotation, endRotation, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        prefab.transform.rotation = endRotation;
        isRotating = false;
    }
}