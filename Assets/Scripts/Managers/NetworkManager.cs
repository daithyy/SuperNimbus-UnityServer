using System;
using System.Collections;
using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

public class NetworkManager : MonoBehaviour
{
    public static NetworkManager Instance;

    public int Port = Constants.DefaultPort;

    public string NakamaIpAddress = Constants.DefaultIp;

    public int NakamaPort = 7350;

    public int MaxPlayers = 8;

    public GameObject PlayerPrefab;

    private string userId;

    private string matchId;

    public Player InstantiatePlayer() =>
        Instantiate(PlayerPrefab, Constants.SpawnPosition, Quaternion.identity).GetComponent<Player>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Debug.Log("Instance already exists, destroying object!");
            Destroy(this);
        }
    }

    private void Start()
    {
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = Constants.TicksPerSecond;

        Server.Start(MaxPlayers, Port);
    }

    private void OnApplicationQuit()
    {
        ServerController.MessageServer(Constants.ServerId, "<color=#FF0041>Server connection has now closed.</color>");
        Server.Stop();
    }

    public void ValidateToken(int clientId, string userId, string matchId, string token)
    {
        this.userId = userId;
        this.matchId = matchId;

        StartCoroutine(RetrieveToken((tokenData) =>
        {
            if (!string.IsNullOrEmpty(tokenData))
            {
                NakamaResult data = JsonUtility.FromJson<NakamaResult>(tokenData);

                if (IsTokenValid(token, data.result.value.token))
                {
                    ServerController.ServerValidate(clientId, $"<color=#00FF00>Your match token has been validated successfully through Nakama!</color>");
                }
                else
                {
                    ServerController.ServerValidate(clientId, $"<color=#FF0041>Your match token is not valid within the Nakama storage database.</color>");
                }
            }
        }));
    }

    private IEnumerator RetrieveToken(Action<string> callback)
    {
        ValidationRequest request = new ValidationRequest { userId = userId, matchId = matchId };

        string url = $"http://{NakamaIpAddress}:{NakamaPort}/v2/rpc/servervalidate?http_key=defaulthttpkey&unwrap";

        string data = JsonUtility.ToJson(request);

        byte[] bodyRaw = Encoding.UTF8.GetBytes(data);

        UnityWebRequest www = UnityWebRequest.Post(url, "POST");

        www.uploadHandler = new UploadHandlerRaw(bodyRaw);
        www.downloadHandler = new DownloadHandlerBuffer();
        www.SetRequestHeader("Content-Type", "application/json");

        yield return www.SendWebRequest();

        callback(www.downloadHandler.text);
    }

    private bool IsTokenValid(string userToken, string nakamaToken)
    {
        return (userToken.Equals(nakamaToken));
    }
}
