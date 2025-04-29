using Oculus.Interaction;
using UnityEngine;

public class ObjectiveCompletedEffect : MonoBehaviour {

	[SerializeField] private ParticleSystem m_mainParticleSystem;
	[SerializeField] private float m_floorHeight = 0.0f;

	private Transform m_mainCameraTransform;
	private PlaygroundObjective m_objective = null;
	private void Awake() {
		PlaygroundEnvironmentManager.Instance.OnLoadEnvironment += OnHandleGetNewObjective;
	}

	private void Start() {
		
		OVRCameraRig cameraRig = FindAnyObjectByType<OVRCameraRig>();
		if (cameraRig != null) {
			m_mainCameraTransform = cameraRig.centerEyeAnchor;
		}

	}

	private void OnHandleGetNewObjective(PlaygroundEnvironment env) {

		if (m_objective != null) {
			m_objective.OnObjectiveCompleted -= OnSpawnNewEffect;
			m_objective = null;
		}

		if (env == null)
			return;

		m_objective = env.GetObjective();
		if( m_objective != null )
			m_objective.OnObjectiveCompleted += OnSpawnNewEffect;

	}

	private void OnSpawnNewEffect() {

		ParticleSystem particleSystem = Instantiate(m_mainParticleSystem);
		
		Vector3 targetPosition = m_mainCameraTransform.position;
		targetPosition.y = m_floorHeight;
		particleSystem.transform.position = targetPosition;

		particleSystem.Play();

	}

}
