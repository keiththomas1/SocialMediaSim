using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class LoadingAnimation : MonoBehaviour {
    [SerializeField]
    private GameObject _picture;

	// Use this for initialization
	void Start () {
        this.RotatePicture(0);
    }
	
	// Update is called once per frame
	void Update () {
		
	}
    
    private void RotatePicture(float delay)
    {
        this._picture.transform.DOLocalRotate(new Vector3(0.0f, 360.0f, 0.0f), 1.7f, RotateMode.LocalAxisAdd)
            .SetDelay(delay)
            .SetEase(Ease.OutCubic)
            .OnComplete(() => this.RotatePicture(0.4f));
    }
}
