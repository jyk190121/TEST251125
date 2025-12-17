using System.Collections;
using UnityEngine;
using static UnityEditor.PlayerSettings;

public class Party_Shop_Night : MonoBehaviour
{
    //10번부턴 1회성
    public GameObject[] particleArray;
    Transform pos;
    void Awake()
    {
        pos = transform;
    }

    //파티클 터트리자!
    public IEnumerator partyToNight()
    {
        float x = Random.Range(-6, 4);
        float z = Random.Range(-7, 4.5f);
        pos.position = new Vector3(x, 1.8f, z);

        int randomParticle = Random.Range(0, particleArray.Length - 1);

        //5~10
        int r = Random.Range(5, 11);

        GameObject[] particles = new GameObject[particleArray.Length - 1];

        for(int i =0; i < particleArray.Length; i++)
        {
            if (randomParticle + i >= particleArray.Length) break;

            particles[i] = Instantiate(particleArray[randomParticle + i], pos);
            yield return new WaitForSeconds(r);
        }

        yield return new WaitForSeconds(r);

        for (int i = 0; i < particleArray.Length - 1; i++)
        {
            Destroy(particles[i]);
            yield return new WaitForSeconds(r);
        }
    }

    public void StopParty()
    {
        StopAllCoroutines();

        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }
    }
}
