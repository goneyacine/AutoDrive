using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using System;
using System.Net;
using System.IO;
using System.Text;
using UnityEngine;
using System.Linq;
using UnityEngine.SceneManagement;

public class AIController : MonoBehaviour
{
    public int sampleRate = 15;
    public int tcpPort = 2184;
    private Socket sender;
    public Vector3 collisonBoxSize = new Vector3(1, 1, 1);
    public LayerMask collisionMask;
    private float speed;
    public float maxCollisionTime = 4;
    private float lastSpeed;
    private float acceleration;
    private Vector3 lastPosition;
    private float lastAngle;
    private float angularVelocity;
    public float score;

    private float lastSampleTime;
    public float startTime;
    private float lastTimeHit = -1;

    private Rigidbody rb;

    void Start()
    {
        startTime = Time.time;
        lastAngle = transform.eulerAngles.y;
        rb = gameObject.GetComponent<Rigidbody>();
        sender = new Socket(AddressFamily.InterNetwork,
                   SocketType.Stream, ProtocolType.Tcp);
        sender.Connect(Dns.GetHostName(), tcpPort);
    }
    void Update()
    {
        if (Time.time - lastSampleTime > 1 / (float)sampleRate)
        {
            angularVelocity = (transform.eulerAngles.y - lastAngle) * (1 / (float)sampleRate);
            lastAngle = transform.eulerAngles.y;
            speed = Mathf.Sqrt(rb.velocity.x * rb.velocity.x
                               + rb.velocity.y * rb.velocity.y + rb.velocity.z * rb.velocity.z);
            acceleration = (speed - lastSpeed) * (1 / (float)sampleRate);
            lastSpeed = speed;
            //Debug.Log("acceleration " + acceleration + " speed " + speed + " angular velocity " + angularVelocity);
            StartCoroutine(CaptureFrame((a) =>
            {
                byte[] frame = a;
                sendData(frame);
                lastSampleTime = Time.time;
            }));
            Vector3 worldCenter = transform.TransformPoint(Vector3.zero);
            Collider[] colliders = Physics.OverlapBox(worldCenter, collisonBoxSize, Quaternion.identity, collisionMask);
            if (colliders.Length > 0)
            {
                if (lastTimeHit == -1)
                    lastTimeHit = Time.time;
                else if (Time.time - lastTimeHit > maxCollisionTime)
                {
                    sender.Close();
                    string desktopPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Desktop);
                    string filePath = Path.Combine(desktopPath, "score.txt");
                    File.WriteAllText(filePath, score.ToString());
                    Scene activeScene = SceneManager.GetActiveScene();
                    SceneManager.LoadScene(activeScene.name);
                }

            }
            else
            {
                score += 1 / (float)sampleRate;
                if (lastTimeHit != -1)
                {
                    score -= (Time.time - lastTimeHit) * 5;
                    lastTimeHit = -1;
                }
            }
        }
    }
     void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.matrix = Matrix4x4.TRS(transform.TransformPoint(Vector3.zero), Quaternion.identity, collisonBoxSize * 2);
        Gizmos.DrawWireCube(Vector3.zero, Vector3.one);
    }

    IEnumerator CaptureFrame(Action<byte[]> action)
    {
        // We should only read the screen buffer after rendering is complete
        yield return new WaitForEndOfFrame();

        // Create a texture the size of the screen, RGB24 format
        int width = Screen.width;
        int height = Screen.height;
        Texture2D tex = new Texture2D(width, height, TextureFormat.RGB24, false);
        tex.ReadPixels(new Rect(0, 0, width, height), 0, 0);
        tex.Apply();
        // Create a temporary RenderTexture with the desired dimensions
        RenderTexture rt = RenderTexture.GetTemporary(32, 32, 0, RenderTextureFormat.Default, RenderTextureReadWrite.Linear);
        // Set the RenderTexture as the active RenderTexture
        RenderTexture.active = rt;

        // Blit the original texture to the RenderTexture
        Graphics.Blit(tex, rt);

        // Create a new Texture2D with the desired dimensions
        Texture2D resizedTexture = new Texture2D(32, 32, tex.format, false);
        resizedTexture.ReadPixels(new Rect(0, 0, 32, 32), 0, 0);
        resizedTexture.Apply();
        // Read screen contents into the texture
        // Encode texture into PNG
        byte[] bytes = resizedTexture.GetRawTextureData();
        Destroy(tex);
        RenderTexture.active = null;
        RenderTexture.ReleaseTemporary(rt);
        action(bytes);
        yield return bytes;
    }

    void sendData(byte[] frame)
    {
        try
        {
            try
            {
                byte[] data = new byte[3 * sizeof(float) + frame.Length];
                BitConverter.GetBytes(acceleration).CopyTo(data, 0);
                BitConverter.GetBytes(speed).CopyTo(data, sizeof(float));
                BitConverter.GetBytes(angularVelocity).CopyTo(data, 2 * sizeof(float));
                for (int i = 3 * sizeof(float); i < data.Length; i++)
                    data[i] = frame[i - 3 * sizeof(float)];
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
