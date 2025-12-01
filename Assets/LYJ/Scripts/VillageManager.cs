using UnityEngine;
using System.Collections.Generic;

public class VillageManager : MonoBehaviour
{
    [SerializeField] private List<VillageLocation> locations = new();

    private VillageLocation currentLocation;

    void Start()
    {
        // 자동으로 모든 VillageLocation 찾기
        locations.AddRange(GetComponentsInChildren<VillageLocation>());

        Debug.Log($"VillageManager 초기화 (장소 수: {locations.Count})");

        // 각 장소 출력
        foreach (var location in locations)
        {
            Debug.Log($"  - {location.GetLocationType()}: {location.name}");
        }
    }

    public void RegisterLocation(VillageLocation location)
    {
        if (!locations.Contains(location))
        {
            locations.Add(location);
        }
    }

    public VillageLocation GetCurrentLocation() => currentLocation;
    public List<VillageLocation> GetAllLocations() => locations;
}

