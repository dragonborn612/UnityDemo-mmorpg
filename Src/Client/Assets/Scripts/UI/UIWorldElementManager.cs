using Entities;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIWorldElementManager : MonoSingleton<UIWorldElementManager> {
    public GameObject nameBarPrefab;
    private Dictionary<Transform, GameObject> elements = new Dictionary<Transform, GameObject>();
	
    public void AddCharacterNameBar(Transform owenr,Character character)
    {
        GameObject goNamebar = Instantiate(nameBarPrefab, this.transform);
        goNamebar.name = "NameBar" + character.entityId;
        goNamebar.GetComponent<UINameBar>().characeter = character;
        goNamebar.GetComponent<UIWorldElement>().owner = owenr;
        goNamebar.SetActive(true);
        elements[owenr] = goNamebar;
    }
    public void RemoveCharacterNameBar(Transform owenr)
    {
        if (elements.ContainsKey(owenr))
        {
            Destroy(elements[owenr]);
            elements.Remove(owenr);
        }
    }
}
