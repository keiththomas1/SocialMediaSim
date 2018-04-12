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

    private Tweener _growScaleTween = null;
    private Tweener _growMoveTween = null;

    // Use this for initialization
    public PostHelper() {
	}

    public void PopulatePostFromData(GameObject post, DelayGramPost data) {
        var postPicture = post.transform.Find("Picture");

        GameObject itemsParent = postPicture.gameObject;
        GameObject background = null;
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
            case "Yacht":
                var yachtBackground = postPicture.transform.Find("YachtBackground");
                if (yachtBackground)
                {
                    background = yachtBackground.gameObject;
                    itemsParent = background.transform.Find("YachtBoat").gameObject;
                }
                break;
            case "Beach":
            default:
                background = postPicture.transform.Find("BeachBackground").gameObject;
                break;
        }
        if (background)
        {
            background.SetActive(true);
        }

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
            avatar.transform.SetParent(itemsParent.transform);
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

        this.PopulatePostWithItems(itemsParent, data.items);

        // Randomize the start time for all animations
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
        var totalFeedback = data.likes + data.dislikes;
        if (totalFeedback > 0)
        {
            likesPercentage = ((float)data.likes) / ((float)totalFeedback);
            dislikePercentage = ((float)data.dislikes) / ((float)totalFeedback);
        }

        var likeBar = likeDislikeArea.Find("LikeBar");
        if (likeBar)
        {
            var scaleAt100Percent = likeBar.transform.localScale.x * 2.0f;
            likeBar.transform.localScale = new Vector3(
                likesPercentage * scaleAt100Percent,
                likeBar.transform.localScale.y,
                likeBar.transform.localScale.z);
        }
        var dislikeBar = likeDislikeArea.transform.Find("DislikeBar");
        if (dislikeBar)
        {
            var scaleAt100Percent = dislikeBar.transform.localScale.x * 2.0f;
            dislikeBar.transform.localScale = new Vector3(
                dislikePercentage * scaleAt100Percent,
                dislikeBar.transform.localScale.y,
                dislikeBar.transform.localScale.z);
        }
    }

    public void SetupAvatarMask(GameObject mask, CharacterProperties properties)
    {
        mask.transform.Find("FemaleAvatar").gameObject.SetActive(false);
        mask.transform.Find("MaleAvatar").gameObject.SetActive(false);

        GameObject avatar = null;
        switch (properties.gender)
        {
            case Gender.Male:
                avatar = mask.transform.Find("MaleAvatar").gameObject;
                avatar.SetActive(true);
                break;
            case Gender.Female:
                avatar = mask.transform.Find("FemaleAvatar").gameObject;
                avatar.SetActive(true);
                break;
            default:
                break;
        }
        var avatarCustomization = avatar.GetComponent<CharacterCustomization>();
        avatarCustomization.SetCharacterLook(properties);
    }

    public List<GameObject> PopulatePostWithItems(GameObject parent, List<PictureItem> items)
    {
        var itemObjects = new List<GameObject>();
        foreach(PictureItem item in items)
        {
            GameObject itemObject = null;
            switch(item.name)
            {
                case "Cat":
                    itemObject = GameObject.Instantiate(Resources.Load("Characters/Cat") as GameObject);
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
                itemObject.transform.parent = parent.transform;
                itemObject.transform.localPosition = new Vector3(
                    item.location.x, item.location.y, item.location.z);
                itemObject.transform.Rotate(0, 0, item.rotation);
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
            postPrefabInstance.name = post.pictureID;
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
        var timeText = postObject.transform.Find("TimeText");
        if (timeText)
        {
            timeText.gameObject.SetActive(showDetails);
            // Need to make rest requester a singleton or try to find the gameobject that has it which is dubious
            var timeSincePost = DateTime.Now - post.dateTime;
            timeText.GetComponent<TextMeshPro>().text = PostRequester.GetPostTimeFromTimeSpan(timeSincePost);
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

        var likeText = postObject.transform.Find("LikeText");
        if (likeText)
        {
            likeText.gameObject.SetActive(showDetails);
            var likeXPosition = Camera.main.ViewportToWorldPoint(new Vector3(0.0f, 0.0f, 0.0f)).x;
            likeText.position = new Vector3(
                likeXPosition,
                likeText.position.y,
                likeText.position.z);
            likeText.GetComponent<TextMeshPro>().text = String.Format("{0} Likes", post.likes.ToString());
        }
        var dislikeText = postObject.transform.Find("DislikeText");
        if (dislikeText)
        {
            dislikeText.gameObject.SetActive(showDetails);
            var dislikeXPosition = Camera.main.ViewportToWorldPoint(new Vector3(1.0f, 0.0f, 0.0f)).x;
            dislikeText.position = new Vector3(
                dislikeXPosition,
                dislikeText.position.y,
                dislikeText.position.z);
            dislikeText.GetComponent<TextMeshPro>().text = String.Format("{0} Dislikes", post.dislikes.ToString());
        }

        var postShadow = postObject.transform.Find("DropShadow");
        if (postShadow)
        {
            postShadow.gameObject.SetActive(showPostShadow);
        }
    }

    public GameObject EnlargeAndCenterPost(DelayGramPostObject post)
    {
        // First, disable drop shadow
        this.SetPostDetails(post.postObject, post.post, false, false);

        // Scale post up and position in middle of screen
        // Both InOutQuint and InOutBack are decent options also
        this._growScaleTween = post.postObject.transform.DOScale(1.0f, 0.4f).SetEase(Ease.OutBack);
        var middleScreenPosition = Camera.main.ViewportToWorldPoint(new Vector3(0.5f, 0.45f, 0.0f));
        middleScreenPosition.z = -2.0f;
        this._growMoveTween = post.postObject.transform.DOMove(middleScreenPosition, 0.5f, false)
            .OnComplete(() => {
                this.SetPostDetails(post.postObject, post.post, true, false); });
        var userStub = this.CreateUserStub(post, middleScreenPosition);

        return userStub;
    }

    private GameObject CreateUserStub(DelayGramPostObject post, Vector3 position)
    {
        var userStub = GameObject.Instantiate(Resources.Load("Posts/UserStub") as GameObject);
        userStub.name = "UserStub";
        userStub.transform.position = position; // post.postObject.transform.position;
        // userStub.transform.SetParent(post.postObject.transform);

        var nameText = userStub.transform.Find("NameText");
        if (nameText)
        {
            nameText.gameObject.SetActive(true);
            nameText.GetComponent<TextMeshPro>().text = post.post.playerName;
        }

        var userPageLink = userStub.transform.Find("UserPageLink");
        if (userPageLink)
        {
            userPageLink.gameObject.SetActive(true);
        }

        var portraitMask = userStub.transform.Find("PortraitMask");
        if (portraitMask)
        {
            portraitMask.gameObject.SetActive(true);
            this.SetupAvatarMask(portraitMask.gameObject, post.post.characterProperties);
        }

        var portraitLevelBanner = userStub.transform.Find("PortraitLevelBanner");
        if (portraitLevelBanner)
        {
            var bannerText = portraitLevelBanner.Find("BannerText");
            bannerText.GetComponent<TextMeshPro>().text =
                String.Format("Level {0}", post.post.characterProperties.avatarLevel);
        }

        return userStub;
    }

    public void ShrinkAndReturnPost(
        DelayGramPostObject post,
        Vector3 originalScale,
        Vector3 originalPosition,
        TweenCallback callback)
    {
        if (!this._growScaleTween.IsComplete())
        {
            this._growScaleTween.Kill();
        }
        if (!this._growMoveTween.IsComplete())
        {
            this._growMoveTween.Kill();
        }

        var userStub = post.postObject.transform.Find("UserStub");
        if (userStub)
        {
            GameObject.Destroy(userStub.gameObject);
        }
        if (post.postObject)
        {
            post.postObject.transform.DOScale(originalScale, 0.4f).SetEase(Ease.OutBack); // .InOutBack);
            post.postObject.transform.DOLocalMove(originalPosition, 0.5f, false).OnComplete(callback);
        }
    }
}
