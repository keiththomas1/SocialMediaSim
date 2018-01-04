using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public struct DelayGramPostObject
{
    public GameObject postObject;
    public DelayGramPost post;

    public DelayGramPostObject(GameObject _postObject, DelayGramPost _post)
    {
        this.postObject = _postObject;
        this.post = _post;
    }
}

public class PostHelper {
    // private float LENGTH_BETWEEN_POSTS = 3.7f;
    private float WIDTH_BETWEEN_THUMBNAILS = 1.7f;
    private float HEIGHT_BETWEEN_THUMBNAILS = 1.32f;

    // Use this for initialization
    public PostHelper() {
	}

    public void PopulatePostFromData(GameObject post, DelayGramPost data) {
        var postPicture = post.transform.Find("Picture");
        if (postPicture)
        {
            GameObject avatar;
            switch (data.characterProperties.gender)
            {
                case Gender.Male:
                    avatar = postPicture.transform.Find("MaleAvatar").gameObject;
                    postPicture.transform.Find("FemaleAvatar").gameObject.SetActive(false);
                    break;
                case Gender.Female:
                default:
                    avatar = postPicture.transform.Find("FemaleAvatar").gameObject;
                    postPicture.transform.Find("MaleAvatar").gameObject.SetActive(false);
                    break;
            }
            avatar.SetActive(true);
            avatar.transform.localPosition = new Vector3(
                data.avatarPosition.x,
                data.avatarPosition.y,
                data.avatarPosition.z);
            avatar.transform.Rotate(0, 0, data.avatarRotation);
            avatar.transform.localScale = new Vector3(data.avatarScale, data.avatarScale, 1);
            avatar.GetComponent<Animator>().Play("Standing", -1, UnityEngine.Random.Range(0.0f, 1.0f));

            var avatarCustomization = avatar.GetComponent<CharacterCustomization>();
            avatarCustomization.SetCharacterLook(data.characterProperties);
        }

        this.PopulatePostWithItems(postPicture.gameObject, data.items);

        switch (data.backgroundName)
        {
            case "City":
                postPicture.transform.Find("CityBackground").gameObject.SetActive(true);
                break;
            case "Louvre":
                postPicture.transform.Find("LouvreBackground").gameObject.SetActive(true);
                break;
            case "Park":
                postPicture.transform.Find("ParkBackground").gameObject.SetActive(true);
                break;
            case "CamRoom":
                postPicture.transform.Find("CamRoomBackground").gameObject.SetActive(true);
                break;
            case "Beach":
            default:
                postPicture.transform.Find("BeachBackground").gameObject.SetActive(true);
                break;
        }

        var likeDislikeArea = post.transform.Find("LikeDislikeArea");
        var likesText = likeDislikeArea.transform.Find("LikeText");
        float likesPercentage = 100.0f;
        float dislikePercentage = 0.0f;
        if (data.likes + data.dislikes > 0)
        {
            likesPercentage = Mathf.Floor(((float)data.likes) / ((float)(data.likes + data.dislikes)) * 100.0f);
            dislikePercentage = Mathf.Ceil(((float)data.dislikes) / ((float)(data.likes + data.dislikes)) * 100.0f);
        }
        likesText.GetComponent<TextMeshPro>().text = likesPercentage.ToString() + "%";

        var likeDislikeBar = likeDislikeArea.transform.Find("LikeDislikeBar");
        var likeBar = likeDislikeBar.transform.Find("LikeBar");
        if (likeBar)
        {
            likeBar.transform.localScale = new Vector3(
                likesPercentage / 100.0f,
                likeBar.transform.localScale.y,
                likeBar.transform.localScale.z);
        }
        var dislikeBar = likeDislikeBar.transform.Find("DislikeBar");
        if (dislikeBar)
        {
            dislikeBar.transform.localScale = new Vector3(
                dislikePercentage / 100.0f,
                dislikeBar.transform.localScale.y,
                dislikeBar.transform.localScale.z);
        }
    }

    public void SetupProfilePicBubble(GameObject bubble, CharacterProperties properties)
    {
        GameObject avatar;
        switch (properties.gender)
        {
            case Gender.Male:
                avatar = bubble.transform.Find("MaleAvatar").gameObject;
                bubble.transform.Find("FemaleAvatar").gameObject.SetActive(false);
                break;
            case Gender.Female:
            default:
                avatar = bubble.transform.Find("FemaleAvatar").gameObject;
                bubble.transform.Find("MaleAvatar").gameObject.SetActive(false);
                break;
        }
        avatar.SetActive(true);
        var avatarCustomization = avatar.GetComponent<CharacterCustomization>();
        avatarCustomization.SetCharacterLook(properties);
    }

