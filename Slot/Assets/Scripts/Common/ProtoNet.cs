// Proto客户端网络操作封装
// 但是不涉及具体的数据
#define U3D

using System.Collections;
using System.Collections.Generic;
using Google.Protobuf;
using System.IO;
using System.Text;
using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Collections.Concurrent;
using System.Threading.Tasks;

#if !U3D
using System.Windows.Forms;
#else
using UnityEngine;
#endif
public delegate void WorkDone();
public class ProtoPacket
{
    public int cmdId { get; set; } // 命令号
    public int msgId { get; set; } // 消息号
    public object proto { get; set; } // proto buf
    public WorkDone callback { get; set; } // 回调函数
}
public class ProtoNet
{
    /// <summary>  
    /// 输出日志
    /// </summary>  
    /// <param name="obj">内容</param>
    public static void WriteLog(object obj)
    {
#if U3D
        DebugConsole.Log(obj.ToString());
#else
        MessageBox.Show(obj.ToString());
#endif
    }

    private string m_ip = "127.0.0.1";
    private int m_port = 5690;
    private Socket m_socket;
    private bool m_running = false; // 是否正在运行
    private ConcurrentQueue<ProtoPacket> m_recvQueue { get; set; } // 接受数据队列
    private ConcurrentQueue<ProtoPacket> m_sendQueue { get; set; } // 发送数据队列
    private Task m_tkRecvMessageFromServer, m_tkSendMessageToServer;// 任务
    private Dictionary<int, MessageParser> m_types = new Dictionary<int, MessageParser>(); // 序列化支持的类型
    private string m_name; // 当前Net的名称
    private float m_rcElapse = 0; // 重连间隔
    private int m_msgId = 10000;
    private Dictionary<int, WorkDone> m_callbackDict = new Dictionary<int, WorkDone>(); // 回调函数列表
    private Dictionary<int, int> m_callBackElapse = new Dictionary<int, int>(); // 回调函数存在时间，过长时间定期删除

    public string Name
    {
        get { return m_name; }
        set { m_name = value; }
    }
    public string Ip
    {
        get { return m_ip; }
        set { m_ip = value; }
    }

    /// <summary>  
    /// 添加支持的序列化类型
    /// </summary>  
    /// <param name="ipStr">IP</param>  
    /// <param name="port">端口</param> 
    public bool Add(int cmdId, MessageParser type)
    {
        if (m_types.ContainsKey(cmdId))
            return false;

        m_types[cmdId] = type;
        return true;
    }

    public bool IsRunning()
    {
        return m_running;
    }

    /// <summary>  
    /// 外部调用的重连
    /// <param name="elapse">几秒后执行</param>  
    /// </summary>  
    public bool CheckReconnect(float elapse = 0)
    {
        if (elapse > 0)
        {
            m_rcElapse = elapse;
            return false;
        }

        if (m_rcElapse > 0)
        {
            m_rcElapse -= Time.deltaTime;
            if (m_rcElapse < 0)
            {
                return Init(m_ip, m_port);
            }
        }

        return false;
    }

    void AddReconnect()
    {
        if (m_rcElapse > 0)
            return;

        ProtoPacket reconnectPacket = new ProtoPacket();
        reconnectPacket.cmdId = Constants.Reconnect;
        reconnectPacket.msgId = 1; // 外部重连
        m_recvQueue.Enqueue(reconnectPacket);
    }

