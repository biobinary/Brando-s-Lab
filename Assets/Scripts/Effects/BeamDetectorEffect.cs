using System.Collections;
using UnityEngine;

public class BeamDetectorEffect : MonoBehaviour {

	[SerializeField] private Collider m_beamDetectorCollider;
	[SerializeField] private float m_deployTime = 0.2f;
	[SerializeField] private float m_retractTime = 0.2f;

	private Coroutine m_currentAdjustBeamCoroutine = null;

	private void Awake() {

		m_beamDetectorCollider.enabled = false;
		transform.localScale = new Vector3(
			transform.localScale.x,
			0.0f,
			0.0f
		);

	}

	public void ActivateBeam() {

		if (m_currentAdjustBeamCoroutine != null) {
			StopCoroutine(m_currentAdjustBeamCoroutine);
			m_currentAdjustBeamCoroutine = null;
		}

		m_beamDetectorCollider.enabled = true;
		m_currentAdjustBeamCoroutine = StartCoroutine(StartAdjustBeamLength(1.0f));

	}

	public void DisableBeam() {
		
		if (m_currentAdjustBeamCoroutine != null) {
			StopCoroutine(m_currentAdjustBeamCoroutine);
			m_currentAdjustBeamCoroutine = null;
		}

		m_beamDetectorCollider.enabled = false;
		m_currentAdjustBeamCoroutine = StartCoroutine(StartAdjustBeamLength(0.0f));

	}

	private IEnumerator StartAdjustBeamLength(float targetLength) {
		
		float currentScale = transform.localScale.z;
		float elapsedTime = 0.0f;

		while (elapsedTime < m_deployTime) {

			float newScale = Mathf.Lerp(currentScale, targetLength, elapsedTime / m_deployTime);
			transform.localScale = new Vector3(
				transform.localScale.x, 
				newScale, 
				newScale
			);

			elapsedTime += Time.deltaTime;
			yield return null;

		}

		transform.localScale = new Vector3(transform.localScale.x, targetLength, targetLength);
		m_currentAdjustBeamCoroutine = null;

	}


}
