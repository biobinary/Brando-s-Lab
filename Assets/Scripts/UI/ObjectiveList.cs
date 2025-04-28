using UnityEngine;

public class ObjectiveList : MonoBehaviour {

	[SerializeField] private GameObject m_objectiveUIComponent;

	private PlaygroundObjective m_currentObjective;

	private void Awake() {
		PlaygroundEnvironmentManager.Instance.OnLoadEnvironment += OnEnvironmentLoaded;
		PlaygroundEnvironmentManager.Instance.OnDestroyEnvironment += OnEnvironmentDestroyed;
	}

	private void OnEnvironmentLoaded(PlaygroundEnvironment env) {

		if (env == null)
			return;

		PlaygroundObjective objective = env.GetObjective();
		if( objective == null) 
			return;

		m_currentObjective = objective;
		m_currentObjective.OnObjectiveCompleted += OnObjectiveCompleteUpdated;

		foreach ( PlaygroundObjective.ObjectiveInstruction instruction in
				  m_currentObjective.GetObjectives()) {

			GameObject componentGameObject = Instantiate(m_objectiveUIComponent, transform);
			ObjectiveUIComponent uiComponent = componentGameObject.GetComponent<ObjectiveUIComponent>();
			
			uiComponent.SetInstructionLabel(instruction.instructions);
			uiComponent.SetCheckBox(instruction.isComplete);

		}

	}

	private void OnEnvironmentDestroyed() {

		if( m_currentObjective != null)
			m_currentObjective.OnObjectiveCompleted -= OnObjectiveCompleteUpdated;

		m_currentObjective = null;

		foreach (Transform child in transform) {
			Destroy(child.gameObject);
		}

	}

	private void OnObjectiveCompleteUpdated() {

		int objectiveCount = m_currentObjective.GetObjectives().Count;

		for(int i = 0; i < objectiveCount; i++) {
			ObjectiveUIComponent uiComponent = transform.GetChild(i).GetComponent<ObjectiveUIComponent>();
			if(m_currentObjective.HasCompleted(i))
				uiComponent.SetCheckBox(true);
		}

	}

}
