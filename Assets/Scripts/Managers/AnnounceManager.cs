using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Event;
using TMPro;

public class AnnounceManager : MonoBehaviour
{
    public static AnnounceManager Instance;
    [SerializeField] private AudioClip[] _announces;
    [SerializeField] private TMP_Text tmpAnnounceKillStreak;
    public AudioSource AudioSource {get; set;}
    private byte currentKillStreak = 0;

    void Awake()
    {
        if(Instance == null) Instance = this;
        AudioSource = GetComponent<AudioSource>();
        //tmpAnnounceKillStreak = GameObject.Find("tmpAnnounceKillStreak").GetComponent<TMP_Text>();
    }

    // Start is called before the first frame update
    void Start()
    {
        this.AddListener(EventID.OnFirstBlood, (param) => PlayAnnounce(ANNOUNCE.FIRST_BLOOD)/*StartCoroutine(WaitToTurnDownVolume(ANNOUNCE.FIRST_BLOOD))*/);

        this.AddListener(EventID.OnOneHundredPoint, (param) => PlayAnnounce(ANNOUNCE.KILLING_SPREE));
        this.AddListener(EventID.OnThreeHundredPoint, (param) => PlayAnnounce(ANNOUNCE.DOMINATING));
        this.AddListener(EventID.OnFiveHundredPoint, (param) => PlayAnnounce(ANNOUNCE.UNSTOPPABLE));
        this.AddListener(EventID.OnOneThousandPoint, (param) => PlayAnnounce(ANNOUNCE.GOD_LIKE));
        this.AddListener(EventID.OnOneThousandFiveHundredPoint, (param) => PlayAnnounce(ANNOUNCE.WICKED_SICK));
        this.AddListener(EventID.OnTwoThousandPoint, (param) => PlayAnnounce(ANNOUNCE.HOLY_SHEET));
        this.AddListener(EventID.OnThreeThousandPoint, (param) => PlayAnnounce(ANNOUNCE.SHOW_ME_MORE));

        this.AddListener(EventID.OnKillStreak, (param) => playAnnounceKillStreak((byte)param));
    }

    void OnEnable()
    {
        //tmpAnnounceKillStreak.gameObject.SetActive(false);
    }

    void OnDisable()
    {
        this.RemoveAllListener();
    }

    private void showAnnounceKillStreak(string str)
    {
        // tmpAnnounceKillStreak.text = str;
        // tmpAnnounceKillStreak.gameObject.SetActive(true);
        Popup.Instance.showPopup(str,5.0f,Popup.ANIMATE.WOBBLE);
    }

    private void playAnnounceKillStreak(byte an)
    {
        // if(!compareCurrentKillStreak(an))
        // {
        //     Popup.Instance.ResetPosition();
        // }
        if(an > 5)
        {
            PlayAnnounce(ANNOUNCE.RAMPAGE);
        }
        else PlayAnnounce((ANNOUNCE)an-1);
    }

    private bool compareCurrentKillStreak(byte an)
    {
        if(currentKillStreak < an) 
        {
            currentKillStreak = an;
            return true;
        }
        else 
        {
            currentKillStreak = 0;
            return false;
        }
    }

    private void PlayAnnounce(ANNOUNCE an)
    {
        switch(an)
        {
            case ANNOUNCE.FIRST_BLOOD:
                AudioSource.PlayOneShot(_announces[0]);
                showAnnounceKillStreak("First Blood");
                this.RemoveListener(EventID.OnFirstBlood);
                break;
            case ANNOUNCE.DOUBLE_KILL:
                AudioSource.PlayOneShot(_announces[1]);
                showAnnounceKillStreak("Double Kill");
                break;
            case ANNOUNCE.TRIPLE_KILL:
                AudioSource.PlayOneShot(_announces[2]);
                showAnnounceKillStreak("Triple Kill");
                break;
            case ANNOUNCE.KILLING_SPREE:
                AudioSource.PlayOneShot(_announces[3]);
                break;
            case ANNOUNCE.ULTRA_KILL:
                AudioSource.PlayOneShot(_announces[4]);
                showAnnounceKillStreak("Ultra Kill");
                break;
            case ANNOUNCE.MEGA_KILL:
                AudioSource.PlayOneShot(_announces[5]);
                break;
            case ANNOUNCE.MONSTER_KILL:
                AudioSource.PlayOneShot(_announces[6]);
                break;
            case ANNOUNCE.RAMPAGE:
                AudioSource.PlayOneShot(_announces[7]);
                showAnnounceKillStreak("Rampage");
                break;
            case ANNOUNCE.DOMINATING:
                AudioSource.PlayOneShot(_announces[8]);
                break;
            case ANNOUNCE.UNSTOPPABLE:
                AudioSource.PlayOneShot(_announces[9]);
                break;
            case ANNOUNCE.WICKED_SICK:
                AudioSource.PlayOneShot(_announces[10]);
                break;
            case ANNOUNCE.GOD_LIKE:
                AudioSource.PlayOneShot(_announces[11]);
                break;
            case ANNOUNCE.HOLY_SHEET:
                AudioSource.PlayOneShot(_announces[12]);
                break;
            case ANNOUNCE.SHOW_ME_MORE:
                AudioSource.PlayOneShot(_announces[13]);
                break;
            default:
                //AudioSource.PlayOneShot(_announces[13]);
                break;
        }
        //StartCoroutine(WaitToDisableAnnounceKillStreak());
    }

    IEnumerator WaitToDisableAnnounceKillStreak()
    {
        yield return new WaitForSeconds(0.5f);
        tmpAnnounceKillStreak.gameObject.SetActive(false);
    }

    IEnumerator WaitToTurnUpVolume()
    {
        while(SoundsManager.Instance.AudioSource.volume < SoundsManager.Instance.currentVolume)
        {
            SoundsManager.Instance.TurnUpVolume();
            yield return null;
        }
        
    }

    IEnumerator WaitToTurnDownVolume(ANNOUNCE an)
    {
        bool check = false;
        float targetVolume = SoundsManager.Instance.AudioSource.volume/2;
        while(!check)
        {
            if(SoundsManager.Instance.AudioSource.volume <= targetVolume) 
            {
                check = true;
                PlayAnnounce(an);
            }
            SoundsManager.Instance.TurnDownVolume();
            yield return null;
        }
        StartCoroutine(WaitToTurnUpVolume());
    }
}
