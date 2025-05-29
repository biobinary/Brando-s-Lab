using UnityEngine;
using BrandosLab.Playgrounds;
using BrandosLab.Playgrounds.Objectives;

namespace BrandosLab.UI {

	public class ObjectiveList : MonoBehaviour {

		[SerializeField] private GameObject m_objectiveUIComponent;
		[SerializeField] private Transform m_desiredSpawnTransform;

		private Transform m_spawnTransform;

		private void Awake() {

			PlaygroundEnvironmentManager.Instance.OnLoadEnvironment += OnEnvironmentLoaded;
			PlaygroundEnvironmentManager.Instance.OnDestroyEnvironment += OnEnvironmentDestroyed;

			m_spawnTransform = m_desiredSpawnTransform != null ? m_desiredSpawnTransform : transform;

		}

		private void OnEnvironmentLoaded(PlaygroundEnvironment env) {

			if (env == null)
				return;

			PlaygroundObjective objective = env.GetObjective();
			if (objective == null)
				return;

			PlaygroundObjective currentObjective = objective;

			foreach (PlaygroundObjective.ObjectiveInstruction instruction in
					  currentObjective.GetObjectives()) {

				GameObject componentGameObject = Instantiate(m_objectiveUIComponent, m_spawnTransform);
				ObjectiveUIComponent uiComponent = componentGameObject.GetComponent<ObjectiveUIComponent>();
				uiComponent.SetNewInstruction(instruction);

			}

		}

		private void OnEnvironmentDestroyed() {

			foreach (Transform child in m_spawnTransform) {
				Destroy(child.gameObject);
			}

		}

	}

}