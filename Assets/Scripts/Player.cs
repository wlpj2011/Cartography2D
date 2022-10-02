using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof (Controller2D))]
public class Player : MonoBehaviour
{
    float accelerationTimeGrounded = 0.1f;
    float moveSpeed = 5;

    Vector3 velocity;
    float velocityXSmoothing;
    float velocityYSmoothing;

    Controller2D controller;

    Vector2 directionalInput;
    int wallDirX;

    // Start is called before the first frame update
    void Start() {
        // Debug.Log("Called Start for Player.cs" ,gameObject);
        controller = GetComponent<Controller2D>();
        
    }
    
    // Update is called once per frame
    void Update() {
        CalculateVelocity();
        
        controller.Move(velocity * Time.deltaTime, directionalInput);

        if (controller.collisions.above || controller.collisions.below) {
            velocity.y = 0;
        }
        if (controller.collisions.left || controller.collisions.right) {
            velocity.x = 0;
        }
    }

    public void SetDirectionalInput(Vector2 input) {
        directionalInput = input;
    }


    void CalculateVelocity() {
        Vector2 targetVelocity = directionalInput.normalized * moveSpeed;
        velocity.x = Mathf.SmoothDamp(velocity.x, targetVelocity.x, ref velocityXSmoothing, accelerationTimeGrounded);
        velocity.y = Mathf.SmoothDamp(velocity.y, targetVelocity.y, ref velocityYSmoothing, accelerationTimeGrounded);
        
    }
}
