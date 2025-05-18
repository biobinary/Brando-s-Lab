using System.Collections;
using UnityEngine;

public class ObjectiveCompletionManager : MonoBehaviour {

	[SerializeField] private float m_completionDelayDuration = 0.5f;

	public static ObjectiveCompletionManager Instance;

	private PlaygroundObjective m_currentObjectives = null;

	private void Awake() {

		if (Instance == null) {
			Instance = this;
		
		} else if (Instance != null && Instance != this) {
			Destroy(gameObject);
			
		}

	}

	private void Start() {
		PlaygroundEnvironmentManager.Instance.OnLoadEnvironment += OnEnvironmentLoaded;
	}

	private void OnEnvironmentLoaded(PlaygroundEnvironment env) {

		m_currentObjectives = null;

		if (env == null)
			return;

		PlaygroundObjective objective = env.GetObjective();
		if (objective == null)
			return;

		m_currentObjectives = objective;

	}

	public void SetObjectiveCompletion(string instruction) {
		if( m_currentObjectives == null) return;
		StartCoroutine(CompletionDelay(instruction));
	}

	private IEnumerator CompletionDelay(string instruction) {
		yield return new WaitForSeconds(m_completionDelayDuration);
		if( m_currentObjectives.HasInstruction(instruction)) {
			if(!m_currentObjectives.HasCompleted(instruction)) {
				m_currentObjectives.SetCompletion(instruction);
			}
		}
	}

}
