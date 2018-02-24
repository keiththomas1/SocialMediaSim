using DG.Tweening;
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

        GameObject background;
        switch (data.backgroundName)
        {
            case "City":
                background = postPicture.transform.Find("CityBackground").gameObject;
                break;
            case "Louvre":
                background = postPicture.transform.Find("LouvreBackground").gameObject;
                break;
            case "Park":
                background = postPicture.transform.Find("ParkBackground").gameObject;
                break;
            case "CamRoom":
                background = postPicture.transform.Find("CamRoomBackground").gameObject;
                break;
            case "Beach":
            default:
                background = postPicture.transform.Find("BeachBackground").gameObject;
                break;
        }
        background.SetActive(true);

        // Look at all animator components in the background and it's children and randomize start time
        var components = background.transform.GetComponentsInChildren<Animator>(true);
        var componentsList = new List<Animator>(components);
        if (background.GetComponent<Animator>())
        {
            componentsList.Add(background.GetComponent<Animator>());
        }
        foreach (Animator animator in componentsList)
        {
            var clipInfo = animator.GetCurrentAnimatorClipInfo(0);
            var info = animator.GetCurrentAnimatorStateInfo(0);
            if (clipInfo.Length > 0)
            {
                var clip = clipInfo[0].clip;
                animator.Play(info.shortNameHash, 0, clip.length * UnityEngine.Random.value);
            }
        }

        var likeDislikeArea = post.transform.Find("LikeDislikeArea");
        float likesPercentage = 100.0f;
        float dislikePercentage = 0.0f;
        if (data.likes + data.dislikes > 0)
        {
            likesPercentage = Mathf.Floor(((float)data.likes) / ((float)(data.likes + data.dislikes)) * 100.0f);
            dislikePercentage = Mathf.Ceil(((float)data.dislikes) / ((float)(data.likes + data.dislikes)) * 100.0f);
        }

        var likeBar = likeDislikeArea.Find("LikeBar");
        if (likeBar)
        {
            likeBar.transform.localScale = new Vector3(
                likesPercentage / 100.0f,
                likeBar.transform.localScale.y,
                likeBar.transform.localScale.z);
        }
        var dislikeBar = likeDislikeArea.transform.Find("DislikeBar");
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
                case "Sylvester":
                    itemObject = GameObject.Instantiate(Resources.Load("Characters/Sylvester") as GameObject);
                    break;
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

                // Look at all of the animations on the object and it's children and randomize the start time
                var components = itemObject.transform.GetComponentsInChildren<Animator>();
                var componentsList = new List<Animator>(components);
                if (itemObject.GetComponent<Animator>())
                {
                    componentsList.Add(itemObject.GetComponent<Animator>());
                }
                foreach(Animator animator in componentsList)
                {
                    var clipInfo = animator.GetCurrentAnimatorClipInfo(0);
                    if (clipInfo.Length > 0)
                    {
                        AnimationClip clip = clipInfo[0].clip;
                        animator.Play(clip.name, 0, clip.length * UnityEngine.Random.value);
                    }
                }

                itemObjects.Add(itemObject);
            }
        }
        return itemObjects;
    }

    // Create a two-photo-width feed within the given scrollArea
    // Fill postObjects with the created posts, given data from posts
    // Returns the length of the feed
    public float GeneratePostFeed(
        GameObject scrollArea,
        List<DelayGramPost> posts,
        List<DelayGramPostObject> postObjects,
        float postXOffset,
        float postYOffset)
    {
        var currentX = postXOffset;
        var currentY = postYOffset;
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

        return postYOffset - currentY + HEIGHT_BETWEEN_THUMBNAILS;
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

            this.SetPostDetails(postPrefabInstance, post, showDetails, true);
            this.PopulatePostFromData(postPrefabInstance, post);

            return postPrefabInstance;
        }
        else
        {
            return null;
        }
    }

    public void SetPostDetails(GameObject postObject, DelayGramPost post, bool showDetails, bool showPostShadow)
    {
        var userHeader = postObject.transform.Find("UserHeader");
        var userHeaderXPosition = Camera.main.ViewportToWorldPoint(new Vector3(0.07f, 0.0f, 0.0f)).x;
        // Position User Header at correct x position for different screen sizes
        userHeader.position = new Vector3(
            userHeaderXPosition,
            userHeader.position.y,
            userHeader.position.z);

        var nameText = userHeader.transform.Find("NameText");
        if (nameText)
        {
            nameText.gameObject.SetActive(showDetails);
            nameText.GetComponent<TextMeshPro>().text = post.playerName;
        }

        var userPageLink = postObject.transform.Find("UserPageLink");
        if (userPageLink)
        {
            userPageLink.gameObject.SetActive(showDetails);
        }

        var timeText = postObject.transform.Find("TimeText");
        if (timeText)
        {
            timeText.gameObject.SetActive(showDetails);
            var timeTextXPosition = Camera.main.ViewportToWorldPoint(new Vector3(0.86f, 0.0f, 0.0f)).x;
            timeText.transform.position = new Vector3(
                timeTextXPosition,
                timeText.transform.position.y,
                timeText.transform.position.z);
            // Need to make rest requester a singleton or try to find the gameobject that has it which is dubious
            // var timeSincePost = DateTime.Now - post.dateTime;
            // timeText.GetComponent<TextMeshPro>().text = this._restRequester.GetPostTimeFromDateTime(timeSincePost);
        }

        var profilePicBubble = userHeader.transform.Find("ProfilePicBubble");
        if (profilePicBubble)
        {
            profilePicBubble.gameObject.SetActive(showDetails);
            this.SetupProfilePicBubble(profilePicBubble.gameObject, post.characterProperties);
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

        var postShadow = postObject.transform.Find("DropShadow");
        if (postShadow)
        {
            postShadow.gameObject.SetActive(showPostShadow);
        }
    }

    public void EnlargeAndCenterPost(DelayGramPostObject post)
    {
        // First, disable drop shadow
        this.SetPostDetails(post.postObject, post.post, false, false);

        // Scale post up and position in middle of screen
        post.postObject.transform.DOScale(1.0f, 0.5f).SetEase(Ease.InOutBack);
        var middleScreenPosition = Camera.main.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, 0.0f));
        middleScreenPosition.z = 0.0f;
        post.postObject.transform.DOMove(middleScreenPosition, 0.5f, false)
            .OnComplete(() =>
                this.SetPostDetails(post.postObject, post.post, true, false));
    }

    public void ShrinkAndReturnPost(
        DelayGramPostObject post,
        Vector3 originalScale,
        Vector3 originalPosition,
        TweenCallback callback)
    {
        if (post.postObject)
        {
            post.postObject.transform.DOScale(originalScale, 0.5f).SetEase(Ease.InOutBack);
            post.postObject.transform.DOLocalMove(originalPosition, 0.5f, false).OnComplete(callback);
        }
    }
}
