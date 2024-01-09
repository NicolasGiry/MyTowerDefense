using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameplayManager : MonoBehaviour
{
    private int pathCost = 20;
    public int gold;
    public Text goldDisplay;
    public float maxHealth;
    public float health;
    public Text healthDisplay;

    public GameObject monster;
    public Vector3 startingPosMonster;

    private Building buildingToPlace;

    public GameObject grille;

    public CustomCursor customCursor;

    public int maxTiles;
    public Tile[] tiles;
    public Vector3[] path;
    public int i = 0;
    public int pathSize = 0;

    public bool isCreatingPath;
    public Button createPathButton;
    public Button playButton;
    public PathBegin pathBegin;
    public PathEnd pathEnd;

    public AudioSource audioSource;
    public AudioClip hammer;

    public Button goToVillage;
    public Button goToDefense;
    public bool isInVillage;
    public Animator cameraAnimator;
    public Animator buildingMenuAnimator;
    public Button[] defenseButtons;
    public Button[] villageButtons;

    public Animator finVague;

    private GameObject[] buildings;
    public bool rangeEnabled;

    public bool isPlaying;

    private int wavesCounter = 0;
    public int nbMonstres = 0;
    private bool endSummon;

    public GameObject lootTextPf;

    private void Start()
    {
        path = new Vector3[maxTiles];
        health = maxHealth;
    }

    void Update()
    {
        goldDisplay.text = gold.ToString();
        healthDisplay.text = health.ToString();
        if (Input.GetMouseButtonDown(0) && buildingToPlace!=null)
        {
            Tile nearestTile = null;
            float shortestDistance = float.MaxValue;
            foreach(Tile tile in  tiles)
            {
                float distance = Vector2.Distance(tile.transform.position, Camera.main.ScreenToWorldPoint(Input.mousePosition));
                if (distance < shortestDistance) { 
                    shortestDistance = distance;
                    nearestTile = tile;
                }
            }

            if (!nearestTile.isOccupied && !nearestTile.isPath && !nearestTile.isTooFar)
            {
                audioSource.clip = hammer;
                audioSource.Play();
                gold -= buildingToPlace.cost;
                GameObject l = Instantiate(lootTextPf, Camera.main.ScreenToWorldPoint(Input.mousePosition) + new Vector3(0, 5, 1), Quaternion.identity);
                l.gameObject.GetComponent<TMP_Text>().text = "-" + buildingToPlace.cost;
                Destroy(l, 0.5f);
                nearestTile.building = Instantiate(buildingToPlace.transform.parent, nearestTile.transform.position, Quaternion.identity).gameObject;
                buildings = GameObject.FindGameObjectsWithTag("Building");
                foreach (GameObject b in buildings)
                {
                    b.transform.GetChild(1).GetComponent<SpriteRenderer>().enabled = rangeEnabled;
                }
                buildingToPlace = null;
                nearestTile.isOccupied = true;
                customCursor.gameObject.SetActive(false);
                Cursor.visible = true;
            }
            else
            {
                Debug.Log("The tile is occupied");
            }
        }

        if (Input.GetMouseButtonDown(1))
        {
            buildingToPlace = null;
            customCursor.gameObject.SetActive(false);
            Cursor.visible = true;

        }

        if (isCreatingPath)
        {
            if(pathBegin.begin && !pathEnd.end)
            {
                foreach (Tile tile in tiles)
                {
                    if (tile.isHover && !tile.isPath)
                    {
                        if (gold>pathCost)
                        {
                            GameObject l = Instantiate(lootTextPf, Camera.main.ScreenToWorldPoint(Input.mousePosition) + new Vector3(0,0,1), Quaternion.identity);
                            l.gameObject.GetComponent<TMP_Text>().text = "-" + pathCost;
                            Destroy(l, 0.5f);
                            tile.isPath = true;
                            tile.indicePath = i;
                            gold -= pathCost;
                            path[i] = tile.transform.position;
                            i++;
                        } else
                        {
                            pathBegin.begin = false;
                            isCreatingPath = false;
                            CreatePath();
                        }
                        
                    }
                }
                pathSize = i;
            }
        }
        if (finVague.GetBool("Fin"))
            finVague.SetBool("Fin", false);

        if (isPlaying && endSummon && nbMonstres == 0)
        {
            isPlaying = false;
            endSummon = false;
            playButton.enabled = true;
            finVague.SetBool("Fin", true);
        }
    }

    public void BuyBuilding(GameObject b)
    {
        Building building = b.transform.GetChild(0).GetComponent<Building>();
        if (gold >= building.cost)
        {
            customCursor.gameObject.SetActive(true);
            customCursor.GetComponent<SpriteRenderer>().sprite = building.GetComponent<SpriteRenderer>().sprite;
            Cursor.visible = false;
            buildingToPlace = building;
            grille.SetActive(true);
            Debug.Log("Press right-click to cancel, left-click to place the building");
        } else
        {
            Debug.Log("Not enough gold");
        }
    }

    public void CreatePath()
    {
        isCreatingPath = true;
        for (int i=0; i<path.Length; i++)
        {
            path[i] = Vector3.zero;
        }
        foreach (Tile tile in tiles)
        {
            if (tile.isPath)
            {
                gold += pathCost;
                tile.isPath = false;
            }
            if (tile.isOccupied)
            {
                gold += 50;
                tile.isOccupied = false;
                Destroy(tile.building);
            }

            tile.left = false;
            tile.right = false;
            tile.up = false;
            tile.down = false;
        }
        i = 0;
        pathBegin.begin = false;
        pathEnd.end = false;
        //createPathButton.gameObject.SetActive(false);
        pathEnd.gameObject.GetComponent<SpriteRenderer>().enabled = true;
        pathEnd.gameObject.GetComponent<CircleCollider2D>().enabled = true;
        pathBegin.gameObject.SetActive(true);
    }

    public void Play()
    {
        playButton.enabled = false;
        isPlaying = true;
        wavesCounter++;
        StartCoroutine(Wave());
        
    }

    IEnumerator Wave ()
    {
        int nb = (wavesCounter-1)*5 + 10;
        for (int i=0; i<nb; i++)
        {
            Instantiate(monster, startingPosMonster, Quaternion.identity);
            nbMonstres++;
            yield return new WaitForSeconds(Random.Range(0.5f, 7f-(i%7)));
            while (isCreatingPath)
            {
                yield return null;
            }
        }
        endSummon = true;
    }

    public void GetDamage(float damage)
    {
        health -= damage;
        if (health<0)
        {
            Application.Quit();
        }
    }

    public void GoToVillage()
    {
        StartCoroutine(GoToVillageRoutine());
    }
    public void GoToDefense()
    {
        StartCoroutine(GoToDefenseRoutine());
    }
    public IEnumerator GoToVillageRoutine()
    {
        goToVillage.gameObject.SetActive(false);
        buildingMenuAnimator.SetBool("Village", true);
        cameraAnimator.SetBool("Village", true);
        yield return new WaitForSeconds(.5f);
        isInVillage = true;
        ChangeMenuButtons();
        yield return new WaitForSeconds(.5f);
        goToDefense.gameObject.SetActive(true);
    }

    public IEnumerator GoToDefenseRoutine()
    {
        goToDefense.gameObject.SetActive(false);
        buildingMenuAnimator.SetBool("Village", false);
        cameraAnimator.SetBool("Village", false);
        yield return new WaitForSeconds(.5f);
        isInVillage = false;
        ChangeMenuButtons();
        yield return new WaitForSeconds(.5f);
        goToVillage.gameObject.SetActive(true);
    }

    private void ChangeMenuButtons()
    {
        if (isInVillage)
        {
            foreach(Button button in villageButtons)
            {
                button.gameObject.SetActive(true);
            }
            foreach (Button button in defenseButtons)
            {
                button.gameObject.SetActive(false);
            }
        } else
        {
            foreach (Button button in villageButtons)
            {
                button.gameObject.SetActive(false);
            }
            foreach (Button button in defenseButtons)
            {
                button.gameObject.SetActive(true);
            }
        }
    }

    public void EnableRange()
    {
        buildings = GameObject.FindGameObjectsWithTag("Building");
        rangeEnabled = !rangeEnabled;
        foreach (GameObject b in buildings)
        {
            b.transform.GetChild(1).GetComponent<SpriteRenderer>().enabled = rangeEnabled;
        }
    }
}
