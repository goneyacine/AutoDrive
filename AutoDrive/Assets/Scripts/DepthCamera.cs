using UnityEngine;

public class DepthCamera : MonoBehaviour
{
    public Camera depthCamera;
    public Shader depthShader;
    public RenderTexture depthTexture;

    void Start()
    {
        // Ensure the depth camera is set up
        if (depthCamera == null)
        {
            depthCamera = GetComponent<Camera>();
        }

        // Create a RenderTexture to hold the depth information
        depthTexture = new RenderTexture(Screen.width, Screen.height, 24, RenderTextureFormat.Depth);
        depthCamera.targetTexture = depthTexture;
        depthCamera.SetReplacementShader(depthShader, "RenderType");

        // Optionally, disable the depth camera rendering to avoid affecting performance
        depthCamera.enabled = false;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            CaptureDepth();
        }
    }

    void CaptureDepth()
    {
        depthCamera.Render();
        SaveDepthTexture();
    }

    void SaveDepthTexture()
    {
        // Create a new Texture2D to hold the depth data
        Texture2D depthImage = new Texture2D(depthTexture.width, depthTexture.height, TextureFormat.RGB24, false);

        // Read the RenderTexture data into the Texture2D
        RenderTexture.active = depthTexture;
        depthImage.ReadPixels(new Rect(0, 0, depthTexture.width, depthTexture.height), 0, 0);
        depthImage.Apply();
        RenderTexture.active = null;

        // Optionally, save the depth image to a file (e.g., PNG)
        byte[] bytes = depthImage.EncodeToPNG();
        System.IO.File.WriteAllBytes(Application.dataPath + "/DepthImage.png", bytes);

        // Clean up
        Destroy(depthImage);
    }
}
