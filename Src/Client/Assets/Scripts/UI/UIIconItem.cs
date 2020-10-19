using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIIconItem : MonoBehaviour {

    public Image mainImage;
    public Image secondImage;

    public Text mainText;


    public void SetMainIcon(string iconName ,string text=null)
    {
        if (mainText!=null)
        {
            this.mainText.text = text;
        }
        this.mainImage.overrideSprite = Resloader.Load<Sprite>(iconName);
        
    }
    public void SetMainIcon (Sprite Icon)
    {
        this.mainImage.overrideSprite = Icon;
    }
}
