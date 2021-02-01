using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;
using UnityEngine;

using System.Threading.Tasks;
using UnityEngine.Networking;

public struct testDataSend
{
    public string data;
}

public struct testDataGet
{
    public string hello;
}

/// <summary>
/// Async version of server manager
/// </summary>
public class TrinaxAsyncServerManager : MonoBehaviour, IManager
{
    private void Awake()
    {
        if (TrinaxManager.trinaxAsyncServerManager == null || TrinaxManager.trinaxAsyncServerManager != this)
        {
            TrinaxManager.trinaxAsyncServerManager = this;
        }
    }

    int executionPriority = 200;
    public int ExecutionPriority
    {
        get { return executionPriority; }
        set { value = executionPriority; }
    }
    public bool IsReady { get; set; }

    public async Task Init()
    {
        await new WaitUntil(() => TrinaxGlobal.Instance.loadNow);
        Debug.Log("Loading ServerManager");
        IsReady = false;

        //LOG_DIR = Application.dataPath + LOG_DIR;

        //if (!Directory.Exists(LOG_DIR))
        //Directory.CreateDirectory(LOG_DIR);

        if (loadingCircle != null)
            loadingCircle.SetActive(false);

        IsReady = true;
        Debug.Log("ServerManager is loaded!");
    }

    public void PopulateValues(GlobalSettings setting)
    {
        ip = setting.IP;
        useServer = setting.useServer;
        useMocky = setting.useMocky;
    }

    public static Action OnConnectionLost;
    public static Action OnMaxRetriesReached;

    public string ip = "127.0.0.1";
    public bool useServer;
    public bool useMocky;
    public GameObject loadingCircle;

    public string userID;

    const string playerServletUrl = "SBTC/Servlet/playerServlet/";
    const string interactionServletUrl = "SBTC/Servlet/interactionApiServlet/";
    const string port = ":8080/";

    public const string ERROR_CODE_200 = "200";
    public const string ERROR_CODE_201 = "201";

    bool isLoading = false;

    string LOG_DIR = "/log/";
    string LOG_FILE = "Async_server_logs.log";

    #region API Funcs
    public async Task TestAPIGET(Action<bool, testDataGet> callback)
    {
        //string sendJson = JsonUtility.ToJson(json);
        string url = "http://www.mocky.io/v2/5eba2eaf2f00005f523c357a";

        testDataGet data = new testDataGet();

        var r = await LoadUrlAsync(url, (request) => {
            if (string.IsNullOrEmpty(request.error))
            {
                string result = request.downloadHandler.text.Trim();
                data = JsonUtility.FromJson<testDataGet>(result);
                Debug.Log("myresult: " + result);
                Debug.Log("data: " + data.hello);
                Debug.Log(request.responseCode);
                if (!string.IsNullOrEmpty(data.hello))
                { 
                    Debug.Log("not empty");
                    callback(true, data);
                }
                else
                {
                    Debug.Log("empty");
                    callback(false, data);
                }
            }
            else
            {
                Debug.Log("request failed");
                //WriteError(request, "StartInteraction");
                data = new testDataGet();
                callback(false, data);
            }
        });
    }

    public async Task TestAPIPOST(testDataSend json, Action<bool, testDataGet> callback)
    {
        string sendJson = JsonUtility.ToJson(json);
        string url = "http://www.mocky.io/v2/5eba30232f00005e523c3585";

        testDataGet data = new testDataGet();

        var r = await LoadPostUrl(sendJson, url, (request) => {
            if (string.IsNullOrEmpty(request.error))
            {
                string result = request.downloadHandler.text.Trim();
                data = JsonUtility.FromJson<testDataGet>(result);
                Debug.Log("myresult: " + result);
                Debug.Log("data: " + data.hello);
                Debug.Log(request.responseCode);
                if (!string.IsNullOrEmpty(data.hello))
                {
                    Debug.Log("not empty");
                    callback(true, data);
                }
                else
                {
                    Debug.Log("empty");
                    callback(false, data);
                }
            }
            else
            {
                Debug.Log("request failed");
                //WriteError(request, "StartInteraction");
                data = new testDataGet();
                callback(false, data);
            }
        });
    }

