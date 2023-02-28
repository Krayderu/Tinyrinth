using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomGrid : MonoBehaviour
{
    public PassageTile[,] cells; //An array of the coordinates of cells in the CustomGrid
    public PassageTile getTile(int x,int y)
    {
        if (cells[x, y] is PassageTile)
        {
            return cells[x,y];
        }
        else
        {
            return null;
        }
    }

    public bool IsTileConnected(Vector3Int cellpos, PassageTile data, int rotation)
    {
        // check that the tile can connect to another tile
        bool canConnect = false;

        //liste des cases voisines
        Vector3Int[] neighbors = new Vector3Int[]
        {
            new Vector3Int(0,0,1), //Up
            new Vector3Int(1,0,0), //Left
            new Vector3Int(0,0,-1), //Down
            new Vector3Int(-1,0,0), //Right
        };
        // liste des connections
        int[] connectionIndices = new int[]
        {
            2,//Up -> Down
            1,//Left -> Right
            0,//Down -> Up
            3,//Right -> Left
        };

        for (int i = 0; i < neighbors.Length; i++)
        {
            Vector3Int neighborPos = cellpos + neighbors[i];
            int neighborX = neighborPos.x;
            int neighborY = neighborPos.z;
            PassageTile neighborTile = getTile(neighborX,neighborY);

            if (neighborTile == null)
            {
                continue;
            }

            bool[] neighborConnections = neighborTile.sockets.ToArray();

            int neighborIndex = (i + neighborTile.rotation) % 4;
            int selfIndex = (i + rotation) % 4;
            if(neighborConnections[connectionIndices[neighborIndex]] && data.sockets[selfIndex])
            {
                canConnect = true;
                break;
            }
        }
        if (!canConnect) return false;

        return true;
    }
}

