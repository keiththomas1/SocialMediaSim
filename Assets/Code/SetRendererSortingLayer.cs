using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetRendererSortingLayer : MonoBehaviour {
    public string _sortingLayer;

	// Use this for initialization
	void Start () {
        this.GetComponent<Renderer>().sortingLayerName = this._sortingLayer;
	}
}
