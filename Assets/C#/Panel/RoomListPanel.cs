using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RoomListPanel : PanelBase {

    public Text playerNameText, recordText;
    public Button addBtn;
    public Transform content,roomInfoTrans;
    public override void Init(params object[] args)
    {
        base.Init(args);
        skinPath = "RoomListPanel";
        layer = PanelkLayer.Panel;
    }

    public override void OnShowing()
    {
        base.OnShowing();
        Transform transform = skin.transform;
        playerNameText = transform.Find("Left/PlayerName").GetComponent<Text>();
        recordText = transform.Find("Left/Record").GetComponent<Text>();
        addBtn = transform.Find("Right/CreateBtn").GetComponent<Button>();
        content = transform.Find("Right/RoomList/Viewport/Content");
        roomInfoTrans = (Resources.Load("RoomInfo")as GameObject).transform;


        addBtn.onClick.AddListener(NewRoomClick);
        NetMgr.srvConn.msgDistribution.AddListener("GetRoomList", RecvGetRoomList);
        NetMgr.srvConn.msgDistribution.AddListener("CreateRoom", RecvCreateRoom);
        GetInfo();
        GetRoomList();
    }

    void GetInfo()
    {
        ProtocolBytes protocol = new ProtocolBytes();
        protocol.AddString("GetInfo");
        NetMgr.srvConn.Send(protocol, RecvGetInfo);
    }

    void GetRoomList()
    {
        ProtocolBytes protocol = new ProtocolBytes();
        protocol.AddString("GetRoomList");
        NetMgr.srvConn.Send(protocol);
    }

    void NewRoomClick()
    {
        ProtocolBytes protocol = new ProtocolBytes();
        protocol.AddString("CreateRoom");
        NetMgr.srvConn.Send(protocol);
    }

    //清空房间列表
    void ClearRoomUnit()
    {
        for(int i=0;i<content.childCount;i++)
        {
            Destroy(content.GetChild(i).gameObject);
        }
    }
    
    //获取房间列表
    void RecvGetRoomList(ProtocolBase protocol)
    {
        ClearRoomUnit();
        ProtocolBytes proto = (ProtocolBytes)protocol;
        int start = 0;
        proto.GetString(start, ref start);
        int count=proto.GetInt(start, ref start);
        for(int i=0;i<count;i++)
        {
            Transform transform = Instantiate(roomInfoTrans) as Transform;
            transform.Find("RoomId").GetComponent<Text>().text = i.ToString();
            transform.Find("RoomNum").GetComponent<Text>().text = proto.GetInt(start, ref start).ToString();
            transform.Find("RoomState").GetComponent<Text>().text = "状态：" + (proto.GetInt(start, ref start) == 0 ? "准备中" : "开战中");
            transform.SetParent(content);
        }
    }


    //获取战绩信息
    void RecvGetInfo(ProtocolBase protocol)
    {
        ProtocolBytes proto = (ProtocolBytes)protocol;
        int start = 0;
        proto.GetString(start, ref start);
        string name = proto.GetString(start, ref start);
        int win = proto.GetInt(start, ref start);
        int lost = proto.GetInt(start, ref start);
        playerNameText.text = "指挥官:"+name;
        recordText.text = string.Format("WIN:{0}  LOST:{1}", win, lost);
    }

    //获取创建房间是否成功
    void RecvCreateRoom(ProtocolBase protocol)
    {
        ProtocolBytes proto = (ProtocolBytes)protocol;
        int start = 0;
        proto.GetString(start, ref start);
        int ret = proto.GetInt(start);
        if (ret == -1)
        {
            Debug.Log("创建房间失败");
        }
        else
        {
            Debug.Log("创建房间成功");
        }

    }
    
}
