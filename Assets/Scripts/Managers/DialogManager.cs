using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.AddressableAssets;

public class DialogManager : MonoBehaviour
{
    public static DialogManager Instance;
    [SerializeField] GameObject _canvas;
    byte _currentNumberDialog = 0;
    // Start is called before the first frame update
    void Start()
    {
        if(Instance == null) Instance = this;
        if(_canvas == null) _canvas = GameObject.Find("Canvas"); 
    }

    public void ShowDialog(string prefabName)
    {
        Time.timeScale = 0f;
        GameManager.Instance.isPausing = true;
        Transform dialog = _canvas.gameObject.transform.Find(prefabName+"(Clone)");
        if(dialog == null)
        {
            //Instantiate(Resources.Load("Prefabs/Dialog/" + prefabName), _canvas.transform);
            StartCoroutine(LoadDialogAsync(prefabName));
        }
        else
        {
            dialog.gameObject.SetActive(true);
        }
        _currentNumberDialog++;
    }

    public void CloseDialog(GameObject dialog)
    {
        _currentNumberDialog--;
        if(_currentNumberDialog == 0)
        {
            Time.timeScale = 1f;
            GameManager.Instance.isPausing = false;
        }
        dialog.gameObject.SetActive(false);
    }

    private IEnumerator LoadDialogAsync(string prefabName)
    {
        
        AsyncOperationHandle<GameObject> handle = Addressables.LoadAssetAsync<GameObject>(prefabName);
        yield return handle;
        if (handle.Result != null)
            Instantiate(handle.Result, _canvas.transform);
    }
}
