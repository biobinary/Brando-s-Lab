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

		foreach( PlaygroundObjective.ObjectiveInstruction instruction in
				 m_currentObjective.GetObjectives()) {

			GameObject componentGameObject = Instantiate(m_objectiveUIComponent, transform);
			ObjectiveUIComponent uiComponent = componentGameObject.GetComponent<ObjectiveUIComponent>();
			uiComponent.SetInstructionLabel(instruction.instructions);

		}

	}

	private void OnEnvironmentDestroyed() {
		
		foreach (Transform child in transform) {
			Destroy(child.gameObject);
		}

	}

}
