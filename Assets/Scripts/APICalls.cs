using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;

using System.Linq;

public static class APICalls
{
    #region APICALLS

    const string DEVICEID = "001";
    public static async Task RunStartInteraction()
    {
        StartInteractionSendJsonData sJson = new StartInteractionSendJsonData
        {
            deviceID = DEVICEID,
        };

        await TrinaxManager.trinaxAsyncServerManager.StartInteraction(sJson, (bool success, StartInteractionReceiveJsonData rJson) =>
        {
            if (success)
            {
                if (!string.IsNullOrEmpty(rJson.data))
                {
                    TrinaxGlobal.Instance.userData.interactionID = rJson.data;
                    Debug.Log("Started interaction!");
                }
            }
            else
            {
                Debug.Log("Error in <StartInteraction> API!");
            }
        });
    }

    public static async Task RunAddInteraction()
    {
        InteractionDetailsSendJsonData sJson = new InteractionDetailsSendJsonData
        {
            interactionID = TrinaxGlobal.Instance.userData.interactionID,
        };

        await TrinaxManager.trinaxAsyncServerManager.AddInteractionDetails(sJson, (bool success, InteractionDetailsReceiveJsonData rJson) =>
        {
            if (success)
            {
                if (rJson.data)
                {
                    Debug.Log("Added interaction!");
                }
            }
            else
            {
                Debug.Log("Error in <AddInteraction> API!");
            }
        });
    }

    public static async Task RunEndInteraction()
    {
        await TrinaxManager.trinaxAsyncServerManager.EndInteraction((bool success, InteractionEndReceiveJsonData rJson) =>
        {
            if (success)
            {
                if (rJson.data)
                {
                    Debug.Log("End interaction!");
                }
            }
            else
            {
                Debug.Log("Error in <EndInteraction> API!");
            }
        });
    }

    public static async Task RunUpdateLeaderboard(LeaderboardDisplay lb)
    {
        await TrinaxManager.trinaxAsyncServerManager.PopulateLeaderboard((bool success, LeaderboardReceiveJsonData rJson) =>
        {
            if (success)
            {
                if (rJson.data != null)
                {
                    Debug.Log("Success populated leaderboard!");
                    lb.PopulateData(rJson);

                    var last = rJson.data[rJson.data.Count - 1];
                    AppManager.gameManager.lastPlaceScore = last.score;
                }
            }
            else
            {
                Debug.Log("Error in <GetTopScore> API!");
                lb.PopulateDefault();
            }
        });
    }

    public static async Task RunAddGameResult()
    {
        AddResultSendJsonData sJson = new AddResultSendJsonData
        {

        };

        await TrinaxManager.trinaxAsyncServerManager.AddResult(sJson, (bool success, AddResultReceiveJsonData rJson) =>
        {
            if (success)
            {
                if (!string.IsNullOrEmpty(rJson.data))
                {
                    Debug.Log("Send " + sJson.name + " | " + sJson.score);
                }
            }
            else
            {
                Debug.Log("Error in <AddResult> API!");
            }
        });
    }
    #endregion
}
