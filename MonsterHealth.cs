using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MonsterHealth : MonoBehaviour
{
    public float health;
    public float maxHealth=100;
    public GameObject healthBar;
    public int lootMax;
    private int loot;
    public GameObject lootTextPf;

    private void Start()
    {
        loot = Random.Range(0, lootMax);
        health = maxHealth;
    }
    private void Update()
    {

        if (health > 100) {
            health = 100;
        } else if (health <= 0)
        {
            health = 0;
            GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameplayManager>().gold += loot;
            GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameplayManager>().nbMonstres--;
            if (loot>0)
            {
                GameObject l = Instantiate(lootTextPf, gameObject.transform.position, Quaternion.identity);
                l.gameObject.GetComponent<TMP_Text>().text = "+" + loot;
                Destroy(l, 1.5f);
            }
            
            Destroy(gameObject);
        }
        healthBar.transform.localScale = new Vector3 (health * 0.001373626f, 0.78125f, 1f);
    }

    public void GetDamage(float damage)
    {
        health -= damage;
    }
}
