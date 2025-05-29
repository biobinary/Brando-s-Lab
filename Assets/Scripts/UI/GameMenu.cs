using TMPro;
using UnityEngine;
using UnityEngine.UI;
using BrandosLab.Playgrounds;

namespace BrandosLab.UI {

	public class GameMenu : MonoBehaviour {

		[SerializeField] private GameObject m_canvasRoot;
		[SerializeField] private GameObject m_tutorialMenu;
		[SerializeField] private GameObject m_objectiveMenu;

		[Header("Environtment Information")]
		[SerializeField] private GameObject m_environmentInformationContent;
		[SerializeField] private TextMeshProUGUI m_environtmentInformationTitle;

		[Header("Game Related Buttons")]
		[SerializeField] private Toggle m_resetButton;
		[SerializeField] private Toggle m_backToSelectionButton;

		private void Awake() {

			m_resetButton.onValueChanged.AddListener((value) =>
				PlaygroundEnvironmentManager.Instance.ResetEnvirontment());

			m_backToSelectionButton.onValueChanged.AddListener((value) =>
				PlaygroundEnvironmentManager.Instance.ClearEnvironment());

			PlaygroundEnvironmentManager.Instance.OnLoadEnvironment += OnEnvironmentLoaded;
			PlaygroundEnvironmentManager.Instance.OnDestroyEnvironment += OnEnvironmentDestroyed;

			m_tutorialMenu.SetActive(true);
			m_objectiveMenu.SetActive(true);

		}

		private void Start() {

			m_tutorialMenu.SetActive(false);
			m_objectiveMenu.SetActive(true);
			m_canvasRoot.SetActive(false);

		}

		private void OnEnvironmentLoaded(PlaygroundEnvironment env) {

			m_canvasRoot.SetActive(true);

			if (env != null)
				m_environtmentInformationTitle.text = env.gameObject.name;
			else
				m_environtmentInformationTitle.text = "Unknown Environtment";

		}

		private void OnEnvironmentDestroyed() {
			m_canvasRoot.SetActive(false);
		}

	}

}