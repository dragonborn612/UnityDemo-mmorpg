using UnityEngine;

public class MessageBox
{
    static Object Cacheobject = null;
    public static UIMessageBox Show(string message, string title="",  MessageBoxType type = MessageBoxType.Information, string btnOk = "", string btnCancel = "")
    {
        if (Cacheobject==null)
        {
            Cacheobject = Resloader.Load<Object>("UI/UIMessageBox");

        }
       
        GameObject go = (GameObject)GameObject.Instantiate(Cacheobject);
        UIMessageBox msgbox = go.GetComponent<UIMessageBox>();
        msgbox.Init(title, message, type, btnOk, btnCancel);
        return msgbox;
    }
	
}
public enum MessageBoxType
{
    Information=1,
    Confirm=2,
    Error=3
}
