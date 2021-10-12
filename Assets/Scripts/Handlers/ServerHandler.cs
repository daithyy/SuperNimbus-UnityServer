using System;
using UnityEngine;

public class ServerHandler
{
    public static void WelcomeReceived(int fromClient, Packet packet)
    {
        int clientIdCheck = packet.ReadInt();
        string username = packet.ReadString();

        Debug.Log($"{Server.Clients[fromClient].Tcp.Socket.Client.RemoteEndPoint} connected successfully and is now Player: {fromClient}");

        if (fromClient != clientIdCheck)
        {
            Debug.Log($"Player \"{username}\" (ID: {fromClient}) has been assigned to the incorrect Client ID {clientIdCheck}");
        }

        Server.Clients[fromClient].CreatePlayer(username);
    }

    public static void PlayerMovement(int fromClient, Packet packet)
    {
        Vector3 inputDirection = packet.ReadVector3();

        Quaternion rotation = packet.ReadQuaternion();

        Vector3 eulerAngles = packet.ReadVector3();

        Server.Clients[fromClient].Player.SetInput(inputDirection, rotation, eulerAngles, new bool[0]);
    }
}