using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireBall : MonoBehaviour
{
    public Vector3 direction = Vector3.forward;
    public float speed = 20.0f;
    
    // Update is called once per frame
    void Update()
    {
        this.transform.position += Time.deltaTime * speed * direction;
    }
}
