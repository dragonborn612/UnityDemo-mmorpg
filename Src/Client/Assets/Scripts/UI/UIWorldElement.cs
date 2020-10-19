using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIWorldElement : MonoBehaviour {
    public Transform owner;
    public float hight = 2.5f;
	// Use this for initialization
	
	
	// Update is called once per frame
	void Update () {
        if (owner != null)
        {
            this.transform.position = owner.position + Vector3.up * hight;
        }
        if (Camera.main!=null)
        {
            this.transform.forward = Camera.main.transform.forward;
        }
	}
}