    /// <summary>  
    /// 初始化socket并连接
    /// </summary>  
    /// <param name="ipStr">IP</param>  
    /// <param name="port">端口</param>  
    public bool Init(string ipStr, int port)
    {
        System.Random rand = new System.Random();
        m_msgId = rand.Next(10000, 20000); // rand start msg id
        m_running = true;
        m_ip = ipStr;
        m_port = port;

        m_recvQueue = new ConcurrentQueue<ProtoPacket>();
        m_sendQueue = new ConcurrentQueue<ProtoPacket>();

        Task taskConnect = Task.Factory.StartNew(() =>
        {
            if (!ConnectServer())
            {
                // 重连展示
                AddReconnect();

                return false;
            }
            else
            {
                ProtoPacket reconnectPacket = new ProtoPacket();
                reconnectPacket.cmdId = Constants.Reconnect;
                reconnectPacket.msgId = 2; // 重连成功
                m_recvQueue.Enqueue(reconnectPacket);

                return true;
            }
        });

        return true;
    }
    /// <summary>  
    ///连接服务器  
    /// </summary>  
    private bool ConnectServer()
    {
        try
        {
            m_socket = new Socket(AddressFamily.InterNetwork,
                SocketType.Stream,
                ProtocolType.Tcp);
            /*
            m_socket.SetSocketOption(SocketOptionLevel.Socket,
                SocketOptionName.SendTimeout,
                1000);*/

            m_socket.Connect(IPAddress.Parse(m_ip), m_port);
            if (!m_socket.Connected)
            {
                DebugConsole.Log("Failed to socket connect.");
            }

            m_tkRecvMessageFromServer = Task.Factory.StartNew(() =>
            {
                ReceiveMessage();
            });

            m_tkSendMessageToServer = Task.Factory.StartNew(() =>
            {
                SendMessage();
            });

            return true;
        }
        catch (ArgumentNullException e)
        {
            WriteLog("ConnectServer Error:" + e.ToString());
            return false;
        }
        catch (SocketException e)
        {
            WriteLog("ConnectServer Error:" + e.ToString());
            return false;
        }
        catch (Exception e)
        {
            WriteLog("ConnectServer Error:" + e.ToString());
            return false;
        }
    }

    /// <summary>  
    /// 关闭
    /// </summary>
    public void Close(bool closeSocket = true)
    {
        try
        {
            if (!closeSocket)
            {
                // 滞后关闭
                m_running = false;
                return;
            }

            // 结束接收线程&发送线程
            m_socket.Shutdown(SocketShutdown.Both);
            //m_socket.Close(); // Close 在IOS下导致崩溃
            m_running = false;
            int sleepTime = 0;
            while (true)
            {
                Thread.Sleep(5);
                sleepTime += 5;

                if (m_tkRecvMessageFromServer.IsCompleted == true
                    && m_tkSendMessageToServer.IsCompleted == true)
                {
                    WriteLog("Tasks end.");
                    break;
                }

                if (sleepTime > 1000)
                    break;
            }

            //m_socket.Shutdown(SocketShutdown.Both);
            //m_socket.Close();

            WriteLog("Sleep time:" + sleepTime);
        }
        catch(Exception e)
        {
            WriteLog("Close Error:" + e.ToString());
        }
    }

    /// <summary>  
    /// 从接收数据队列取数据
    /// </summary>  
    /// <param name="packet">取出的包</param>
    /// <returns>返回成功与否</returns>
    public bool RecvTryDequeue(ref ProtoPacket packet)
    {
        return m_recvQueue.TryDequeue(out packet);
    }

    /// <summary>  
    /// 向发送数据队列添加数据
    /// </summary>  
    /// <param name="packet">发出的包</param>
    public void SendEnqueue(ProtoPacket packet)
    {
        m_sendQueue.Enqueue(packet);
    }

    /// <summary>  
    /// 向发送数据队列添加数据
    /// </summary>  
    /// <param name="cmdId">命令号</param>
    /// <param name="msgId">消息号</param>
    /// <param name="obj">proto</param>
    public void SendEnqueue(int cmdId, int msgId, object obj, WorkDone cb = null)
    {
        // 检查回调函数列表
        if (cb != null)
        {
            m_callbackDict.Add(m_msgId, cb);
            m_callBackElapse.Add(m_msgId, System.DateTime.Now.Second);
        }

        ProtoPacket packet = new ProtoPacket();
        packet.cmdId = cmdId;
        packet.msgId = m_msgId++;
        packet.proto = obj;

        DebugConsole.Log("m_sendQueue enqueue:cmd=" + cmdId.ToString() + ", msgid=" + packet.msgId.ToString());
        m_sendQueue.Enqueue(packet);
    }

