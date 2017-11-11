using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

[Serializable]
public class PictureItem
{
    public string name;
    public SerializableVector3 location;
}

[Serializable]
public class PictureArrayJson
{
    public PictureModelJsonReceive[] pictureModels;
}
[Serializable]
public class PictureModelJsonSend
{
    public string playerName;
    public SerializableVector3 avatarPosition;
    public SerializableColor skinColor;
    public SerializableColor hairColor;
    public SerializableColor shirtColor;
    public string gender;
    public string bodySprite;
    public string backgroundName;
    public List<PictureItem> items;
}
[Serializable]
public class PictureModelJsonReceive
{
    public string _id;
    public string createdDate;
    public int likes;
    public int dislikes;
    public string playerName;
    public SerializableVector3 avatarPosition;
    public SerializableColor skinColor;
    public SerializableColor hairColor;
    public SerializableColor shirtColor;
    public string gender;
    public string bodySprite;
    public string backgroundName;
    public List<PictureItem> items;
}

public delegate void GetLastTenCallback(PictureArrayJson pictures, bool success);

public class RESTRequester
{
    private DateTime? _lastTenPostsRequest = null;
    private PictureArrayJson _lastTenPosts;

    public RESTRequester() {
    }

    public async void RequestLastTenPosts(GetLastTenCallback finishCallback)
    {
        HttpWebRequest request = (HttpWebRequest)WebRequest.Create(@"http://13.59.159.27/lastTenPictures");
        // HttpWebRequest request = (HttpWebRequest)WebRequest.Create(@"localhost:3000/lastTenPictures");
        if (this._lastTenPostsRequest == null || (DateTime.Now - this._lastTenPostsRequest) > TimeSpan.FromMinutes(1))
        {
            HttpWebResponse response = null;
            var sendRequest = new Action(()=> {
                response = (HttpWebResponse)request.GetResponse();
            });
            await Task.Run(sendRequest);

            if (response != null && response.StatusCode == HttpStatusCode.OK)
            {
                Stream stream = response.GetResponseStream();
                StreamReader reader = new StreamReader(stream);
                var responseBody = reader.ReadToEnd();

                string JSONToParse = "{\"pictureModels\":" + responseBody + "}";
                PictureArrayJson pictures = JsonUtility.FromJson<PictureArrayJson>(JSONToParse);
                this._lastTenPosts = pictures;
                this._lastTenPostsRequest = DateTime.Now;

                finishCallback(pictures, true);
            } else {
                var blankArray = new PictureArrayJson();
                blankArray.pictureModels = new PictureModelJsonReceive[0];
                finishCallback(blankArray, false);
            }
        } else {
            finishCallback(this._lastTenPosts, true);
        }
    }

    public IEnumerator PostPicture(DelayGramPost post)
    {
        // Create a picture with information from picture
        var newPicture = new PictureModelJsonSend();
        newPicture.avatarPosition = post.avatarPosition;
        newPicture.playerName = post.playerName;
        newPicture.bodySprite = post.characterProperties.bodySprite;
        newPicture.skinColor = post.characterProperties.skinColor;
        newPicture.hairColor = post.characterProperties.hairColor;
        newPicture.shirtColor = post.characterProperties.shirtColor;
        newPicture.gender = post.characterProperties.gender.ToString();
        newPicture.backgroundName = post.backgroundName;
        newPicture.items = post.items;

        var jsonifiedPicture = JsonUtility.ToJson(newPicture);
        byte[] pictureData = Encoding.ASCII.GetBytes(jsonifiedPicture.ToCharArray());

        var headers = new Dictionary<string, string>();
        headers.Add("Content-Type", "application/json");
        //var www = new WWW("http://13.59.159.27//pictures", pictureData, headers);
        var www = new WWW("localhost:3000//pictures", pictureData, headers);
        yield return www;
    }

    public IEnumerator AddLikeToPicture(string pictureID)
    {
        UnityWebRequest www = UnityWebRequest.Put("http://13.59.159.27//liked//" + pictureID, "{}");
        // UnityWebRequest www = UnityWebRequest.Put("localhost:3000/liked//" + pictureID, "{}");
        yield return www.Send();

        if (www.isNetworkError)
        {
            Debug.Log("Error adding like to " + pictureID);
        }
    }
    public IEnumerator AddDislikeToPicture(string pictureID)
    {
        UnityWebRequest www = UnityWebRequest.Put("http://13.59.159.27//disliked//" + pictureID, "{}");
        // UnityWebRequest www = UnityWebRequest.Put("localhost:3000//disliked//" + pictureID);
        yield return www.Send();

        if (www.isNetworkError)
        {
            Debug.Log("Error adding dislike to " + pictureID);
        }
    }

    /* Helper methods */

    public DelayGramPost ConvertJsonPictureIntoDelayGramPost(PictureModelJsonReceive picture)
    {
        var newPost = new DelayGramPost();
        newPost.avatarPosition = picture.avatarPosition;
        newPost.playerName = picture.playerName;
        newPost.backgroundName = picture.backgroundName;
        newPost.characterProperties = new CharacterProperties();
        newPost.characterProperties.gender = (Gender)Enum.Parse(typeof(Gender), picture.gender);
        newPost.characterProperties.bodySprite = picture.bodySprite;
        newPost.characterProperties.skinColor = picture.skinColor;
        newPost.characterProperties.hairColor = picture.hairColor;
        newPost.characterProperties.shirtColor = picture.shirtColor;
        newPost.likes = picture.likes;
        newPost.dislikes = picture.dislikes;
        newPost.imageID = picture._id; // this.newPostController.GetRandomImageID();
        newPost.items = picture.items;

        CultureInfo enUS = new CultureInfo("en-US");
        DateTime parsedDate;
        if (DateTime.TryParseExact(picture.createdDate, "yyyy-MM-ddTHH:mm:ss.fffZ", enUS,
                                    DateTimeStyles.None, out parsedDate))
        {
            newPost.dateTime = parsedDate;
        }
        else
        {
            newPost.dateTime = DateTime.Now;
        }

        return newPost;
    }

    public string GetPostTimeFromDateTime(TimeSpan timeSincePost)
    {
        string postTime = "";
        if (timeSincePost.Days > 0)
        {
            postTime = timeSincePost.Days.ToString() + " days ago";
        }
        else if (timeSincePost.Hours > 0)
        {
            postTime = timeSincePost.Hours.ToString() + " hours ago";
        }
        else
        {
            postTime = timeSincePost.Minutes.ToString() + " mins ago";
        }
        return postTime;
    }
}
