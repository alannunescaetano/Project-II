using System;
using System.Collections;
using System.Threading.Tasks;
using Model;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CaptureButton : MonoBehaviour
{
    public Button Button;

    public GameObject CaptureArea;
    public Camera Camera;
    public GameObject Label;
    private TextMeshProUGUI _labelText;

    public static Word IdentifiedWord;

    private const string SERVER = "http://192.168.1.18:8088/";
    
    void Start()
    {
        DontDestroyOnLoad(this);
        
        IdentifiedWord = null;
        
        Button _button = Button.GetComponent<Button>();
        _button.onClick.AddListener(onCaptureClick);

        _labelText = Label.GetComponent<TextMeshProUGUI>();

        TTSManager.BootUpTTS();
    }

    private void onCaptureClick()
    {
        CaptureArea.GetComponent<CanvasRenderer>().SetAlpha(0);
        StartCoroutine(takeSnapshot());
    }

    private IEnumerator takeSnapshot()
    {
        _labelText.text = "Processing...";
        yield return new WaitForEndOfFrame();
        
        var texture = ScreenCapture.CaptureScreenshotAsTexture();

        Vector3 screenPos = CaptureArea.transform.position;
        
        int width = (int) ((RectTransform) CaptureArea.transform).rect.width;
        int height = (int) ((RectTransform) CaptureArea.transform).rect.height;
        int x = (int) screenPos.x - (width/2);
        int y = (int) screenPos.y - (height/2);
        
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

        using (UnityWebRequest req = UnityWebRequest.Put(SERVER, json))
        {
            req.SetRequestHeader("Content-Type", "application/json");
            req.method = "POST";
            req.downloadHandler = new DownloadHandlerBuffer();
            
            yield return req.SendWebRequest();
            
            if (req.result == UnityWebRequest.Result.Success)
            {
                Debug.Log("- "+req.downloadHandler.text);
                Word word = JsonUtility.FromJson<Word>(req.downloadHandler.text);

                Debug.Log(word == null);
                Debug.Log(word.Syllables == null);
                Debug.Log(word.Syllables.Count);

                if (word == null || word.Syllables == null || word.Syllables.Count == 0)
                {
                    _labelText.text = "Word not identified. \nTry again:";
                }
                else
                {
                    IdentifiedWord = word;
                    SceneManager.LoadScene(sceneName:"SyllablesScene");
                }
            }
            else
            {
                _labelText.text = "Error";
            }
        }
        
        CaptureArea.GetComponent<CanvasRenderer>().SetAlpha(1);
    }

    private class ImageData
    {
        public string Data;
    }
}
