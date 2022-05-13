using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;

public class CaptureButton : MonoBehaviour
{
    public Button Button;

    private TextMeshPro _textLog;
    
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

        byte[] image = texture.EncodeToPNG();
        
        string imageBase64 = Convert.ToBase64String(image);
        
        StartCoroutine(uploadImage(imageBase64));
    }
    
    private IEnumerator uploadImage(string image)
    {
        WWWForm form = new WWWForm();
        
        _textLog.text = "antes do using";
        
        var jsonString = string.Format("{ \"name\":\"{0}\" }", image);

        using (UnityWebRequest www = UnityWebRequest.Post("http://192.168.1.22/", form))
        {
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                _textLog.text += "\n" + www.result + " --- " +  www.error;
            }
            else
            {
                _textLog.text = "\n Sucess";
            }
        }
    }
}
