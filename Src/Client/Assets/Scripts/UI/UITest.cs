using Assets.Scripts.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UITest : UIWindow {

    public Text title;

    internal void UpdataTitle(string title)
    {
        this.title.text = title;
    }
}
