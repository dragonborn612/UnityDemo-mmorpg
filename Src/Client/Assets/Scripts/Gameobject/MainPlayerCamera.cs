using Models;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainPlayerCamera : MonoSingleton<MainPlayerCamera> {

    public Camera camera;
    public GameObject player;
    public Transform viewPoint;
	
    private void LateUpdate()//每帧后调用
    {
        if (player==null)
        {
            player = User.Instance.currentCharacterObject;
        }
        if (player==null)
        {
            return;
        }
        this.transform.position = player.transform.position;
        this.transform.rotation = player.transform.rotation;
    }
}
