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
    //private int m_gold = 0; // 总金额
    //private int m_key = 0; // Key
    private bool m_login; // 是否登录
    private ProtoNet m_net; // 网络处理

    // Use this for initialization
	void Start () {
        //m_key = 123456;
        //m_gold = 0;
        m_net = new ProtoNet();
        m_login = false;

        // 初始化ProtoNet
        m_net = new ProtoNet();

        // 增加前台支持的网络包类型
        m_net.Add(Constants.Server_LoginResp, LoginResp.Parser);
        m_net.Add(Constants.Server_RedirectResp, RedirectResp.Parser);
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

        string btnName2 = "poker";
        GameObject btnObj2 = GameObject.Find(btnName2);
        Button btn2 = btnObj2.GetComponent<Button>();
        btn2.onClick.AddListener(delegate()
        {
            this.OnClick(btnObj2);
        });
        // 登录获取UId
        //Login();
	}

    void OnClick(GameObject sender)
    {
        if (sender.name == "slot")
        {
            // 检查是否登录
            if (m_login)
            {
                Redirect();
            }
            else
            {
                WorkDone callBack = new WorkDone(Redirect);
                Login(callBack);
            }
        }
        else if (sender.name == "poker")
        {
            DialogQuit.Show();
        }
    }

    void Login(WorkDone callBack = null)
    {
        LoginReq loginReq = new LoginReq();
        loginReq.Version = 1;
        loginReq.Args.Add("TEST");
        loginReq.Args.Add("1");
        loginReq.Args.Add("wdz");

        m_net.SendEnqueue(Constants.Client_LoginReq,
            0, 
            loginReq,
            callBack);
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
        RedirectReq rdReq = new RedirectReq();
        rdReq.UserId = Lobby.getInstance().UId;
        rdReq.Svc = 100; // 老虎机
        rdReq.Version = 1;
        rdReq.SubSvc = 0;

        m_net.SendEnqueue(Constants.Client_RedirectReq, 0, rdReq);
    }
    void OnApplicationQuit()
    {
        m_net.Close();
    }

	// Update is called once per frame
	void Update () {
        if (Input.GetKey(KeyCode.Escape))
        {
            if (DialogQuit.Actived())
            {
                Debug.Log("Hide");
                DialogQuit.Hide();
            }
            else
            {
                Debug.Log("Show");
                DialogQuit.Show();
            }
        }

        if (!m_net.IsRunning())
        {
            // 主动结束了
            return;
        }

        if (m_net.CheckReconnect())
        {
            CheckLogin();

            DialogReconnect.Hide();
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
                        Lobby.getInstance().UId = loginResp.UserId;
                        m_login = true;

                        if (packet.callback != null)
                        {
                            Debug.Log("Call back here");
                            packet.callback();
                        }
                    }
                    break;
                case Constants.Server_RedirectResp:
                    {
                        RedirectResp rdResp = (RedirectResp)packet.proto;
                        Lobby lobby = Lobby.getInstance();
                        lobby.Domain = rdResp.Domain;
                        lobby.Port = rdResp.Port;
                        lobby.Key = rdResp.Key;

                        // 切换到游戏场景中
                        m_net.Close(false);
                        Debug.Log("Reception enter slot scene");
                        Global.NextSceneName = "slot";
                        SceneManager.LoadScene("loading");
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
                            DialogReconnect.Show();
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
