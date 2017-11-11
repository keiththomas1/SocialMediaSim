using UnityEngine;

public class FollowTextAnimation : MonoBehaviour
{
    private float incrementTimer;
    private Vector2 directionVector;
    //private float rotationSpeed;

    void Start()
    {
        directionVector = new Vector2((Random.value - 0.5f) / 6.0f, (Random.value / 2.0f));
        // directionVector.Normalize();
        directionVector /= 10;

        if (directionVector.x < 0.0f)
        {
            //rotationSpeed = 0.5f;
        }
        else
        {
            //rotationSpeed = -0.5f;
        }
    }

    void Update()
    {
        transform.Translate(directionVector);
        directionVector.y -= 0.005f;
        // transform.Rotate(0.0f, 0.0f, rotationSpeed);
    }
}