    public async Task StartInteraction(StartInteractionSendJsonData json, Action<bool, StartInteractionReceiveJsonData> callback)
    {
        StartInteractionReceiveJsonData data;
        if (!useServer)
        {
            data = new StartInteractionReceiveJsonData();
            callback(true, data);
            return;
        }

        string sendJson = JsonUtility.ToJson(json);
        string url;

        if (!useMocky)
        {
            //Debug.Log("Using actual server url...");
            url = "http://" + ip + port + interactionServletUrl + "startInteraction";
        }
        else
        {
            //Debug.Log("Using mocky url...");
            url = "http://www.mocky.io/v2/5b6122fb3000007f006a3ffc";
        }

        var r = await LoadPostUrl(url, sendJson, (request) =>
        {
            if (string.IsNullOrEmpty(request.error))
            {
                string result = request.downloadHandler.text.Trim();
                data = JsonUtility.FromJson<StartInteractionReceiveJsonData>(result);
                //Debug.Log("myresult: " + result);

                if (data.error.error_code == ERROR_CODE_200)
                {
                    callback(true, data);
                }
                else
                {
                    callback(false, data);
                }
            }
            else
            {
                WriteError(request, "StartInteraction");
                data = new StartInteractionReceiveJsonData();
                callback(false, data);
            }
        });

        //Debug.Log(r);
    }

    public async Task AddInteractionDetails(InteractionDetailsSendJsonData json, Action<bool, InteractionDetailsReceiveJsonData> callback)
    {
        InteractionDetailsReceiveJsonData data;
        if (!useServer)
        {
            data = new InteractionDetailsReceiveJsonData();
            callback(true, data);
            return;
        }

        string sendJson = JsonUtility.ToJson(json);
        string url;

        if (!useMocky)
        {
            //Debug.Log("Using actual server url...");
            url = "http://" + ip + port + interactionServletUrl + "addInteraction";
        }
        else
        {
            //Debug.Log("Using mocky url...");
            url = "http://www.mocky.io/v2/5b6122fb3000007f006a3ffc";
        }

        var r = await LoadPostUrl(url, sendJson, (request) =>
        {
            if (string.IsNullOrEmpty(request.error))
            {
                string result = request.downloadHandler.text.Trim();
                data = JsonUtility.FromJson<InteractionDetailsReceiveJsonData>(result);
                //Debug.Log("myresult: " + result);

                if (data.error.error_code == ERROR_CODE_200)
                {
                    callback(true, data);
                }
                else
                {
                    callback(false, data);
                }
            }
            else
            {
                WriteError(request, "InteractionDetails");
                data = new InteractionDetailsReceiveJsonData();
                callback(false, data);
            }
        });

        //Debug.Log(r);
    }

    public async Task EndInteraction(Action<bool, InteractionEndReceiveJsonData> callback)
    {
        InteractionEndReceiveJsonData data;
        if (!useServer)
        {
            data = new InteractionEndReceiveJsonData();
            callback(true, data);
            return;
        }

        string url;

        if (!useMocky)
        {
            //Debug.Log("Using actual server url...");
            url = "http://" + ip + port + interactionServletUrl + "endInteraction/" + TrinaxGlobal.Instance.userData.interactionID;
        }
        else
        {
            //Debug.Log("Using mocky url...");
            url = "http://www.mocky.io/v2/5b6122fb3000007f006a3ffc";
        }

        var r = await LoadUrlAsync(url, (request) =>
        {
            if (string.IsNullOrEmpty(request.error))
            {
                string result = request.downloadHandler.text.Trim();
                data = JsonUtility.FromJson<InteractionEndReceiveJsonData>(result);
                //Debug.Log("myresult: " + result);

                if (data.error.error_code == ERROR_CODE_200)
                {
                    callback(true, data);
                }
                else
                {
                    callback(false, data);
                }
            }
            else
            {
                WriteError(request, "EndInteraction");
                data = new InteractionEndReceiveJsonData();
                callback(false, data);
            }
        });

        //Debug.Log(r);
    }

