using UnityEngine;
using BrandosLab.Playgrounds;

namespace BrandosLab.UI {

	public class TutorialInfoMenu : MonoBehaviour {

		private PlaygroundEnvironment m_currentEnv = null;
		private bool m_isTutorialInitialized = false;

		[SerializeField] private VideoPlayerController m_videoPlayer;
		[SerializeField] private GameObject m_tutorialListGameobject;
		[SerializeField] private Transform m_desiredSpawnTransform;
		[SerializeField] private GameObject m_tutorialButtonObject;

		private void Awake() {

			PlaygroundEnvironmentManager.Instance.OnLoadEnvironment += (PlaygroundEnvironment env) => {
				m_currentEnv = env;
			};

			PlaygroundEnvironmentManager.Instance.OnDestroyEnvironment += () => {

				m_tutorialListGameobject.SetActive(true);
				m_videoPlayer.gameObject.SetActive(false);

				m_currentEnv = null;
				m_isTutorialInitialized = false;

			};

		}

		private void OnEnable() {

			if (!m_isTutorialInitialized)
				InitializedTutorial();

		}

		private void InitializedTutorial() {

			if (m_currentEnv == null) return;

			m_isTutorialInitialized = true;

			var playgroundTutorials = m_currentEnv.GetPlaygroundTutorials();
			if (playgroundTutorials == null) return;

			var tutorials = playgroundTutorials.GetAllTutorials();
			if (tutorials == null || tutorials.Count == 0) return;

			int totalTutorials = tutorials.Count;
			int currentButtonCount = m_desiredSpawnTransform.childCount;

			for (int i = totalTutorials; i < currentButtonCount; i++) {
				m_desiredSpawnTransform.GetChild(i).gameObject.SetActive(false);
			}

			for (int i = 0; i < totalTutorials; i++) {

				GameObject tutorialButtonObject;

				if (i >= currentButtonCount) {
					tutorialButtonObject = Instantiate(m_tutorialButtonObject, m_desiredSpawnTransform);

				} else {
					tutorialButtonObject = m_desiredSpawnTransform.GetChild(i).gameObject;
					tutorialButtonObject.SetActive(true);
				}

				var tutorialButton = tutorialButtonObject.GetComponent<TutorialButton>();
				tutorialButton.SetupButton(this, tutorials[i]);

			}


		}

		public void OnHandleTutorialButtonPressed(TutorialButton button) {

			if (button.tutorial?.tutorialVideo == null) return;

			m_videoPlayer.SetupNewVideo(button.tutorial.tutorialVideo);

			m_tutorialListGameobject.SetActive(false);
			m_videoPlayer.gameObject.SetActive(true);


		}

	}

}