using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class TrinaxSaves
{
    public GlobalSettings globalSettings;
    public GameSettings gameSettings;
    public AudioSettings audioSettings;
    public KinectSettings kinectSettings;
}

[System.Serializable]
public struct GlobalSettings
{
    public string IP;
    public string photoPath;
    public float idleInterval;

    public string COMPORT1;

    public bool useServer;
    public bool useMocky;
    public bool useKeyboard;
    public bool muteAllSounds;
}

[System.Serializable]
public struct AudioSettings
{
    public float masterVolume;
    public float musicVolume;
    public float SFXVolume;
    public float SFX2Volume;
    public float SFX3Volume;
    public float SFX4Volume;
    public float UI_SFXVolume;
    public float UI_SFX2Volume;
}

[System.Serializable]
public struct GameSettings
{
    //public int gameDuration;
    public float spawnInterval;

    public float minPowerupSpawn;
    public float maxPowerupSpawn;

    public int pointsPerObject;

    public int xpPerObject;
    public float xpPerObjectMultipler; // scale down
    public int MAX_XP_REQUIRED_PER_BONUS;

    public float timeToActivateNextPattern;
    public float DifficultyIncreaseInterval;
    public int probabilityScale;

    public float firstAidRecover;
    public int pointsPerCoin;

    public float coinFallSpeed;
    public int invunerableDuration;
    public float umbrellaProbabilityChance;

    public GameObjs GameObjs;
}

[System.Serializable]
public struct GameObjs
{
    public List<GameObj> GameObj;
}

[System.Serializable]
public struct GameObj
{
    public string name;
    //[HideInInspector]
    public ObjectProperties objectProperties;
}

[System.Serializable]
public struct ObjectProperties
{
    public float dropSpeed;
    public float damage;
}

[System.Serializable]
public struct KinectSettings
{
    public bool isTrackingBody;
    public bool isTrackingHead;

    public float minUserDistance;
    public float maxUserDistance;
    public float maxLeftRightDistance;
}

#region INTERACTION
[System.Serializable]
public struct StartInteractionSendJsonData
{
    public string deviceID;
}

[System.Serializable]
public struct StartInteractionReceiveJsonData
{
    public requestClass request;
    public errorClass error;
    public string data;
}

[System.Serializable]
public struct InteractionDetailsSendJsonData
{
    public string interactionID;
    public string location;
    public string isShow;
    public string isInfoCollect;
    public string isFinalPage;
}

[System.Serializable]
public struct InteractionDetailsReceiveJsonData
{
    public requestClass request;
    public errorClass error;
    public bool data;
}

[System.Serializable]
public struct InteractionEndReceiveJsonData
{
    public requestClass request;
    public errorClass error;
    public bool data;
}

#endregion

#region REGISTER SEND
[System.Serializable]
public struct RegisterSendJsonData
{
    public string playID;
    public string name;
    public string contact;
}

[System.Serializable]
public struct RegisterReceiveJsonData
{
    public requestClass request;
    public errorClass error;
    public bool data;
}
#endregion

#region ADD RESULT
[System.Serializable]
public struct AddResultSendJsonData
{
    public string name;
    public string score;
    public string mobileno;
    public string email;
}

[System.Serializable]
public struct AddResultReceiveJsonData
{
    public requestClass request;
    public errorClass error;
    public string data;
}
#endregion

#region LEADERBOARD
[System.Serializable]
public struct LeaderboardReceiveJsonData
{
    public requestClass request;
    public errorClass error;
    public List<LeaderboardData> data;
}

[System.Serializable]
public struct LeaderboardData
{
    public string name;
    public string mobileno;
    public string email;
    public int score;
}
#endregion

[System.Serializable]
public struct requestClass
{
    public string api;
    public string result;
}

[System.Serializable]
public struct errorClass
{
    public string error_code;
    public string error_message;
}

/// <summary>
/// For using playerPrefs with.
/// </summary>
public interface ITrinaxPersistantVars
{
    string ip { get; set; }
    string photoPath { get; set; }
    bool useServer { get; set; }
}

