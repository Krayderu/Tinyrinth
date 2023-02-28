using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CellData", menuName = "Cell Data")]
public class CellData : ScriptableObject
{
    public List<bool> sockets = new List<bool>(4);
}

