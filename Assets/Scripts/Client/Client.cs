
using UnityEngine;

public partial class Client
{
    public readonly TCP Tcp;

    public readonly UDP Udp;

    public Player Player;

    private const int BufferConstant = 4096;

    private static readonly int DataBufferSize = BufferConstant;

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
                    SendController.SpawnPlayer(id, client.Player);
                }
            }
        }

        foreach (Client client in Server.Clients.Values)
        {
            if (client.Player != null)
            {
                SendController.SpawnPlayer(client.id, Player);
            }
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
    }
}