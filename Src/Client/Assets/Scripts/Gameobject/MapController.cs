using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts.Managers;

public class MapController : MonoBehaviour {
    public Collider miniMapBroundingBox;
	// Use this for initialization
	void Start () {
        MiniMapManager.Instance.UpdtaMinimap(miniMapBroundingBox);
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
