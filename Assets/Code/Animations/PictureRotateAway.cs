using UnityEngine;

public class PictureRotateAway : MonoBehaviour {
    private bool started;
    private float incrementTimer;
    private Vector2 directionVector;
    private float rotationSpeed;

    public enum RotateDirection
    {
        Left,
        Right,
        Random
    }

	void Start () {
        started = false;
	}
	
	void Update () {
        if (started)
        {
            transform.Translate(directionVector);
            transform.Rotate(0.0f, 0.0f, rotationSpeed);
        }
	}

    public void StartAnimation(RotateDirection direction) {
        started = true;

        directionVector = new Vector2(Random.value / 2.0f, Random.value);
        directionVector.Normalize();
        directionVector /= 10;

        if (direction == RotateDirection.Random)
        {
            var rand = Random.Range(0, 1);
            if (rand == 0)
            {
                direction = RotateDirection.Left;
            } else {
                direction = RotateDirection.Right;
            }
        }

        if (direction == RotateDirection.Left)
        {
            directionVector.x *= -1.0f;
            rotationSpeed = 0.5f;
        } else {
            rotationSpeed = -0.5f;
        }
    }
}
