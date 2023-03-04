using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PassageTile : MonoBehaviour
{
    public List<bool> sockets = new List<bool>(4);
    public int rotation;
    public bool mobile = true;

    public bool lit = true;

    public void EnableLights(bool value){
        var lights = GetComponentsInChildren<Light>(true);
        foreach (var light in lights){
            light.enabled = value;
        }
        lit = value;
    }
}
