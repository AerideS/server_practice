using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System;
using System.Net.Sockets;
using System.Text;
using System.Threading;

public class NetworkManager : MonoBehaviour 
{
    string server = "203.255.57.136";
    int port = 8080;
    TcpClient client;
    Thread receiveThread;
    NetworkStream stream;
    void Start()
    {
        try
        {
            client = new TcpClient(server, port);
            Debug.Log("Connected to server");

            // 서버로 데이터를 보내기 위한 네트워크 스트림 생성
            stream = client.GetStream();

            // 데이터 수신 및 송신 스레드 생성
            receiveThread = new Thread(() => ReceiveData(client, stream));
            receiveThread.IsBackground = true;  // 백그라운드 스레드로 설정
            receiveThread.Start();

            string message = "Hello client";

            byte[] data = Encoding.UTF8.GetBytes(message);
            stream.Write(data, 0, data.Length);

            SendMessage("Hello Server");
        }
        catch (Exception e)
        {
            Debug.LogException(e);
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.A)) {
            string message = "Hello client";

            byte[] data = Encoding.UTF8.GetBytes(message);
            stream.Write(data, 0, data.Length);
        }
    }

    void ReceiveData(TcpClient client, NetworkStream stream)
    {
        byte[] buffer = new byte[1024];
        int bytesRead;

        Debug.Log("Listening for data from server...");

        try
        {
            while ((bytesRead = stream.Read(buffer, 0, buffer.Length)) != 0)
            {
                string response = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                Debug.Log("Received: " + response);
            }
        }
        catch (Exception e)
        {
            Debug.Log("Exception in receive thread: " + e.Message);
        }
        finally
        {
            client.Close();
        }
    }

    void SendMessage(string message)
    {
        if (stream != null)
        {
            byte[] data = Encoding.UTF8.GetBytes("To Server");
            stream.Write(data, 0, data.Length);
            Debug.Log("Send: To Server");
        }
    }
}
