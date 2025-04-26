using UnityEngine;

public class NPCProfessorVision : MonoBehaviour {

	[SerializeField] private float m_waitUntilExplainTime = 3.0f;

	private IExplainable m_selectedExplainableObject = null;
	private float m_currentObjectTimeout = 0.0f;

	private void OnTriggerEnter(Collider other) {
		
		if (!TryGetExplainable(other, out var explainable)) return;

		if (m_selectedExplainableObject == null && explainable.IsCanExplain()) {
			m_selectedExplainableObject = explainable;
		}

	}

	private void OnTriggerStay(Collider other) {

		if ( NPCProfessor.Instance.isSpeaking )
			return;

		if (!TryGetExplainable(other, out var explainable)) return;
		if (explainable != m_selectedExplainableObject) return;

		if (!m_selectedExplainableObject.IsCanExplain()) {
			ResetSelection();
			return;

		}

		m_currentObjectTimeout += Time.deltaTime;

		if (m_currentObjectTimeout > m_waitUntilExplainTime) {
			ExplainObject();
		}

	}

	private void OnTriggerExit(Collider other) {
		
		if (!TryGetExplainable(other, out var explainable)) return;

		if (explainable == m_selectedExplainableObject) {
			ResetSelection();
		}

	}

	private bool TryGetExplainable(Collider other, out IExplainable explainable) {
		
		explainable = null;
		if (!other.CompareTag("Tools")) 
			return false;

		if (other.gameObject.TryGetComponent(out explainable)) 
			return true;

		var rb = other.attachedRigidbody;
		if (rb == null) 
			return false;

		return rb.gameObject.TryGetComponent(out explainable);

	}

	private void ResetSelection() {
		m_selectedExplainableObject = null;
		m_currentObjectTimeout = 0.0f;
	}

	private void ExplainObject() {
		NPCProfessor.Instance.PlayMonologue(m_selectedExplainableObject.GetExplanationVoiceOverClip());
		ResetSelection();
	}

}
