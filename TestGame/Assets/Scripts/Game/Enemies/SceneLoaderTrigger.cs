using System.Collections.Generic;
using Gamekit3D;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoaderTrigger : MonoBehaviour
{
    public string targetSceneName = "Level02";

    public List<RequiredKill> requiredKillsList;

    public LayerMask playerLayer;

    private bool isUnlocked;

    private void Update()
    {
        if (!isUnlocked)
            if (CheckKillRequirements())
            {
                isUnlocked = true;
            }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (isUnlocked && (playerLayer.value & (1 << other.gameObject.layer)) != 0)
        {
            SceneManager.LoadScene(targetSceneName);
        }
        else if (!isUnlocked && (playerLayer.value & (1 << other.gameObject.layer)) != 0)
        {
            
        }
    }

    private bool CheckKillRequirements()
    {
        foreach (var requirement in requiredKillsList)
            if (EnemyCounter.GetKillCount(requirement.enemyType) < requirement.count)
                return false;
        return true;
    }
}