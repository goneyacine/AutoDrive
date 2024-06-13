using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using System;
using System.Net;
using System.Text;
using UnityEngine;
using System.Linq;

public class AIController : MonoBehaviour
{
        public int sampleRate = 15;
    public int tcpPort = 2184;
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
                sendData(frame);
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
    void sendData(byte[] frame)
    {
        try
        {
            try
            {
                sender.Send(frame);
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
