using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    public bool isOccupied;
    public Animator animator;
    public Color occupiedColor;
    public Color vacantColor;
    public Color pathColor;
    public Color farColor;

    public Sprite pathSprite;
    public Sprite pathTurnSprite;
    public Sprite defaultSprite;

    private SpriteRenderer sr;

    public bool isHover;
    public bool isPath;
    public bool isTooFar;

    public GameObject building;

    private Tile[] tiles;
    private float dist = 7f;
    public Tile[] adjTiles = new Tile[8];

    public float hoverScale;

    public int indicePath;

    public bool left;
    public bool right;
    public bool up;
    public bool down;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        tiles = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameplayManager>().tiles;
        FindAdjacentTiles();
        left = false;
        right = false;
        up = false;
        down = false;
}

    void Update()
    {
        if (isOccupied)
        {
            sr.color = occupiedColor;
            gameObject.GetComponent<SpriteRenderer>().sprite = defaultSprite;
        } else
        {
            sr.color = vacantColor;
            gameObject.GetComponent<SpriteRenderer>().sprite = defaultSprite;
        }

        if (isPath)
        {

            sr.color = pathColor;
            isTooFar = false;

            if ( indicePath < GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameplayManager>().pathSize-1 || (!GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameplayManager>().isCreatingPath))
            {
                CalculerPathSprite();
            }
            else
            {
                gameObject.GetComponent<SpriteRenderer>().sprite = defaultSprite;
            }
        }
        if (isTooFar)
        {
            sr.color = farColor;
            gameObject.GetComponent<SpriteRenderer>().sprite = defaultSprite;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Cursor") && !isPath && !isTooFar)
        {
            transform.localScale += new Vector3(hoverScale, hoverScale, hoverScale);
            isHover = true; 
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Cursor") && !isPath && !isTooFar)
        {
            transform.localScale -= new Vector3(hoverScale, hoverScale, hoverScale);
            isHover = false;
        }
    }

    private void OnMouseOver()
    {
        isHover = true;
    }

    private void OnMouseExit()
    {
        isHover = false;
    }

    private void FindAdjacentTiles()
    {
        int i = 0;
        foreach (Tile tile in tiles)
        {
            float d = Vector2.Distance(tile.transform.position, transform.position);
            if ( d <= dist && d != 0)
            {
                // tile est dans les 8 tiles les plus proches
                adjTiles[i] = tile;
                i++;
            }
        }

    }

    private bool IsNextToPath()
    {
        foreach(Tile tile in adjTiles)
        {
            if (tile != null && tile.isPath)
            {
                return true;
            }
        }
        return false;
    }

    public void CheckDistance()
    {
        isTooFar = !(isPath || (IsNextToPath() && !isOccupied));
    }

    public void CalculerPathSprite()
    {

        Vector3[] path = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameplayManager>().path;
        Vector3 posPrec;
        Vector3 posSuiv;

        if (indicePath == 0)
        {
            posPrec = new Vector3(transform.position.x -5, 8, 0);
        } else
        {
            posPrec = path[indicePath-1];
        }

        if (indicePath == GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameplayManager>().pathSize -1)
        {
            posSuiv = new Vector3(transform.position.x + 5, 8, 0);
        } else
        {
            posSuiv = path[indicePath+1];
        }

        (left, right) = SetBoolDistance(posPrec.x, transform.position.x, left, right);
        (down, up) = SetBoolDistance(posPrec.y, transform.position.y, down, up);
        (left, right) = SetBoolDistance(posSuiv.x, transform.position.x, left, right);
        (down, up) = SetBoolDistance(posSuiv.y, transform.position.y, down, up);
        //Debug.Log("left: " + left + " right: " + right + " up: " + up + " down: " + down);

        if (left && down)
        {
            gameObject.GetComponent<SpriteRenderer>().sprite = pathTurnSprite;
            transform.rotation = Quaternion.Euler(0, 0, 90);
        }
        else if (left && up)
        {
            gameObject.GetComponent<SpriteRenderer>().sprite = pathTurnSprite;
            transform.rotation = Quaternion.Euler(0, 0, 0);
        }
        else if (up && down)
        {
            gameObject.GetComponent<SpriteRenderer>().sprite = pathSprite;
            transform.rotation = Quaternion.Euler(0, 0, 0);
        }
        else if (up && right)
        {
            gameObject.GetComponent<SpriteRenderer>().sprite = pathTurnSprite;
            transform.rotation = Quaternion.Euler(0, 0, -90);
        }
        else if (left && right)
        {
            gameObject.GetComponent<SpriteRenderer>().sprite = pathSprite;
            transform.rotation = Quaternion.Euler(0, 0, 90);
        }
        else if (down && right)
        {
            gameObject.GetComponent<SpriteRenderer>().sprite = pathTurnSprite;
            transform.rotation = Quaternion.Euler(0, 0, 180);
        }

    }

    private (bool, bool) SetBoolDistance(float other, float me, bool bool1, bool bool2)
    {
        if (other < (me-0.5f))
        {
            bool1 = true;
        }else if (other > (me+0.5f))
        {
            bool2 = true;
        }

        return (bool1, bool2);
    }
}
