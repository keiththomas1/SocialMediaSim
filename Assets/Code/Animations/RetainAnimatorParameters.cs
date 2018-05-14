using System.Collections.Generic;
using UnityEngine;

public class RetainAnimatorParameters : MonoBehaviour {
    private Animator _animator;

    private struct AnimatorBoolParameter
    {
        public string name;
        public bool value;
    }
    private List<AnimatorBoolParameter> _boolParameters
        = new List<AnimatorBoolParameter>();

	// Use this for initialization
	void Awake() {
        this._animator = this.GetComponent<Animator>();
	}

    private void Start()
    {
        var paramaters = this._animator.parameters;
        foreach (var parameter in paramaters)
        {
            var boolParameter = new AnimatorBoolParameter();
            boolParameter.name = parameter.name;
            boolParameter.value = this._animator.GetBool(parameter.name);
            this._boolParameters.Add(boolParameter);
        }
    }

    private void OnEnable()
    {
        foreach (var parameter in this._boolParameters)
        {
            this._animator.SetBool(parameter.name, parameter.value);
        }
    }
}
