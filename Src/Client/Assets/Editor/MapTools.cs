﻿
using Common.Data;
using Gameobject;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MapTools
{
    [MenuItem("Map Tools/Export Teleporters")]
    public static void ExportTeleporters()
    {
        DataManager.Instance.Load();

        Scene current = EditorSceneManager.GetActiveScene();
        string currentScene = current.name;
        if (current.isDirty)//是否修改
        {
            EditorUtility.DisplayDialog("提示","请先保存当前场景", "确定");
            return;
        }
        List<TeleporterObject> allTeleporters = new List<TeleporterObject>();
        foreach (var map in DataManager.Instance.Maps)
        {
            string sceneFile = "Assets/Levels/" + map.Value.Resource + ".unity";
            if (!System.IO.File.Exists(sceneFile))
            {
                Debug.LogFormat("Scene{0}不存在", sceneFile);
                return;
            }
            EditorSceneManager.OpenScene(sceneFile, OpenSceneMode.Single);

            TeleporterObject[] teleporters = GameObject.FindObjectsOfType<TeleporterObject>();
            foreach (var teleporter  in teleporters)
            {
                if (!DataManager.Instance.Teleporters.ContainsKey(teleporter.ID))
                {
                    EditorUtility.DisplayDialog("错误", string.Format("地图：{0}中配置的Teleporter:[{1}]中不存在",map.Value.Resource,teleporter.ID),"确定");
                    return;
                }
                TeleporterDefine define = DataManager.Instance.Teleporters[teleporter.ID];
                if (define.MapID!=map.Value.ID)
                {
                    EditorUtility.DisplayDialog("错误", string.Format("地图：{0}中配置的Teleporter：[{1}]MapId:{2}错误",map.Value.Resource,teleporter.ID,define.MapID),"确定");
                    return;
                }
                define.Position = GameObjectTool.WorldToLogicN(teleporter.transform.position);
                define.Direction = GameObjectTool.WorldToLogicN(teleporter.transform.forward);
            }
          
        }
        DataManager.Instance.SaveTeleporters();
        EditorSceneManager.OpenScene("Aessts/Levels/" + current + ".unity");
        EditorUtility.DisplayDialog("提示", "传送点导出完成", "确定");
    }
	
}