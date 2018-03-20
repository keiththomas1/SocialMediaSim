using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;

public class SpeechBubbleFade : MonoBehaviour {
    public float _finalXLocation = 3.3f;

    private bool _fading = false;
    private CharacterSerializer _characterSerializer;
    private SpriteRenderer _selfSpriteRenderer;
    private TextMeshPro _textSpriteRenderer;

    private List<string> _genericText = new List<string>() {
        "Can we go do something...",
        "Why are you staring at me...",
        "Great .. can't wait to take more selfies ..",
        "I'd almost rather be in that Kardashian Game...",
        };
    private List<string> _chunkyText = new List<string>() {
        // "If you played more maybe I would look better...",
        };

    // Use this for initialization
    void Start () {
        this._characterSerializer = CharacterSerializer.Instance;
        this._selfSpriteRenderer = GetComponent<SpriteRenderer>();
        var text = transform.Find("Text");
        this._textSpriteRenderer = text.GetComponent<TextMeshPro>();
        this._textSpriteRenderer.text = this.GetRandomText();

        var originalScale = this.transform.localScale;
        this.transform.localScale = new Vector3(0.0f, 0.0f, 0.0f);
        this.transform.DOScale(originalScale, 0.8f)
            .SetEase(Ease.InCubic)
            .OnComplete(() => { this.FadeRight(); });
        var currentLocation = this.transform.position;
        this.transform.DOMoveX(currentLocation.x + 0.5f, 1.8f);
	}
	
	// Update is called once per frame
	void Update () {
		if (this._fading)
        {
            var oldColor = this._selfSpriteRenderer.color;
            oldColor.a -= 0.006f;
            this._selfSpriteRenderer.color = oldColor;
            var oldColor2 = this._textSpriteRenderer.color;
            oldColor2.a -= 0.006f;
            this._textSpriteRenderer.color = oldColor2;
        }
	}

    private void FadeRight()
    {
        // this._fading = true;
        this.transform.DOMoveX(this._finalXLocation, 1.5f)
            .SetEase(Ease.InExpo)
            .SetDelay(1.5f)
            .OnPlay(() => { this._fading = true; })
            .OnComplete(() => { GameObject.Destroy(this.gameObject); });
    }

    private string GetRandomText()
    {
        var finalTextList = new List<string>(this._genericText);
        if (this._characterSerializer.FitnessLevel < 3)
        {
            finalTextList.AddRange(this._chunkyText);
        }

        var index = Random.Range(0, finalTextList.Count);
        return finalTextList[index];
    }
}
