using TMPro;
using UnityEngine;

public class TextTypingAnimation : MonoBehaviour {
    private TextMeshPro _textMesh;
    private int _currentLength = 0;
    private int _currentTextLength = 0;

	// Use this for initialization
	void Awake () {
        this._textMesh = GetComponent<TextMeshPro>();
        this._currentLength = 0;
        this.UpdateText();
	}
	
	// Update is called once per frame
	void Update ()
    {
        if (this._currentLength < this._currentTextLength)
        {
            this._currentLength++;
            this.UpdateText();
        }
    }

    public bool FinishText()
    {
        if (this._currentLength >= this._currentTextLength)
        {
            return true;
        }

        this._currentLength = this._currentTextLength;
        this.UpdateText();

        return false;
    }

    public void ResetText(string newText = "")
    {
        if (newText != "")
        {
            this._textMesh.text = newText;
        }
        this._currentLength = 0;
        var textInfo = this._textMesh.GetTextInfo(this._textMesh.text);
        this._currentTextLength = textInfo.characterCount;
        this.UpdateText();
    }

    private void UpdateText()
    {
        this._textMesh.maxVisibleCharacters = this._currentLength;
    }
}
