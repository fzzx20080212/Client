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

    GameObject AddPlayer(string id, Vector3 pos)
    {
        GameObject player = Instantiate(prefab, pos, Quaternion.identity);
        if (id != playerId)
            player.transform.GetComponentInChildren<Camera>().gameObject.SetActive(false);
        //player.GetComponentInChildren<TextMesh>().text = id + ":" + score;
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
            //players[id].GetComponentInChildren<TextMesh>().text = id + ":" + score;
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
            AddPlayer(id, pos);
    }

    //更新角度
    public void UpdateRotate(string id, Quaternion quaternion)
    {
        if (id == playerId)
        {
            return;
        }
        if (players.ContainsKey(id))
        {
            players[id].transform.rotation = quaternion;
        }
        else
            AddPlayer(id, Vector3.zero);
    }

    //开始进入游戏
    public void StartGame(string id)
    {
        playerId = id;
        float x = Random.Range(-5, 5);
        float y = Random.Range(5, 6);
        float z = Random.Range(-5, 5);
        Vector3 pos = new Vector3(x, y, z);
        myObject=AddPlayer(playerId, pos);
        Debug.Log(pos);
        SendPos();
        ProtocolBytes protocolBytes = new ProtocolBytes();
        protocolBytes.AddString("GetList");
        NetMgr.srvConn.Send(protocolBytes, GetList);
        NetMgr.srvConn.msgDistribution.AddListener("UpdateInfo", UpdateInfo);
        NetMgr.srvConn.msgDistribution.AddListener("PlayerLeave", PlayerLeave);
        NetMgr.srvConn.msgDistribution.AddListener("UpdateRot", UpdateRotation);

        //进入游戏关闭摄像机
        //Camera.main.gameObject.SetActive(false);
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

    void SendRotate()
    {
        GameObject player = players[playerId];
        Quaternion rot = player.transform.rotation;
        ProtocolBytes protocol = new ProtocolBytes();
        protocol.AddString("UpdateRot");
        protocol.AddFloat(rot.x);
        protocol.AddFloat(rot.y);
        protocol.AddFloat(rot.z);
        protocol.AddFloat(rot.w);
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

    public void UpdateRotation(ProtocolBase protocol)
    {
        ProtocolBytes proto = (ProtocolBytes)protocol;
        int start = 0;
        string name = proto.GetString(start, ref start);
        string id = proto.GetString(start, ref start);
        float x = proto.GetFloat(start, ref start);
        float y = proto.GetFloat(start, ref start);
        float z = proto.GetFloat(start, ref start);
        float w = proto.GetFloat(start, ref start);
        UpdateRotate(id, new Quaternion(x, y, z, w));
    }

    public void PlayerLeave(ProtocolBase protocol)
    {
        ProtocolBytes proto = (ProtocolBytes)protocol;
        int start = 0;
        string name = proto.GetString(start, ref start);
        string id = proto.GetString(start, ref start);
        DelPlayer(id);
    }

    public float flySpeed = 10;
    public float turnSpeed = 100;
    void Move()
    {

        //if (Input.GetKey(KeyCode.UpArrow))
        //    myObject.transform.position += myObject.transform.forward * flySpeed * Time.deltaTime;
        //if (Input.GetKey(KeyCode.LeftArrow))
        //    myObject.transform.Rotate(myObject.transform.up, -turnSpeed * Time.deltaTime);
        //if (Input.GetKey(KeyCode.RightArrow))
        //    myObject.transform.Rotate(myObject.transform.up, turnSpeed * Time.deltaTime);

        float deltaTime = Time.time - lastMoveTime;
        if (deltaTime < 1 / 60f)
            return;
        if (Input.GetKey(KeyCode.UpArrow))
        {
            myObject.transform.position += myObject.transform.forward*flySpeed* deltaTime;
            SendPos();
        }
        else if (Input.GetKey(KeyCode.LeftArrow))
        {
            myObject.transform.Rotate(myObject.transform.up, -turnSpeed * deltaTime);
            SendRotate();
        }
        else if (Input.GetKey(KeyCode.RightArrow))
        {
            myObject.transform.Rotate(myObject.transform.up, turnSpeed * deltaTime);
            SendRotate();
        }
        else if (Input.GetKey(KeyCode.DownArrow))
        {
            myObject.transform.position += -myObject.transform.forward * flySpeed * deltaTime;
            SendPos();
        }

        lastMoveTime = Time.time;
    }
    private void Update()
    {
        if (myObject == null)
            return;
        Move();
    }


 


}
