using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using TMPro;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using ZXing;

public class QRScanner : MonoBehaviour
{
    // Declaring required variables
    [SerializeField]
    private GameObject _placeBtn;
    [SerializeField]
    private TextMeshProUGUI textField;
    private IBarcodeReader reader;
    private ARCameraManager _arCamera; // Reference to our camera
    private Texture2D _arCameraTexture; // Camera texture object
    private bool onlyOnce; // Used to check if the barcode is checking the image
    private List<string> model_names = new List<string>();

    string[] models = {"Old building",};

    // Start is called before the first frame update
    void Start()
    {
        _placeBtn.SetActive(false);
        foreach (string name in models)
        {
            model_names.Add(name);
        }
        _arCamera = FindObjectOfType<ARCameraManager>();
        reader = new BarcodeReader();
        reader.Options.TryHarder = true;
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
            outputDimensions = new Vector2Int(image.width / 2, image.height / 2),
            outputFormat = TextureFormat.RGB24,
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

        byte[] barcodeBitmap = _arCameraTexture.GetRawTextureData();

        LuminanceSource source = new RGBLuminanceSource(barcodeBitmap, _arCameraTexture.width, _arCameraTexture.height);

        if(!onlyOnce){
            onlyOnce = true;
            
            Result result = reader.Decode(source);

            textField.text = "Decoding";

            if (result != null && result.Text != ""){
                if (checkModel(result.Text))
                {
                    textField.text = result.Text;
                    _placeBtn.SetActive(true);
                }
                else
                {
                    textField.text = "Model Not Found";
                }
            }
            else 
            {
                textField.text = "Failed";
            }

            onlyOnce = false;
        }
    }

    private void getModels(){
        string[] filepaths = Directory.GetFiles(@"Assets/Resources");
        foreach (string path in filepaths)
        {
            model_names.Add(Path.GetFileNameWithoutExtension(path));
        }
    }
    private bool checkModel(string model_name){
        return model_names.Contains(model_name);
    }

}
