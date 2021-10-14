
using UnityEngine;

public partial class Client
{
    public readonly TCP Tcp;

    public readonly UDP Udp;

    public Player Player;

    private static readonly int DataBufferSize = Constants.BufferConstant;

    private readonly int id;

    public Client(int clientId)
    {
        id = clientId;

        Tcp = new TCP(id);
        Udp = new UDP(id);
    }

    public void CreatePlayer(string playerName)
    {
        Player = NetworkManager.Instance.InstantiatePlayer();
        Player.Initialize(id, playerName);

        foreach (Client client in Server.Clients.Values)
        {
            if (client.Player != null)
            {
                if (client.id != id)
                {
                    ServerController.SpawnPlayer(id, client.Player);
                }
            }
        }

        foreach (Client client in Server.Clients.Values)
        {
            if (client.Player != null)
            {
                ServerController.SpawnPlayer(client.id, Player);
            }
        }

        foreach (Spawner item in Spawner.Spawners.Values)
        {
            ServerController.CreateSpawner(id, item.Id, item.transform.position, item.HasItem);
        }
    }

    private void Disconnect()
    {
        Debug.Log($"{Tcp.Socket.Client.RemoteEndPoint} has disconnected.");

        ThreadManager.ExecuteOnMainThread(() =>
        {
            Object.Destroy(Player.gameObject);
            Player = null;
        });

        Tcp.Disconnect();
        Udp.Disconnect();

        ServerController.PlayerDisconnected(id);
    }
}