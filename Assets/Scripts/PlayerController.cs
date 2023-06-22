using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    MovementController movementController;

    public SpriteRenderer sprite;
    public Animator animator;
    // Start is called before the first frame update
    void Awake()
    {
        movementController = GetComponent<MovementController>();
        sprite = GetComponentInChildren<SpriteRenderer>();
        animator = GetComponentInChildren<Animator>();
    }

    // Update is called once per frame
    void Update() // control de pacman avec les fleches du clavier
    {
        animator.SetBool("isMoving", true);

        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            movementController.SetDirection("left");
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            movementController.SetDirection("right");
        }
        else if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            movementController.SetDirection("up");
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            movementController.SetDirection("down");
        }

        //logic animation de pacman

        bool flipHorizontal = false;
        bool flipVertical = false;

        if (movementController.lastMovingDirection == "left")
        {
            animator.SetInteger("direction", 0);
        }
        else if (movementController.lastMovingDirection == "right")
        {
            animator.SetInteger("direction", 0);
            flipHorizontal = true;
        }
        else if (movementController.lastMovingDirection == "up")
        {
            animator.SetInteger("direction", 1);
        }
        else if (movementController.lastMovingDirection == "down")
        {
            animator.SetInteger("direction", 1);
            flipVertical = true;
        }
        sprite.flipX = flipHorizontal;
        sprite.flipY = flipVertical;
    }

}
