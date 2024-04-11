using System.Collections;
using UnityEngine;

public class TreasureChest : MonoBehaviour
{
    public float launchForce = 200f;
    public Vector2 launchDirection = new Vector2(1, 2);

    // Duration of the scaling animation
    public float scaleDuration = 0.5f;

    // Time in seconds before turning off gravity so chest doesn't fall off screen
    public float gravityOffDelay = 1.5f; 

    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        // Apply a force to launch the chest
        rb.AddForce(launchDirection.normalized * launchForce);

        // Start the coroutine to scale the chest, simulating Z-axis launch
        StartCoroutine(SimulateZAxisLaunch());

        // Start the coroutine to turn off gravity
        StartCoroutine(DisableGravityAndStopAfterDelay(gravityOffDelay));
    }

    // Scales chest to look like its flying out of the ground
    IEnumerator SimulateZAxisLaunch()
    {
        float elapsedTime = 0;

        // Original scale of chest
        Vector3 originalScale = transform.localScale; 

        // Cut original scale in half to simulate its further away save as launchScale
        Vector3 launchScale = originalScale * 0.5f; 

        // Turn chest into smaller launchScale size
        transform.localScale = launchScale; 

        // Runs till scaling duration ends
        while (elapsedTime < scaleDuration)
        {
            // transforms chest for continually from launchScale to originalScale using Linear Interpolation
            // factor (Lerp) from zero to 1 represented by (elapsedTime/scaleDuration) starting at 0
            // as elapsed time increases from zero the quotient moves from 0 to 1 and the scale is changed
            // using this Lerp ration. 
            transform.localScale = Vector3.Lerp(launchScale, originalScale, (elapsedTime / scaleDuration));
            elapsedTime += Time.deltaTime;

            yield return null;
        }

        // Ensures chest ends at the correct scale
        transform.localScale = originalScale; 
    }

    // Chest falls of Tilemap unless gravity is stopped, this function sets the chest's gravity
    // to zero after a period of time
    IEnumerator DisableGravityAndStopAfterDelay(float delay)
    {
        // Wait for the specified delay
        yield return new WaitForSeconds(delay);

        // Turn off gravity by setting the gravity scale to 0
        rb.gravityScale = 0;

        // Make the chest's velocity zero to make it stop moving
        rb.velocity = Vector2.zero;

        // Prevent further movement and rotation
        rb.constraints = RigidbodyConstraints2D.FreezeAll;
    }
}