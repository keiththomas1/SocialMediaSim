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
public struct PictureItem
{
    public PictureItem(
        string _name,
        SerializableVector3 _location,
        float _rotation,
        float _scale,
        SerializableColor _color)
    {
        name = _name;
        location = _location;
        rotation = _rotation;
        scale = _scale;
        color = _color;
    }

    public string name;
    public SerializableVector3 location;
    public float rotation;
    public float scale;
    public SerializableColor color;
}

[Serializable]
public class PictureArrayJson
{
    public PictureModelJsonReceive[] pictureModels;
}
[Serializable]
public struct CharacterPropertiesModelJson
{
    [Serializable]
    public struct SpriteProperties
    {
        public string hairSprite;
        public string eyeSprite;
        public string birthmark;
    }
    [Serializable]
    public struct ColorProperties
    {
        public SerializableColor skinColor;
        public SerializableColor hairColor;
        public SerializableColor shirtColor;
        public SerializableColor pantsColor;
    }
    [Serializable]
    public struct LevelProperties
    {
        public int avatarLevel;
        public int happinessLevel;
        public int fitnessLevel;
        public int hygieneLevel;
    }
    public string gender;
    public SerializableVector3 position;
    public float rotation;
    public float scale;
    public string poseName;
    public SpriteProperties spriteProperties;
    public ColorProperties colorProperties;
    public LevelProperties levelProperties;
}
[Serializable]
public struct PictureModelJsonSend
{
    public string localPictureId;
    public string playerId;
    public string playerName;
    public string backgroundName;

    public CharacterPropertiesModelJson characterProperties;

    public List<PictureItem> items;
}
[Serializable]
public struct PictureModelJsonReceive
{
    public string _id;
    public string localPictureId;
    public string playerId;
    public string playerName;
    public string backgroundName;

    public CharacterPropertiesModelJson characterProperties;

    public int likes;
    public int dislikes;
    public int totalFeedback;
    public List<PictureItem> items;
    public string createdDate;
}

public delegate void GetPicturesCallback(PictureArrayJson pictures, bool success);

public class PostRequester
{
    private DateTime? _lastTenPostsRequest;
    private PictureArrayJson _lastTenPosts;

#if UNITY_EDITOR
    public static readonly string SERVER_URL = "http://localhost:3000";
#else
    public static readonly string SERVER_URL = "http://13.59.159.27";
#endif

    public PostRequester() {
    }

    public IEnumerator PostPicture(DelayGramPost post)
    {
#if UNITY_EDITOR
        yield return null;
#endif
        // Create a picture with information from picture
        var newPicture = new PictureModelJsonSend();
        newPicture.localPictureId = post.pictureID;
        newPicture.playerId = post.playerName; // post.playerId;
        newPicture.playerName = post.playerName;
        newPicture.backgroundName = post.backgroundName;

        newPicture.characterProperties = new CharacterPropertiesModelJson();
        newPicture.characterProperties.gender = post.characterProperties.gender.ToString();
        newPicture.characterProperties.position = post.avatarPosition;
        newPicture.characterProperties.rotation = post.avatarRotation;
        newPicture.characterProperties.scale = post.avatarScale;
        newPicture.characterProperties.poseName = post.avatarPoseName;

        newPicture.characterProperties.spriteProperties = new CharacterPropertiesModelJson.SpriteProperties();
        newPicture.characterProperties.spriteProperties.hairSprite = post.characterProperties.hairSprite;
        newPicture.characterProperties.spriteProperties.eyeSprite = post.characterProperties.eyeSprite;
        newPicture.characterProperties.spriteProperties.birthmark = post.characterProperties.birthmark.ToString();

        newPicture.characterProperties.levelProperties = new CharacterPropertiesModelJson.LevelProperties();
        newPicture.characterProperties.levelProperties.avatarLevel = post.characterProperties.avatarLevel;
        newPicture.characterProperties.levelProperties.happinessLevel = post.characterProperties.happinessLevel;
        newPicture.characterProperties.levelProperties.fitnessLevel = post.characterProperties.fitnessLevel;
        newPicture.characterProperties.levelProperties.hygieneLevel = post.characterProperties.hygieneLevel;

        newPicture.characterProperties.colorProperties = new CharacterPropertiesModelJson.ColorProperties();
        newPicture.characterProperties.colorProperties.skinColor = post.characterProperties.skinColor;
        newPicture.characterProperties.colorProperties.hairColor = post.characterProperties.hairColor;
        newPicture.characterProperties.colorProperties.shirtColor = post.characterProperties.shirtColor;
        newPicture.characterProperties.colorProperties.pantsColor = post.characterProperties.pantsColor;

        newPicture.items = post.items;

        var jsonifiedPicture = JsonUtility.ToJson(newPicture);
        byte[] pictureData = Encoding.UTF8.GetBytes(jsonifiedPicture.ToCharArray());

        var headers = new Dictionary<string, string>();
        headers.Add("Content-Type", "application/json");

        var route = String.Format(@"{0}/pictures", SERVER_URL);
        var www = new WWW(route, pictureData, headers);
        yield return www;
    }

