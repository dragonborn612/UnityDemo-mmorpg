using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SkillBridge.Message;
public class UICharacterSelect : MonoBehaviour {

    
    public Image[] Titles;
    public InputField characterName;
    public Text descs;
    public UICharacterView characterView;
    public Text[] name;

    private CharacterClass characterClass;


    // Use this for initialization
    void Start ()
    {
        DataManager.Instance.Load();
        CareerNameInit();
        CareerInit();

    }
	
	// Update is called once per frame
	void Update () {
		
	}
    /// <summary>
    /// 职业名字动态加载
    /// </summary>
    public void CareerNameInit()
    {
        for (int i = 0; i < name.Length; i++)
        {
            name[i].text = DataManager.Instance.Characters[i + 1].Name;
            Debug.LogFormat("记载{0}", i+1);
        }
    }
    public void OnSelectClass(int characterClass)
    {
        this.characterClass = (CharacterClass)characterClass;
        characterView.CurrentCharacter = characterClass;
        for (int i = 0; i < Titles.Length; i++)
        {
            if (i==characterClass)
            {
                Titles[i].gameObject.SetActive(true);
            }
            else
            {
                Titles[i].gameObject.SetActive(false);
            }
        }
        descs.text = DataManager.Instance.Characters[characterClass + 1].Description;
    }
    /// <summary>
    /// 除始化 职业模型、标题、描述
    /// </summary>
    public void CareerInit()
    {
        for (int i = 0; i < 3; i++)
        {
            if (i==0)
            {
                characterView.characterModels[i].SetActive(true);
                Titles[i].gameObject.SetActive(true);
            }
            else
            {
                characterView.characterModels[i].SetActive(false);
                Titles[i].gameObject.SetActive(false);
            }
        }
        descs.text = DataManager.Instance.Characters[1].Description;
    }
    public void OnClickCreate()
    {
        //验证用户名是不是空
        //验证完后 发送服务器 参数(角色名字，角色职业)
    }
}
