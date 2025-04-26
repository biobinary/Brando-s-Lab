using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameMenu : MonoBehaviour {

	[SerializeField] private GameObject m_settingsContent;

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

	}

	private void OnEnable() {
		
		m_environmentInformationContent.SetActive(true);
		m_settingsContent.SetActive(false);

		if (PlaygroundEnvironmentManager.Instance != null) { 
			
			PlaygroundEnvironment currentEnv = PlaygroundEnvironmentManager.Instance.GetCurrentEnvironment();
			
			if (currentEnv != null) { 
				m_environtmentInformationTitle.text = currentEnv.gameObject.name;			
			
			} else {
				m_environtmentInformationTitle.text = "Unknown Environtment";
			
			}

		}

	}

}
