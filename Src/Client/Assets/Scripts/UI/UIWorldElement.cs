using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIWorldElement : MonoBehaviour {
    public Transform owner;
    public float hight = 1.5f;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if (owner != null)
        {
            this.transform.position = owner.position + Vector3.up * hight;
        }
	}
}
