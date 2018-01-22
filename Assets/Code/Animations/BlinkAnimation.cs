using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlinkAnimation : MonoBehaviour {
    [SerializeField]
    private Sprite _eyesOpen;
    [SerializeField]
    private Sprite _eyesHalfClosed;
    [SerializeField]
    private Sprite _eyesClosed;

    private SpriteRenderer _spriteRenderer;

    private float _blinkTimer;

    private enum BlinkState {
        StartOpen,
        FirstHalfBlink,
        FullBlink,
        SecondHalfBlink,
        EndOpen
    }
    private BlinkState _currentBlinkState;
    private float _blinkAnimationTimer;
    private float BLINK_TIME;

	// Use this for initialization
	void Start () {
        this._spriteRenderer = GetComponent<SpriteRenderer>();
        this._blinkTimer = Random.Range(2.0f, 5.0f);
        this._currentBlinkState = BlinkState.StartOpen;
        this._blinkAnimationTimer = 0.0f;
        this.BLINK_TIME = Random.Range(0.04f, 0.06f);
    }
	
	// Update is called once per frame
	void Update () {
		if (this._blinkTimer > 0.0f)
        {
            this._blinkTimer -= Time.deltaTime;
            if (this._blinkTimer <= 0.0f)
            {
                this._blinkAnimationTimer = this.BLINK_TIME;
            }
        }
        if (this._blinkAnimationTimer > 0.0f)
        {
            this._blinkAnimationTimer -= Time.deltaTime;
            if (this._blinkAnimationTimer <= 0.0f)
            {
                switch (this._currentBlinkState)
                {
                    case BlinkState.EndOpen:
                    case BlinkState.StartOpen:
                        this._spriteRenderer.sprite = this._eyesHalfClosed;
                        this._currentBlinkState = BlinkState.FirstHalfBlink;
                        this._blinkAnimationTimer = this.BLINK_TIME;
                        break;
                    case BlinkState.FirstHalfBlink:
                        this._spriteRenderer.sprite = this._eyesClosed;
                        this._currentBlinkState = BlinkState.FullBlink;
                        this._blinkAnimationTimer = this.BLINK_TIME;
                        break;
                    case BlinkState.FullBlink:
                        this._spriteRenderer.sprite = this._eyesHalfClosed;
                        this._currentBlinkState = BlinkState.SecondHalfBlink;
                        this._blinkAnimationTimer = this.BLINK_TIME;
                        break;
                    case BlinkState.SecondHalfBlink:
                        this._spriteRenderer.sprite = this._eyesOpen;
                        this._currentBlinkState = BlinkState.EndOpen;
                        this._blinkTimer = Random.Range(2.0f, 5.0f);
                        break;
                }
            }
        }
	}
}
