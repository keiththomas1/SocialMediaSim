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
    public PictureItem() {}
    public PictureItem(string _name, SerializableVector3 _location, float _rotation, float _scale)
    {
        name = _name;
        location = _location;
        rotation = _rotation;
        scale = _scale;
    }

    public string name;
    public SerializableVector3 location;
    public float rotation;
    public float scale;
}

[Serializable]
public class PictureArrayJson
{
    public PictureModelJsonReceive[] pictureModels;
}
[Serializable]
public class CharacterPropertiesModelJson
{
    [Serializable]
    public class SpriteProperties
    {
        public string hairSprite;
        public string eyeSprite;
    }
    [Serializable]
    public class ColorProperties
    {
        public SerializableColor skinColor;
        public SerializableColor hairColor;
        public SerializableColor shirtColor;
        public SerializableColor pantsColor;
    }
    [Serializable]
    public class LevelProperties
    {
        public int happinessLevel;
        public int fitnessLevel;
        public int styleLevel;
    }
    public string gender;
    public SerializableVector3 position;
    public float rotation;
    public float scale;
    public SpriteProperties spriteProperties;
    public ColorProperties colorProperties;
    public LevelProperties levelProperties;
}
[Serializable]
public class PictureModelJsonSend
{
    public string playerName;
    public string backgroundName;

    public CharacterPropertiesModelJson characterProperties;

    public List<PictureItem> items;
}
[Serializable]
public class PictureModelJsonReceive
{
    public string _id;
    public string playerName;
    public string backgroundName;

    public CharacterPropertiesModelJson characterProperties;

    public int likes;
    public int dislikes;
    public List<PictureItem> items;
    public string createdDate;
}

public delegate void GetLastTenCallback(PictureArrayJson pictures, bool success);

public class RESTRequester
{
    private DateTime? _lastTenPostsRequest = null;
    private PictureArrayJson _lastTenPosts;

    public RESTRequester() {
    }

    public IEnumerator PostPicture(DelayGramPost post)
    {
        // Create a picture with information from picture
        var newPicture = new PictureModelJsonSend();
        newPicture.playerName = post.playerName;
        newPicture.backgroundName = post.backgroundName;

        newPicture.characterProperties = new CharacterPropertiesModelJson();
        newPicture.characterProperties.gender = post.characterProperties.gender.ToString();
        newPicture.characterProperties.position = post.avatarPosition;
        newPicture.characterProperties.rotation = post.avatarRotation;
        newPicture.characterProperties.scale = post.avatarScale;

        newPicture.characterProperties.spriteProperties = new CharacterPropertiesModelJson.SpriteProperties();
        newPicture.characterProperties.spriteProperties.hairSprite = post.characterProperties.hairSprite;
        newPicture.characterProperties.spriteProperties.eyeSprite = post.characterProperties.eyeSprite;

        newPicture.characterProperties.levelProperties = new CharacterPropertiesModelJson.LevelProperties();
        newPicture.characterProperties.levelProperties.happinessLevel = post.characterProperties.happinessLevel;
        newPicture.characterProperties.levelProperties.fitnessLevel = post.characterProperties.fitnessLevel;
        newPicture.characterProperties.levelProperties.styleLevel = post.characterProperties.styleLevel;

        newPicture.characterProperties.colorProperties = new CharacterPropertiesModelJson.ColorProperties();
        newPicture.characterProperties.colorProperties.skinColor = post.characterProperties.skinColor;
        newPicture.characterProperties.colorProperties.hairColor = post.characterProperties.hairColor;
        newPicture.characterProperties.colorProperties.shirtColor = post.characterProperties.shirtColor;
        newPicture.characterProperties.colorProperties.pantsColor = post.characterProperties.pantsColor;

        newPicture.items = post.items;

        var jsonifiedPicture = JsonUtility.ToJson(newPicture);
        byte[] pictureData = Encoding.ASCII.GetBytes(jsonifiedPicture.ToCharArray());

        var headers = new Dictionary<string, string>();
        headers.Add("Content-Type", "application/json");
        var www = new WWW(@"http://13.59.159.27/pictures", pictureData, headers);
        // var www = new WWW(@"http://localhost:3000/pictures", pictureData, headers);
        yield return www;
    }

