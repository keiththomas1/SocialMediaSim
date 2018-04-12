using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;

/// <summary>
/// Color picker for UI Image
/// 
/// Last Update: 5.2.2018 [d/m/y]
/// Author: Mato Vanco
/// Organization: Matt's Creations
/// </summary>
[AddComponentMenu("Matt's Creations/Color Picker")]
[RequireComponent(typeof(Image))]
public class ColorPicker : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler, IDragHandler
{
    [Header("* Texture Source")]
    public Texture2D TextureSource; //---Texture Source [color palette]
    [Header("* Image Pointer")]
    public Image Pointer; //---Color Pointer

    [Space(15)]
    [Header("Final Color Result")]
    public Color Result; //---Final Result

    [Space(15)]
    [Header("Picking Color Event")]
    public UnityEvent Event; //---Event if user is picking a color

    private Image Texture_Image; //---Current texture image

    private Vector2 TextureSize; //---Original texture size

    private bool cursorEnter = false; //---Overlaping image with cursor

    #region Pointer Events
    public void OnPointerEnter(PointerEventData e)
    {
        cursorEnter = true;
    }
    public void OnPointerExit(PointerEventData e)
    {
        cursorEnter = false;
    }
    public void OnPointerClick(PointerEventData e)
    {
        OnPickingColor();
    }
    public void OnDrag(PointerEventData e)
    {
        OnPickingColor();
    }
    private void OnPickingColor()
    {
        if (!cursorEnter)
            return;

        var mpos = Input.mousePosition;

        Pointer.transform.position = mpos;

        mpos = Texture_Image.rectTransform.InverseTransformPoint(mpos);

        Texture_Image.rectTransform.sizeDelta = TextureSize;

        if ((mpos.x <= TextureSize.x && mpos.x >= 0) && (mpos.y <= TextureSize.y && mpos.y >= 0))
            Result = TextureSource.GetPixel((int)mpos.x, (int)mpos.y);

        if (Event != null)
            Event.Invoke();
    }
    #endregion

    private void Awake()
    {
        Texture_Image = GetComponent<Image>();
        if (!TextureSource || !Texture_Image || !Pointer)
        {
            Debug.LogError("Color Picker: Missing script objects. Please check all required objects.");
            if (!Texture_Image)
                Debug.LogError("Color Picker - Additional: The script is not assigned to the UI Image object!");
            this.enabled = false;
            return;
        }

        TextureSize = new Vector2(TextureSource.width, TextureSource.height);
    }

    /// <summary>
    /// Show/hide color picker panel by its activation
    /// </summary>
    public void PUBLIC_ShowHide()
    {
        if (gameObject.activeInHierarchy)
            gameObject.SetActive(false);
        else
            gameObject.SetActive(true);
    }
    /// <summary>
    /// Show/ hide color picker panel by boolean
    /// </summary>
    public void PUBLIC_ShowHide(bool Active)
    {
        gameObject.SetActive(Active);
    }

    /// <summary>
    /// Get picked color
    /// </summary>
    /// <returns>returns final picked color</returns>
    public Color PUBLIC_GetColor()
    {
        return Result;
    }

    #region Set-Colors
    /// <summary>
    /// Set color to UI image
    /// </summary>
    public void PUBLIC_SetColor(Image @Image)
    {
        Image.color = Result;
    }
    /// <summary>
    /// Set color to sprite
    /// </summary>
    public void PUBLIC_SetColor(SpriteRenderer @SpriteRenderer)
    {
        SpriteRenderer.color = Result;
    }
    /// <summary>
    /// Set color to UI text
    /// </summary>
    public void PUBLIC_SetColor(Text @Text)
    {
        Text.color = Result;
    }
    /// <summary>
    /// Set color to Mesh Renderer material color
    /// </summary>
    public void PUBLIC_SetColor(MeshRenderer @MeshRenderer)
    {
        @MeshRenderer.material.color = Result;
    }
    /// <summary>
    /// Set color to Material
    /// </summary>
    public void PUBLIC_SetColor(Material @Material)
    {
        Material.color = Result;
    }
    /// <summary>
    /// Set color to TextMesh
    /// </summary>
    public void PUBLIC_SetColor(TextMesh @TextMesh)
    {
        TextMesh.color = Result;
    }
    /// <summary>
    /// Set color to Renderer objects with typed Tag
    /// </summary>
    public void PUBLIC_SetColor_FindObjectsByTag(string Tag)
    {
        foreach (GameObject gm in GameObject.FindGameObjectsWithTag(Tag))
        {
            if(gm.GetComponent<Renderer>())
                gm.GetComponent<Renderer>().material.color = Result;
        }
    }
    private string targetVar;
    /// <summary>
    /// MONOBEHAVIOUR CONNECTOR: Set Color in internal variable
    /// </summary>
    public void PUBLIC_SetColor_Mono(string VariableName)
    {
        targetVar = VariableName;
    }
    /// <summary>
    /// MONOBEHAVIOUR CONNECTOR: Set Color in internal variable
    /// </summary>
    public void PUBLIC_SetColor_Mono(MonoBehaviour @MonoBehaviour)
    {
        if (MonoBehaviour.GetType().GetField(targetVar) != null && MonoBehaviour.GetType().GetField(targetVar).GetValue(MonoBehaviour).GetType() == typeof(Color))
            MonoBehaviour.GetType().GetField(targetVar).SetValue(MonoBehaviour, Result);
    }
    /// <summary>
    /// Set Color in internal Monobehaviour variable
    /// </summary>
    public void PUBLIC_SetColor_Mono(MonoBehaviour @MonoBehaviour, string Variable)
    {
        if (MonoBehaviour.GetType().GetField(Variable) != null && MonoBehaviour.GetType().GetField(Variable).GetValue(MonoBehaviour).GetType() == typeof(Color))
            MonoBehaviour.GetType().GetField(Variable).SetValue(MonoBehaviour, Result);
    }
    #endregion
}
