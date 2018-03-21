using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Dog.Proto;
using Login.Proto;
using System.Threading;
using UnityEngine.SceneManagement;

// 前台-目前只负责一个客户
// 负责大大厅里传送消息
// 转达客户请求到不同的柜台
public class Reception : MonoBehaviour
{
    // 需要客户的基本信息
    private int m_gold = 0; // 总金额
    private int m_key = 0; // Key
    private bool m_login; // 是否登录
    private ProtoNet m_net; // 网络处理

    public Dictionary<int, int> CallbackDict = new Dictionary<int,int>();

	// Use this for initialization
	void Start () {
        m_key = 123456;
        m_gold = 0;
        m_net = new ProtoNet();
        m_login = false;

        // 初始化ProtoNet
        m_net = new ProtoNet();
        // 增加前台支持的网络包类型
        m_net.Add(Constants.Server_LoginResp, typeof(LoginResp));
        m_net.Add(Constants.Server_RedirectResp, typeof(RedirectResp));
        m_net.Add(Constants.Client_Reconnect, null);
        m_net.Add(Constants.Server_Error, null);
        m_net.Name = "Reception";

        // 启动登录
        if (false == m_net.Init("182.92.74.240", 7900))
        {
            // 这里不重连，在发送请求失败后再重连
            Debug.Log("Client init failed!");
        }
        
        string btnName = "slot";
        GameObject btnObj = GameObject.Find(btnName);
        Button btn = btnObj.GetComponent<Button>();
        btn.onClick.AddListener(delegate()
        {
            this.OnClick(btnObj);
        });
        // 登录获取UId
        //Login();
	}

    void OnClick(GameObject sender)
    {
        if (sender.name == "slot")
        {
            Debug.Log("Reception redirect for slot");
            Redirect();
        }
    }

    void Login()
    {
        LoginReq loginReq = new LoginReq();
        loginReq.version = 1;
        loginReq.args.Add("TEST");
        loginReq.args.Add("1");
        loginReq.args.Add("wdz");

        m_net.SendEnqueue(Constants.Client_LoginReq, 0, loginReq);
    }

    void CheckLogin()
    {
        if (!m_login)
        {
            Login();
        }
    }

    void Redirect()
    {
        CheckLogin();

        RedirectReq rdReq = new RedirectReq();
        rdReq.user_id = Lobby.getInstance().UId;
        rdReq.svc = 100; // 老虎机
        rdReq.version = 1;
        rdReq.sub_svc = 0;

        m_net.SendEnqueue(Constants.Client_RedirectReq, 0, rdReq);
    }
    void OnApplicationQuit()
    {
        m_net.Close();
    }
	
	// Update is called once per frame
	void Update () {
        if (m_net.CheckReconnect())
        {
            CheckLogin();
        }

        ProtoPacket packet = new ProtoPacket();
        if (m_net.RecvTryDequeue(ref packet))
        {
            Debug.Log("Reception handle cmdId:" + packet.cmdId);
            switch (packet.cmdId)
            {
                case Constants.Server_LoginResp:
                    {
                        LoginResp loginResp = (LoginResp)packet.proto;
                        Lobby.getInstance().UId = loginResp.user_id;
                        m_login = true;
                    }
                    break;
                case Constants.Server_RedirectResp:
                    {
                        RedirectResp rdResp = (RedirectResp)packet.proto;
                        Lobby lobby = Lobby.getInstance();
                        lobby.Domain = rdResp.domain;
                        lobby.Port = rdResp.port;
                        lobby.Key = rdResp.key;

                        // 切换到游戏场景中
                        Debug.Log("Reception enter slot scene");
                        SceneManager.LoadScene("slot");
                    }
                    break;
                case Constants.Client_Reconnect:
                    {
                        // 展示重连对话框，直到重连成功
                        ProtoNet.WriteLog("Reconnecting...");
                        if (packet.msgId > 0)
                        {
                            // 3s后Display中重连
                            m_net.CheckReconnect(3);
                        }
                    }
                    break;
                case Constants.Server_Error:
                    {
                        // 展示错误
                    }
                    break;
                default:
                    {
                        Debug.Log("Reception invalid cmdId:" + packet.cmdId);
                    }
                    break;
            }
        }
	}
}
