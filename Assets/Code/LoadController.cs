using System;
using UnityEngine;

public class LoadController : MonoBehaviour {
    private bool _gameLoaded = false;
    private CharacterSerializer _characterSerializer;
    private GoalSerializer _goalSerializer;
    private MessagesSerializer _messagesSerializer;
    private UserSerializer _userSerializer;
    private NotificationSerializer _notificationSerializer;

    // Use this for initialization
    void Awake()
    {
        this.LoadState();
    }

	// Update is called once per frame
	void Update () {
    }

    // Consider OnApplicationFocus also (when keyboard is brought up on android for instance)
    void OnApplicationPause(bool pauseStatus)
    {
        if (pauseStatus) // Paused game
        {
            this.SaveState();
        }
        else // Resumed game
        {
            this._gameLoaded = false;
            this.LoadState();
        }
    }

    private void OnApplicationQuit()
    {
        this.SaveState();
    }

    void SaveState()
    {
        this._characterSerializer.SaveFile();
        this._goalSerializer.SaveGame();
        this._messagesSerializer.SaveFile();
        this._userSerializer.SaveFile();
        this._notificationSerializer.SaveGame();
    }

    void LoadState()
    {
        if (!this._gameLoaded)
        {
            this._characterSerializer = CharacterSerializer.Instance;
            this._characterSerializer.LoadGame();
            this._goalSerializer = GoalSerializer.Instance;
            this._goalSerializer.LoadGame();
            this._messagesSerializer = MessagesSerializer.Instance;
            this._messagesSerializer.LoadGame();
            this._userSerializer = UserSerializer.Instance;
            this._userSerializer.LoadGame();
            this._notificationSerializer = NotificationSerializer.Instance;
            this._notificationSerializer.LoadGame();

            if (this._userSerializer.PostedPhoto && ((DateTime.Now - this._userSerializer.LastUpdate) > TimeSpan.FromDays(1f)))
            {
                this._characterSerializer.Smelly = true;
            }

            this._gameLoaded = true;
        }
    }
}