    public List<GameObject> PopulatePostWithItems(GameObject pictureObject, List<PictureItem> items)
    {
        var itemObjects = new List<GameObject>();
        foreach(PictureItem item in items)
        {
            GameObject itemObject = null;
            switch(item.name)
            {
                case "Bulldog":
                    itemObject = GameObject.Instantiate(Resources.Load("Characters/Bulldog") as GameObject);
                    break;
                case "D-Rone":
                    itemObject = GameObject.Instantiate(Resources.Load("Characters/D-Rone") as GameObject);
                    break;
            }
            if (itemObject != null)
            {
                itemObject.name = item.name;
                itemObject.transform.parent = pictureObject.transform;
                itemObject.transform.localPosition = new Vector3(
                    item.location.x, item.location.y, item.location.z);
                itemObject.transform.Rotate(0, 0, item.rotation); //  = Quaternion.Euler(0, 0, item.rotation);
                itemObject.transform.localScale = new Vector3(item.scale, item.scale, 1);
                itemObjects.Add(itemObject);
            }
        }
        return itemObjects;
    }

    public void GeneratePostFeed(
        GameObject scrollArea,
        List<DelayGramPost> posts,
        List<DelayGramPostObject> postObjects,
        float postXOffset,
        float postYOffset)
    {
        var currentX = postXOffset;
        var currentY = scrollArea.transform.localPosition.y + postYOffset;
        foreach (DelayGramPost post in posts)
        {
            var newPost = SetupPostPrefab(post, currentX, currentY, scrollArea, false);

            if (currentX == postXOffset)
            {
                currentX = postXOffset + WIDTH_BETWEEN_THUMBNAILS;
            }
            else
            {
                currentX = postXOffset;
                currentY -= HEIGHT_BETWEEN_THUMBNAILS;
            }
            postObjects.Add(new DelayGramPostObject(newPost, post));
        }

        var scrollController = scrollArea.AddComponent<ScrollController>();
        scrollController.UpdateScrollArea(scrollArea, scrollArea.transform.localPosition.y, -currentY);
    }

    public string GetMessageTimeFromDateTime(DateTime postTime)
    {
        var timeSincePost = DateTime.Now - postTime;
        if (timeSincePost.Days > 0)
        {
            return timeSincePost.Days.ToString() + "d";
        }
        else if (timeSincePost.Hours > 0)
        {
            return timeSincePost.Hours.ToString() + "h";
        }
        else
        {
            return timeSincePost.Minutes.ToString() + "m";
        }
    }

    private GameObject SetupPostPrefab(
        DelayGramPost post,
        float xPosition,
        float yPosition,
        GameObject scrollArea,
        bool showDetails)
    {
        GameObject postPrefab = Resources.Load("Posts/NewPost") as GameObject;

        if (postPrefab)
        {
            var postPrefabInstance = GameObject.Instantiate(postPrefab);
            postPrefabInstance.name = post.imageID;
            postPrefabInstance.transform.parent = scrollArea.transform;
            postPrefabInstance.transform.localPosition = new Vector3(xPosition, yPosition, 0.0f);
            postPrefabInstance.transform.localScale = new Vector3(0.4f, 0.4f, 1.0f);

            this.SetPostDetails(postPrefabInstance, post, showDetails);
            this.PopulatePostFromData(postPrefabInstance, post);

            return postPrefabInstance;
        }
        else
        {
            return null;
        }
    }

    public void SetPostDetails(GameObject postObject, DelayGramPost post, bool showDetails)
    {
        var nameText = postObject.transform.Find("NameText").gameObject;
        if (nameText)
        {
            nameText.SetActive(showDetails);
            nameText.GetComponent<TextMeshPro>().text = post.playerName;
        }

        var timeText = postObject.transform.Find("TimeText").gameObject;
        if (timeText)
        {
            timeText.SetActive(showDetails);
            var timeTextXPosition = Camera.main.ViewportToWorldPoint(new Vector3(0.86f, 0.0f, 0.0f)).x;
            timeText.transform.position = new Vector3(
                timeTextXPosition,
                timeText.transform.position.y,
                timeText.transform.position.z);
            // Need to make rest requester a singleton or try to find the gameobject that has it which is dubious
            // var timeSincePost = DateTime.Now - post.dateTime;
            // timeText.GetComponent<TextMeshPro>().text = this._restRequester.GetPostTimeFromDateTime(timeSincePost);
        }

        var profilePicBubble = postObject.transform.Find("ProfilePicBubble").gameObject;
        if (profilePicBubble)
        {
            profilePicBubble.SetActive(showDetails);
            var profileBubbleXPosition = Camera.main.ViewportToWorldPoint(new Vector3(0.07f, 0.0f, 0.0f)).x;
            profilePicBubble.transform.position = new Vector3(
                profileBubbleXPosition,
                profilePicBubble.transform.position.y,
                profilePicBubble.transform.position.z);
            this.SetupProfilePicBubble(profilePicBubble, post.characterProperties);
        }

        var likeDislikeArea = postObject.transform.Find("LikeDislikeArea").gameObject;
        if (likeDislikeArea)
        {
            likeDislikeArea.SetActive(showDetails);
            var likeDislikeAreaXPosition = Camera.main.ViewportToWorldPoint(new Vector3(0.67f, 0.0f, 0.0f)).x;
            likeDislikeArea.transform.position = new Vector3(
                likeDislikeAreaXPosition,
                likeDislikeArea.transform.position.y,
                likeDislikeArea.transform.position.z);
        }
    }
}
