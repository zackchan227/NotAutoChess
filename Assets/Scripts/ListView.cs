using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PooledScrollList.Data;

public class ListView : MonoBehaviour
{
    public Transform _content;
    public int _amountPool = 5;
    public List<ListViewItem> _pooledItems;
    [SerializeField] ListViewItem _listViewItemPrefab;
    [SerializeField] Transform _progressWheel;

    void Awake()
    {
        _pooledItems = new List<ListViewItem>();
        // ListViewItem temp;
        // for(int i = 0; i < _amountPool; i++)
        // {
        //     temp = CreateItem();
        //     AddItemToPool(temp);
        // }
    }

    // public override List<PooledData> GetData()
    // {
    //     var data = new List<PooledData>(1000);

    //     for (var i = 0; i < 1000; i++)
    //     {
    //         //data.Add(new ListViewItem());
    //     }

    //     return data;
    // }

    private ListViewItem CreateItem()
    {
        ListViewItem item = Instantiate(_listViewItemPrefab, _content);
        //item.SetPool(_itemPool);
        item.gameObject.SetActive(false);
        return item;
    }

    public void AddItemToPool(ListViewItem item)
    {
        _pooledItems.Add(item);
    }


    // public void SetItem(ListViewItem item)
    // {
    //     ListViewItem tempItem = GetPooledItem();
    //     for(byte i = 0; i < item._lstItemDatas.Count; i++)
    //     {
    //         tempItem.SetReferenceData(i, item._lstItemDatas[i]);
    //     }
    // }

    public void ResetItem()
    {

    }


    public int IndexOf(ListViewItem item)
    {
        return _pooledItems.IndexOf(item);
    }

    public void UpdateListViewUI()
    {

    }

    public ListViewItem GetPooledItem()
    {
        for (int i = 0; i < _amountPool; i++)
        {
            if (!_pooledItems[i].gameObject.activeInHierarchy)
            {
                return _pooledItems[i];
            }
        }
        return null;
    }

    public void Clear()
    {
        //_pooledItems.Clear();
        for(int i = 0; i < _pooledItems.Count; i++)
        {
            Destroy(_pooledItems[i]);
        }
    }

    public void PlayProgressWheel()
    {
        _progressWheel.gameObject.SetActive(true);
        StartCoroutine(Rotate());
    }

    public void StopProgressWheel()
    {
        StopCoroutine(Rotate());
        _progressWheel.gameObject.SetActive(false);
    }

    IEnumerator Rotate()
    {
        while(_pooledItems.Count == 0)
        {
            _progressWheel.Rotate(90, Time.deltaTime / 100, 0);
            yield return null;
        }
    }
}