    /// <summary>  
    /// 构建快速登录消息数据包  
    /// </summary>  
    /// <param name="cmdId">命令号</param>  
    /// <param name="msgId">消息号</param>  
    /// <param name="quickLoginInfo">快速登录信息</param>  
    /// <returns>返回构建完整的数据包</returns>  
    private byte[] Pack(int cmdId,
        int msgId,
        object protoObj)
    {
        try
        {
            // 消息头各个分类数据转换为字节数组（非字符型数据需先转换为网络序  HostToNetworkOrder:主机序转网络序）  
            byte[] bufCmdId = BitConverter.GetBytes(IPAddress.HostToNetworkOrder(cmdId));
            byte[] bufMsgId = BitConverter.GetBytes(IPAddress.HostToNetworkOrder(msgId));

            //using (MemoryStream ms = new MemoryStream())
            {
                //new PBMessageSerializer().Serialize(ms, protoObj);
                //byte[] bufProto = ms.ToArray();
                IMessage msg = (IMessage)protoObj;
                byte[] bufProto = msg.ToByteArray();

                // 总长度=消息长度+8
                int msgLength = 8 + bufProto.Length;
                //byte[] bufMsgLength = BitConverter.GetBytes(IPAddress.HostToNetworkOrder(msgLength));

                // 返回数据包
                byte[] data = new byte[12 + bufProto.Length];
                // 长度
                BitConverter.GetBytes(IPAddress.HostToNetworkOrder(msgLength)).CopyTo(data, 0);
                // 命令号
                bufCmdId.CopyTo(data, 4);
                // 消息号
                bufMsgId.CopyTo(data, 8);
                // Proto
                bufProto.CopyTo(data, 12);

                return data;
            }
        }
        catch (Exception e)
        {
            WriteLog("Pack Error:" + e.ToString());
            return null;
        }
    }

    /// <summary>  
    /// 直接发送缓冲 
    /// </summary>
    /// <param name="sendBytes">缓冲区</param>
    /// <returns>返回成功与否</returns>
    public bool SendMessageSync(byte[] sendBytes)
    {
        try
        {
            if (m_socket.Connected)
            {
                // 获取远程终结点的IP和端口信息  
                //IPEndPoint ipe = (IPEndPoint)m_socket.RemoteEndPoint;
                int sentBytes = m_socket.Send(sendBytes, sendBytes.Length, SocketFlags.None);
                if (sentBytes != sendBytes.Length)
                {
                    WriteLog("SendMessageSync Error: sentBytes != sendBytes.Length");
                }

                return true;
            }
            else
            {
                WriteLog("SendMessage Error:!Connected");
                return false;
            }
        }
        catch (Exception e)
        {
            WriteLog("SendMessage Error:" + e.ToString());
            return false;
        }
    }

    /// <summary>  
    /// 发送队列处理函数
    /// </summary>  
    private void SendMessage()
    {
        DebugConsole.Log("SendMessage enter");

        try
        {
            while (m_running)
            {
                ProtoPacket packet;
                if (m_sendQueue.TryDequeue(out packet))
                {
                    // 数据转换为二进制在工作线程中
                    byte[] buf = Pack((int)packet.cmdId,
                        packet.msgId,
                        packet.proto);

                    if (buf != null)
                    {
                        if (!SendMessageSync(buf))
                        {
                            // 退出重连
                            AddReconnect();

                            break;
                        }
                        /*
                        bool success = false;
                        while (true)
                        {
                            success = SendMessageSync(buf);
                            if (!success)
                            {
                                // 检查是否连接出错了
                                if (!m_socket.Connected)
                                {
                                    ProtoPacket reconnectPacket = new ProtoPacket();
                                    reconnectPacket.cmdId = Constants.Client_Reconnect;
                                    reconnectPacket.msgId = 0;

                                    m_recvQueue.Enqueue(reconnectPacket);
                                    // 一直重连直到成功
                                    ConnectServer();
                                    Thread.Sleep(3000);
                                }
                            }
                            else
                            {
                                break;
                            }
                        }*/
                    }
                }
                else
                {
                    Thread.Sleep(5);
                }
            }
            //if (m_socket.Connected)
            //{
            //    m_socket.Shutdown(SocketShutdown.Both);
            //    m_socket.Close();
            //}

            WriteLog("SendMessage thread exit.");
        }
        catch(Exception e)
        {
            WriteLog("SendMessage Error:" + e.Message);
            AddReconnect();
        }
    }

