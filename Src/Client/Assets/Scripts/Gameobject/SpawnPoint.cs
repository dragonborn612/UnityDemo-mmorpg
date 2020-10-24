using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEditor;

namespace Assets.Scripts.Gameobject
{[ExecuteInEditMode]//ExecuteInEditMode属性的作用是在EditMode下也可以执行脚本。
    public class SpawnPoint : MonoBehaviour
    {
        Mesh mesh = null;
        public int ID;

        void Start()
        {
            this.mesh = this.GetComponent<MeshFilter>().sharedMesh;
        }
#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            Vector3 pos = this.transform.position + Vector3.up * this.transform.localScale.y * 0.5f;
            Gizmos.color = Color.red;
            if (this.mesh!=null)
            {
                Gizmos.DrawWireMesh(this.mesh, pos, this.transform.rotation, this.transform.localScale);
            }
            Handles.color = Color.red;
            Handles.ArrowHandleCap(0,pos,this.transform.rotation,1f,EventType.Repaint);//画箭头
            Handles.Label(pos, "spawnPoint:" + this.ID);
        }
#endif

    }

}
