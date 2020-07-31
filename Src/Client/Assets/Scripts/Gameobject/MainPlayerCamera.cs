using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainPlayerCamera : MonoSingleton<MainPlayerCamera> {

    public Camera camera;
    public GameObject player;
    public Transform viewPoint;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
    private void LateUpdate()//每帧后调用
    {
        if (player==null)
        {
            return;
        }
        this.transform.position = player.transform.position;
        this.transform.rotation = player.transform.rotation;
    }
}
