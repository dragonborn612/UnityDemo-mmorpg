using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts.UI
{
    public enum WindowResult
    {
        None=0,
        Yes,
        No,
    }
    public  class UIWindow:MonoBehaviour
    {
        public delegate void CloseHander(UIWindow sender, WindowResult windowResult );
        public event CloseHander Onclose;

        public virtual Type Type { get { return this.GetType(); } }

        public void Close(WindowResult windowResult= WindowResult.None)
        {
            UIManager.Instance.Close(this.Type);
            if (this.Onclose!=null)
            {
                Onclose.Invoke(this, windowResult);
            }
            Onclose = null;
        }

        public virtual void OnClikeClose()
        {
            this.Close();
        }

        public virtual void OnClikeYes()
        {
            this.Close(WindowResult.Yes);
        }
    }
}
