using UnityEngine;

public class DeathByTimer : MonoBehaviour {
    public float deathTimeInSeconds;
    float currentTimerValue;

	// Use this for initialization
	void Start () {
        currentTimerValue = deathTimeInSeconds;
	}
	
	// Update is called once per frame
	void Update () {
        currentTimerValue -= Time.deltaTime;
        if (currentTimerValue <= 0.0f)
        {
            GameObject.Destroy(gameObject);
        }
	}
}
