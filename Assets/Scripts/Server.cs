using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using UnityEngine;

public class Server
{
    public static readonly Dictionary<int, Client> Clients = new Dictionary<int, Client>();
    
    public static Dictionary<int, PacketHandler> PacketHandlers;

    private static TcpListener tcpListener;

    private static UdpClient udpListener;

    public delegate void PacketHandler(int fromClient, Packet packet);

    public static int MaxPlayers { get; private set; }

    public static int Port { get; private set; }

    public static void Start(int maxPlayers, int port)
    {
        MaxPlayers = maxPlayers;
        Port = port;

        Debug.Log("Server starting ...");

        InitializeServerData();

        tcpListener = new TcpListener(IPAddress.Any, Port);
        tcpListener.Start();

        tcpListener.BeginAcceptTcpClient(new AsyncCallback(TCPConnectCallback), null);

        udpListener = new UdpClient(Port);
        udpListener.BeginReceive(UdpReceiveCallback, null);

        Debug.Log($"Server started on {Port}");
    }

    public static void Stop()
    {
        try
        {
            tcpListener.Stop();
            udpListener.Close();
        }
        catch (Exception ex)
        {
            Debug.Log("INFO: TCP/UDP Listeners failed to close successfully.");
        }
    }

    public static void SendUDPData(IPEndPoint clientEndPoint, Packet packet)
    {
        try
        {
            if (clientEndPoint != null)
            {
                udpListener.BeginSend(packet.ToArray(), packet.Length(), clientEndPoint, null, null);
            }
        }
        catch (Exception ex)
        {
            Debug.Log($"ERROR: Sending data to {clientEndPoint} via UDP: {ex}");
        }
    }

    /// <summary>
    /// Listen for Clients.
    /// </summary>
    /// <param name="result">Callback result.</param>
    private static void TCPConnectCallback(IAsyncResult result)
    {
        TcpClient client = tcpListener.EndAcceptTcpClient(result);

        tcpListener.BeginAcceptTcpClient(new AsyncCallback(TCPConnectCallback), null);

        Debug.Log($"Incoming connection from {client.Client.RemoteEndPoint} ...");

        for (int i = 1; i <= MaxPlayers; i++)
        {
            if (Clients[i].Tcp.Socket == null)
            {
                Clients[i].Tcp.Connect(client);
                return;
            }
        }
    }

    private static void UdpReceiveCallback(IAsyncResult result)
    {
        try
        {
            IPEndPoint clientEndPoint = new IPEndPoint(IPAddress.Any, 0);

            byte[] data = udpListener.EndReceive(result, ref clientEndPoint);

            udpListener.BeginReceive(UdpReceiveCallback, null);

            if (data.Length < 4)
            {
                return;
            }

            using (Packet packet = new Packet(data))
            {
                int clientId = packet.ReadInt();

                if (clientId == 0)
                {
                    return;
                }

                if (Clients[clientId].Udp.EndPoint == null)
                {
                    Clients[clientId].Udp.Connect(clientEndPoint);
                    return;
                }

                if (Clients[clientId].Udp.EndPoint.ToString() == clientEndPoint.ToString())
                {
                    Clients[clientId].Udp.HandleData(packet);
                }
            }
        }
        catch (ObjectDisposedException ex)
        {
            Debug.Log("Tried to access a closed UDP stream.");
        }
        catch (Exception ex)
        {
            Debug.Log($"ERROR: Receiving UDP data: {ex}");
        }
    }

    private static void InitializeServerData()
    {
        for (int i = 1; i <= MaxPlayers; i++)
        {
            Clients.Add(i, new Client(i));
        }

        PacketHandlers = new Dictionary<int, PacketHandler>()
            {
                { (int)ClientPackets.WelcomeReceived, ServerHandler.WelcomeReceived },
                { (int)ClientPackets.PlayerMovement, ServerHandler.PlayerMovement },
            };

        Debug.Log("Initialized packets.");
    }
}