using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

public class BossCutScene : MonoBehaviour
{
    private PlayableDirector pd;
    public TimelineAsset[] ta;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        pd = GetComponent<PlayableDirector>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Cut")
        {
            other.gameObject.SetActive(false);
            pd.Play(ta[0]);
        }
    }
}