    /// <summary>  
    /// 接收队列处理函数
    /// </summary>  
    private void ReceiveMessage()
    {
        DebugConsole.Log("ReceiveMessage enter");
        // Sleep 测试接收短线！！！
        try
        {
            while (m_running)
            {
                // 接受消息头: 长度<4>，命令号<4>，消息号（后端内部用）<4>，proto
                // 其中长度=x+8
                const int HeadLength = 12;
                int remainHeadLength = HeadLength;
                // 存储消息头的所有字节数  
                byte[] recvBytesHead = new byte[HeadLength];
                // 如果当前需要接收的字节数大于0，则循环接收  
                while (remainHeadLength > 0)
                {
                    byte[] recvBytes1 = new byte[HeadLength];
                    // 将本次传输已经接收到的字节数置0  
                    int receivedBytes = 0;
                    // 如果当前需要接收的字节数大于缓存区大小，则按缓存区大小进行接收，相反则按剩余需要接收的字节数进行接收 
                    if (remainHeadLength >= recvBytes1.Length)
                    {
                        receivedBytes = m_socket.Receive(recvBytes1, recvBytes1.Length, 0);
                    }
                    else
                    {
                        receivedBytes = m_socket.Receive(recvBytes1, remainHeadLength, 0);
                    }
                    // 将接收到的字节数保存  
                    recvBytes1.CopyTo(recvBytesHead, recvBytesHead.Length - remainHeadLength);
                    // 减去已经接收到的字节数  
                    remainHeadLength -= receivedBytes;
                }

                // 接收消息体（消息体的长度存储在消息头的0至4索引位置的字节里）  
                byte[] bytes = new byte[4];
                Array.Copy(recvBytesHead, 0, bytes, 0, 4);
                int bodyLength = IPAddress.NetworkToHostOrder(BitConverter.ToInt32(bytes, 0));
                bodyLength -= 8;
                if (bodyLength < 0)
                {
                    // 错误的长度
                    WriteLog("Receive invalid body length=" + bodyLength);
                    continue;
                }

                // 存储消息体的所有字节数  
                byte[] recvBytesBody = new byte[bodyLength];
                //如果当前需要接收的字节数大于0，则循环接收  
                while (bodyLength > 0)
                {
                    byte[] recvBytes2 = new byte[bodyLength < 1024 ? bodyLength : 1024];
                    // 将本次传输已经接收到的字节数置0  
                    int receivedBytes = 0;
                    // 如果当前需要接收的字节数大于缓存区大小，则按缓存区大小进行接收，相反则按剩余需要接收的字节数进行接收  
                    if (bodyLength >= recvBytes2.Length)
                    {
                        receivedBytes = m_socket.Receive(recvBytes2, recvBytes2.Length, 0);
                    }
                    else
                    {
                        receivedBytes = m_socket.Receive(recvBytes2, bodyLength, 0);
                    }
                    // 将接收到的字节数保存  
                    recvBytes2.CopyTo(recvBytesBody, recvBytesBody.Length - bodyLength);
                    // 减去已经接收到的字节数  
                    bodyLength -= receivedBytes;
                }

                //一个消息包接收完毕，解析消息包  
                UnPack(recvBytesHead, recvBytesBody);
            }

            WriteLog("ReceiveMessage thread exit.");
            //if (m_socket.Connected)
            //{
            //    m_socket.Shutdown(SocketShutdown.Both);
            //    m_socket.Close();
            //}
        }
        catch (System.Exception e)
        {
            WriteLog("ReceiveMessage Error:" + e.Message);
            AddReconnect();
        }
    }

