using System;
using UnityEngine;

public class ServerHandler
{
    public static void WelcomeReceived(int fromClient, Packet packet)
    {
        int clientIdCheck = packet.ReadInt();
        string username = packet.ReadString();
        string matchId = packet.ReadString();
        string userId = packet.ReadString();
        string token = packet.ReadString();

        Debug.Log($"{Server.Clients[fromClient].Tcp.Socket.Client.RemoteEndPoint} connected successfully and is now Player: {fromClient}");

        if (fromClient != clientIdCheck)
        {
            Debug.Log($"Player \"{username}\" (ID: {fromClient}) has been assigned to the incorrect Client ID {clientIdCheck}");
        }

        Server.Clients[fromClient].CreatePlayer(username);

        NetworkManager.Instance.ValidateToken(fromClient, userId, matchId, token);
    }

    public static void PlayerMovement(int fromClient, Packet packet)
    {
        Vector3 inputDirection = packet.ReadVector3();

        Quaternion rotation = packet.ReadQuaternion();

        Vector3 eulerAngles = packet.ReadVector3();

        bool[] actions = new bool[packet.ReadInt()];

        for (int i = 0; i < actions.Length; i++)
        {
            actions[i] = packet.ReadBool();
        }

        Server.Clients[fromClient].Player.ReadInput(inputDirection, rotation, eulerAngles, actions);
    }

    public static void MessageClient(int fromClient, Packet packet)
    {
        string message = packet.ReadString();

        ServerController.MessageServer(fromClient, message);
    }
}