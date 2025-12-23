using System.Collections;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

public class BossCutScene : MonoBehaviour
{
    private PlayableDirector pd;
    public TimelineAsset[] ta;

    void Start()
    {
        pd = GetComponent<PlayableDirector>();
    }

    private void OnTriggerEnter(Collider other)
    {
        
        if (other.CompareTag("Cut"))
        {
            other.gameObject.SetActive(false); // 트리거 중복 방지

            // 타임라인 재생
            if (ta.Length > 0)
            {
                pd.Play(ta[0]);
            }
        }
    }
}
