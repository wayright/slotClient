using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Dog.Proto;
using Login.Proto;
using Common.Proto;
using System.Threading.Tasks;
using System.Net;

// 启动加载
public class StartLoading : MonoBehaviour {
    private AsyncOperation async;//异步加载操作  
    private int process;//加载的进度  
    private string startScene = "lobby";
    private bool m_login = false; // 是否登录
    private ProtoNet m_net; // 网络
    private int m_rcCount = 0;
    private bool m_auto = false;
    
	void Start () {
        //StartCoroutine(LoadingScene());
        // Login first
        m_login = false;

        // 初始化ProtoNet
        m_net = new ProtoNet();

        // 增加前台支持的网络包类型
        m_net.Add(Constants.Dog_Login, LoginResp.Parser);
        m_net.Add(Constants.Dog_Redirect, RedirectResp.Parser);
        m_net.Add(Constants.Error, Status.Parser);
        m_net.Name = "Door";
        m_net.Init("182.92.74.240", 7900);

        GameObject.Find("RcText").GetComponent<Text>().text = "Loading...";
        //string myUID = SystemInfo.deviceUniqueIdentifier;
        //DialogBase.Show("UID", myUID);

        if (GlobalVars.instance.Testing || GlobalVars.instance.SwitchUser)
        {
            m_auto = false;
            DialogLogin.Show(LoginAsGuest, LoginWithFsID, null);
        }
        else
        {
            m_auto = true;
        }
        GlobalVars.instance.SwitchUser = false;
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
        // GetIP
        if (GlobalVars.instance.LoginType == Constants.Login_UUID)
        {            
            LoginReq loginReq = new LoginReq();
            loginReq.Version = 1;
            loginReq.Args.Add("TEST"); // Platform
            loginReq.Args.Add("1"); // AppId

            string loginGuid = GlobalVars.instance.LoginGuid;
            if (loginGuid == "")
            {                
                loginGuid = System.Guid.NewGuid().ToString();
                DebugConsole.Log("Create UUID:" + loginGuid);
                GlobalVars.instance.LoginGuid = loginGuid;
            }
            loginReq.Args.Add(loginGuid); // UUID
            DebugConsole.Log("Login with UUID:" + loginGuid);
            m_net.SendEnqueue(Constants.Dog_Login,
                0,
                loginReq,
                callBack); 
        }
        else
        {            
            LoginReq loginReq = new LoginReq();
            loginReq.Version = 2; // 1-UUID, 2-email+passMD5
            loginReq.Args.Add("TEST"); // Platform
            loginReq.Args.Add("1"); // AppId
            loginReq.Args.Add(GlobalVars.instance.LoginEmail);
            DebugConsole.Log("Login with Email:" + GlobalVars.instance.LoginEmail);
            loginReq.Args.Add(GlobalVars.instance.LoginPwd);
            m_net.SendEnqueue(Constants.Dog_Login,
                0,
                loginReq,
                callBack);
        }
    }
    void CheckLogin()
    {
        if (!m_login)
        {
            Login();
        }
    }
    void OnOK()
    {
        m_rcCount = 0;
    }
    void OnCancel()
    {
        m_net.Close();
        Application.Quit();
    }
	// Update is called once per frame
	void Update () {
        if (DialogLogin.Actived())
        {
            return;
        }

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
        if (m_rcCount > 3)
        {
            DialogWarning.Show("No Network!",
                        "Your internet seems to be down.\nPlease check your network settings.",
                        "Retry",
                        "Exit",
                        OnOK,
                        OnCancel,
                        OnOK);
            return;
        }

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
                            m_rcCount++;
                        }
                        else if (packet.msgId == 2)
                        {
                            DebugConsole.Log("Door:Reconnect successful.");
                            //DialogBase.Hide();
                            GameObject.Find("RcText").GetComponent<Text>().text = "Connect successfully.";
                            // 启动默认登录
                            if (m_auto)
                                Login(Redirect);
                        }
                    }
                    break;
                case Constants.Error:
                    {
                        // 这里一定是登录错误？
                        Status stat = (Status)packet.proto;
                        string err = "Error:" + stat.Code.ToString() + "-" + stat.Desc;
                        GameObject.Find("RcText").GetComponent<Text>().text = err;

                        // 打开登录对话框
                        DialogLogin.Show(LoginAsGuest, LoginWithFsID, null);
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
    void LoginWithFsID()
    {
        // 强制以email登录
        GlobalVars.instance.LoginType = Constants.Login_Email;
        GlobalVars.instance.LoginEmail = DialogLoginByEmail.GetInstance().Email;
        GlobalVars.instance.LoginPwd = Tools.GetMD5(DialogLoginByEmail.GetInstance().Password);

        Login(Redirect);
    }
    void LoginAsGuest()
    {
        // 若当前loginType为Email或者其他
        if (GlobalVars.instance.LoginType != Constants.Login_UUID)
        {           
            // 保证loginType为UUID
            GlobalVars.instance.LoginType = Constants.Login_UUID;
        }

        Login(Redirect);
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
