using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Assets.Scripts.UI
{
    public  class UIInputBox:MonoBehaviour
    {
        public Text title;
        public Text message;
        public Text tips;
        public Button buttonYes;
        public Button buttonNo;
        public InputField input;

        public Text buttonYesTitle;
        public Text buttonNoTitle;

        public delegate bool SubmitHandler(string input, out string tips);
        public event SubmitHandler OnSubmit;
        public UnityAction OnCanel;

        public string emptyTips;

        public void Init(string title, string message, string btnOk = "", string btnCanel = "", string emptyTips = "")
        {
            if (!string.IsNullOrEmpty(title))
            {
                this.title.text = title;
            }
            this.message.text = message;
            this.tips.text = null;
            this.OnSubmit = null;
            this.emptyTips = emptyTips;

            if (!string.IsNullOrEmpty(btnOk)) this.buttonNoTitle.text = title;
            if (!string.IsNullOrEmpty(btnCanel)) this.buttonNoTitle.text = title;

            this.buttonYes.onClick.AddListener(OnClickYes);
            this.buttonNo.onClick.AddListener(OnClickNo);
        }

        private void OnClickNo()
        {
            Destroy(this.gameObject);
            if (this.OnCanel!=null)
            {
                this.OnCanel.Invoke();
            }
        }

        private void OnClickYes()
        {
            this.tips.text = "";
            if (string.IsNullOrEmpty(input.text))
            {
                this.tips.text = this.emptyTips;
                return;
            }
            if (OnSubmit!=null)
            {
                string tips;
                if (!OnSubmit(this.input.text,out tips))
                {
                    this.tips.text = tips;
                    return;
                }
            }
            Destroy(this.gameObject);
        }
    }
}
