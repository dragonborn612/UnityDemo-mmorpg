using UnityEngine;


public abstract class MonoSingleton<T> : MonoBehaviour where T : MonoBehaviour
{
    public bool global = true;//是否全局使用
    static T instance;
    public static T Instance//如果没有【1】的话赋值的时候为第一次在其他地方使用
    {
        get
        {
            if (instance == null)
            {
                instance =(T)FindObjectOfType<T>();
            }
            return instance;
        }

    }

    void Awake()//子类的逻辑不能用Awake(),不然会被子类覆盖不调用这里Awake(),要重写OnStart();
    {
        if (global)
        {
            if (instance!=null&&instance!=this.gameObject.GetComponent<T>())
            {
                Destroy(this.gameObject);
                return;
            }
            DontDestroyOnLoad(this.gameObject);
            instance = this.gameObject.GetComponent<T>();//【1】
            //没有new是因为游戏一运行就实例好了，因为是monobehaviour类
        }
           
        this.OnStart();
    }

    protected virtual void OnStart()
    {

    }
}