using UnityEngine;

public class Light_Shop : MonoBehaviour
{
    public void OnLight()
    {
        gameObject.SetActive(true);
    }

    public void OffLight()
    {
        gameObject.SetActive(false);
    }
}
