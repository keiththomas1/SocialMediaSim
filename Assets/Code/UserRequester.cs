using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

[Serializable]
public struct UserModelJsonSend
{
    public string displayName;
    public string createdDate;
}
[Serializable]
public struct UserModelJsonReceive
{
    public string userId;
    public string displayName;
    public string createdDate;
}

public class UserRequester
{
    public IEnumerator PostUser(string displayName)
    {
        // Create a picture with information from picture
        var newUser = new UserModelJsonSend();
        newUser.displayName = displayName;

        var jsonifiedUser = JsonUtility.ToJson(newUser);
        byte[] pictureData = Encoding.UTF8.GetBytes(jsonifiedUser.ToCharArray());

        var headers = new Dictionary<string, string>();
        headers.Add("Content-Type", "application/json");

        var route = String.Format(@"{0}/users", PostRequester.SERVER_URL);
        var www = new WWW(route, pictureData, headers);
        yield return www;
    }
}
