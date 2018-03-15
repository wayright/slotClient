// 客户端网络操作封装
#define U3D

using System.Collections;
using System.Collections.Generic;
using ProtoBuf;
using Login;
using User;
using Tiger.Info;
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

namespace slotClient
{
    public class SlotConstants
    {
        // 客户端
        public const int Client_QuickLoginInfo = 1002; // 快速登录
        public const int Client_TigerReq = 10000;// spin请求
        public const int Client_Reconnect = 99999; // 本地重连

        // 服务器
        public const int Server_UserInfo = 1002;// 用户信息（快速登录返回）
        public const int Server_TigerResp = 10000;// spin返回
        public const int Server_Error = -1;// 服务器错误
    }

    public class ProtoPacket
    {
        public int cmdId { get; set; } // 命令号
        public int msgId { get; set; } // 消息号
        public object proto { get; set; } // proto
    }
    public class SlotClientNet
    {
        /// <summary>  
        /// 输出日志
        /// </summary>  
        /// <param name="obj">内容</param>
        public static void WriteLog(object obj)
        {
#if U3D
            Debug.Log(obj);
#else
            MessageBox.Show(obj.ToString());
#endif
        }

        private static string m_ip = "127.0.0.1";
        private static int m_port = 5690;
        private static Socket m_socket;
        private static bool m_running = true; // 是否正在运行
        private static ConcurrentQueue<ProtoPacket> m_recvQueue { get; set; } // 接受数据队列
        private static ConcurrentQueue<ProtoPacket> m_sendQueue { get; set; } // 发送数据队列
        private static Task m_tkRecvMessageFromServer, m_tkSendMessageToServer;// 任务

        /// <summary>  
        /// 初始化socket并连接
        /// </summary>  
        /// <param name="ipStr">IP</param>  
        /// <param name="port">端口</param>  
        public bool Init(string ipStr, int port)
        {
            m_ip = ipStr;
            m_port = port;

            m_recvQueue = new ConcurrentQueue<ProtoPacket>();
            m_sendQueue = new ConcurrentQueue<ProtoPacket>();

            return ConnectServer();
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

                m_socket.Connect(IPAddress.Parse(m_ip), m_port);
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
        public void Close()
        {
            try
            {
                // 结束接收线程&发送线程
                m_socket.Close();
                m_running = false;
                Thread.Sleep(1000);

                if (m_tkRecvMessageFromServer.IsCompleted == true
                    && m_tkSendMessageToServer.IsCompleted == true)
                {
                    WriteLog("Tasks end.");
                }
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
        public void SendEnqueue(int cmdId, int msgId, object obj)
        {
            ProtoPacket packet = new ProtoPacket();
            packet.cmdId = cmdId;
            packet.msgId = msgId;
            packet.proto = obj;

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

                using (MemoryStream ms = new MemoryStream())
                {
                    new PBMessageSerializer().Serialize(ms, protoObj);
                    byte[] bufProto = ms.ToArray();

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
                    int sentBytes = m_socket.Send(sendBytes, sendBytes.Length, 0);
                    if (sentBytes != sendBytes.Length)
                    {
                        WriteLog("SendMessageSync Error: sentBytes != sendBytes.Length");
                    }

                    return true;
                }
                else
                {
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
                                        reconnectPacket.cmdId = SlotConstants.Client_Reconnect; // reconnect

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
                            }
                        }
                    }
                    else
                    {
                        Thread.Sleep(5);
                    }
                }

                WriteLog("SendMessage thread exit.");
            }
            catch(Exception e)
            {
                WriteLog("SendMessage Error:" + e.Message);
            }
        }

        /// <summary>  
        /// 接收队列处理函数
        /// </summary>  
        private void ReceiveMessage()
        {
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
                    if (bodyLength <= 0)
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
            }
            catch (System.Exception e)
            {
                WriteLog("ReceiveMessage Error:" + e.Message);
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
                // 二进制转为Proto
                //byte[] bytes = new byte[4];
                //Array.Copy(Head, 0, bytes, 0, 4);
                //int msgLength = IPAddress.NetworkToHostOrder(BitConverter.ToInt32(bytes, 0));

                byte[] cmd = new byte[4];
                Array.Copy(Head, 4, cmd, 0, 4);
                int cmdId = IPAddress.NetworkToHostOrder(BitConverter.ToInt32(cmd, 0));

                byte[] msg = new byte[4];
                Array.Copy(Head, 8, msg, 0, 4);
                int msgId = IPAddress.NetworkToHostOrder(BitConverter.ToInt32(msg, 0));

                ProtoPacket packet = new ProtoPacket();
                packet.cmdId = cmdId;
                packet.msgId = msgId;

                switch (cmdId)
                {
                    case SlotConstants.Server_UserInfo:
                        {
                            MemoryStream ms = new MemoryStream(Body, 0, Body.Length);
                            Tiger.Info.UserInfo usrInfo
                                = new PBMessageSerializer().Deserialize(ms, null, typeof(Tiger.Info.UserInfo)) as Tiger.Info.UserInfo;
                                                        
                            if (usrInfo != null)
                            {
                                packet.proto = usrInfo;
                                m_recvQueue.Enqueue(packet);
                            }
                        }
                        break;
                    case SlotConstants.Server_TigerResp:
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
                    case SlotConstants.Server_Error:
                        {

                        }
                        break;
                    default:
                        WriteLog("Unknown cmd");
                        break;
                }
            }
            catch(Exception e)
            {
                WriteLog("UnPack Error:" + e.ToString());
            }
        }
    }
}

