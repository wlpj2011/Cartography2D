using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Controller2D : RaycastController
{
   

    public CollisionInfo collisions; // Stores all info about the collisions a gameObject is experiencing
    [HideInInspector]
    public Vector2 playerInput; //Stores player input for the current frame

    public override void Start() {
    /*
    * Start method called once at beginning of gameObject's lifetime
    */
        base.Start(); // Calculates ray spacing for the gameObject
        // Debug.Log("Called Start for Controller2D.cs" ,gameObject);
        collisions.horizontalFaceDir = 1; //Sets facing direction to right
    }

    public void Move(Vector2 moveAmount) {
        Move(moveAmount, Vector2.zero); //Calls move with no input
    }

    public void Move(Vector2 moveAmount, Vector2 input) {
    /*
    * Moves the gameObject by moveAmount after correcting for upcoming collisions
    */
        collisions.Reset(); //Resets to the standard no-collisions state
        collisions.moveAmountOld = moveAmount; //Stores the distance moved previous frame
        playerInput = input; //renames input

        UpdateRaycastOrigins(); 


        if (moveAmount.x != 0) {
            collisions.horizontalFaceDir = (int)Mathf.Sign(moveAmount.x); // If not standing still, sets the facing direction appropriately
            HorizontalCollisions(ref moveAmount); // Checks for collisions in the horizontal direction and appropriately adjust the distance to move }
        }


        if(moveAmount.y != 0){
            collisions.verticalFaceDir = (int)Mathf.Sign(moveAmount.y);
            VerticalCollisions(ref moveAmount); // If moving vertically checks for collisions in the vertical direction and appropriately adjust the distance to move
        }        

        transform.Translate(moveAmount); // Translate the gameObject by the adjusted moveAmount

    }

    void HorizontalCollisions(ref Vector2 moveAmount) {
    /*
    * Takes in a reference to the amount that the gameObject is planning to move and adjusts the moveAmount
    * based on the projected collisions along the horizontal axis
    */
        float directionX = collisions.horizontalFaceDir; // Sets the direction along the x-axis to be the direction the gameObject is facing
        float rayLength = Mathf.Abs(moveAmount.x) + skinWidth; // Sets the length of the ray to be cast to be the distance planned to move + the distance from the ray origin to the gameObject surface

        if(Mathf.Abs(moveAmount.x) < skinWidth) {
            rayLength = 2* skinWidth; // Increases the ray length to 2*skinWidth if it is less that 2*skinWidth
        }

        for ( int i = 0; i < horizontalRayCount; i++){
            Vector2 rayOrigin = (directionX == -1)?raycastOrigins.bottomLeft:raycastOrigins.bottomRight; // Picks the bottom left to cast the first ray from if moving left and bottom right if moving right
            rayOrigin += Vector2.up * (horizontalRaySpacing * i); // Shifts up the raycast origin to space out rays
            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.right * directionX, rayLength, collisionMask); // Cast ray of length rayLength and record if it hit anything in the collision mask
            Debug.DrawRay(rayOrigin, Vector2.right * directionX * rayLength, Color.red); // Draw rays if desired

            if (hit) {
                Debug.Log("Horizontal Collision Detected");

                if(hit.distance == 0) {
                    continue; // If gameObject is intersecting with the colliding object, make no adjustments to moveAmount
                }

                moveAmount.x = (hit.distance - skinWidth) * directionX; // Adjust move distance to account for the skin width
                rayLength = hit.distance; // Make sure the next rays will not check further out than the collision

                collisions.left = directionX == -1; // Set collisions direction
                collisions.right = directionX == 1;
            }
        }
    }

    void VerticalCollisions(ref Vector2 moveAmount) {
    /*
    * Takes in a reference to the amount that the gameObject is planning to move and adjusts the moveAmount
    * based on the projected collisions along the vertical axis
    */
        float directionY = Mathf.Sign(moveAmount.y); //Sets the direction along the y-axis based on the projected movement passed in
        float rayLength = Mathf.Abs(moveAmount.y) + skinWidth; // Increases ray length by skinWidth

        for ( int i = 0; i < verticalRayCount; i++){
            Vector2 rayOrigin = (directionY == -1)?raycastOrigins.bottomLeft:raycastOrigins.topLeft; // Picks the bottom left to cast the first ray from if moving down and top left if moving up
            rayOrigin += Vector2.right * (verticalRaySpacing * i + moveAmount.x); // Shifts over the raycast origin to space out rays
            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.up * directionY, rayLength, collisionMask); // Cast ray of length rayLength and record if it hit anything in the collision mask

            Debug.DrawRay(rayOrigin, Vector2.up * directionY * rayLength, Color.red); // Draws rays if desired

            if (hit) {
                moveAmount.y = (hit.distance - skinWidth) * directionY;
                rayLength = hit.distance;

                collisions.below = directionY == -1;
                collisions.above = directionY == 1;
            }
        }

    }

    

    public struct CollisionInfo {
    /*
    * Structure storing info about the collisions the gameObject is experiencing. 
    * This includes bools telling which sides have collisions
    * How bools telling how the gameObject is interacting with slopes
    * Information about the current and previous slope and movement
    * Facing information
    * Comes with a function to reset everything appropriately each frame
    */
        public bool above, below;
        public bool left, right;
        
        
        public Vector2 moveAmountOld;
        public int horizontalFaceDir;
        public int verticalFaceDir;

        public void Reset() {
            above = below = false;
            left = right = false;
            
        }
    }
}