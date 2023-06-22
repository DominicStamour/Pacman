using System.Collections;
using System.Collections.Generic;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public GameObject leftTeleport;
    public GameObject rightTeleport;
    public GameObject pacman;
    public GameObject redGhost;
    public GameObject blueGhost;
    public GameObject pinkGhost;
    public GameObject orangeGhost;
    private bool isGameOver = false;

    public enum GhostBehavior
    {
        chase,
        runAway // to do
    }
    public GhostBehavior behavior;

    // Start is called before the first frame update
    void Awake()
    {
        pacman = GameObject.Find("Player");
        redGhost = GameObject.Find("Red");
        blueGhost = GameObject.Find("Blue");
        behavior = GhostBehavior.chase;

        if (isGameOver)
        {
            //todo
        }
    }

    public void PacmanCaughtByGhost()
    {
        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.buildIndex);
    }
}
