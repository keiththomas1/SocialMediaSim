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
    private bool _startedGame = false;

	// Use this for initialization
	void Start () {
        DontDestroyOnLoad(this.gameObject);
        // this.LoadMainScene();

        this._loadingBarWidth = this._loadingBar.rect.width;

        StartCoroutine(this.LoadMainScene());
    }

    // Update is called once per frame
    void Update () {
        if (!this._finishedLoading)
        {
            this._loadingBar.sizeDelta = new Vector2(this._currentProgress * this._loadingBarWidth, this._loadingBar.sizeDelta.y);
            this._loadingText.text = String.Format("{0}%", Mathf.Floor(this._currentProgress * 100.0f));
        }
        else
        {
            if (!this._startedGame)
            {
                this._startedGame = true;
                var uiController = GameObject.FindObjectOfType<UIController>();
                // uiController.StartCoroutine(uiController.StartGameEndOfFrame());
                uiController.EnterGame();
            }
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

        AsyncOperation unloadAsync = SceneManager.UnloadSceneAsync("loading");
        yield return unloadAsync;

        this._finishedLoading = true;

        // var uiController = GameObject.FindObjectOfType<UIController>();
        // if (uiController)
        // {
        //     uiController.EnterGame();
        // }
    }
}
