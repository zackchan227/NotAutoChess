using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundsManager : MonoBehaviour
{
    public static SoundsManager Instance;
    public AudioSource AudioSource;

    void Awake()
    {
        Instance = this;
        AudioSource = this.GetComponent<AudioSource>();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }
}
