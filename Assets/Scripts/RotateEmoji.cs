using System.Collections;
using UnityEngine;

public class RotateEmoji : MonoBehaviour
{
    public float rotationSpeed = 180f; // velocity of rotation
    private bool stopRotation = true;

    // Handle the rotation of one Emoji ----------------------------------------------------
    public void RotationHandler()
    {
        if(stopRotation == true)
        {
            stopRotation = false;
            StartCoroutine(RotateObject());      // start rotation of one Emoji
        }
        else
        {
            stopRotation = true;
        }
    }

    // Rotate the Emoji asynchrously ------------------------------------------------------
    private IEnumerator RotateObject()
    {
        float targetAngle = 360f;
        float currentAngle = 0f;

        while(!stopRotation)  // keep rotating until stopRotation is true
        {
            float deltaAngle = rotationSpeed * Time.deltaTime;
            transform.Rotate(0, 0, deltaAngle);
            currentAngle += deltaAngle;
            yield return null;
        }
        transform.Rotate(0, 0, targetAngle - currentAngle);
    }
}
