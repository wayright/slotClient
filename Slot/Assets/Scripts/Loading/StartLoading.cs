using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Dog.Proto;
using Login.Proto;
using System.Threading.Tasks;

// 启动加载
public class StartLoading : MonoBehaviour {
    private AsyncOperation async;//异步加载操作  
    private int process;//加载的进度  
    private string startScene = "lobby";
    private bool m_login = false; // 是否登录
    private ProtoNet m_net; // 网络

	void Start () {
        //StartCoroutine(LoadingScene());
        // Login first
        m_login = false;

        // 初始化ProtoNet
        m_net = new ProtoNet();

        // 增加前台支持的网络包类型
        m_net.Add(Constants.Dog_Login, LoginResp.Parser);
        m_net.Add(Constants.Dog_Redirect, RedirectResp.Parser);
        m_net.Name = "Door";
        m_net.Init("182.92.74.240", 7900);

        GameObject.Find("RcText").GetComponent<Text>().text = "Loading...";
        //string myUID = SystemInfo.deviceUniqueIdentifier;
        //DialogBase.Show("UID", myUID);
	}
    void Redirect()
    {
        RedirectReq rdReq = new RedirectReq();
        rdReq.UserId = Lobby.getInstance().UId;
        rdReq.Svc = Constants.Svc_Lion; // Dog->Lion
        rdReq.Version = 1;
        rdReq.SubSvc = 0;

        m_net.SendEnqueue(Constants.Dog_Redirect, 0, rdReq);
    }
    void Login(WorkDone callBack = null)
    {
        LoginReq loginReq = new LoginReq();
        loginReq.Version = 1;
        loginReq.Args.Add("TEST");
        loginReq.Args.Add("1");
        loginReq.Args.Add("wdz");

        m_net.SendEnqueue(Constants.Dog_Login,
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
	// Update is called once per frame
	void Update () {
        if (!m_net.IsRunning())
        {
            // 主动结束了
            return;
        }

        // Step
        m_net.CheckReconnect();
        //if (m_net.CheckReconnect())
        //{
            //DebugConsole.Log("Door:Reconnect successful.");
            //CheckLogin();

            //DialogBase.Hide();
        //}

        ProtoPacket packet = new ProtoPacket();
        if (m_net.RecvTryDequeue(ref packet))
        {
            DebugConsole.Log("Door:Reception handle cmdId:" + packet.cmdId);
            switch (packet.cmdId)
            {
                case Constants.Dog_Login:
                    {
                        LoginResp loginResp = (LoginResp)packet.proto;
                        Lobby.getInstance().UId = loginResp.UserId;
                        DebugConsole.Log("UId:" + loginResp.UserId);
                        m_login = true;
                        
                        // 登录成功，重定向
                        if (packet.callback != null)
                        {
                            // 重定向
                            packet.callback();
                        }                        
                    }
                    break;
                case Constants.Dog_Redirect:
                    {                        
                        RedirectResp rdResp = (RedirectResp)packet.proto;
                        Lobby lobby = Lobby.getInstance();
                        lobby.Domain = rdResp.Domain;
                        lobby.Port = rdResp.Port;
                        lobby.Key = rdResp.Key;

                        m_net.Close();
                        // 重定向到大厅
                        DebugConsole.Log("Door:Redirect to lobby:" +
                            lobby.Domain + ":" + lobby.Port);

                        StartCoroutine(LoadingScene());
                    }
                    break;
                case Constants.Reconnect:
                    {
                        // 展示重连对话框，直到重连成功                        
                        if (packet.msgId == 1)
                        {
                            ProtoNet.WriteLog("Door:Reconnecting...");
                            // 3s后Display中重连
                            m_net.CheckReconnect(5);
                            //DialogBase.Show("RECONNECT", "reconnecting");
                            GameObject.Find("RcText").GetComponent<Text>().text = "Reconnecting...";
                            m_net.Ip = "182.92.74.240";
                        }
                        else if (packet.msgId == 2)
                        {
                            DebugConsole.Log("Door:Reconnect successful.");
                            //DialogBase.Hide();
                            GameObject.Find("RcText").GetComponent<Text>().text = "Connect successfully.";
                            WorkDone callBack = new WorkDone(Redirect);
                            Login(callBack);                            
                        }
                    }
                    break;
                case Constants.Error:
                    {
                        // 展示错误
                    }
                    break;
                default:
                    {
                        DebugConsole.Log("Door:Invalid cmdId:" + packet.cmdId);
                    }
                    break;
            }
        }
		//process = (int)(async.progress * 100); 
	}

    IEnumerator LoadingScene()  
    {
        async = SceneManager.LoadSceneAsync(startScene);
        yield return async;

        int displayProgress = 0;
        int toProgress = 0;
        AsyncOperation op = SceneManager.LoadSceneAsync(startScene);
        op.allowSceneActivation = false;
        while (op.progress < 0.9f)
        {
            toProgress = (int)op.progress * 100;
            while (displayProgress < toProgress)
            {
                ++displayProgress;
                GameObject.Find("RcText").GetComponent<Text>().text = "Loading..." + displayProgress.ToString() + "%";
                DebugConsole.Log("Door:Progress:" + displayProgress);
                //SetLoadingPercentage(displayProgress);
                yield return new WaitForEndOfFrame();
            }
        }

        toProgress = 100;
        while (displayProgress < toProgress)
        {
            ++displayProgress;
            GameObject.Find("RcText").GetComponent<Text>().text = "Loading..." + displayProgress.ToString() + "%";
            //SetLoadingPercentage(displayProgress);
            DebugConsole.Log("Door:Progress:" + displayProgress);
            yield return new WaitForEndOfFrame();
        }
        op.allowSceneActivation = true;
    }
}
