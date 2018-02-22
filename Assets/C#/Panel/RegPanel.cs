using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RegPanel : PanelBase {

    private InputField idInput, pwInput;
    private Button regBtn, closeBtn;

    public override void Init(params object[] args)
    {
        base.Init(args);
        skinPath = "RegPanel";
        layer = PanelkLayer.Panel;
    }

    public override void OnShowing()
    {
        base.OnShowing();
        Transform transform = skin.transform;
        idInput = transform.Find("IDInput").GetComponent<InputField>();
        pwInput = transform.Find("PWInput").GetComponent<InputField>();
        closeBtn = transform.Find("CloseBtn").GetComponent<Button>();
        regBtn = transform.Find("RegBtn").GetComponent<Button>();

        closeBtn.onClick.AddListener(OnCloseClick);
        regBtn.onClick.AddListener(OnRegClick);
    }

    public void OnCloseClick()
    {
        PanelMgr.instance.OpenPanel<LoginPanel>("");
        Close();
    }

    public void OnRegClick()
    {
        if (idInput.text == "" || pwInput.text == "")
        {
            Debug.Log("用户名密码不能为空");
            return;
        }
        if (NetMgr.srvConn.status == Connection.Status.None)
        {
            NetMgr.srvConn.Connect();
        }
        ProtocolBytes protocol = new ProtocolBytes();
        protocol.AddString("Register");
        protocol.AddString(idInput.text);
        protocol.AddString(pwInput.text);
        NetMgr.srvConn.Send(protocol, OnRegBack);
    }

    public void OnRegBack(ProtocolBase protocol)
    {
        ProtocolBytes proto = (ProtocolBytes)protocol;
        int start = 0;
        string name = proto.GetString(start, ref start);
        int ret = proto.GetInt(start, ref start);
        if (ret == 0)
        {
            Debug.Log("注册成功");
            Close();
            PanelMgr.instance.OpenPanel<LoginPanel>("",idInput.text);

        }
        else
        {
            Debug.Log("注册失败");
        }
    }
}
