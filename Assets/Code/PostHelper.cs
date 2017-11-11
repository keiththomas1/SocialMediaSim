using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PostHelper {

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
            var avatarCustomization = avatar.GetComponent<CharacterCustomization>();
            avatarCustomization.SetCharacterLook(data.characterProperties);
        }

        var itemObjects = this.PopulatePostWithItems(postPicture.gameObject, data.items);

        switch (data.backgroundName)
        {
            case "City":
                postPicture.transform.Find("CityBackground").gameObject.SetActive(true);
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
        foreach(var item in items)
        {
            GameObject itemObject = null;
            switch(item.name)
            {
                case "Bulldog":
                    itemObject = GameObject.Instantiate(Resources.Load("Characters/Bulldog") as GameObject);
                    break;
            }
            if (itemObject != null)
            {
                itemObject.transform.parent = pictureObject.transform;
                itemObject.transform.localPosition = new Vector3(
                    item.location.x, item.location.y, item.location.z);
                itemObjects.Add(itemObject);
            }
        }
        return itemObjects;
    }
}
