using System.IO;
using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class ExampleClass : MonoBehaviour
{

    void Update()
    {
        StartCoroutine("UploadPNG");
    }

    IEnumerator UploadPNG()
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
        Debug.Log("width " + width + " height" + height);
        // Encode texture into PNG
        byte[] bytes = tex.EncodeToPNG();
        Destroy(tex);
        yield return null;
    }
}