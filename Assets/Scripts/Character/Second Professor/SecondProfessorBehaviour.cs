using UnityEngine;
using UnityEngine.Animations.Rigging;

public class SecondProfessorBehaviour : MonoBehaviour {

	[Header("Tracking Settings")]
	[SerializeField] private float m_lookAtPlayerSpeed = 0.3f;
	[SerializeField] private Rig m_headTrackingRig;
	[SerializeField] private Transform m_customHeadTrackingTarget;

	private Transform m_playerTransform;
	private float m_lookAtVelocity = 0.0f;

	private void OnTriggerEnter(Collider other) {

		if (!other.CompareTag("Player"))
			return;

		m_playerTransform = other.gameObject.transform;

	}

	private void OnTriggerExit(Collider other) {

		if (!other.CompareTag("Player"))
			return;

		m_playerTransform = null;

	}

	private void Update() {

		if (m_headTrackingRig == null)
			return;

		if (m_customHeadTrackingTarget == null)
			return;

		if( m_playerTransform == null ) {
			
			float newLookAtValue = Mathf.SmoothDamp(
				m_headTrackingRig.weight, 0.0f, ref m_lookAtVelocity, m_lookAtPlayerSpeed);
			
			m_headTrackingRig.weight = newLookAtValue;
		
		} else {

			m_customHeadTrackingTarget.position = m_playerTransform.position;

			float newLookAtValue = Mathf.SmoothDamp(
				m_headTrackingRig.weight, 1.0f, ref m_lookAtVelocity, m_lookAtPlayerSpeed);
			
			m_headTrackingRig.weight = newLookAtValue;

		}

	}

}