    public async void RequestRecentPosts(int count, GetPicturesCallback finishCallback)
    {
        var route = String.Format(@"{0}/listPictures/{1}", SERVER_URL, count);
        HttpWebRequest request = (HttpWebRequest)WebRequest.Create(route);

        // TODO: Will need to forego this if looking for different timestamps
        // If we have not requested yet OR it has been 30 seconds since last request
        if (!this._lastTenPostsRequest.HasValue || ((DateTime.Now - this._lastTenPostsRequest.Value) > TimeSpan.FromMinutes(1)))
        {
            this._lastTenPostsRequest = DateTime.Now;
            this._lastTenPosts = await this.MakePicturesRequest(request, finishCallback);
        } else {
            finishCallback(this._lastTenPosts, true);
        }
    }

    public void RequestNeededFeedbackPosts(int count, GetPicturesCallback finishCallback)
    {
        var route = String.Format(@"{0}/listFeedbackNeededPictures/{1}", SERVER_URL, count);
        HttpWebRequest request = (HttpWebRequest)WebRequest.Create(route);

        this.MakePicturesRequest(request, finishCallback);
    }

    public void RequestAllUserPosts(string username, GetPicturesCallback finishCallback)
    {
        var route = String.Format(@"{0}/listUserPictures/{1}", SERVER_URL, username);
        HttpWebRequest request = (HttpWebRequest)WebRequest.Create(route);

        this.MakePicturesRequest(request, finishCallback);
    }

    private async Task<PictureArrayJson> MakePicturesRequest(HttpWebRequest request, GetPicturesCallback finishCallback)
    {
        HttpWebResponse response = null;
        var sendRequest = new Action(() => {
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

                finishCallback(pictures, true);
                return pictures;
            }
            catch (Exception exception)
            {
                Debug.Log("Error reading/deserializing response stream: " + exception.ToString());
            }
        }
        else
        {
            var blankArray = new PictureArrayJson();
            blankArray.pictureModels = new PictureModelJsonReceive[0];
            finishCallback(blankArray, false);
        }

        return null;
    }

    public IEnumerator AddLikeToPicture(string pictureID, string userID)
    {
        var route = String.Format(@"{0}/liked/{1}/{2}", SERVER_URL, pictureID, userID);
        UnityWebRequest www = UnityWebRequest.Put(route, "{}");
        yield return www.SendWebRequest();

        if (www.isNetworkError)
        {
            Debug.Log("Error adding like to " + pictureID);
        }
    }
    public IEnumerator AddDislikeToPicture(string pictureID, string userID)
    {
        var route = String.Format(@"{0}/disliked/{1}/{2}", SERVER_URL, pictureID, userID);
        UnityWebRequest www = UnityWebRequest.Put(route, "{}");
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
        newPost.pictureID = picture.localPictureId;
        newPost.playerName = picture.playerName;
        newPost.backgroundName = picture.backgroundName;

        newPost.characterProperties = new CharacterProperties();
        try
        {
            newPost.characterProperties.gender =
                (Gender)Enum.Parse(typeof(Gender), picture.characterProperties.gender);
        }
        catch (Exception)
        {
            newPost.characterProperties.gender = Gender.Female;
        }
        newPost.avatarPosition = picture.characterProperties.position;
        newPost.avatarRotation = picture.characterProperties.rotation;
        newPost.avatarScale = picture.characterProperties.scale;
        newPost.avatarPoseName = picture.characterProperties.poseName;

        newPost.characterProperties.hairSprite = picture.characterProperties.spriteProperties.hairSprite;
        newPost.characterProperties.eyeSprite = picture.characterProperties.spriteProperties.eyeSprite;
        try
        {
            newPost.characterProperties.birthmark =
                (BirthMarkType)Enum.Parse(typeof(BirthMarkType), picture.characterProperties.spriteProperties.birthmark);
        }
        catch (Exception)
        {
            newPost.characterProperties.birthmark = BirthMarkType.None;
        }

        newPost.characterProperties.avatarLevel = picture.characterProperties.levelProperties.avatarLevel;
        newPost.characterProperties.happinessLevel = picture.characterProperties.levelProperties.happinessLevel;
        newPost.characterProperties.fitnessLevel = picture.characterProperties.levelProperties.fitnessLevel;
        newPost.characterProperties.hygieneLevel = picture.characterProperties.levelProperties.hygieneLevel;

        newPost.characterProperties.skinColor = picture.characterProperties.colorProperties.skinColor;
        newPost.characterProperties.hairColor = picture.characterProperties.colorProperties.hairColor;
        newPost.characterProperties.shirtColor = picture.characterProperties.colorProperties.shirtColor;
        newPost.characterProperties.pantsColor = picture.characterProperties.colorProperties.pantsColor;

        newPost.likes = picture.likes;
        newPost.dislikes = picture.dislikes;
        newPost.pictureID = picture._id; // this.newPostController.GetRandomImageID();
        newPost.items = picture.items;

        newPost.dateTime = ParseDateTimeFromServer(picture.createdDate);

        return newPost;
    }

    public static DateTime ParseDateTimeFromServer(string createdDate)
    {
        CultureInfo enUS = new CultureInfo("en-US");
        DateTime parsedDate;
        if (DateTime.TryParseExact(
            createdDate, "yyyy-MM-ddTHH:mm:ss.fffZ", enUS, DateTimeStyles.None, out parsedDate))
        {
            return parsedDate;
        }
        else
        {
            return DateTime.Now;
        }
    }

    public static string GetPostTimeFromTimeSpan(TimeSpan timeSincePost)
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
