using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class Global
{
    public static string NextSceneName;
}

public class Loading : MonoBehaviour {
    private AsyncOperation async;//异步加载操作  
    int process;//加载的进度  
	// Use this for initialization
	void Start () {
        StartCoroutine(LoadingScene());
	}
	
	// Update is called once per frame
	void Update () {
		//process = (int)(async.progress * 100); 
	}

    IEnumerator LoadingScene()  
    {
        async = SceneManager.LoadSceneAsync(Global.NextSceneName);
        yield return async;
        
        int displayProgress = 0;
        int toProgress = 0;
        AsyncOperation op = SceneManager.LoadSceneAsync(Global.NextSceneName);
        op.allowSceneActivation = false;
        while (op.progress < 0.9f)
        {
            toProgress = (int)op.progress * 100;
            while (displayProgress < toProgress)
            {
                ++displayProgress;
                Debug.Log("Progress:" + displayProgress);
                //SetLoadingPercentage(displayProgress);
                yield return new WaitForEndOfFrame();
            }
        }

        toProgress = 100;
        while (displayProgress < toProgress)
        {
            ++displayProgress;
            //SetLoadingPercentage(displayProgress);
            Debug.Log("Progress:" + displayProgress);
            yield return new WaitForEndOfFrame();
        }
        op.allowSceneActivation = true;
    }
}
