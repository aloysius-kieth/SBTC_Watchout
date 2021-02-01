using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.IO;

public enum COIN_PATTERN
{
    COIN_PATTERN_FILE_ONE,
    COIN_PATTERN_FILE_TWO,
    TOTAL,
};

public class CoinSpawner : MonoBehaviour
{
    //public List<TextAsset> textFiles = new List<TextAsset>();
    public List<string[]> fileData = new List<string[]>();
    public List<GameObject> coins = new List<GameObject>();

    public COIN_PATTERN coinPattern;

    public void RemoveCoinsFromList(GameObject obj)
    {
        coins.Remove(obj);
    }
    public void ClearCoinList()
    {
        coins.Clear();
    }

    string[] sData;
    float tileSize;
    float mapOffsetY = 100f; // hardcoded number of lines in file

    Vector3 start;
    const string PREFAB_TO_SPAWN = "Coin";
    public GameObject prefab;

    void Start()
    {
        Setup();
        LoadAllFiles();
    }

    void Setup()
    {
        tileSize = prefab.GetComponent<SpriteRenderer>().bounds.size.x;
        //Debug.Log(tileSize);
    }

    void Update()
    {
        //if (Input.GetKeyDown(KeyCode.F1))
        //{
        //    Spawn(fileData[(int)COIN_PATTERN.COIN_PATTERN_FILE_TWO]);
        //}
    }

    public void SetPattern(COIN_PATTERN pattern)
    {
        PickPattern();
        switch (pattern)
        {
            case COIN_PATTERN.COIN_PATTERN_FILE_ONE:
                Spawn(fileData[(int)COIN_PATTERN.COIN_PATTERN_FILE_ONE]);
                break;
            case COIN_PATTERN.COIN_PATTERN_FILE_TWO:
                Spawn(fileData[(int)COIN_PATTERN.COIN_PATTERN_FILE_TWO]);
                break;
            default:
                Debug.LogWarning("No such coin pattern file exists!");
                break;
        }
    }

    void PickPattern()
    {
        int index = Random.Range((int)COIN_PATTERN.COIN_PATTERN_FILE_ONE, (int)COIN_PATTERN.TOTAL);
        coinPattern = (COIN_PATTERN)index;
        //Debug.Log(coinPattern.ToString());
    }

    void Spawn(string[] data)
    {
        bool isOddLine = true;
        string[] mapData = data;
        int mapX = mapData[0].ToCharArray().Length;

        if (mapX % 2 == 0) isOddLine = false;
        else isOddLine = true;

        int mapY = mapData.Length;
        if (isOddLine)
        {
            start.x = -Mathf.CeilToInt((float)mapX / 2);
        }
        else
        {
            start.x = -Mathf.CeilToInt((float)mapX / 2) * tileSize + (tileSize / 2);
        }

        start.y = AppManager.gameManager.cameraProperties.cameraScreenToWorldPointPosition.y + mapOffsetY * tileSize;

        for (int y = 0; y < mapY; y++)
        {
            char[] newTiles = mapData[y].ToCharArray();

            for (int x = 0; x < mapX; x++)
            {
                SpawnCoin(newTiles[x].ToString(), x, y, new Vector3(start.x, start.y));
            }
        }
    }

    void SpawnCoin(string tileType, int x, int y, Vector3 start)
    {
        //Debug.Log(tileType);
        if (string.IsNullOrEmpty(tileType) || string.IsNullOrWhiteSpace(tileType)) return;
        int tileIndex = int.Parse(tileType);
        if (tileIndex == 1)
        {
            GameObject newTile = ObjectPooler.Instance.GetPooledObject(PREFAB_TO_SPAWN);
            if (newTile != null)
            {
                //tileSize = newTile.GetComponent<SpriteRenderer>().sprite.bounds.size.x;
                Coin c = newTile.GetComponent<Coin>();
                newTile.transform.position = new Vector3(start.x + (tileSize * x), start.y - (tileSize * y));
                newTile.SetActive(true);
                c.shadow.SetShadowOnGround();

                coins.Add(newTile);
            }
            else Debug.LogWarning(newTile.name + " does not exist!");
        }
    }

    void LoadAllFiles()
    {
        //Object[] t = Resources.LoadAll("CoinPatterns", typeof(TextAsset));

        var info = new DirectoryInfo(Application.persistentDataPath + "/CoinPatterns");
        //Debug.Log(Application.persistentDataPath + "/CoinPatterns");

        FileInfo[] fileInfos = info.GetFiles("*.txt");
        foreach (FileInfo file in fileInfos)
        {
            Debug.Log(file.FullName);
            if (File.ReadAllLines(file.FullName).Length != 100)
            {
                Debug.Log("Pattern not 100 lines long!");
                return;
            }
            string content = File.ReadAllText(file.FullName);
            content = content.Replace(System.Environment.NewLine, string.Empty);
            sData = content.Split('-');
            fileData.Add(sData);
        }

        //for (int i = 0; i < fileData.Count; i++)
        //{
        //    for (int j = 0; j < fileData[i].Length; j++)
        //    {
        //        Debug.Log(fileData[i][j]);
        //    }
        //}

        //for (int j = 0; j < textFiles.Count; j++)
        //{
        //    string s = textFiles[j].text.Replace(System.Environment.NewLine, string.Empty);
        //    //Debug.Log(s);
        //    sData = s.Split('-');
        //    fileData.Add(sData);
        //}
    }
}