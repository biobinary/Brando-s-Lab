using System.Collections.Generic;
using UnityEngine;

public abstract class PourInteraction<T> : MonoBehaviour where T : ChemicalBaseData {

	[SerializeField] protected MonoBehaviour m_containerMonoBehaviour;
	[SerializeField] protected float m_pourRadius = 1.0f;
	[SerializeField] protected float m_pourRate = 10.0f;
	[SerializeField] protected ParticleSystem m_pourParticles;
	[SerializeField] protected LayerMask m_pourLayerMask;

	protected IChemicalContainer<T> m_container;

	protected float m_pourParticlesMinRadius = 0.0f;
	protected float m_pourParticlesMaxRadius = 0.0f;

	protected float m_tiltStrength = 0.0f;
	protected Vector3 m_newPosition = Vector3.zero;

	protected const float MAX_POUR_DISTANCE = 5.0f;
	protected const float ANGLE_THRESHOLD = 70.0f;

	private void Awake() {

		if (m_containerMonoBehaviour != null) { 
			if( m_containerMonoBehaviour is IChemicalContainer<T> )
				m_container = m_containerMonoBehaviour as IChemicalContainer<T>;
		}

		if (m_pourParticles != null) {
			m_pourParticlesMaxRadius = m_pourParticles.shape.angle;
			m_pourParticlesMinRadius = m_pourParticles.shape.radius;
		}

	}

	protected virtual bool ShouldPour() {
		return Vector3.Dot(transform.up, Vector3.down) > 0.0f && m_container.GetCurrentVolume() > 0.0f;
	}

	private void Update() {

		if (m_pourParticles == null)
			return;

		if (ShouldPour()) {

			if (!m_pourParticles.isPlaying)
				m_pourParticles.Play();

			Vector3 perpendicularVector = Vector3.Cross(transform.up, Vector3.down);
			Vector3 forwardPerpendicular = -Vector3.Cross(transform.up, perpendicularVector.normalized);

			float tiltStrength = perpendicularVector.magnitude;
			float clampedStrength = Mathf.Lerp(0.0f, m_pourRadius, tiltStrength);

			Vector3 newPosition = transform.InverseTransformDirection(forwardPerpendicular).normalized * clampedStrength;
			newPosition = transform.TransformPoint(newPosition);

			m_tiltStrength = tiltStrength;
			m_newPosition = newPosition;

			HandlePouring();

		} else if (m_pourParticles.isPlaying) {
			m_pourParticles.Stop();

		}

	}

	protected void FindPourable(Vector3 originPos, float originRadius, List<ChemicalPortion<T>> chemicals) {

		if (Physics.SphereCast(
			originPos, originRadius,
			Vector3.down, out RaycastHit hit,
			MAX_POUR_DISTANCE, m_pourLayerMask,
			QueryTriggerInteraction.Collide)) {

			GameObject foundGameObject = hit.transform.gameObject;

			if (foundGameObject.TryGetComponent(out IPourable<T> foundPourableObject)) {
				PourObject(hit, foundPourableObject, chemicals);
			
			}

		}

	}

	protected abstract void HandlePouring();
	protected abstract void PourObject(RaycastHit hit, IPourable<T> pourable, List<ChemicalPortion<T>> chemicals);

	private void OnDrawGizmos() {

		if (m_pourRadius <= 0.0f)
			return;

		Gizmos.color = Color.yellow;

		Vector3 center = transform.position;
		float segments = 20;
		float angleStep = (2.0f * Mathf.PI) / segments;

		Vector3 prevPoint = center + new Vector3(m_pourRadius, 0, 0);

		for (int i = 1; i <= segments; i++) {

			float angle = i * angleStep;
			Vector3 newPoint = center + new Vector3(
				Mathf.Cos(angle) * m_pourRadius,
				0,
				Mathf.Sin(angle) * m_pourRadius
			);

			Gizmos.DrawLine(prevPoint, newPoint);

			prevPoint = newPoint;

		}

	}

}
