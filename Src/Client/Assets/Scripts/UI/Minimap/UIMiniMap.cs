using Assets.Scripts.Managers;
using Models;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIMiniMap : MonoBehaviour {//只实例化一次
    public Collider minimapBorundingBox;

    public Image miniMap;//地图本身
    public Image arrow;//箭头
    public Text mapName;

    private Transform playerTransform;
	// Use this for initialization
	void Start ()
    {
        MiniMapManager.Instance.miniMap = this;
        UpdataMap();
       
	}
	
	// Update is called once per frame
	void Update ()
    {
        if (playerTransform==null)
        {
            
            playerTransform = MiniMapManager.Instance.PlayerTansform;
        }
        if (minimapBorundingBox==null||playerTransform==null)
        {
            return;
        }
        float realWeidth = minimapBorundingBox.bounds.size.x;
        float realHight = minimapBorundingBox.bounds.size.z;

        float realX = playerTransform.position.x - minimapBorundingBox.bounds.min.x;
        float realY = playerTransform.position.z - minimapBorundingBox.bounds.min.z;

        float pivotX = realX / realWeidth;
        float pivotY = realY / realHight;

        miniMap.rectTransform.pivot = new Vector2(pivotX, pivotY);
        miniMap.rectTransform .localPosition= Vector2.zero;

        arrow.transform.eulerAngles=new Vector3(0,0,-playerTransform.eulerAngles.y);
	}
    public void UpdataMap()
    {
        mapName.text = User.Instance.currenMapData.Name;
        
        
        miniMap.overrideSprite = MiniMapManager.Instance.LoadcurrentMiniMap();
       
        miniMap.SetNativeSize();//初始化图片尺寸
        miniMap.transform.localPosition = Vector3.zero;//初始化图片位置
        this.minimapBorundingBox = MiniMapManager.Instance.MiniMapBroundingBox;
        this.playerTransform = null;

    }
}
