using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Assets.Scripts.UI
{
    class InputBox
    {
        static Object cachObject = null;

        public static UIInputBox Show(string message, string title="", string btnOk = "", string btnCanel = "", string emptyTips = "")
        {
            if (cachObject==null)
            {
                cachObject = Resloader.Load<Object>("UI/UIInputBox");
            }

            GameObject go = (GameObject)GameObject.Instantiate(cachObject);
            UIInputBox inputBox = go.GetComponent<UIInputBox>();
            inputBox.Init(title, message, btnOk, btnCanel, emptyTips);
            return inputBox;
        }
    }
}
