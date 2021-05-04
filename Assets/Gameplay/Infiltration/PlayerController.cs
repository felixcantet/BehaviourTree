using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float movementSpeed = 5.0f;
    public Vector2 moveDirection;
    public Vector3 lookAt;
    public bool clic;
    public CharacterController controller;
    public void Update()
    {
        this.PollInput();
        Rotate();
        Move();
        if (this.clic)
        {
            this.Attack();
        }
    }


    public void Move()
    {
        this.controller.Move(Vector2ToVector3(this.moveDirection) * movementSpeed * Time.deltaTime);
    }

    public void Rotate()
    {
        this.lookAt = new Vector3(lookAt.x, this.transform.position.y, lookAt.z);
        var forward = this.lookAt - this.transform.position;
        this.transform.rotation = Quaternion.LookRotation(forward.normalized, Vector3.up);
    }

    public void Attack()
    {
        // Implement Attack
    }


    public void PollInput()
    {
        this.moveDirection = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        this.moveDirection = this.moveDirection.normalized;

        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if(Physics.Raycast(ray, out hit)) 
        {
            this.lookAt = hit.point;
        }

        clic = Input.GetKeyDown(KeyCode.Mouse0);
    }

    public Vector3 Vector2ToVector3(Vector2 input)
    {
        return new Vector3(input.x, 0, input.y);
    }
}
