using Oculus.Interaction;
using UnityEngine;

public class GasLighter : TriggerBasedTool {

	[Header("Fire Settings")]
	[SerializeField] private ParticleSystem m_fireEffect;
	[SerializeField] private float m_maxFireLength = 0.8f;
	[SerializeField] private float m_fireRadius = 0.008f;

	[Header("Trigger Visual")]
	[SerializeField] private float m_triggerInitalPosition = 0.0202947371f;
	[SerializeField] private float m_triggerPressedPosition = 0.0075f;
	[SerializeField] private GameObject m_triggerGameObject;

	[Header("SFX")]
	[SerializeField] private AudioSource m_fireSoundEffect;

	private bool m_isCastFire = false;
	private IBurnable m_currentBurnable= null;

	protected override void OnHandleTriggerPressed() {

		m_triggerGameObject.transform.localPosition = new Vector3(
			m_triggerGameObject.transform.localPosition.x,
			m_triggerGameObject.transform.localPosition.y,
			m_triggerPressedPosition
		);

		m_fireEffect.Play();
		m_fireSoundEffect.Play();

		m_isCastFire = true;

	}

	protected override void Update() {
		
		base.Update();
		
		if( m_isCastFire )
			CastFire();

	}

	protected override void OnHandleTriggerReleased() {

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
		m_fireSoundEffect.Stop();

		m_isCastFire = false;

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

			Vector3 point1 = center +
							 m_fireEffect.transform.right * Mathf.Cos(angle1) * radius +
							 m_fireEffect.transform.up * Mathf.Sin(angle1) * radius;

			Vector3 point2 = center +
							 m_fireEffect.transform.right * Mathf.Cos(angle2) * radius +
							 m_fireEffect.transform.up * Mathf.Sin(angle2) * radius;

			Gizmos.DrawLine(point1, point2);
		
		}

		Vector3 start = m_fireEffect.transform.position;
		Vector3 end = start + m_fireEffect.transform.forward * m_maxFireLength;

		Gizmos.color = Color.red;
		Gizmos.DrawLine(start, end);

	}

}
