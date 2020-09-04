using Assets.Scripts.Managers;
using Common.Data;
using Models;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCController : MonoBehaviour {
    public int npcID;
    private Animator animator;
    private NPCDefine npc;
    private SkinnedMeshRenderer renderer;
    /// <summary>
    /// 原本颜色
    /// </summary>
    private Color orignColor;
    /// <summary>
    /// 是不是进行交互
    /// </summary>
    private bool inIteracive = false;

	// Use this for initialization
	void Start () {
        animator = GetComponent<Animator>();
        npc= NPCManager.Instance.GetNpcDefine(npcID);
        renderer = GetComponentInChildren<SkinnedMeshRenderer>();
        orignColor = renderer.sharedMaterial.color;
        this.StartCoroutine(Actions());
	}
    IEnumerator Actions()
    {
        while (true)
        {
            if (inIteracive)
            {
                yield return new WaitForSeconds(2f);//等待两秒后继续
            }
            else
            {
                yield return new WaitForSeconds(UnityEngine.Random.Range(5f, 10f));
            }
            Relax();
        }       
    }
    private void OnMouseDown()
    {
        Debug.Log(npc.Name);
        Interactive();
    }
    void Interactive()
    {
        if (!inIteracive)
        {
            inIteracive = true;
            StartCoroutine(DoInteractive());
        }
    }
    IEnumerator DoInteractive()
    {
        yield return FaceToPlayer();//面向玩家
        if (NPCManager.Instance.Interactive(npc))
        {
            animator.SetTrigger("Talk");
        }
        //3s后再点NPC才有效
        yield return new WaitForSeconds(3f);
        inIteracive = false;
    }
    IEnumerator FaceToPlayer()
    {
        Vector3 faceto = (User.Instance.currentCharacterObject.transform.position - this.transform.position).normalized;//Vector3.normalizd取模
        while(Mathf.Abs(Vector3.Angle(this.transform.forward,faceto))>5)//小于5度不再插值
        {
           this.transform.forward= Vector3.Lerp(this.transform.forward, faceto, Time.deltaTime * 5f);
            yield return null;
        }
    }
    private void OnMouseEnter()
    {
        Highlight(true);
    }
    private void OnMouseOver()//悬停
    {
        Highlight(true);
    }
    private void OnMouseExit()
    {
        Highlight(false);
    }
    private void Relax()
    {
        animator.SetTrigger("Relax");
    }

   
    private void Highlight(bool highlight)
    {
        if (highlight)
        {
            if (renderer.material.color!=Color.white)
            {
                renderer.material.color = Color.white;
            }
        }
        else
        {
            if (renderer.material.color != orignColor)
            {
                renderer.material.color = orignColor;
            }
        }
    }
}
