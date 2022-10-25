using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.AddressableAssets;
using UnityEngine.AddressableAssets.ResourceLocators;

// public class AssetReferenceAudioClip : AssetReference<AudioClip>
// {
//     public AssetReferenceAudioClip(string guid):base(guid) {}
// }

public class AddressablesManager : MonoBehaviour
{
    public static AddressablesManager Instance;
    [SerializeField] AssetReference _player;
    [SerializeField] AssetReference _enemy;
    [SerializeField] AssetReference leaderboardDialog;
    [SerializeField] AssetReference settingsDialog;
    [SerializeField] AssetReference toastDialog;
    [SerializeField] AssetReference background;
    [SerializeField] AssetReference soundsManager;
    [SerializeField] AssetReference popup;
    [SerializeField] Material defaultMaterial;

    Transform _canvas;
    Transform _managers;


    void Awake()
    {
        if (Instance == null) Instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        if (_canvas == null) _canvas = GameObject.Find("Canvas").transform;
        if (_managers == null) _managers = GameObject.Find("Managers").transform;
        Addressables.InitializeAsync().Completed += AddressablesManager_Completed;
    }

    private void AddressablesManager_Completed(AsyncOperationHandle<IResourceLocator> obj)
    {
        background.InstantiateAsync(Camera.main.transform).Completed += (go) =>
        {
            //go.Result.GetComponent<SpriteRenderer>().material = defaultMaterial;
        };

        soundsManager.InstantiateAsync(_managers).Completed += (go) =>
        {
            GameManager.Instance.SoundsCheck(GameManager.Instance.isSoundsOn);
        };

        toastDialog.InstantiateAsync(_canvas).Completed += (go) =>
        {
            // order sorting layer : make this object display in front of lower sorting layer
            go.Result.GetComponent<Canvas>().overrideSorting = true;
        };

        popup.InstantiateAsync(_canvas).Completed += (go) =>
        {
            // order sorting layer : make this object display in front of lower sorting layer
            go.Result.GetComponent<Canvas>().overrideSorting = true;
        };
    }

    private void OnLoadDone(AsyncOperationHandle<GameObject> obj)
    {
        GameObject myGO = obj.Result;
        myGO.transform.parent = _canvas;
        //FK.GetComponent<SomeScript>().target = _canvas.GetChild(0).gameObject;
    }
}
