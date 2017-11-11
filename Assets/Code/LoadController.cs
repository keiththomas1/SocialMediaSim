using UnityEngine;

public class LoadController : MonoBehaviour {
    private bool gameLoaded = false;
    private GlobalVars globalVars;
    private UserSerializer _userSerializer;
    private MessagesSerializer messagesSerializer;

    // Use this for initialization
    void Awake()
    {
        LoadState();
    }
	
	// Update is called once per frame
	void Update () {
    }

    // Consider OnApplicationFocus also (when keyboard is brought up on android for instance)
    void OnApplicationPause(bool pauseStatus)
    {
        if (pauseStatus) // Paused game
        {
            SaveState();
        }
        else // Resumed game
        {
            gameLoaded = false;
            LoadState();
        }
    }

    void SaveState()
    {
        if (this._userSerializer != null)
        {
            this._userSerializer.SaveFile();
        }
        if (globalVars != null)
        {
            globalVars.SaveFile();
        }
    }

    void LoadState()
    {
        if (!gameLoaded)
        {
            globalVars = GlobalVars.Instance;
            this._userSerializer = UserSerializer.Instance;
            messagesSerializer = MessagesSerializer.Instance;
            globalVars.LoadGame();
            this._userSerializer.LoadGame();
            messagesSerializer.LoadGame();

            gameLoaded = true;
        }
    }
}
