using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour {
    [SerializeField]
    private TextMeshProUGUI _loadingText;
    [SerializeField]
    private RectTransform _loadingBar;

    private float _currentProgress = 0.0f;
    private float _loadingBarWidth;
    private bool _finishedLoading = false;

	// Use this for initialization
	void Start () {
        DontDestroyOnLoad(this.gameObject);
        StartCoroutine(this.LoadMainScene());
        // this.LoadMainScene();

        this._loadingBarWidth = this._loadingBar.rect.width;
    }

    // Update is called once per frame
    void Update () {
        if (!this._finishedLoading)
        {
            this._loadingBar.sizeDelta = new Vector2(this._currentProgress * this._loadingBarWidth, this._loadingBar.sizeDelta.y);
            this._loadingText.text = String.Format("{0}%", Mathf.Floor(this._currentProgress * 100.0f));
        }
    }

    private IEnumerator LoadMainScene()
    {
        // SceneManager.LoadScene("main", LoadSceneMode.Single);
        AsyncOperation loadAsync = SceneManager.LoadSceneAsync("main", LoadSceneMode.Additive);

        while (!loadAsync.isDone)
        {
            this._currentProgress = loadAsync.progress;
            yield return null;
        }
        this._finishedLoading = true;

        AsyncOperation unloadAsync = SceneManager.UnloadSceneAsync("loading");
        yield return unloadAsync;

        // var uiController = GameObject.FindObjectOfType<UIController>();
        // if (uiController)
        // {
        //     uiController.EnterGame();
        // }
    }
}
