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

    private CharacterController controller;
    [SerializeField] private float movementSpeed = 0.05f;
    


    private void Start()
    {
        //Finding grid and setting the playerPos in the Grid
        grid = FindObjectOfType<CustomGrid>();
        PassageTile startingTile = grid.cells[0,0];
        currentTile = startingTile;

        //Get the prefabs array from the GridGenerator script, pick a random prefab, and instantiate it
        GridGenerator generator = FindObjectOfType<GridGenerator>();
        prefabs = generator.prefabs;

        // cursor tile
        chosenPrefab = PickPrefab();
        currentPrefab = Instantiate(chosenPrefab);

        //Instantiate HoverPrefab
        //currentPickedPrefab = Instantiate(HoverPrefab(prefabIndex));
        
        controller = GetComponent<CharacterController>();
        
    }
    private void Update()
    {
        Vector3 movementDirection = Vector3.zero;
        var mousePos = GetMouseWorldPosition();
        Vector3 snapPos = grid.SnapToGrid(mousePos);
        
        #region PlayerMovement
        if (Input.GetKey(KeyCode.W))
        {
            movementDirection += new Vector3(1, 0, 0);
        }
        if (Input.GetKey(KeyCode.S))
        {
            movementDirection += new Vector3(-1, 0, 0);
        }
        if (Input.GetKey(KeyCode.A))
        {
            movementDirection += new Vector3(0, 0, 1);
        }
        if (Input.GetKey(KeyCode.D))
        {
            movementDirection += new Vector3(0, 0, -1);
        }
        movementDirection.Normalize();
    
        // move character
        if (!grid.isMoving){
            controller.Move(movementDirection * movementSpeed * Time.deltaTime);
            // rotate in direction of movement
            if (movementDirection != Vector3.zero){
                //transform.rotation = Quaternion.LookRotation(movementDirection);
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation (movementDirection), Time.deltaTime * 10f);
            }
        }
        #endregion

        #region GridInteractions
        // TODO: if mousePos is inside the grid, hide the cursor
        bool isInBounds = !grid.IsWithinBounds(grid.GetGridCellPosition(mousePos));
        currentPrefab.gameObject.SetActive(isInBounds);
        
        // TODO: show a different version of the cursor when can't build
        currentPrefab.transform.position = snapPos;
        //currentPickedPrefab.transform.position = snapPos;

        // ROTATE PIECE
        if (Input.GetKeyUp(KeyCode.R) && !isRotating){
            // rotate visually
            StartCoroutine(SpinAnimation(currentPrefab));
            //StartCoroutine(SpinAnimation(currentPickedPrefab));
            //chosenPrefab.rotation = (chosenPrefab.rotation + 1) % 4;

            // rotate data
            currentPrefab.rotation = (currentPrefab.rotation + 1) % 4;
            //currentPickedPrefab.rotation = (currentPickedPrefab.rotation + 1) % 4;
            //Debug.Log(chosenPrefab.rotation);
            //Debug.Log(currentPickedPrefab.rotation);
        }

        // PLACE PIECE
        if (Input.GetMouseButtonUp(0)){
            //changer de tile pour current prefab
            //Debug.Log(grid.SnapToGrid(click));

            Vector3Int gridCellPos = grid.GetGridCellPosition(snapPos);

            // Verify we can build and the grid is not moving
            if (!grid.IsPlaceable(gridCellPos) || grid.isMoving) return;


            // Find shift direction and shift row/column according to insertion place
            Utils.Direction direction;

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

            // Here, we place the currentPrefab by stopping to move it:
            // we choose a new prefab to replace the currentPrefab.
            chosenPrefab = PickPrefab();
            currentPrefab = Instantiate(PickPrefab());
            //currentPickedPrefab = Instantiate(HoverPrefab(prefabIndex));
            //currentPickedPrefab.transform.position = snapPos;
            currentPrefab.transform.position = snapPos;
        }
        #endregion
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