using System.Collections.Generic;
using UnityEngine;

public class PlaygroundEnvironmentManager : MonoBehaviour {

	public static PlaygroundEnvironmentManager Instance;
	public event System.Action<PlaygroundEnvironment> OnLoadEnvironment;
	public event System.Action OnDestroyEnvironment;
	public event System.Action OnResetEnvironment;

	[Header("Playground Selection UI")]
	[SerializeField] private GameObject m_playgroundSelection;

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
		m_playgroundSelection.SetActive(true);
	}

	public void LoadEnvironment(int environmentIdx) {

		if ( IsOnProgress )
			return;

		IsOnProgress = true;
		m_currentEnvironment = m_environments[environmentIdx];

		System.Action onFinished = () => { 
			IsOnProgress = false;
			OnLoadEnvironment?.Invoke( m_currentEnvironment );
		};
		
		m_currentEnvironment.Load(onFinished);

		m_playgroundSelection.SetActive(false);

	}

	public void ResetEnvirontment() {

		if ( IsOnProgress )
			return;

		IsOnProgress = true;
		
		System.Action onFinished = () => { 
			IsOnProgress = false;
			OnResetEnvironment?.Invoke();
		};

		m_currentEnvironment.Reset(onFinished);

	}

	public void ClearEnvironment() {

		if (IsOnProgress)
			return;

		IsOnProgress = true;

		System.Action onFinished = () => {
			IsOnProgress = false;
			m_playgroundSelection.SetActive(true);
			OnDestroyEnvironment?.Invoke();
		};

		m_currentEnvironment.Clear(onFinished);

	}

}
