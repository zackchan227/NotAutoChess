using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;
using System;

public class SceneLoader : MonoBehaviour
{
    public static SceneLoader Instance;
    [SerializeField] private AssetReference _scene;
    [SerializeField] private List<AssetReference> _references = new List<AssetReference>();
    [SerializeField] private Slider downloadProgress;
    [SerializeField] private TMPro.TMP_Text tmpProgress;
    [SerializeField] private TMPro.TMP_Text tmpTotalDownloaded;
    [SerializeField] private GameObject goBlur;
    [SerializeField] private Button _btTapToStart;
    private AsyncOperationHandle<SceneInstance> handle, downloadScene;
    private AsyncOperationHandle download;
    public GameObject _camera;
    //long DownloadSize = 0;

    void Awake()
    {
        if(!Instance) Instance = this;
        DontDestroyOnLoad(this);
        downloadProgress.minValue = 0;
        downloadProgress.maxValue = 100;
        _btTapToStart.onClick.RemoveAllListeners();
        _btTapToStart.onClick.AddListener(OnClickStart);
    }

    // Start is called before the first frame update
    void Start()
    {
        download = Addressables.DownloadDependenciesAsync(_scene);
        download.Completed += DownloadComplete;
        StartCoroutine(DownloadScene());
    }

    // void Update()
    // {
    //     if(_btTapToStart.gameObject.activeSelf) 
    //     {
    //         _startBanner.SetActive((Time.unscaledTime % 0.1f < .2));
    //     }
    // }

    IEnumerator DownloadScene()
    {
        
        Debug.Log("Starting download asset");

        while(!download.IsDone)
        {
            var status = download.GetDownloadStatus();
            float progress = status.Percent;
            double downloadedMB = Math.Round((float)status.DownloadedBytes/1024/1024, 2);
            double totalMB = Math.Round((float)status.TotalBytes/1024/1024,2);
            downloadProgress.value = (int)(progress * 100);
            tmpProgress.text = downloadProgress.value.ToString() + "%";
            tmpTotalDownloaded.text = downloadedMB + "MB/" + totalMB + "MB";
            if(totalMB == 0) 
            {
                goBlur.SetActive(false);
                //tmpTotalDownloaded.gameObject.SetActive(false);
            }
            yield return null;
        }

        Debug.Log("Download Complete");
        downloadProgress.value = 100;
        tmpProgress.text = downloadProgress.value.ToString() + "%";

        
    }

    private void SceneDownloadComplete(AsyncOperationHandle<SceneInstance> _handle)
    {
        //_camera.SetActive(false);
        //SceneManager.UnloadSceneAsync(0);
    }

    private void DownloadComplete(AsyncOperationHandle _handle)
    {
        if(_handle.Status == AsyncOperationStatus.Succeeded)
        {
            //Debug.Log(_handle.Result.ToString());
            StartCoroutine(WaitSeconds(2f,_handle));
        }
    }

    IEnumerator WaitSeconds(float seconds, AsyncOperationHandle _handle)
    {
        yield return new WaitForSeconds(seconds);
        downloadProgress.gameObject.SetActive(false);
        //StartCoroutine(UnloadScene());
        _btTapToStart.gameObject.SetActive(true);
        // downloadScene = Addressables.LoadSceneAsync(_scene, LoadSceneMode.Additive);
        // downloadScene.Completed += SceneDownloadComplete;
    }



    IEnumerator UnloadScene()
    {
        // yield return new WaitForSeconds(10);
        // Addressables.UnloadSceneAsync(handle, true).Completed += op =>
        // {
        //     if(op.Status == AsyncOperationStatus.Succeeded)
        //     {
        //         _camera.SetActive(true);
        //         downloadProgress.gameObject.SetActive(true);
        //         Debug.Log("Successfully unloaded the scene");
        //     }
        // };

        yield return new WaitForSeconds(1);
        SceneManager.UnloadSceneAsync(0);
    }

    public void OnClickStart()
    {
        _btTapToStart.gameObject.SetActive(false);
        downloadScene = Addressables.LoadSceneAsync(_scene, LoadSceneMode.Single);
        //downloadScene.Completed += SceneDownloadComplete;
    }

    public void Restart()
    {
        Addressables.LoadSceneAsync(_scene, LoadSceneMode.Single);
    }

}
