using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementController : MonoBehaviour
{
    public GameManager gameManager;
    public GameObject currentDot;

    public float speed = 3f;
    public string direction = "";
    public string lastMovingDirection = "";
    public bool canTeleport = true;
    public bool isGhost = false;


    // Start is called before the first frame update
    void Awake()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
    }

    // Update is called once per frame
    void Update()
    {
        DotController currentDotController = currentDot.GetComponent<DotController>();

        transform.position = Vector2.MoveTowards(transform.position, currentDot.transform.position, speed * Time.deltaTime);

        // logic tu teleport de chaque coter du labyrinthe

        if (currentDotController.isLeftTeleportDot && canTeleport)
        {
            currentDot = gameManager.rightTeleport;
            direction = "left";
            lastMovingDirection = "left";
            transform.position = currentDot.transform.position;
            canTeleport = false;
        }
        else if (currentDotController.isRightTeleportDot && canTeleport)
        {
            currentDot = gameManager.leftTeleport;
            direction = "right";
            lastMovingDirection = "right";
            transform.position = currentDot.transform.position;
            canTeleport = false;
        }
        else
        {
            if (transform.position == currentDot.transform.position )
            {
                // call la methode qui controle le ghost
                if (isGhost)
                {
                    GetComponent<GhostController>().ReachedDestination(currentDotController);
                }

                GameObject dot = currentDotController.GetDotFromDirection(direction);

                if (dot != null)
                {
                    currentDot = dot;
                    lastMovingDirection = direction;
                }
                else
                {
                    direction = lastMovingDirection;
                    dot = currentDotController.GetDotFromDirection(direction);
                    if (dot != null)
                    {
                        currentDot = dot;
                    }
                }
            }
            else
            {
                if (!isGhost)
                {
                    canTeleport = true;
                }
            }
        }
    }

    public void SetDirection(string newDirection)
    {
        direction = newDirection;
    }
}
