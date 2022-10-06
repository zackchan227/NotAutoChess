using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogManager : MonoBehaviour
{
    public static DialogManager Instance;
    [SerializeField] GameObject _canvas;
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
            Instantiate(Resources.Load("Prefabs/Dialog/" + prefabName), _canvas.transform);
        }
        else
        {
            dialog.gameObject.SetActive(true);
        }
    }

    public void CloseDialog(GameObject dialog)
    {
        Time.timeScale = 1f;
        dialog.gameObject.SetActive(false);
    }
}
