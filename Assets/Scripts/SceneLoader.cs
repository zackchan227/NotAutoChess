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
    [SerializeField] private AssetReference _scene;
    [SerializeField] private List<AssetReference> _references = new List<AssetReference>();
    [SerializeField] private Slider downloadProgress;
    [SerializeField] private TMPro.TMP_Text tmpProgress;
    [SerializeField] private TMPro.TMP_Text tmpTotalDownloaded;
    private AsyncOperationHandle<SceneInstance> handle;
    public GameObject _camera;
    long DownloadSize = 0;

    void Awake()
    {
        DontDestroyOnLoad(gameObject);
        downloadProgress.minValue = 0;
        downloadProgress.maxValue = 100;
    }

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(DownloadScene());
    }

    IEnumerator DownloadScene()
    {
        var downloadScene = Addressables.LoadSceneAsync(_scene, LoadSceneMode.Additive);
        downloadScene.Completed += SceneDownloadComplete;
        Debug.Log("Starting scene download");

        while(!downloadScene.IsDone)
        {
            var status = downloadScene.GetDownloadStatus();
            float progress = status.Percent;
            double downloadedMB = Math.Round((float)status.DownloadedBytes/1024/1024, 2);
            double totalMB = Math.Round((float)status.TotalBytes/1024/1024,2);
            downloadProgress.value = (int)(progress * 100);
            tmpProgress.text = downloadProgress.value.ToString() + "%";
            tmpTotalDownloaded.text = downloadedMB + "MB/" + totalMB + "MB";
            yield return null;
        }

        Debug.Log("Download Complete");
        downloadProgress.value = 100;
        tmpProgress.text = downloadProgress.value.ToString() + "%";
    }

    private void SceneDownloadComplete(AsyncOperationHandle<SceneInstance> _handle)
    {
        if(_handle.Status == AsyncOperationStatus.Succeeded)
        {
            Debug.Log(_handle.Result.Scene.name + " successfully loaded.");
            _camera.SetActive(false);
            downloadProgress.gameObject.SetActive(false);
            handle = _handle;
            //StartCoroutine(UnloadScene());
        }
    }

    IEnumerator UnloadScene()
    {
        yield return new WaitForSeconds(10);
        Addressables.UnloadSceneAsync(handle, true).Completed += op =>
        {
            if(op.Status == AsyncOperationStatus.Succeeded)
            {
                _camera.SetActive(true);
                downloadProgress.gameObject.SetActive(true);
                Debug.Log("Successfully unloaded the scene");
            }
        };

        yield return new WaitForSeconds(5);
        StartCoroutine(DownloadScene());
    }

}