    /// <summary>  
    /// 解析二进制包，并入队列
    /// </summary>  
    /// <param name="Head">包头</param>
    /// <param name="Head">包尾</param>
    public void UnPack(byte[] Head, byte[] Body)
    {
        try
        {
            byte[] cmd = new byte[4];
            Array.Copy(Head, 4, cmd, 0, 4);
            int cmdId = IPAddress.NetworkToHostOrder(BitConverter.ToInt32(cmd, 0));

            byte[] msg = new byte[4];
            Array.Copy(Head, 8, msg, 0, 4);
            int msgId = IPAddress.NetworkToHostOrder(BitConverter.ToInt32(msg, 0));

            ProtoPacket packet = new ProtoPacket();
            packet.cmdId = cmdId;
            packet.msgId = msgId;            

            // 检查回调函数列表
            if (m_callbackDict.ContainsKey(msgId))
            {
                packet.callback = m_callbackDict[msgId];
                m_callbackDict.Remove(msgId);
            }

            // 检查回调超时
            int secCur = System.DateTime.Now.Second;
            List<int> delList = new List<int>();
            foreach (var item in m_callBackElapse)
            {
                int sec = item.Value;
                if (secCur - sec > 300)
                {
                    delList.Add(item.Key);
                }
            }

            for (int i = 0; i < delList.Count; ++i )
            {
                int key = delList[i];
                DebugConsole.Log("Delete time out callback");
                m_callbackDict.Remove(key);
                m_callBackElapse.Remove(key);
            }

            if (!m_types.ContainsKey(cmdId))
            {
                DebugConsole.Log("Not supported cmdId:" + cmdId + " in ProtoNet:" + m_name);
            }
            else
            {
                //MemoryStream ms = new MemoryStream(Body, 0, Body.Length);
                //Type type = m_types[cmdId];
                //object obj = new PBMessageSerializer().Deserialize(ms, null, type);
                object obj = m_types[cmdId].ParseFrom(Body, 0, Body.Length);
                if (obj == null)
                {
                    DebugConsole.Log("Deserialize error for cmdId:" + cmdId + " in ProtoNet:" + m_name);
                }
                else
                {
                    packet.proto = obj;
                    m_recvQueue.Enqueue(packet);
                }
            }
            /*
            switch (cmdId)
            {
                case SlotClientConstants.Server_UserInfo:
                    {
                        MemoryStream ms = new MemoryStream(Body, 0, Body.Length);
                        UserInfo usrInfo
                            = new PBMessageSerializer().Deserialize(ms, null, typeof(UserInfo)) as UserInfo;
                                                        
                        if (usrInfo != null)
                        {
                            packet.proto = usrInfo;
                            m_recvQueue.Enqueue(packet);
                        }
                    }
                    break;
                case SlotClientConstants.Server_TigerResp:
                    {
                        MemoryStream ms = new MemoryStream(Body, 0, Body.Length);
                        TigerResp tigerResp
                            = new PBMessageSerializer().Deserialize(ms, null, typeof(TigerResp)) as TigerResp;

                        if (tigerResp != null)
                        {
                            packet.proto = tigerResp;
                            m_recvQueue.Enqueue(packet);
                        }
                    }
                    break;
                case SlotClientConstants.Server_LoginResp:
                    {
                        MemoryStream ms = new MemoryStream(Body, 0, Body.Length);
                        LoginResp loginResp
                            = new PBMessageSerializer().Deserialize(ms, null, typeof(LoginResp)) as LoginResp;

                        if (loginResp != null)
                        {
                            packet.proto = loginResp;
                            m_recvQueue.Enqueue(packet);
                        }
                    }
                    break;
                case SlotClientConstants.Server_RedirectResp:
                    {
                        MemoryStream ms = new MemoryStream(Body, 0, Body.Length);
                        RedirectResp rdResp
                            = new PBMessageSerializer().Deserialize(ms, null, typeof(RedirectResp)) as RedirectResp;

                        if (rdResp != null)
                        {
                            packet.proto = rdResp;
                            m_recvQueue.Enqueue(packet);
                        }
                    }
                    break;
                case SlotClientConstants.Server_Error:
                    {

                    }
                    break;
                default:
                    WriteLog("Unknown cmd");
                    break;
            }*/
        }
        catch(Exception e)
        {
            WriteLog("UnPack Error:" + e.ToString());
        }
    }
}

