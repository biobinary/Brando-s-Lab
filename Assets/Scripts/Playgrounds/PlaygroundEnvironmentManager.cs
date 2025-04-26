using System.Collections.Generic;
using UnityEngine;

public class PlaygroundEnvironmentManager : MonoBehaviour {

	public static PlaygroundEnvironmentManager Instance;

	[Header("UI References")]
	[SerializeField] private GameObject m_playgroundSelectionButtons;
	[SerializeField] private GameMenu m_gameMenuPanel;

	[Header("Environments")]
	[SerializeField] private List<PlaygroundEnvironment> m_environments;
	private PlaygroundEnvironment m_currentEnvironment = null;

	[HideInInspector] public bool IsOnProgress = false;

	public PlaygroundEnvironment GetCurrentEnvironment() => m_currentEnvironment;

	private void Awake() {
		
		if( Instance == null ) {
			Instance = this;
		
		} else if( Instance != null && Instance != this ) {
			Destroy( gameObject );

		}

		foreach (var env in m_environments) { 
			env.gameObject.SetActive(true);
		}

	}

	private void Start() {
		HideGamePanel();

	}

	public void LoadEnvironment(int environmentIdx) {

		if ( IsOnProgress )
			return;

		IsOnProgress = true;
		m_currentEnvironment = m_environments[environmentIdx];

		System.Action onFinished = () => { IsOnProgress = false; };
		m_currentEnvironment.Load(onFinished);
		
		ShowGamePanel();

	}

	public void ResetEnvirontment() {

		if ( IsOnProgress )
			return;

		IsOnProgress = true;
		System.Action onFinished = () => { IsOnProgress = false; };
		m_currentEnvironment.Reset(onFinished);

	}

	public void ClearEnvironment() {

		if (IsOnProgress)
			return;

		IsOnProgress = true;

		System.Action onFinished = () => {
			HideGamePanel();
			IsOnProgress = false;
		};

		m_currentEnvironment.Clear(onFinished);

	}

	private void ShowGamePanel() {
		m_playgroundSelectionButtons.SetActive(false);
		m_gameMenuPanel.gameObject.SetActive(true);
	}

	private void HideGamePanel() {
		m_playgroundSelectionButtons.SetActive(true);
		m_gameMenuPanel.gameObject.SetActive(false);
	}

}
