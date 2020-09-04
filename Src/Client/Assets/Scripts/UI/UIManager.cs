﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts.UI
{
    class UIManager:Singleton<UIManager>
    {
        class UIElment
        {
           public string Resouce;
           public bool  Cache;
           public GameObject Instance;
        }
        Dictionary<Type, UIElment> UIResouces = new Dictionary<Type, UIElment>();
        public UIManager()
        {
            UIResouces.Add(typeof(UITest), new UIElment() { Resouce = "UI/UITest", Cache = true, });
        }
        ~UIManager()
        {

        }


        public T Show<T>()
        {
            Type type = typeof(T);
            if (UIResouces.ContainsKey(type))
            {
                UIElment info = UIResouces[type];
                if (info.Instance==null)
                {
                    UnityEngine.Object prefab = Resources.Load(info.Resouce);
                    if (prefab==null)
                    {
                        return default(T);
                    }
                    info.Instance =(GameObject) GameObject.Instantiate(prefab);
                }
                else
                {
                    info.Instance.SetActive(true);
                }
                return info.Instance.GetComponent<T>();
            }
            return default(T);
        }

        public void Close(Type type)
        {
            if (UIResouces.ContainsKey(type))
            {
                UIElment info = UIResouces[type];
                if (info.Cache)
                {
                    info.Instance.SetActive(false);
                }
                else
                {
                    GameObject.Destroy(info.Instance);
                    info.Instance = null;
                }
            }
        }
    }
}