    public async void RequestLastTenPosts(GetLastTenCallback finishCallback)
    {
        HttpWebRequest request = (HttpWebRequest)WebRequest.Create(@"http://13.59.159.27/listPictures/10");
        // HttpWebRequest request = (HttpWebRequest)WebRequest.Create(@"http://localhost:3000/listPictures/10");
        if (this._lastTenPostsRequest == null || (DateTime.Now - this._lastTenPostsRequest) > TimeSpan.FromMinutes(1))
        {
            HttpWebResponse response = null;
            var sendRequest = new Action(()=> {
                try
                {
                    response = (HttpWebResponse)request.GetResponse();
                }
                catch (Exception exception)
                {
                    Debug.Log("Bad/No response from server? Exception making web request:" + exception.ToString());
                }
            });
            try
            {
                await Task.Run(sendRequest);
            }
            catch (Exception exception)
            {
                Debug.Log("No internet? Exception making web request:" + exception.ToString());
            }

            if (response != null && response.StatusCode == HttpStatusCode.OK)
            {
                try
                {
                    Stream stream = response.GetResponseStream();
                    StreamReader reader = new StreamReader(stream);
                    var responseBody = reader.ReadToEnd();

                    string JSONToParse = "{\"pictureModels\":" + responseBody + "}";
                    PictureArrayJson pictures = JsonUtility.FromJson<PictureArrayJson>(JSONToParse);
                    this._lastTenPosts = pictures;
                    this._lastTenPostsRequest = DateTime.Now;

                    finishCallback(pictures, true);
                }
                catch (Exception exception)
                {
                    Debug.Log("Error reading/deserializing response stream: " + exception.ToString());
                }
            } else {
                var blankArray = new PictureArrayJson();
                blankArray.pictureModels = new PictureModelJsonReceive[0];
                finishCallback(blankArray, false);
            }
        } else {
            finishCallback(this._lastTenPosts, true);
        }
    }

    public IEnumerator AddLikeToPicture(string pictureID)
    {
        UnityWebRequest www = UnityWebRequest.Put("http://13.59.159.27//liked//" + pictureID, "{}");
        // UnityWebRequest www = UnityWebRequest.Put("localhost:3000/liked//" + pictureID, "{}");
        yield return www.SendWebRequest();
        // yield return www.Send();

        if (www.isNetworkError)
        {
            Debug.Log("Error adding like to " + pictureID);
        }
    }
    public IEnumerator AddDislikeToPicture(string pictureID)
    {
        UnityWebRequest www = UnityWebRequest.Put("http://13.59.159.27//disliked//" + pictureID, "{}");
        // UnityWebRequest www = UnityWebRequest.Put("localhost:3000//disliked//" + pictureID);
        yield return www.SendWebRequest();
        // yield return www.Send();

        if (www.isNetworkError)
        {
            Debug.Log("Error adding dislike to " + pictureID);
        }
    }

    /* Helper methods */

    public DelayGramPost ConvertJsonPictureIntoDelayGramPost(PictureModelJsonReceive picture)
    {
        var newPost = new DelayGramPost();
        newPost.playerName = picture.playerName;
        newPost.backgroundName = picture.backgroundName;

        newPost.characterProperties = new CharacterProperties();
        newPost.characterProperties.gender =
            (Gender)Enum.Parse(typeof(Gender), picture.characterProperties.gender);
        newPost.avatarPosition = picture.characterProperties.position;
        newPost.avatarRotation = picture.characterProperties.rotation;
        newPost.avatarScale = picture.characterProperties.scale;

        newPost.characterProperties.hairSprite = picture.characterProperties.spriteProperties.hairSprite;
        newPost.characterProperties.eyeSprite = picture.characterProperties.spriteProperties.eyeSprite;

        newPost.characterProperties.happinessLevel = picture.characterProperties.levelProperties.happinessLevel;
        newPost.characterProperties.fitnessLevel = picture.characterProperties.levelProperties.fitnessLevel;
        newPost.characterProperties.styleLevel = picture.characterProperties.levelProperties.styleLevel;

        newPost.characterProperties.skinColor = picture.characterProperties.colorProperties.skinColor;
        newPost.characterProperties.hairColor = picture.characterProperties.colorProperties.hairColor;
        newPost.characterProperties.shirtColor = picture.characterProperties.colorProperties.shirtColor;
        newPost.characterProperties.pantsColor = picture.characterProperties.colorProperties.pantsColor;

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
