using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Party_Shop_Night))]
public class Party_Shop_Night : MonoBehaviour
{
    //10번부턴 1회성
    public GameObject[] particleArray;
    List<GameObject> particles;
    Transform pos;
    void Awake()
    {
        pos = transform;
    }

    //파티클 터트리자!
    public IEnumerator partyToNight()
    {
        float x = Random.Range(-2, 2);
        //z값 2 or -8
        float z = Random.Range(0, 2);
        if (z == 0) z = 2;
        else if (z == 1) z = -8;
        pos.position = new Vector3(x, 1, z);

        int randomParticle = Random.Range(0, 10);

        //5~10
        int r = Random.Range(5, 11);

        particles = new List<GameObject>();
        GameObject[] temp = new GameObject[particleArray.Length - 1];

        for(int i =0; i < particleArray.Length; i++)
        {
            if (randomParticle + i >= particleArray.Length - 1) break;

            temp[i] = Instantiate(particleArray[randomParticle + i], pos);
            particles.Add(temp[i]);
            yield return new WaitForSeconds(r);

            if (particles != null && i > 1)
            {
                if (particles[i] == null) break;
                Destroy(particles[i]);
                yield return new WaitForSeconds(r);
            }
        }
        yield return new WaitForSeconds(r);

        foreach (GameObject particle in particles)
        {
            if (particle != null) Destroy(particle);
        }

    }

    public void StopParty()
    {
        if (particles == null) return;

        foreach (GameObject particle in particles)
        {
            if (particle != null) Destroy(particle);
        }

        particles.Clear();

        StopAllCoroutines();
    }
}
