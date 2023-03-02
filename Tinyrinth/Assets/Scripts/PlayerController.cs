using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private PassageTile currentTile;
    private PassageTile startingTile;

    private void Start()
    {
        //Setting the playerPos in the Grid
        CustomGrid findGrid = FindObjectOfType<CustomGrid>();
        CustomGrid grid = findGrid.GetComponent<CustomGrid>();
        PassageTile startingTile = grid.cells[0,0];
        currentTile = startingTile;
    }
    private void Update()
    {
        GetMouseWorldPosition();
        Debug.Log(GetMouseWorldPosition());
        //Player Movement
        if (Input.GetKey(KeyCode.W))
        {

        }

    }

    public Vector3 GetMouseWorldPosition()
    {
        // Get the position on the tilemap that the mouse is pointing to
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Plane plane = new Plane(Camera.main.transform.forward, transform.position);
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

    void OnMouseUp()
    {
        Vector3 click = GetMouseWorldPosition();
        Debug.Log(click);
        //CustomGrid findGrid = FindObjectOfType<CustomGrid>();
        //CustomGrid grid = findGrid.GetComponent<CustomGrid>();

        //Vector3 mousePos = Input.mousePosition;
        //mousePos.z = Camera.main.nearClipPlane; // Set the z-coordinate to the near clip plane distance
        //Vector3 worldPos = Camera.main.ScreenToWorldPoint(mousePos);

        //Vector3Int proposedPos = grid.GetGridCellPosition(worldPos);
        //PassageTile proposedTile = GetComponent<GridGenerator>().prefabs[Random.Range(0, GetComponent<GridGenerator>().prefabs.Length)];

        //Debug.Log(proposedPos);

        //if (grid.IsPlaceable(proposedPos, proposedTile))
        //{
        //    Debug.Log("une tile est placeable à " + proposedPos);
        //    // Tile is placeable, snap to grid and set cell data
        //    Vector3 snappedPos = grid.GetSnappedPosition(proposedPos);
        //    grid.cells[proposedPos.x, proposedPos.z] = proposedTile;
        //    Instantiate(proposedTile, snappedPos, Quaternion.identity);
        //}
    }
}
