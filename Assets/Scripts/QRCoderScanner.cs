using System.Collections;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using ZXing;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Android;

public class QRCoderScanner : MonoBehaviour
{
    [SerializeField]
    private RawImage _rawImageBackground;
    [SerializeField]
    private AspectRatioFitter _aspectRatioFitter;
    [SerializeField]
    private TextMeshProUGUI _textOut;
    [SerializeField]
    private RectTransform _scanZone;
    
    private List<string> model_names;
    public GameObject placeButton;
    public GameObject canvas;

    private bool _isCamAvaible;
    private WebCamTexture _cameraTexture;
    
    void Start()
    {
        model_names = new List<string>();
        getModels();

        Application.RequestUserAuthorization(UserAuthorization.WebCam);
        SetUpCamera();

        // Can add a variable to disable the button (lower opacity) in future updates
        placeButton.SetActive(false);
    }

    private void OnGUI() {
        canvas.SetActive(true);
        _rawImageBackground.texture = _cameraTexture;
    }

    // Update is called once per frame
    void Update()
    {   
        UpdateCameraRender();
    }

    private void SetUpCamera()
    {
        WebCamDevice[] devices = WebCamTexture.devices;
        if (devices.Length == 0)
        {
            _isCamAvaible = false;
            return;
        }
        // for (int i = 0; i < devices.Length; i++)
        // {   
        //     if (devices[i].isFrontFacing == false)
        //     {
        //         _textOut.text = devices[i].name;
        //         _cameraTexture = new WebCamTexture(devices[i].name, (int)_scanZone.rect.width, (int)_scanZone.rect.height);
        //         break;
        //     }
        // }
        _textOut.text = devices[0].name;
        _cameraTexture = new WebCamTexture(devices[0].name, (int)_scanZone.rect.width, (int)_scanZone.rect.height);
        _cameraTexture.Play();
        _isCamAvaible = true;
    }

    private void UpdateCameraRender()
    {
        if (_isCamAvaible == false)
        {
            return;
        }
        float ratio = (float)_cameraTexture.width / (float)_cameraTexture.height;
        _aspectRatioFitter.aspectRatio = ratio;

        int orientation = _cameraTexture.videoRotationAngle;
        orientation = orientation * 3;
        _rawImageBackground.rectTransform.localEulerAngles = new Vector3(0, 0, orientation);

    }
    public void OnClickScan()
    {
        Scan();
    }
    private void Scan()
    {
        try
        {
            IBarcodeReader barcodeReader = new BarcodeReader();
            Result result = barcodeReader.Decode(_cameraTexture.GetPixels32(), _cameraTexture.width, _cameraTexture.height);
            if (result != null)
            {
                _textOut.text = result.Text;

            }
            else {
                _textOut.text = "Failed to Read QR CODE";
            }
            checkModel(_textOut.text); // Enables the Placement button if prefab avalable in the directory
        }
        catch
        {
            _textOut.text = "FAILED IN TRY";
        }
    }
    private void getModels(){
        string[] filepaths = Directory.GetFiles(@"Assets/Prefab");
        foreach (string path in filepaths)
        {
            model_names.Add(Path.GetFileNameWithoutExtension(path));
        }
    }
    private bool checkModel(string model_name){
        return model_names.Contains(model_name);
    }

}
