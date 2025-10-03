// Имя файла: SceneLoaderTrigger.cs

using System.Collections.Generic;
using Gamekit3D;
using UnityEngine;
using UnityEngine.SceneManagement;
// Обязательно для перехода между сценами
// Подключаем наш EnemyCounter

// Для использования List

public class SceneLoaderTrigger : MonoBehaviour
{
    [Header("Настройки сцены")]
    // Имя сцены, на которую нужно перейти (Обязательно добавьте сцену в Build Settings!)
    public string targetSceneName = "Level02";

    [Header("Условие триггера")]
    // Список врагов, которых нужно убить для активации
    public List<RequiredKill> requiredKillsList;

    // Слой игрока, чтобы триггер реагировал только на него
    public LayerMask playerLayer;

    private bool isUnlocked;

    private void Update()
    {
        // Проверяем условие, пока триггер не активирован
        if (!isUnlocked)
            if (CheckKillRequirements())
            {
                isUnlocked = true;
                Debug.Log("Триггер разблокирован! Все необходимые враги убиты.");
                // Тут можно добавить эффект, звук или поменять материал объекта-триггера
            }
    }

    // Срабатывает, когда коллайдер входит в область триггера
    private void OnTriggerEnter(Collider other)
    {
        // Проверяем, что это игрок (по слою) И что триггер разблокирован
        if (isUnlocked && (playerLayer.value & (1 << other.gameObject.layer)) != 0)
        {
            Debug.Log("Переход на сцену: " + targetSceneName);
            // Загрузка указанной сцены
            SceneManager.LoadScene(targetSceneName);
        }
        else if (!isUnlocked && (playerLayer.value & (1 << other.gameObject.layer)) != 0)
        {
            // Добавим более подробный лог для отладки
            foreach (var requirement in requiredKillsList)
            {
                var currentKills = EnemyCounter.GetKillCount(requirement.enemyType);
                if (currentKills < requirement.count)
                    Debug.Log("Триггер заблокирован. Нужно убить " + requirement.count + " врагов типа " +
                              requirement.enemyType + ". Убито пока: " + currentKills);
            }
        }
    }

    private bool CheckKillRequirements()
    {
        foreach (var requirement in requiredKillsList)
            if (EnemyCounter.GetKillCount(requirement.enemyType) < requirement.count)
                return false; // Если хотя бы одно условие не выполнено, выходим
        return true; // Все условия выполнены
    }
}