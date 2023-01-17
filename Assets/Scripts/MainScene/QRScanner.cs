using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using ZXing;

public class QRScanner : MonoBehaviour
{
    // Declaring required variables
    [SerializeField] private GameObject _placeBtn;
    [SerializeField] private TextMeshProUGUI textField;
    [SerializeField] private RectTransform _scanZone;
    [SerializeField] private Canvas canvas;
    private IBarcodeReader reader;
    private ARCameraManager _arCamera; // Reference to our camera
    private Texture2D _arCameraTexture; // Camera texture object
    private bool onlyOnce = false; // Used to check if the barcode is checking the image
    public GetModel getModelScript;
    public GameObject Scanned_QR_Value_Model = null;

    // Start is called before the first frame update
    void Start()
    {
        _placeBtn.SetActive(false);
        _arCamera = FindObjectOfType<ARCameraManager>();
        reader = new BarcodeReader();
        reader.Options.TryHarder = true;
        _arCamera.frameReceived += OnCameraFrameRecieved;
    }

    void OnCameraFrameRecieved(ARCameraFrameEventArgs eventArgs)
    {
        var maxConfig = getMaxConifg();
        var currentConfig = _arCamera.currentConfiguration.Value;

        if (currentConfig.width != maxConfig.width)
        {
            _arCamera.currentConfiguration = maxConfig;
        }
    }

    public void ScanQR(){
        XRCpuImage image;
        if(_arCamera.TryAcquireLatestCpuImage(out image)){
            textField.text = "Scanning...";
            StartCoroutine(ProcessQRCode(image));
            image.Dispose();
        }
    }

    IEnumerator ProcessQRCode(XRCpuImage image){
        var request = image.ConvertAsync(new XRCpuImage.ConversionParams{
            inputRect = new RectInt(0, 0, image.width, image.height),
            outputDimensions = new Vector2Int(image.width, image.height),
            outputFormat = TextureFormat.RGB24,
            transformation = XRCpuImage.Transformation.MirrorY
        });

        while(!request.status.IsDone())
        yield return null;

        if (request.status != XRCpuImage.AsyncConversionStatus.Ready)
        {
            Debug.LogErrorFormat("Request failed with status {0}", request.status);
            textField.text = "Failed Conversion";

            request.Dispose();
            yield break;            
        }

        var rawData = request.GetData<byte>();

        if (_arCameraTexture == null) {
            _arCameraTexture = new Texture2D(
                request.conversionParams.outputDimensions.x,
                request.conversionParams.outputDimensions.y,
                request.conversionParams.outputFormat,
                false
            );
        }

        _arCameraTexture.LoadRawTextureData(rawData);
        _arCameraTexture.Apply();

        Texture2D croppedTexture = cropTexture(_arCameraTexture);

        byte[] barcodeBitmap = croppedTexture.GetRawTextureData();

        LuminanceSource source = new RGBLuminanceSource(barcodeBitmap, croppedTexture.width, croppedTexture.height);

        if(!onlyOnce){
            onlyOnce = true;
            
            Result result = reader.Decode(source);

            textField.text = "Decoding...";

            if (result != null && result.Text != ""){
                Object[] models = getModelScript.models;
                string[] modelNames = getModelScript.modelNames;
                // Printing all model names.
                 for (int i = 0; i < modelNames.Length; i++) {
                    if (modelNames[i] == result.Text){
                        textField.text = modelNames[i];
                        _placeBtn.SetActive(true);
                        Scanned_QR_Value_Model = models[i] as GameObject;
                    }
                }
            }
            else 
            {
                textField.text = "Model Not Found";
            }

            onlyOnce = false;
        }
    }

    private Texture2D cropTexture(Texture2D originalTexture)
    {   
        Vector2 centerPos = new Vector2(originalTexture.width / 2, originalTexture.height / 2);
        var scanPos = RectTransformUtility.PixelAdjustRect(_scanZone, canvas);

        Texture2D croppedTexture = new Texture2D((int) scanPos.width / 2, (int) scanPos.height / 2);
        
        croppedTexture.SetPixels(originalTexture.GetPixels(
            (int) (scanPos.x / 2 + centerPos.x + 280),
            (int) (scanPos.y / 2 + centerPos.y + 20),
            (int) scanPos.width / 2,
            (int) scanPos.height / 2
        ));

        croppedTexture.Apply();

        return croppedTexture;
    }

    private void LogConfig(){
        using (var configurations = _arCamera.GetConfigurations(Unity.Collections.Allocator.Temp))
        {
            var current = _arCamera.currentConfiguration;

            foreach (var config in configurations)
            {
                if (current == config) Debug.Log("Current: ");
                Debug.Log($"{config.width}x{config.height}{(config.framerate.HasValue ? $" at {config.framerate.Value} Hz" : "")}{(config.depthSensorSupported == Supported.Supported ? " depth sensor" : "")}");
            }
        }
    }

    private XRCameraConfiguration getMaxConifg()
    {
        using (var configurations = _arCamera.GetConfigurations(Unity.Collections.Allocator.Temp))
        {
            if (configurations.Length <= 0 || !configurations.IsCreated) return new XRCameraConfiguration();

            int maxWidth = 0, maxWidthIndex = 0;
            for (int i = 0; i < configurations.Length; i++)
            {
                if (maxWidth < configurations[i].width)
                {
                    maxWidth = configurations[i].width;
                    maxWidthIndex = i;
                }
            }
            var config = configurations[maxWidthIndex];
            
            return config;
        }
    }
}
