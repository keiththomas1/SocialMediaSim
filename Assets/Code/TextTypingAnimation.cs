using TMPro;
using UnityEngine;

public class TextTypingAnimation : MonoBehaviour {
    [SerializeField]
    private float _typeTime = 0.01f;

    private TextMeshPro _textMesh;
    private string _cachedString;
    private int _currentLength;

    private float _typingTimer;

	// Use this for initialization
	void Awake () {
        this._textMesh = GetComponent<TextMeshPro>();
        this._cachedString = this._textMesh.text;
        this._currentLength = 0;
        this.UpdateText();
        this._typingTimer = this._typeTime;
	}
	
	// Update is called once per frame
	void Update () {
		if (this._typingTimer > 0.0f)
        {
            this._typingTimer -= Time.deltaTime;
            if (this._typingTimer <= 0.0f)
            {
                this._currentLength++;
                this.UpdateText();

                if (this._currentLength < this._cachedString.Length)
                {
                    this._typingTimer = this._typeTime;
                }
            }
        }
	}

    public bool FinishText()
    {
        if (this._currentLength == this._cachedString.Length)
        {
            return true;
        }

        this._typingTimer = 0.0f;
        this._currentLength = this._cachedString.Length;
        this.UpdateText();

        return false;
    }

    public void ResetText(string newText = "")
    {
        if (newText != "")
        {
            this._cachedString = newText;
        }
        this._currentLength = 0;
        this._typingTimer = this._typeTime;
    }

    private void UpdateText()
    {
        this._textMesh.text = this._cachedString.Substring(0, this._currentLength);
    }
}
