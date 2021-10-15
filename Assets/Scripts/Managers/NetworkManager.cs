using System;
using UnityEngine;

public class NetworkManager : MonoBehaviour
{
    public static NetworkManager Instance;

    public GameObject PlayerPrefab;

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

        Server.Start(50, 26950);
    }

    private void OnApplicationQuit()
    {
        ServerController.MessageServer(Constants.ServerId, "<color=#FF0041>Server connection has now closed.</color>", DateTime.Now.ToString());
        Server.Stop();
    }
}
