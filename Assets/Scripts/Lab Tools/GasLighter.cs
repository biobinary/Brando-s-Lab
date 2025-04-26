using Oculus.Interaction;
using Oculus.Interaction.Input;
using UnityEngine;

public class GasLighter : MonoBehaviour {

	[SerializeField] private GrabInteractable m_mainGrabbable;

	[Header("Fire Settings")]
	[SerializeField] private ParticleSystem m_fireEffect;
	[SerializeField] private float m_maxFireLength = 0.8f;
	[SerializeField] private float m_fireRadius = 0.008f;

	[Header("Trigger Visual")]
	[SerializeField] private float m_triggerInitalPosition = 0.0202947371f;
	[SerializeField] private float m_triggerPressedPosition = 0.0075f;
	[SerializeField] private GameObject m_triggerGameObject;

	ControllerRef m_currentControllerRef = null;
	ParticleSystem.ShapeModule m_shapeModule;

	private IBurnable m_currentBurnable= null;

	private void Awake() {
		m_shapeModule = m_fireEffect.shape;
	}

	private void OnEnable() {
		m_mainGrabbable.WhenSelectingInteractorViewAdded += OnHandleSelectedByInteractor;
		m_mainGrabbable.WhenSelectingInteractorViewRemoved += OnHandleUnselectedByInteractor;

	}

	private void OnDisable() {
		m_currentControllerRef = null;
		m_mainGrabbable.WhenSelectingInteractorViewAdded -= OnHandleSelectedByInteractor;
		m_mainGrabbable.WhenSelectingInteractorViewRemoved -= OnHandleUnselectedByInteractor;
	
	}

	private void OnHandleSelectedByInteractor(IInteractorView interactor) {
		
		object interactorData = interactor.Data;
		GrabInteractor grabInteractor = interactorData as GrabInteractor;
		ControllerRef controllerRef = grabInteractor.GetComponent<ControllerRef>();

		if( controllerRef != null )
			m_currentControllerRef = controllerRef;

	}

	private void OnHandleUnselectedByInteractor(IInteractorView interactor) {
		m_currentControllerRef = null;
	}

	private void Update() {
		
		if (m_currentControllerRef != null) {

			if (m_currentControllerRef.ControllerInput.TriggerButton) {

				m_triggerGameObject.transform.localPosition = new Vector3(
					m_triggerGameObject.transform.localPosition.x,
					m_triggerGameObject.transform.localPosition.y,
					m_triggerPressedPosition
				);

				m_fireEffect.Play();
				CastFire();
				return;

			}

		}

		if (m_currentBurnable != null) {
			m_currentBurnable.StopBurning();
			m_currentBurnable = null;

		}

		m_triggerGameObject.transform.localPosition = new Vector3(
			m_triggerGameObject.transform.localPosition.x,
			m_triggerGameObject.transform.localPosition.y,
			m_triggerInitalPosition
		);

		m_fireEffect.Stop();

	}

	private void CastFire() {

		if (Physics.SphereCast(
				m_fireEffect.transform.position, 
				m_fireRadius, 
				m_fireEffect.transform.forward,
				out RaycastHit hit, m_maxFireLength)) {

			IBurnable burnable = hit.transform.GetComponent<IBurnable>();

			if (burnable != null && burnable != m_currentBurnable) {

				if (m_currentBurnable != null) {
					m_currentBurnable.StopBurning();
				}

				m_currentBurnable = burnable;

				if (!burnable.IsBurning)
					burnable.StartBurning();

			} else if (burnable != null && burnable == m_currentBurnable) {

				if (!burnable.IsBurning)
					burnable.StartBurning();

			} else if (burnable == null && m_currentBurnable != null) { 
				
				m_currentBurnable.StopBurning();
				m_currentBurnable = null;

			}

		}

	}

	private void OnDrawGizmos() {

		Gizmos.color = Color.cyan;

		float radius = m_fireRadius;
		int segments = 20;

		Vector3 center = m_fireEffect.transform.position;
		float angleStep = 360f / segments;

		for (int i = 0; i < segments; i++) {
			float angle1 = Mathf.Deg2Rad * angleStep * i;
			float angle2 = Mathf.Deg2Rad * angleStep * (i + 1);

			// Karena mengelilingi sumbu Z, kita gambar lingkaran di bidang XY
			Vector3 point1 = center +
							 m_fireEffect.transform.right * Mathf.Cos(angle1) * radius +
							 m_fireEffect.transform.up * Mathf.Sin(angle1) * radius;

			Vector3 point2 = center +
							 m_fireEffect.transform.right * Mathf.Cos(angle2) * radius +
							 m_fireEffect.transform.up * Mathf.Sin(angle2) * radius;

			Gizmos.DrawLine(point1, point2);
		
		}

		// Titik awal di posisi objek	
		Vector3 start = m_fireEffect.transform.position;

		// Titik akhir di sepanjang sumbu X lokal sepanjang "length"
		Vector3 end = start + m_fireEffect.transform.forward * m_maxFireLength;

		// Warna garis (optional)
		Gizmos.color = Color.red;

		// Gambar garis
		Gizmos.DrawLine(start, end);

	}

}
