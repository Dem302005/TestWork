using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Gamekit3D
{
    public class SceneController : MonoBehaviour
    {
        protected static SceneController instance;

        public SceneTransitionDestination initialSceneTransitionDestination;

        protected Scene m_CurrentZoneScene;
        protected PlayerInput m_PlayerInput;
        protected bool m_Transitioning;
        protected SceneTransitionDestination.DestinationTag m_ZoneRestartDestinationTag;

        public static SceneController Instance
        {
            get
            {
                if (instance != null)
                    return instance;

                instance = FindObjectOfType<SceneController>();

                if (instance != null)
                    return instance;

                Create();

                return instance;
            }
        }

        public static bool Transitioning => Instance.m_Transitioning;

        private void Awake()
        {
            if (Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            DontDestroyOnLoad(gameObject);

            m_PlayerInput = FindObjectOfType<PlayerInput>();

            if (initialSceneTransitionDestination != null)
            {
                SetEnteringGameObjectLocation(initialSceneTransitionDestination);
                ScreenFader.SetAlpha(1f);
                StartCoroutine(ScreenFader.FadeSceneIn());
                initialSceneTransitionDestination.OnReachDestination.Invoke();
            }
            else
            {
                m_CurrentZoneScene = SceneManager.GetActiveScene();
                m_ZoneRestartDestinationTag = SceneTransitionDestination.DestinationTag.A;
            }
        }

        public static SceneController Create()
        {
            var sceneControllerGameObject = new GameObject("SceneController");
            instance = sceneControllerGameObject.AddComponent<SceneController>();

            return instance;
        }

        public static void RestartZone(bool resetHealth = true)
        {
            Instance.StartCoroutine(Instance.Transition(Instance.m_CurrentZoneScene.name,
                Instance.m_ZoneRestartDestinationTag));
        }

        public static void RestartZoneWithDelay(float delay, bool resetHealth = true)
        {
            Instance.StartCoroutine(CallWithDelay(delay, RestartZone, resetHealth));
        }

        public static void TransitionToScene(TransitionPoint transitionPoint)
        {
            Instance.StartCoroutine(Instance.Transition(transitionPoint.newSceneName,
                transitionPoint.transitionDestinationTag, transitionPoint.transitionType));
        }

        public static SceneTransitionDestination GetDestinationFromTag(
            SceneTransitionDestination.DestinationTag destinationTag)
        {
            return Instance.GetDestination(destinationTag);
        }

        protected IEnumerator Transition(string newSceneName, SceneTransitionDestination.DestinationTag destinationTag,
            TransitionPoint.TransitionType transitionType = TransitionPoint.TransitionType.DifferentZone)
        {
            m_Transitioning = true;
            PersistentDataManager.SaveAllData();

            if (m_PlayerInput == null)
                m_PlayerInput = FindObjectOfType<PlayerInput>();
            if (m_PlayerInput) m_PlayerInput.ReleaseControl();
            yield return StartCoroutine(ScreenFader.FadeSceneOut(ScreenFader.FadeType.Loading));
            PersistentDataManager.ClearPersisters();
            yield return SceneManager.LoadSceneAsync(newSceneName);
            m_PlayerInput = FindObjectOfType<PlayerInput>();
            if (m_PlayerInput) m_PlayerInput.ReleaseControl();
            PersistentDataManager.LoadAllData();
            var entrance = GetDestination(destinationTag);
            SetEnteringGameObjectLocation(entrance);
            SetupNewScene(transitionType, entrance);
            if (entrance != null)
                entrance.OnReachDestination.Invoke();
            yield return StartCoroutine(ScreenFader.FadeSceneIn());
            if (m_PlayerInput)
                m_PlayerInput.GainControl();

            m_Transitioning = false;
        }

        protected SceneTransitionDestination GetDestination(SceneTransitionDestination.DestinationTag destinationTag)
        {
            var entrances = FindObjectsOfType<SceneTransitionDestination>();
            for (var i = 0; i < entrances.Length; i++)
                if (entrances[i].destinationTag == destinationTag)
                    return entrances[i];
            Debug.LogWarning("No entrance was found with the " + destinationTag + " tag.");
            return null;
        }

        protected void SetEnteringGameObjectLocation(SceneTransitionDestination entrance)
        {
            if (entrance == null)
            {
                Debug.LogWarning("Entering Transform's location has not been set.");
                return;
            }

            var entranceLocation = entrance.transform;
            var enteringTransform = entrance.transitioningGameObject.transform;
            enteringTransform.position = entranceLocation.position;
            enteringTransform.rotation = entranceLocation.rotation;
        }

        protected void SetupNewScene(TransitionPoint.TransitionType transitionType, SceneTransitionDestination entrance)
        {
            if (entrance == null)
            {
                Debug.LogWarning("Restart information has not been set.");
                return;
            }

            if (transitionType == TransitionPoint.TransitionType.DifferentZone)
                SetZoneStart(entrance);
        }

        protected void SetZoneStart(SceneTransitionDestination entrance)
        {
            m_CurrentZoneScene = entrance.gameObject.scene;
            m_ZoneRestartDestinationTag = entrance.destinationTag;
        }

        private static IEnumerator CallWithDelay<T>(float delay, Action<T> call, T parameter)
        {
            yield return new WaitForSeconds(delay);
            call(parameter);
        }
    }
}