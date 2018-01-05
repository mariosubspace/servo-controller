using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System;

public class ServoClient : MonoBehaviour
{
    int currentAngle = 100;
    bool waitingForCommandToFinish = false;

	public byte[] ipAddress;

    Socket socket = null;

    void Start()
    {
        Debug.Log("ServoClient:: Constructing socket.");
        ConstructSocket();
    }

    void ConstructSocket()
    {
        try
        {
            int conPort = 80;
			IPAddress hostAddress = new IPAddress(ipAddress);
            IPEndPoint hostEndPoint = new IPEndPoint(hostAddress, conPort);

            socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            socket.Connect(hostEndPoint);
        }
        catch (Exception e)
        {
            Debug.Log("Error in ServoClient:: " + e.ToString());
        }
    }

    void OnDisable()
    {
        socket.Close();
    }

    void SendInfo(string msg)
    {
        //Debug.LogFormat("ServoClient:: Attempting to send message {0}", msg);

        // abort if already waiting for a command.
        if (waitingForCommandToFinish)
        {
            Debug.Log("ServoClient:: Waiting for another command to finish.");
            return;
        }
        waitingForCommandToFinish = true;

        if (socket == null)
        {
            Debug.Log("ServoClient:: Socket doesn't exist, constructing socket.");
            ConstructSocket();
        }

        if (socket == null)
        {
            Debug.Log("ServoClient:: Could not constuct socket.");
        }

        SocketAsyncEventArgs e = new SocketAsyncEventArgs();
        byte[] msgBytes = Encoding.ASCII.GetBytes(msg);
        e.SetBuffer(msgBytes, 0, msgBytes.Length);
        e.Completed += (sender, f) => { waitingForCommandToFinish = false; };
        socket.SendAsync(e);
    }
       
    public void SetAngle(int angle)
    {
        SendInfo(angle.ToString());
    }

    public void TurnRight()
    {
        SetAngle(++currentAngle);
    }

    public void TurnLeft()
    {
        SetAngle(--currentAngle);
    }
}
