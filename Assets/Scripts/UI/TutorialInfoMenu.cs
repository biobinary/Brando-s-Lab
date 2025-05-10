using System.Collections.Generic;
using System.Linq;
using UnityEngine;

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

		IReadOnlyList<PlaygroundTutorials.Tutorial> tutorials =
			m_currentEnv.GetPlaygroundTutorials().GetAllTutorials();

		if (!tutorials.Any()) return;

		int currentTutorialButtonCount = m_desiredSpawnTransform.childCount;

		for (int i = currentTutorialButtonCount - 1; i > (tutorials.Count - 1); i--) {
			m_desiredSpawnTransform.GetChild(i).gameObject.SetActive(false);
		}

		int index = 0;

		while (index != tutorials.Count) {

			GameObject tutorialButtonGameobject;

			if( index > (currentTutorialButtonCount - 1) )
				tutorialButtonGameobject = Instantiate(m_tutorialButtonObject, m_desiredSpawnTransform);
			else
				tutorialButtonGameobject = m_desiredSpawnTransform.GetChild(index).gameObject;

			TutorialButton tutorialButton = tutorialButtonGameobject.GetComponent<TutorialButton>();
			tutorialButton.SetupButton(this, tutorials[index]);

			index++;

		}

	}

	public void OnHandleTutorialButtonPressed(TutorialButton button) {

		if (button.tutorial?.tutorialVideo == null) return;

		Debug.Log("Called Here!");
		m_videoPlayer.SetupNewVideo(button.tutorial.tutorialVideo);

		m_tutorialListGameobject.SetActive(false);
		m_videoPlayer.gameObject.SetActive(true);


	}

}
