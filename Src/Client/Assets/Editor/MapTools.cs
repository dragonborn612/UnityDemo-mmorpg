
using Common.Data;
using Gameobject;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using Assets.Scripts.Gameobject;

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



	[MenuItem("Map Tools/Export spawnPoint")]
    public static void ExportSpawnPoint()
    {
        DataManager.Instance.Load();

        Scene current = EditorSceneManager.GetActiveScene();
        string currentScene = current.name;
        if (current.isDirty)//是否修改
        {
            EditorUtility.DisplayDialog("提示", "请先保存当前场景", "确定");
            return;
        }
        if (DataManager.Instance.SpawnPoints==null)
        {
            DataManager.Instance.SpawnPoints = new Dictionary<int, Dictionary<int, SpawnPointDefine>>();
        }
        foreach (var map in DataManager.Instance.Maps)
        {
            string sceneFile = "Assets/Levels/" + map.Value.Resource + ".unity";
            if (!System.IO.File.Exists(sceneFile))
            {
                Debug.LogWarningFormat("Scene {0} not existed!", sceneFile);
                continue;
            }
            EditorSceneManager.OpenScene(sceneFile, OpenSceneMode.Single);
            SpawnPoint[] spawnPoints = GameObject.FindObjectsOfType<SpawnPoint>();

            if (!DataManager.Instance.SpawnPoints.ContainsKey(map.Value.ID))
            {
                DataManager.Instance.SpawnPoints[map.Value.ID] = new Dictionary<int, SpawnPointDefine>();
            }
            foreach (var sp in spawnPoints)
            {
                if (!DataManager.Instance.SpawnPoints[map.Value.ID].ContainsKey(sp.ID))
                {
                    DataManager.Instance.SpawnPoints[map.Value.ID][sp.ID] = new SpawnPointDefine();
                }

                SpawnPointDefine def = DataManager.Instance.SpawnPoints[map.Value.ID][sp.ID];
                def.ID = sp.ID;
                def.MapID = map.Value.ID;
                def.Position = GameObjectTool.WorldToLogicN(sp.transform.position);
                def.Direction = GameObjectTool.WorldToLogicN(sp.transform.forward);
            }
        }
        DataManager.Instance.SaveSpawnPoints();
        EditorSceneManager.OpenScene("Assets/Levels/" + currentScene + ".unity");
        EditorUtility.DisplayDialog("提示","刷怪点导出完成", "确定");
    }

}