    public async Task AddResult(AddResultSendJsonData json,Action<bool, AddResultReceiveJsonData> callback)
    {
        AddResultReceiveJsonData data;
        if (!useServer)
        {
            data = new AddResultReceiveJsonData();
            callback(true, data);
            return;
        }

        string sendJson = JsonUtility.ToJson(json);
        string url;
        if (!useMocky)
        {
            url = "http://" + ip + port + playerServletUrl + "add";
        }
        else
        {
            url = "http://www.mocky.io/v2/5ba30c362f00005c008d2eed";
        }

        var r = await LoadPostUrl(url, sendJson, (request) =>
        {
            if (string.IsNullOrEmpty(request.error))
            {
                string result = request.downloadHandler.text.Trim();
                data = JsonUtility.FromJson<AddResultReceiveJsonData>(result);
                //Debug.Log("myresult: " + result);

                if (data.error.error_code == ERROR_CODE_200)
                {
                    callback(true, data);
                }
                else
                {
                    callback(false, data);
                }
            }
            else
            {
                WriteError(request, "AddResult");
                data = new AddResultReceiveJsonData();
                callback(false, data);
            }
        });
    }

    public async Task PopulateLeaderboard(Action<bool, LeaderboardReceiveJsonData> callback)
    {
        LeaderboardReceiveJsonData data;
        if (!useServer)
        {
            data = new LeaderboardReceiveJsonData();
            callback(true, data);
            return;
        }

        string url;
        if (!useMocky)
        {
            Debug.Log("Using actual server url...");
            url = "http://" + ip + port + playerServletUrl + "getLeaderBoard";
        }
        else
        {
            Debug.Log("Using mocky url...");
            url = "http://www.mocky.io/v2/5ba30c4e2f000077008d2eee";
        }

        var r = await LoadUrlAsync(url, (request) =>
        {
            if (string.IsNullOrEmpty(request.error))
            {
                string result = request.downloadHandler.text.Trim();
                data = JsonUtility.FromJson<LeaderboardReceiveJsonData>(result);
                //Debug.Log("myresult: " + result);

                if (data.error.error_code == ERROR_CODE_200)
                {
                    callback(true, data);
                }
                else
                {
                    callback(false, data);
                }
            }
            else
            {
                WriteError(request, "PopulateLeaderboard");
                data = new LeaderboardReceiveJsonData();
                callback(false, data);
            }
        });
    }

    #endregion

    async Task<string> LoadUrlAsync(string url, Action<UnityWebRequest> callback)
    {
        isLoading = true;

        DelayLoadingCircle();

        Debug.Log("Loading url: " + url);
        UnityWebRequest request = UnityWebRequest.Get(url);
        await request.SendWebRequest();
        Debug.Log(url + "\n" + request.downloadHandler.text);

        //if (TrinaxGlobal.Instance.state == PAGES.SCREENSAVER)
            //GameManager.Instance.startGameBtn.interactable = false;

        await ApiCallback(request, url, (UnityWebRequest r) => { request = r; });
        //await new WaitForSeconds(3.0f); // artifical wait

        callback(request);

        isLoading = false;
        if (loadingCircle != null)
        {
            loadingCircle.SetActive(false);
        }

        //if (TrinaxGlobal.Instance.state == PAGES.SCREENSAVER)
            //GameManager.Instance.startGameBtn.interactable = true;
        //Debug.Log(request.text);
        return request.downloadHandler.text;
    }

    //WWW requestPost, resultGet;
    async Task<string> LoadPostUrl(string url, string jsonString, Action<UnityWebRequest> callback)
    {
        isLoading = true;
        //then set the headers Dictionary headers=form.headers; headers["Content-Type"]="application/json";

        DelayLoadingCircle();

        byte[] jsonSenddata = null;
        if (!string.IsNullOrEmpty(jsonString))
        {
            Debug.Log(jsonString);
            jsonSenddata = System.Text.Encoding.UTF8.GetBytes(jsonString);
        }


        Debug.Log("Loading url: " + url);

        using (UnityWebRequest www = UnityWebRequest.Post(url, jsonString))
        {
            www.SetRequestHeader("Content-Type", "application/json");

            await www.SendWebRequest();

            if (www.isNetworkError)
            {
                Debug.Log(www.error);
            }
            else
            {
                Debug.Log(www.downloadHandler.text);
            }
            return www.downloadHandler.text;
        }

        ////UnityWebRequest request = await UnityWebRequest.Post(url, jsonSenddata, headers);
        //Debug.Log(url + "\n" + request.text);

        //await ApiCallback(request, url, jsonSenddata, headers, (WWW r) => { request = r; });

        ////await new WaitForSeconds(3f); // artifical wait for 150ms

        //callback(request);

        //isLoading = false;
        //if (loadingCircle != null)
        //{
        //    loadingCircle.SetActive(false);
        //}
        ////Debug.Log(request.text);
        //return request.text;
    }

