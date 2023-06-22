using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DotController : MonoBehaviour
{
    public bool canMoveLeft = false;
    public bool canMoveRight = false;
    public bool canMoveUp = false;
    public bool canMoveDown = false;
    public bool isLeftTeleportDot = false;
    public bool isRightTeleportDot = false;
    public bool isStartingDot = false;
    public bool isVisible = false;

    public GameObject dotLeft;
    public GameObject dotRight;
    public GameObject dotUp;
    public GameObject dotDown;

    public SpriteRenderer dotSprite;

    void Awake()
    {
        if(transform.childCount > 0) // si le dot a un enfant il est donc un dot a manger pour pacman (ceux qui en ont pas sont pour les teleports gauche et droite)
        {
            isStartingDot = true;
            isVisible = true;
            dotSprite = GetComponentInChildren<SpriteRenderer>();
        }

        CheckAdjacentDots(-Vector2.up, ref canMoveDown, ref dotDown);
        CheckAdjacentDots(Vector2.up, ref canMoveUp, ref dotUp);
        CheckAdjacentDots(-Vector2.right, ref canMoveLeft, ref dotLeft);
        CheckAdjacentDots(Vector2.right, ref canMoveRight, ref dotRight);
    }

    void CheckAdjacentDots(Vector2 direction, ref bool canMove, ref GameObject dot) // définie si on peut se deplacer vers un autre dot en envoyant un rayon dans une direction recu
    {
        RaycastHit2D[] hits = Physics2D.RaycastAll(transform.position, direction);

        for (int i = 0; i < hits.Length; i++) 
        {
            float distance = Mathf.Abs(Vector2.Distance(hits[i].point, transform.position)); // regarde la distance entre 2 dots

            if (distance < 0.5f && hits[i].collider.tag == "Dot") // si la distance est plus petite que 0,5f on considere que pacman peut s y deplacer (canMove = true)
            {
                canMove = true;
                dot = hits[i].collider.gameObject;
                break;
            }
        }
    }
    public GameObject GetDotFromDirection(string direction) // définie quel dot se retrouve dans la direction recu en parametre
    {
        switch (direction)
        {
            case "left" when canMoveLeft:
                return dotLeft;
            case "right" when canMoveRight:
                return dotRight;
            case "down" when canMoveDown:
                return dotDown;
            case "up" when canMoveUp:
                return dotUp;
            default:
                return null;
        }
    }
    private void OnTriggerEnter2D(Collider2D collision) // rend invisible les dot "manger" par pacman
    {
        if(collision.tag == "Player" && isStartingDot)
        {
            isVisible = false;
            dotSprite.enabled = false;
        }
    }

}
