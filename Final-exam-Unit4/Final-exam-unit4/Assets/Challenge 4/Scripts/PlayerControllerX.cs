using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControllerX : MonoBehaviour
{
    private Rigidbody playerRb;
    private float speed = 500;
    private GameObject focalPoint;

    public bool hasPowerup;
    public GameObject powerupIndicator;
    public int powerUpDuration = 5;

    private float normalStrength = 10; // how hard to hit enemy without powerup
    private float powerupStrength = 25; // how hard to hit enemy with powerup
    private bool isBoosting = false;
    private float boostCooldown = 2.0f; // Cooldown in seconds before the next boost
    private float lastBoostTime = -10.0f; // Initialize with a value to ensure the boost can be used immediately
    public ParticleSystem speedBoostEffect; // Reference to the particle system for the speed boost

    void Start()
    {
        playerRb = GetComponent<Rigidbody>();
        focalPoint = GameObject.Find("Focal Point");
    }

    void Update()
    {
        // Add force to player in direction of the focal point (and camera)
        float verticalInput = Input.GetAxis("Vertical");
        playerRb.AddForce(focalPoint.transform.forward * verticalInput * speed * Time.deltaTime); 

        // Check for spacebar press to activate the speed boost
        if (Input.GetKeyDown(KeyCode.Space) && !isBoosting && Time.time - lastBoostTime > boostCooldown)
        {
            StartCoroutine(SpeedBoost());
        }

        // Set powerup indicator position to beneath the player
        powerupIndicator.transform.position = transform.position + new Vector3(0, -0.6f, 0);
    }

    IEnumerator SpeedBoost()
    {
        isBoosting = true;
        lastBoostTime = Time.time;

        // Reduce the speed boost force or adjust it as needed
        float boostMultiplier = 2.0f; // Adjust this multiplier to control the boost strength
        playerRb.AddForce(focalPoint.transform.forward * speed * boostMultiplier, ForceMode.Impulse);

        speedBoostEffect.Play();

        // Boost duration
        yield return new WaitForSeconds(1.0f); // Adjust the boost duration as necessary
        
        isBoosting = false;
    }


    // If Player collides with powerup, activate powerup
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Powerup"))
        {
            Destroy(other.gameObject);
            hasPowerup = true;
            powerupIndicator.SetActive(true);
            StartCoroutine(PowerupCooldown());  // Start the coroutine here
        }
    }


    // Coroutine to count down powerup duration
    IEnumerator PowerupCooldown()
    {
        yield return new WaitForSeconds(powerUpDuration);
        hasPowerup = false;
        powerupIndicator.SetActive(false);
    }

    // If Player collides with enemy
    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Enemy"))
        {
            Rigidbody enemyRigidbody = other.gameObject.GetComponent<Rigidbody>();
            Vector3 awayFromPlayer = other.gameObject.transform.position - transform.position;

            if (hasPowerup) // if have powerup hit enemy with powerup force
            {
                enemyRigidbody.AddForce(awayFromPlayer.normalized * powerupStrength, ForceMode.Impulse);
            }
            else // if no powerup, hit enemy with normal strength 
            {
                enemyRigidbody.AddForce(awayFromPlayer.normalized * normalStrength, ForceMode.Impulse);
            }
        }
    }

}
