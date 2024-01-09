using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PathEnd : MonoBehaviour
{
    public bool end;
    public Button repathButton;
    public Button playButton;
    public Button buildingButton;
    public PathBegin pathBegin;
    public Text delay;
    public GameplayManager gameManager;

    private void Start()
    {
        gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameplayManager>();
    }

    private void Update()
    {
        if (Vector2.Distance(transform.position, Camera.main.ScreenToWorldPoint(Input.mousePosition))<1.5f && pathBegin.begin)
        {
            Cursor.lockState = CursorLockMode.None;
            end = true;
            gameObject.GetComponent<SpriteRenderer>().enabled = false;
            pathBegin.begin = false;
            repathButton.gameObject.SetActive(true);
            playButton.gameObject.SetActive(true);
            buildingButton.interactable = true;
            Tile[] t = gameManager.tiles;
            foreach (Tile tile in t)
            {
                tile.CheckDistance();
            }
            StartCoroutine(repathDelay());
            gameManager.pathSize = gameManager.i;
        }
    }

    IEnumerator repathDelay()
    {
        if (gameManager.isPlaying)
        {
            delay.text = "3";
            yield return new WaitForSeconds(1f);
            delay.text = "2";
            yield return new WaitForSeconds(1f);
            delay.text = "1";
            yield return new WaitForSeconds(1f);
            yield return new WaitForSeconds(1f);
            delay.text = "";
            yield return new WaitForSeconds(1f);
        }
        gameManager.isCreatingPath = false;
    }
}


