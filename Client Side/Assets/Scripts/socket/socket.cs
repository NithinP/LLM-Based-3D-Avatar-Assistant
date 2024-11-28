using UnityEngine;
using WebSocketSharp;
using System;
using System.Threading;

public class WsClient : MonoBehaviour
{
    public string receivedText = "";
    public string language = "";
    private WebSocket primaryWs;
    private WebSocket secondaryWs;
    private const string PrimaryWebSocketURL = "ws://localhost:9002";
    private bool isPrimaryConnected = false;
    private bool isSecondaryConnected = false;
    private Thread primaryWsThread;
    private Thread secondaryWsThread;
    private float reconnectInterval = 5f;
    private float timeSinceLastPrimaryAttempt = 0f;
    private float timeSinceLastSecondaryAttempt = 0f;

    public delegate void MessageReceivedHandler(string message);
    public event MessageReceivedHandler OnMessageReceived;

    public delegate void ServerBusyHandler();
    public event ServerBusyHandler OnServerBusy;

    private void Start()
    {
        StartPrimaryWebSocketConnection();
    }

    private void StartPrimaryWebSocketConnection()
    {
        if (primaryWsThread != null && primaryWsThread.IsAlive)
        {
            primaryWsThread.Abort();
        }
        primaryWsThread = new Thread(ConnectPrimaryWebSocket);
        primaryWsThread.IsBackground = true;
        primaryWsThread.Start();
    }

    private void ConnectPrimaryWebSocket()
    {
        try
        {
            primaryWs = new WebSocket(PrimaryWebSocketURL);
            primaryWs.OnError += (sender, e) => Debug.LogError($"Primary WebSocket Error: {e.Message}");
            primaryWs.OnOpen += (sender, e) =>
            {
                Debug.Log("Primary connection opened");
                isPrimaryConnected = true;
            };
            primaryWs.OnMessage += (sender, e) =>
            {
                Debug.Log("Message Received from Primary: " + e.Data);
                receivedText = e.Data;
            };
            primaryWs.Connect();
        }
        catch (Exception ex)
        {
            Debug.LogError($"Primary WebSocket connection failed: {ex.Message}");
            isPrimaryConnected = false;
        }
    }

    private void Update()
    {
        if (!isPrimaryConnected || (primaryWs != null && !primaryWs.IsAlive))
        {
            isPrimaryConnected = false;
            timeSinceLastPrimaryAttempt += Time.deltaTime;
            if (timeSinceLastPrimaryAttempt >= reconnectInterval)
            {
                Debug.Log("Attempting to reconnect to primary server...");
                timeSinceLastPrimaryAttempt = 0f;
                StartPrimaryWebSocketConnection();
            }
        }
    }

    private void OnApplicationQuit()
    {
        if (primaryWs != null && primaryWs.IsAlive)
        {
            primaryWs.Close();
        }
        if (primaryWsThread != null && primaryWsThread.IsAlive)
        {
            primaryWsThread.Abort();
        }
    }
}