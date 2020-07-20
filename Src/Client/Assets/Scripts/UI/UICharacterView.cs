using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UICharacterView : MonoBehaviour
{
    public GameObject[] characterModels;
    private int currentCharacter=0;//默认战士 0战士 1法师 2弓箭手

    public int CurrentCharacter
    {
        get
        {
            return currentCharacter;
        }

        set
        {
            currentCharacter = value;
            UpdateCharacter();
        }
    }

    // Use this for initialization
    void Start ()
    {
		
	}
	
	// Update is called once per frame
	void Update ()
    {
		
	}
    private void UpdateCharacter()
    {
        for (int i = 0; i < characterModels.Length; i++)
        {
            if (i==currentCharacter)
            {
                characterModels[i].SetActive(true);
            }
            else
            {
                characterModels[i].SetActive(false);
            }
        }
    }
}
