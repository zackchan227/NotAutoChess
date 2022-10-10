using System.Collections.Generic;
using UnityEngine;
using PooledScrollList.View;
using PooledScrollList.Data;
using TMPro;
using System;

[SerializeField] 
public class ListViewItemData : PooledData
{
    public List<object> _datas;
    public ListViewItemData()
    {
        
    }
}

public class ListViewItem : PooledView
{
    public List<GameObject> _lstItemDatas = new List<GameObject>();

    private void SetReferenceDatas(ListViewItemData data)
    {
        for(int i = 0; i < data._datas.Count; i++)
        {
            _lstItemDatas[i].GetComponent<TMP_Text>().text = data._datas[i].ToString();
        }
    }

    public override void SetData(PooledData data)
    {
        base.SetData(data);

        ListViewItemData exampleData = data as ListViewItemData;
        SetReferenceDatas(exampleData);
    }
}
