using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class LoginPanel : PanelBase {

    private InputField idInput, pwInput,hostInput;
    private Button loginBtn, regBtn;
    
    public override void Init(params object[] args)
    {
        base.Init(args);
        skinPath = "LoginPanel";
        layer = PanelkLayer.Panel;
    }

    public override void OnShowing()
    {
        base.OnShowing();
        Transform transform = skin.transform;
        idInput = transform.Find("IDInput").GetComponent<InputField>();
        pwInput = transform.Find("PWInput").GetComponent<InputField>();
        hostInput = transform.Find("hostInput").GetComponent<InputField>();
        loginBtn = transform.Find("LoginBtn").GetComponent<Button>();
        regBtn = transform.Find("RegBtn").GetComponent<Button>();

        loginBtn.onClick.AddListener(OnLoginClick);
        regBtn.onClick.AddListener(OnRegClick);
        hostInput.text = "192.168.50.31";
        if (args.Length>0)
            idInput.text =(string)args[0];
    }

    public void OnRegClick()
    {
        PanelMgr.instance.OpenPanel<RegPanel>("");
        Close();
    }

    public void OnLoginClick()
    {
        if(idInput.text==""||pwInput.text=="")
        {
            Debug.Log("用户名密码不能为空");
            return;
        }
        if (NetMgr.srvConn.status == Connection.Status.None)
        {
            NetMgr.srvConn.hostAddress = hostInput.text;
            NetMgr.srvConn.Connect();
        }
        ProtocolBytes protocol = new ProtocolBytes();
        protocol.AddString("Login");
        protocol.AddString(idInput.text);
        protocol.AddString(pwInput.text);
        NetMgr.srvConn.Send(protocol, OnLoginBack);
    }

    public void OnLoginBack(ProtocolBase protocol)
    {
        ProtocolBytes proto = (ProtocolBytes)protocol;
        int start = 0;
        string name = proto.GetString(start, ref start);
        int ret = proto.GetInt(start, ref start);
        if (ret == 0)
        {
            Debug.Log("登入成功");
            Walk.instance.StartGame(idInput.text);
            //PanelMgr.instance.OpenPanel<RoomListPanel>("RoomListPanel");
            Close();
        }
        else
        {
            Debug.Log("登入失败");
        }
    }
}
