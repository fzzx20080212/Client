using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;
using System.Net.Sockets;
using System;
using System.Linq;
public class Connection {

    Socket socket;
    public string recvStr;
    private static int BUFFSIZE = 1024;
    private byte[] readBuff = new byte[BUFFSIZE];
    private int buffCount = 0;
    //粘包分包
    private Int32 msgLength = 0;
    public byte[] lenBytes = new byte[sizeof(UInt32)];
    //心跳时间
    public float lastTickTime = 0;
    public float heartBeatTime = 30;
    //消息分发
    public MsgDistribution msgDistribution = new MsgDistribution();
    //协议
    public ProtocolBase proto=new ProtocolBytes();
    public enum Status
    {
        None,
        Connected,
    }
    public Status status = Status.None;

    public void Update()
    {
        msgDistribution.Update();
        if (status == Status.Connected)
        {
            if (Time.time - lastTickTime > heartBeatTime)
            {
                ProtocolBase protocol = NetMgr.GetHeartBeatProtocol();
                Send(protocol);
                lastTickTime = Time.time;
            }
        }
    }

    public bool Connect()
    {
        try
        {
            string host = "127.0.0.1";
            int port = 1234;
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            socket.Connect(host, port);
            socket.BeginReceive(readBuff, buffCount, BUFFSIZE-buffCount, SocketFlags.None, ReceiveCb, null);
            status = Status.Connected;
            return true;
        }
        catch (Exception e)
        {
            Debug.Log("连接失败" + e.Message);
            return false;
        }
    }

    //关闭连接
    public bool Close()
    {
        try
        {
            socket.Close();
            return true;
        }
        catch (Exception e)
        {
            Debug.Log("关闭失败" + e.Message);
            return false;
        }
    }

    private void ReceiveCb(IAsyncResult ar)
    {
        try
        {
            //获取接受的字节数
            int count =socket.EndReceive(ar);
            //数据处理
            buffCount += count;
            ProcessData();

            socket.BeginReceive(readBuff, buffCount, BUFFSIZE-buffCount, SocketFlags.None, ReceiveCb, null);

        }
        catch (Exception e)
        {
            Console.WriteLine("ReceiveCb失败" + e.Message);
            status = Status.None;
        }
    }

    //粘包分包处理
    private void ProcessData()
    {
        if (buffCount < sizeof(Int32))
            return;
        //将前四个字节复制到
        Array.Copy(readBuff,lenBytes, sizeof(Int32));
        msgLength = BitConverter.ToInt32(lenBytes, 0);
        if (buffCount <msgLength + sizeof(Int32))
            return;
        ProtocolBase protocol = proto.Decode(readBuff, sizeof(Int32), msgLength);
        lock (msgDistribution.msgList)
        {
            msgDistribution.msgList.Add(protocol);
           
        }
        //清除已处理的消息
        int count = buffCount - sizeof(Int32) - msgLength;
        Array.Copy(readBuff, sizeof(Int32) + msgLength,readBuff, 0, count);
        buffCount = count;
        if (buffCount > 0)
            ProcessData();
    }

    public bool Send(ProtocolBase protocol,MsgDistribution.Delegate cb)
    {
        string name = protocol.GetName();
        return Send(protocol, cb, name);
    }

    public bool Send(ProtocolBase protocol,MsgDistribution.Delegate cb,string name)
    {
        if (status != Status.Connected)
            return false;
        msgDistribution.AddOnceListener(name, cb);
        return Send(protocol);
    }

    public bool Send(ProtocolBase protocol)
    {
        if (status != Status.Connected)
        {
            Debug.Log("未连接无法发送");
            return false;
        }
        byte[] bytes = protocol.Encode();
        byte[] length = BitConverter.GetBytes(bytes.Length);
        byte[] sendbuff = length.Concat(bytes).ToArray();
        socket.Send(sendbuff);
        return true;
    }
}