    bool apiDone = false;
    async Task ApiCallback(UnityWebRequest _request, string _url, WWWForm _form, Action<UnityWebRequest> _result)
    {
        await LoopResultPost(_request, _url, _form, _result);
    }

    async Task ApiCallback(UnityWebRequest _request, string _url, Action<UnityWebRequest> _result)
    {
        await LoopResultGet(_request, _url, _result);
    }

    int numOfRetries = 10;
    public GameObject lostConnectionPanel;
    IEnumerator LoopResultPost(UnityWebRequest _request, string _url, WWWForm _form, Action<UnityWebRequest> _result)
    {
        int tries = 0;
        while (true)
        {
            if (string.IsNullOrEmpty(_request.downloadHandler.text))
            {
                OnConnectionLost?.Invoke();
                UnityWebRequest r = UnityWebRequest.Post(_url, _form);
                yield return r.SendWebRequest();
                if (string.IsNullOrEmpty(r.downloadHandler.text))
                {
                    if (!lostConnectionPanel.activeSelf)
                    {
                        lostConnectionPanel.SetActive(true);
                    }
                    tries++;
                    if (tries >= numOfRetries)
                    {
                        Debug.Log("Tried " + numOfRetries + " !" + " Going back to start...");
                        OnMaxRetriesReached?.Invoke();
                        yield return new WaitForSeconds(5f);
                        AppManager.uiManager.ToScreensaver();
                        lostConnectionPanel.SetActive(false);
                        yield break;
                    }
                    Debug.Log("Request from server is empty! Getting a new request...");

                    yield return new WaitForSeconds(3f);
                }
                else
                {
                    //requestPost = r;
                    _result(r);
                    Debug.Log("Result: " + r.downloadHandler.text);
                    lostConnectionPanel.SetActive(false);
                    yield break;
                }
            }
            else yield break;
        }
    }

    IEnumerator LoopResultGet(UnityWebRequest _request, string _url, Action<UnityWebRequest> _result)
    {
        int tries = 0;
        while (true)
        {
            if (string.IsNullOrEmpty(_request.downloadHandler.text))
            {
                OnConnectionLost?.Invoke();
                UnityWebRequest r = UnityWebRequest.Get(_url);
                yield return r.SendWebRequest();
                if (string.IsNullOrEmpty(r.downloadHandler.text))
                {
                    if (!lostConnectionPanel.activeSelf)
                    {
                        lostConnectionPanel.SetActive(true);
                    }
                    tries++;
                    Debug.Log(tries);
                    if (tries >= numOfRetries)
                    {
                        Debug.Log("Tried " + numOfRetries + " !" + " Going back to start...");
                        OnMaxRetriesReached?.Invoke();
                        yield return new WaitForSeconds(5f);
                        AppManager.uiManager.ToScreensaver();
                        lostConnectionPanel.SetActive(false);
                        yield break;
                    }
                    Debug.Log("Request from server is empty! Getting a new request...");

                    yield return new WaitForSeconds(3f);
                }
                else
                {
                    _result(r);
                    Debug.Log("Result: " + r.downloadHandler.text);
                    lostConnectionPanel.SetActive(false);
                    yield break;
                }
            }
            else yield break;
        }
    }

    async void DelayLoadingCircle()
    {
        await new WaitForSeconds(1.5f);

        if (isLoading && loadingCircle != null)
            loadingCircle.SetActive(true);
    }

    void WriteError(UnityWebRequest request, string api)
    {
        //string error = "<" + api + "> --- Begin Error Message: " + request.error + " >> Url: " + request.url + System.Environment.NewLine;
        //File.AppendAllText(LOG_DIR + LOG_FILE, error);
    }

    void WriteError(string errorStr, string api)
    {
        //string error = "<" + api + "> --- Begin Error Message: " + errorStr + System.Environment.NewLine;
        //File.AppendAllText(LOG_DIR + LOG_FILE, error);
    }
}
