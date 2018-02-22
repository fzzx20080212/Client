using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//处理位置同步相关功能
public class Walk : MonoBehaviour
{

    public GameObject prefab,myObject;
    Dictionary<string, GameObject> players = new Dictionary<string, GameObject>();
    //self
    public string playerId = "";
    public static Walk instance;

    //移动时间间隔
    float lastMoveTime;
    // Use this for initialization
    void Start()
    {
        instance = this;
        prefab = Resources.Load("PlayerPrefab") as GameObject;
    }

    GameObject AddPlayer(string id, Vector3 pos, int score)
    {
        GameObject player = Instantiate(prefab, pos, Quaternion.identity);
        player.GetComponentInChildren<TextMesh>().text = id + ":" + score;
        players.Add(id, player);
        return player;

    }

    void DelPlayer(string id)
    {
        if (players.ContainsKey(id))
        {
            Destroy(players[id]);
            players.Remove(id);
        }
    }

    public void UpdateScore(string id, int score)
    {
        if (players.ContainsKey(id))
        {
            players[id].GetComponentInChildren<TextMesh>().text = id + ":" + score;
        }
    }

    //更新信息
    public void UpdateInfo(string id, Vector3 pos, int score)
    {
        if (id == playerId)
        {
            UpdateScore(id, score);
            return;
        }
        if (players.ContainsKey(id))
        {
            players[id].transform.position = pos;
            UpdateScore(id, score);
        }
        else
            AddPlayer(id, pos, score);
    }

    //开始进入游戏
    public void StartGame(string id)
    {
        playerId = id;
        float x = Random.Range(0, 10);
        float y = Random.Range(0, 10);
        float z = 0;
        Vector3 pos = new Vector3(x, y, z);
        myObject=AddPlayer(playerId, pos, 0);
        Debug.Log(pos);
        SendPos();
        ProtocolBytes protocolBytes = new ProtocolBytes();
        protocolBytes.AddString("GetList");
        NetMgr.srvConn.Send(protocolBytes, GetList);
        NetMgr.srvConn.msgDistribution.AddListener("UpdateInfo", UpdateInfo);
        NetMgr.srvConn.msgDistribution.AddListener("PlayerLeave", PlayerLeave);
    }

    void SendPos()
    {
        GameObject player = players[playerId];
        Vector3 pos = player.transform.position;
        ProtocolBytes protocol = new ProtocolBytes();
        protocol.AddString("UpdateInfo");
        protocol.AddFloat(pos.x);
        protocol.AddFloat(pos.y);
        protocol.AddFloat(pos.z);
        NetMgr.srvConn.Send(protocol);
    }

    void GetList(ProtocolBase protocol)
    {
        ProtocolBytes proto = (ProtocolBytes)protocol;
        int start = 0;
        string name = proto.GetString(start, ref start);
        int count = proto.GetInt(start, ref start);
        for (int i = 0; i < count; i++)
        {
            string id = proto.GetString(start, ref start);
            float x = proto.GetFloat(start, ref start);
            float y = proto.GetFloat(start, ref start);
            float z = proto.GetFloat(start, ref start);
            int score = proto.GetInt(start, ref start);
            UpdateInfo(id, new Vector3(x, y, z), score);

        }
    }

    public void UpdateInfo(ProtocolBase protocol)
    {
        ProtocolBytes proto = (ProtocolBytes)protocol;
        int start = 0;
        string name = proto.GetString(start, ref start);
        string id = proto.GetString(start, ref start);
        float x = proto.GetFloat(start, ref start);
        float y = proto.GetFloat(start, ref start);
        float z = proto.GetFloat(start, ref start);
        int score = proto.GetInt(start, ref start);
        UpdateInfo(id, new Vector3(x, y, z), score);


    }

    public void PlayerLeave(ProtocolBase protocol)
    {
        ProtocolBytes proto = (ProtocolBytes)protocol;
        int start = 0;
        string name = proto.GetString(start, ref start);
        string id = proto.GetString(start, ref start);
        DelPlayer(id);
    }

    void Move()
    {
        if (Time.time - lastMoveTime < 0.1)
            return;
        if (Input.GetKey(KeyCode.UpArrow))
        {
            myObject.transform.position += new Vector3(0, 1, 0);
            SendPos();
        }
        else if (Input.GetKey(KeyCode.LeftArrow))
        {
            myObject.transform.position += new Vector3(-1, 0, 0);
            SendPos();
        }
        else if(Input.GetKey(KeyCode.RightArrow))
        {
            myObject.transform.position += new Vector3(1, 0, 0);
            SendPos();
        }
        else if(Input.GetKey(KeyCode.DownArrow))
        {
            myObject.transform.position += new Vector3(0, -1, 0);
            SendPos();
        }
        lastMoveTime = Time.time;
    }
    private void Update()
    {
        Move();
    }

}
