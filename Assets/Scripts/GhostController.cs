using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GhostController : MonoBehaviour
{
    public enum GhostStatusEnum // enum pour aider a sortir du 'home' les ghosts
    {
        fleeing,
        respawn,
        leftDot,
        rightDot,
        middleLeftDot,
        middleRightDot,
        Moving,
        gate,
        start
    }
    public enum GhostColorEnum // enum pour différencier les ghosts
    {
        red,
        blue,
        pink,
        orange
    }

    public GhostStatusEnum status;
    public GhostColorEnum color;

    public GameObject ghostDotLeft;
    public GameObject ghostDotRight;
    public GameObject ghostDotMiddleLeft;
    public GameObject ghostDotMiddleRight;
    public GameObject ghostDotStart;
    public GameObject startingDot;
    public GameObject ghostDotGate;

    public MovementController movementController;

    public GameManager GameManager { get; private set; }

    public bool readyToLeaveHome = false;
    public bool goingToTheGate = false;

    void Awake() // initialise la position des ghost et quand vont-il sortir de 'home'
    {
        movementController = GetComponent<MovementController>();
        GameManager = GameObject.Find("GameManager").GetComponent<GameManager>();

        switch (color)
        {
            case GhostColorEnum.red:
                status = GhostStatusEnum.leftDot;
                startingDot = ghostDotLeft;
                readyToLeaveHome = true;
                break;

            case GhostColorEnum.pink:
                status = GhostStatusEnum.middleLeftDot;
                startingDot = ghostDotMiddleLeft;
                StartCoroutine(ActivateReadyToLeaveHomeAfterDelay(5f));
                break;

            case GhostColorEnum.blue:
                status = GhostStatusEnum.middleRightDot;
                startingDot = ghostDotMiddleRight;
                StartCoroutine(ActivateReadyToLeaveHomeAfterDelay(10f));

                break;

            case GhostColorEnum.orange:
                status = GhostStatusEnum.rightDot;
                startingDot = ghostDotRight;
                StartCoroutine(ActivateReadyToLeaveHomeAfterDelay(15f));
                break;
        }

        movementController.currentDot = startingDot;
        transform.position = startingDot.transform.position;
    }

    private IEnumerator ActivateReadyToLeaveHomeAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        readyToLeaveHome = true;
    }

    public void ReachedDestination(DotController dotController) // active l'ia de chaque ghost quand il devient moving
    {
        if (status == GhostStatusEnum.Moving)
        {
            switch (color)
            {
                case GhostColorEnum.red:
                    RedGhostAi();
                    break;

                case GhostColorEnum.blue:
                    BlueGhostAi();
                    break;

                case GhostColorEnum.pink:
                    PinkGhostAi();
                    break;

                case GhostColorEnum.orange:
                    OrangeGhostAi();
                    break;
            }
        }

        else if (status == GhostStatusEnum.respawn)
        {
            // to do
        }
        else
        {
            if (readyToLeaveHome) // deplace hors de home les ghosts
            {
                if (status == GhostStatusEnum.leftDot || status == GhostStatusEnum.rightDot)
                {
                    status = status == GhostStatusEnum.leftDot ? GhostStatusEnum.middleLeftDot : GhostStatusEnum.middleRightDot;
                    movementController.SetDirection(status == GhostStatusEnum.middleLeftDot ? "right" : "left");
                    goingToTheGate = true;
                }

                else if (status == GhostStatusEnum.middleLeftDot || status == GhostStatusEnum.middleRightDot)
                {
                    status = GhostStatusEnum.gate;
                    movementController.SetDirection("up");

                }
                else if (status == GhostStatusEnum.gate)
                {
                    status = GhostStatusEnum.start;
                    movementController.SetDirection("up");
                }
                else if (status == GhostStatusEnum.start)
                {
                    status = GhostStatusEnum.Moving;
                    movementController.SetDirection("left");
                }

            }
        }
    }
    string CatchPacman(Vector2 target) // logic pour trouver la localisation de pacman et la définir comme direction
    {
        float minimumDistance = float.PositiveInfinity;
        string lastMovingDirection = movementController.lastMovingDirection;
        string newDirection = "";
        DotController dotController = movementController.currentDot.GetComponent<DotController>();

        void UpdateDirectionIfCloser(GameObject dot, string direction)
        {
            float distance = Vector2.Distance(dot.transform.position, target);
            if (distance < minimumDistance)
            {
                minimumDistance = distance;
                newDirection = direction;
            }
        }

        if (dotController.canMoveUp && lastMovingDirection != "down")
        {
            UpdateDirectionIfCloser(dotController.dotUp, "up");
        }

        if (dotController.canMoveDown && lastMovingDirection != "up")
        {
            UpdateDirectionIfCloser(dotController.dotDown, "down");
        }

        if (dotController.canMoveLeft && lastMovingDirection != "right")
        {
            GameObject dotLeft = dotController.dotLeft;
            UpdateDirectionIfCloser(dotLeft, dotLeft.GetComponent<DotController>().isLeftTeleportDot ? "right" : "left");
        }

        if (dotController.canMoveRight && lastMovingDirection != "left")
        {
            GameObject dotRight = dotController.dotRight;
            UpdateDirectionIfCloser(dotRight, dotRight.GetComponent<DotController>().isRightTeleportDot ? "left" : "right");
        }

        return newDirection;
    }

    void RedGhostAi() // tente d attraper pacman en tous temps en se dirigeant vers sa position
    {
        string direction = CatchPacman(GameManager.pacman.transform.position);
        movementController.SetDirection(direction);
    }
    void BlueGhostAi() // essaye d etre entre pacman et red
    {
        string pacmanDirection = GameManager.pacman.GetComponent<MovementController>().lastMovingDirection;
        float distancePacmanDestination = 0.4f;

        Vector2 pacman = GameManager.pacman.transform.position;

        if (pacmanDirection == "left")
        {
            pacman.x -= distancePacmanDestination * 2;
        }
        else if (pacmanDirection == "right")
        {
            pacman.x += distancePacmanDestination * 2;
        }
        else if (pacmanDirection == "up")
        {
            pacman.y += distancePacmanDestination * 2;
        }
        else if (pacmanDirection == "down")
        {
            pacman.y -= distancePacmanDestination * 2;
        }

        GameObject redGhost = GameManager.redGhost;

        float horizontalDistance = pacman.x - redGhost.transform.position.x;
        float verticalDistance = pacman.y - redGhost.transform.position.y;

        Vector2 blueGhostTarget = new(pacman.x + horizontalDistance, pacman.y + verticalDistance);

        string direction = CatchPacman(blueGhostTarget);

        movementController.SetDirection(direction);
    }
    void PinkGhostAi() //tente d attrapper pac man en tout temps mais focus sur ou il va aller dans le futur
    {
        string pacmanDirection = GameManager.pacman.GetComponent<MovementController>().lastMovingDirection;
        float distancePacmanDestination = 0.4f;

        Vector2 target = GameManager.pacman.transform.position;

        if (pacmanDirection == "left")
        {
            target.x -= distancePacmanDestination * 2;
        }
        else if (pacmanDirection == "right")
        {
            target.x += distancePacmanDestination * 2;
        }
        else if (pacmanDirection == "up")
        {
            target.y += distancePacmanDestination * 2;
        }
        else if (pacmanDirection == "down")
        {
            target.y -= distancePacmanDestination * 2;
        }

        string direction = CatchPacman(target);
        movementController.SetDirection(direction);
    }
    void OrangeGhostAi()// si orange est trop loin de pacman il va le chasser sinon il bouge de facon aléatoire
    {
        float distance = 10f;
        string lastMovingDirection = movementController.lastMovingDirection;

        if (Vector2.Distance(transform.position, GameManager.pacman.transform.position) >= distance)
        {
            string direction = CatchPacman(GameManager.pacman.transform.position);
            movementController.SetDirection(direction);
        }
        else
        {
            string[] directions = { "up", "down", "left", "right" };

            if (lastMovingDirection == "up")
                directions = directions.Where(d => d != "down").ToArray();
            else if (lastMovingDirection == "down")
                directions = directions.Where(d => d != "up").ToArray();
            else if (lastMovingDirection == "left")
                directions = directions.Where(d => d != "right").ToArray();
            else if (lastMovingDirection == "right")
                directions = directions.Where(d => d != "left").ToArray();

            if (directions.Length > 0)
            {
                int randomIndex = Random.Range(0, directions.Length);
                string randomDirection = directions[randomIndex];

                // Inverser la direction si le point est un isLeftTeleportDot ou isRightTeleportDot
                if (randomDirection == "left" && movementController.currentDot.GetComponent<DotController>().isLeftTeleportDot)
                    randomDirection = "right";
                else if (randomDirection == "right" && movementController.currentDot.GetComponent<DotController>().isRightTeleportDot)
                    randomDirection = "left";

                movementController.SetDirection(randomDirection);
            }
        }
    }
    private void OnTriggerEnter2D(Collider2D collision) // quand un ghost hit pacman
    {
        if (collision.CompareTag("Player"))
        {
            if(status == GhostStatusEnum.fleeing)
            {
                transform.position = startingDot.transform.position;
                status = GhostStatusEnum.respawn;
            }
            if(status == GhostStatusEnum.Moving)
            {
                GameManager.PacmanCaughtByGhost();
            }
        }
    }
}
