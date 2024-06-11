using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using System;
using System.Net;
using System.Text;
using UnityEngine;
using System.Linq;

public class DataCapture : MonoBehaviour
{
    public int sampleRate = 15;
    public int tcpPort = 4321;
    private Socket sender;

    private float lastSampleTime;
    void Start()
    {
        sender = new Socket(AddressFamily.InterNetwork,
                   SocketType.Stream, ProtocolType.Tcp);
        sender.Connect(Dns.GetHostName(), tcpPort);
    }
    void Update()
    {
        if (Time.time - lastSampleTime > 1 / sampleRate)
        {
            StartCoroutine(CaptureFrame((a) =>
            {
                byte[] frame = a;
                Debug.Log(frame.Length);
                int[] playerInputs = {Input.GetKey(KeyCode.W) ? 1 : 0,
             Input.GetKey(KeyCode.S) ? 1 : 0,Input.GetKey(KeyCode.A) ? 1 : 0,Input.GetKey(KeyCode.D) ? 1 : 0};
                sendData(frame, playerInputs);
                lastSampleTime = Time.time;
            }));
        }
    }


    IEnumerator CaptureFrame(Action<byte[]> action)
    {
        // We should only read the screen buffer after rendering is complete
        yield return new WaitForEndOfFrame();

        // Create a texture the size of the screen, RGB24 format
        int width = Screen.width;
        int height = Screen.height;
        Texture2D tex = new Texture2D(width, height, TextureFormat.RGB24, false);

        // Read screen contents into the texture
        tex.ReadPixels(new Rect(0, 0, width, height), 0, 0);
        tex.Apply();
        // Encode texture into PNG
        byte[] bytes = tex.GetRawTextureData();
        Destroy(tex);
        action(bytes);
        yield return bytes;
    }
    void sendData(byte[] frame, int[] playerInputs)
    {
        try
        {
            try
            {

                //first 4 element of the data array contains player inputs & the rest contains frame data
                byte[] data = new byte[playerInputs.Length * sizeof(int) + frame.Length];
                for (int i = 0; i < playerInputs.Length; i++)
                    BitConverter.GetBytes(playerInputs[i]).CopyTo(data, i * sizeof(int));
                for (int i = sizeof(int) * playerInputs.Length; i < data.Length; i++)
                    data[i] = frame[i - sizeof(int) * playerInputs.Length];
                sender.Send(data);
            }

            // Manage of Socket's Exceptions
            catch (ArgumentNullException ane)
            {
                Debug.LogError(ane.ToString());
            }
            catch (SocketException se)
            {
                Debug.LogError(se.ToString());

            }
            catch (Exception e)
            {
                Debug.LogError(e.ToString());

            }
        }

        catch (Exception e)
        {

            Debug.LogError(e.ToString());
        }
    }
}
