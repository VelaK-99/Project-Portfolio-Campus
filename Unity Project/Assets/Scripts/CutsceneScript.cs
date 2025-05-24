using UnityEngine;
using UnityEngine.Playables;

public class CutsceneScript : MonoBehaviour
{
    [SerializeField] public PlayableDirector timeline;
    public bool hasPlayed = false;
    [SerializeField] public GameObject realBoss;
    [SerializeField] public GameObject cutsceneBoss;
    [SerializeField] public Camera cutsceneCam;
    [SerializeField] public Camera gameCam;
    [SerializeField] GameObject playerObject;
    [SerializeField] GameObject dustPrefab;
    [SerializeField] Transform dustSpawnPoint;
    [SerializeField] AudioClip landingSFX;
    [SerializeField] AudioSource aud;
    [SerializeField] GameObject UI;
    [SerializeField] GameObject bossName;
    [SerializeField] GameObject bossTag;

    private void Start()
    {
        cutsceneCam.gameObject.SetActive(false);
        
        timeline.stopped += OnCutsceneEnd;
        realBoss.SetActive(false);

    }

    void OnCutsceneEnd (PlayableDirector playable)
    {
        if (playable == timeline)
        {
            cutsceneBoss.SetActive(false);
            UI.SetActive(true);
            realBoss.transform.position = cutsceneBoss.transform.position;
            realBoss.SetActive(true);

            gameCam.gameObject.SetActive(true);
            cutsceneCam.gameObject.SetActive(false);
            playerObject.gameObject.SetActive(true);
            bossName.SetActive(false);
            bossTag.SetActive(false);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(!hasPlayed && other.CompareTag("Player"))
        {
            hasPlayed = true;

            gameCam.gameObject.SetActive(false);
            cutsceneCam.gameObject.SetActive(true);
            playerObject.gameObject.SetActive(false);
            UI.SetActive(false);

            timeline.Play();
        }
    }

    public void SpawnDust()
    {
        Instantiate(dustPrefab, dustSpawnPoint.position, Quaternion.identity);
        aud.PlayOneShot(landingSFX);
    }

    public void ShowBossName()
    {
        bossName.SetActive(true);
        bossTag.SetActive(true);
    }

}
