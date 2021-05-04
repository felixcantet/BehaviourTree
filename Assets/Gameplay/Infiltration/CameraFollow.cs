using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{ 
    [SerializeField] public Transform target;
    public Vector3 offset;

    private void Awake()
    {
        this.offset = this.transform.position - target.transform.position;
    }

    public void LateUpdate()
    {
        this.transform.position = target.transform.position + offset;
    }

}
