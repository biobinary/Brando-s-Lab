using UnityEngine;

public class ObjectiveList : MonoBehaviour {

	[SerializeField] private GameObject m_objectiveUIComponent;

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

		PlaygroundObjective currentObjective = objective;

		foreach ( PlaygroundObjective.ObjectiveInstruction instruction in
				  currentObjective.GetObjectives()) {

			GameObject componentGameObject = Instantiate(m_objectiveUIComponent, transform);
			ObjectiveUIComponent uiComponent = componentGameObject.GetComponent<ObjectiveUIComponent>();
			uiComponent.SetNewInstruction(instruction);

		}

	}

	private void OnEnvironmentDestroyed() {

		foreach (Transform child in transform) {
			Destroy(child.gameObject);
		}

	}

}
