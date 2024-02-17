using UnityEngine;
using System.Collections.Generic;

public class LevelBuilder : MonoBehaviour
{
    public static LevelBuilder Instance;

    public GameObject[] SpawnerPrefabs;
    private List<GameObject> _spawners = new List<GameObject>();


    private void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(this);
        else
            Instance = this;
    }
    public void BuildLevel()
    {
        foreach (GameObject spawner in SpawnerPrefabs)
        {
            var currentSpawner = Instantiate(spawner);

            _spawners.Add(currentSpawner);

            currentSpawner.GetComponent<ILevelSpawner>().Build();
        }
    }
    public void ResetLevel()
    {
        foreach (GameObject spawner in _spawners)
        {
            spawner.GetComponent<ILevelSpawner>().Clear();
            Destroy(spawner);
        }
        _spawners.Clear();
    }
    public void StartLevel()
    {
        ResetLevel();
        BuildLevel();
    }
}