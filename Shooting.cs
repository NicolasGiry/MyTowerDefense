using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shooting : MonoBehaviour
{
    public GameObject projectilePrefab;
    bool isShooting;
    public float force = 1f;
    public AudioSource shootSound;
    private void OnTriggerStay2D (Collider2D collision)
    {
        if (collision.CompareTag("Monster") && !isShooting)
        {
            StartCoroutine(Shoot(collision.gameObject));
        }
    }

    IEnumerator Shoot(GameObject target)
    {
        shootSound.Play();
        isShooting = true;
        float angle = Vector3.Angle(transform.position, target.transform.position);

        var projectile = (GameObject)Instantiate(projectilePrefab, transform.position, Quaternion.identity);
        if (projectile == null)
        {
            Debug.Log("null");
        }
        else
        {
            Vector2 shootDir = (target.transform.position - transform.position).normalized;
            projectile.transform.GetChild(0).GetComponent<Projectile>().Setup(shootDir);

            Rigidbody2D rb = projectile.transform.GetChild(0).GetComponent<Rigidbody2D>();
            rb.AddForce(shootDir * force, ForceMode2D.Impulse);
            Destroy(projectile, 2f);
            yield return new WaitForSeconds(0.7f);
        }
        isShooting = false;
    }
}
