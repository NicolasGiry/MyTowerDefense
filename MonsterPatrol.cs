using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterPatrol : MonoBehaviour
{
    public Vector3[] path;

    public Rigidbody2D rb;
    public float velocity;
    public float damage;

    private Vector3 nextPoint;
    public int i;
    private int pathSize;

    private GameObject gameManager;
    private bool needPath;

    private float bruitX;
    private float bruitY;


    private void Start()
    {
        gameManager = GameObject.FindGameObjectWithTag("GameManager");
        path = gameManager.GetComponent<GameplayManager>().path;
        pathSize = gameManager.GetComponent<GameplayManager>().pathSize;
        i = pathSize - 1;

        bruitX = Random.Range(-1.5f, 1.5f);
        bruitY = Random.Range(-1.5f, 1.5f);
        nextPoint = path[i] + new Vector3(bruitX, bruitY, 0);
    }

    private void FixedUpdate()
    {
        if (gameManager.GetComponent<GameplayManager>().isCreatingPath)
        {
            needPath = true;
        }
        else
        {
            if (needPath)
            {
                i = FindNearestPath();
                needPath = false;
            }
            else
            {
                if (transform.position == nextPoint)
                {
                    i--;
                    if (i == -1)
                    {
                        GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameplayManager>().GetDamage(damage);
                        GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameplayManager>().nbMonstres--;
                        Destroy(gameObject);
                    } else
                    {
                        
                        nextPoint = path[i] + new Vector3(bruitX, bruitY, 0);
                    }
                    

                }
                else
                {
                    transform.position = Vector2.MoveTowards(transform.position, nextPoint, velocity * Time.fixedDeltaTime);
                }
            }
        }
    }

    private int FindNearestPath()
    {
        int i;
        int nearestPath = 0;
        float shortestDistance = Vector2.Distance(transform.position, path[nearestPath]);
        for (i=0; i<path.Length; i++)
        {
            float distance = Vector2.Distance(transform.position, path[i]);
            if (distance < shortestDistance)
            {
                shortestDistance = distance;
                nearestPath = i;
            }
        }
        nextPoint = path[nearestPath] + new Vector3(bruitX, bruitY, 0);
        return nearestPath;
    }
}
