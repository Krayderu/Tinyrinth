using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    private bool isMoving = false;
    private Vector3 lastMovementDirection = Vector3.left;

    // Start is called before the first frame update
    void Start()
    {
        _Start();
    }

    // Update is called once per frame
    void Update()
    {
        // face last movement direction (smooth)
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(lastMovementDirection), Time.deltaTime * 10f);
        _Update();
    }

    void _Start(){
        // children start
    }

    void _Update(){
        // children update
    }

    void OnTurn(){
        // Code to run on each turn
    }

    private IEnumerator Move(Vector3 start, Vector3 end){
        isMoving = true;
        float duration = .2f;
        float t = 0.0f;

        while (t < duration){
            transform.position = Vector3.Lerp(start, end, t / duration);
            t += Time.deltaTime;
            yield return null;
        }

        transform.position = end;
        isMoving = false;
    }
}
