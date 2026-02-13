using System;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.VFX;

public class PoliceSpawner : MonoBehaviour
{
    [SerializeField] private GameObject[] spawners;
    [SerializeField] private GameObject police;
    [SerializeField] private float spawnTime;
    [SerializeField] private int maxPolice;

    private bool spawnDebounce = false;
    IEnumerator SpawnCop()
    {
        spawnDebounce = true;

        int index = UnityEngine.Random.Range(0, spawners.Length);
        Instantiate(police, spawners[index].transform.position, Quaternion.identity);

        yield return new WaitForSeconds(spawnTime);

        spawnDebounce = false;

    }

    
    void Update()
    {
        int policeCount = GameObject.FindGameObjectsWithTag("PoliceVehicle").Length; // Can slow performance down 
        
        if (spawnDebounce || policeCount >= maxPolice) { return; }
        
        StartCoroutine(SpawnCop());
    }
}
