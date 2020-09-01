using Common.Data;
using Services;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class TeleporterObject : MonoBehaviour {

    public int ID;
    Mesh mesh = null;

	// Use this for initialization
	void Start () {
        mesh = GetComponent<MeshFilter>().sharedMesh;
	}
	
	// Update is called once per frame
	
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Trigger触发");
        PlayerInputerController pc = other.GetComponent<PlayerInputerController>();
        if (pc!=null&&pc.isActiveAndEnabled)
        {
            TeleporterDefine td = DataManager.Instance.Teleporters[this.ID];
            if (td==null)
            {
                Debug.LogFormat("TeleporterObject:Character[{0}] Enter TeleporterObject[{1}],But TeleporterDefine not exixted",
                    pc.character.nCharacterInfo.Name, this.ID);
                return;
            }
            Debug.LogFormat("TeleporterObject:Character[{0}] Enter TeleporterObject[{1}:{2}]",
                   pc.character.nCharacterInfo.Name, this.ID,td.Name);
            if (td.LinkTo>0)
            {
                if (DataManager.Instance.Teleporters.ContainsKey(td.ID))
                {
                    MapService.Instance.SendMapTeleport(this.ID);
                }
                else
                    Debug.LogFormat("Teleporter ID:{0} LinkId{1} error!", td.ID, td.LinkTo);
            }
        }
    }
#if UNITY_EDITOR
    private void OnDrawGizmos()//gizmo 小玩意儿
    {
        Gizmos.color = Color.red;
        if (this.mesh=null)
        {
            Gizmos.DrawWireMesh(this.mesh, this.transform.position + Vector3.up * this.transform.localScale.y * .5f, this.transform.rotation, this.transform.localScale);

        }
        UnityEditor.Handles.color = Color.red;
        UnityEditor.Handles.ArrowHandleCap(0, this.transform.position, this.transform.rotation, 1f, EventType.Repaint);

    }
#endif
}
