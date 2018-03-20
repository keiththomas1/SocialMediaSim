using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour {
    [SerializeField]
    private GameObject _progressText;

	// Use this for initialization
	void Start () {
        // StartCoroutine(this.LoadMainScene());
        this.LoadMainScene();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    // private IEnumerator LoadMainScene()
    private void LoadMainScene()
    {
        SceneManager.LoadScene("main", LoadSceneMode.Single);
        // AsyncOperation async = SceneManager.LoadSceneAsync("main", LoadSceneMode.Additive);

        // while (!async.isDone)
        // {
        //     this._progressText.GetComponent<TextMeshProUGUI>().text = async.progress.ToString();
        //     yield return null;
        // }

        // AsyncOperation async = SceneManager.UnloadSceneAsync("loading");
    }
}
