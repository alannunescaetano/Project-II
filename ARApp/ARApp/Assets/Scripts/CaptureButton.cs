using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;

public class CaptureButton : MonoBehaviour
{
    public Button Button;

    private TextMeshPro _textLog;
    public GameObject CaptureArea;
    public Camera camera;
    
    void Start()
    {
        Button _button = Button.GetComponent<Button>();
        _button.onClick.AddListener(onCaptureClick);
        
        var goLog = GameObject.FindGameObjectsWithTag("Log")[0];
        _textLog = goLog.GetComponent<TextMeshPro>();
        _textLog.text = "Start";
    }

    private void onCaptureClick()
    {
        StartCoroutine(takeSnapshot());
    }

    private IEnumerator takeSnapshot()
    {
        yield return new WaitForEndOfFrame();
        
        var texture = ScreenCapture.CaptureScreenshotAsTexture();

        Vector3 screenPos = CaptureArea.transform.position;
        
        int x = (int) screenPos.x - 150;
        int y = (int) screenPos.y - 100;
        int width = (int) ((RectTransform) CaptureArea.transform).rect.width;
        int height = (int) ((RectTransform) CaptureArea.transform).rect.height;
        
        Color[] c = texture.GetPixels
        (
            x, 
            y,
            width,
            height
        );
        
        Texture2D cropped = new Texture2D (width, height);
        cropped.SetPixels (c);
        cropped.Apply ();

        byte[] bytes = cropped.EncodeToPNG();
        
        string imageBase64 = Convert.ToBase64String(bytes);
        
        StartCoroutine(uploadImage(imageBase64));
    }
    
    private IEnumerator uploadImage(string image)
    {
        WWWForm form = new WWWForm();

        ImageData imageData = new ImageData { Data = image };
        string json = JsonUtility.ToJson(imageData);
        
        using (UnityWebRequest req = UnityWebRequest.Put("http://192.168.1.22:8088/", json))
        {
            req.SetRequestHeader("Content-Type", "application/json");
            req.method = "POST";
            
            yield return req.SendWebRequest();
            
            if (req.result != UnityWebRequest.Result.Success)
            {
                _textLog.text += "\n" + req.result + " --- " +  req.error;
            }
            else
            {
                _textLog.text += "\n Sucess";
            }
        }
    }

    private class ImageData
    {
        public string Data;
    }
}
