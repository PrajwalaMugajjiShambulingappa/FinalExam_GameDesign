using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyX : MonoBehaviour
{
    public float speed;
    private Rigidbody enemyRb;
    private GameObject playerGoal;

    // Start is called before the first frame update
    void Start()
    {
        enemyRb = GetComponent<Rigidbody>();
        playerGoal = GameObject.Find("Player Goal");
        if (playerGoal == null)
        {
            Debug.LogError("Player Goal object not found. Please ensure it is named correctly and active in the scene.");
            this.enabled = false; // Disable this script to prevent further errors.
            return;
        }
    }


    // Update is called once per frame
    void Update()
    {
        if (playerGoal != null)
        {
            Vector3 lookDirection = (playerGoal.transform.position - transform.position).normalized;
            enemyRb.AddForce(lookDirection * speed);  // Removed Time.deltaTime

            // Debugging output
            Debug.Log("Applying force: " + lookDirection * speed);
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        // If enemy collides with either goal, destroy it
        if (other.gameObject.name == "Enemy Goal" || other.gameObject.name == "Player Goal")
        {
            Destroy(gameObject);
        }
    }

}
