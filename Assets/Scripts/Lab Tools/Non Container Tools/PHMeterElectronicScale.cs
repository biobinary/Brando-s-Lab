using Oculus.Interaction;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class PHMeterElectronicScale : MonoBehaviour {

	[SerializeField] private TextMeshProUGUI m_phInfoCard;
	[SerializeField] private ParticleSystem m_rippleEffect;

	private IChemicalContainer<ChemicalData> _currentHighlightedContainer;
	private IChemicalContainer<ChemicalData> m_currentHighlightedContainer {
		
		get {
			return _currentHighlightedContainer;
		}

		set {

			_currentHighlightedContainer = value;
			
			if ( _currentHighlightedContainer == null ) {
				m_rippleEffect.gameObject.SetActive(true);
				m_rippleEffect.Play();
			
			} else {
				m_rippleEffect.gameObject.SetActive(false);
				m_rippleEffect.Stop();
			
			}

		}

	}

	private List<Rigidbody> m_containerCandidates = new();

	private void OnEnable() {
		m_rippleEffect.gameObject.SetActive(true);
		m_rippleEffect.Play();
	}

	private void OnDisable() {
		m_currentHighlightedContainer = null;
		m_containerCandidates.Clear();
	}

	private void OnTriggerEnter(Collider other) {

		var rb = other.attachedRigidbody;
		if (rb == null) return;

		if (!rb.TryGetComponent(out IChemicalContainer<ChemicalData> container)) return;

		var grabInteractable = rb.GetComponentInChildren<GrabInteractable>();
		if (grabInteractable == null) return;

		if (!m_containerCandidates.Contains(rb))
			m_containerCandidates.Add(rb);

		if (!grabInteractable.SelectingInteractorViews.Any())
			m_currentHighlightedContainer = container;

	}

	private void OnTriggerStay(Collider other) {

		var rb = other.attachedRigidbody;
		if (rb == null || !m_containerCandidates.Contains(rb)) return;

		var container = rb.GetComponent<IChemicalContainer<ChemicalData>>();
		var grabInteractable = rb.GetComponentInChildren<GrabInteractable>();

		if (grabInteractable.SelectingInteractorViews.Any()) {
		
			if (m_currentHighlightedContainer == container)
				m_currentHighlightedContainer = null;
		
		} else {
			
			if (m_currentHighlightedContainer == null)
				m_currentHighlightedContainer = container;

		}

	}

	private void OnTriggerExit(Collider other) {

		var rb = other.attachedRigidbody;
		if (rb == null || !m_containerCandidates.Contains(rb)) return;

		var container = rb.GetComponent<IChemicalContainer<ChemicalData>>();
		if (m_currentHighlightedContainer == container)
			m_currentHighlightedContainer = null;

		m_containerCandidates.Remove(rb);

	}

	private void Update() {

		if (m_currentHighlightedContainer != null) {

			List<ChemicalPortion<ChemicalData>> contents = m_currentHighlightedContainer.GetChemicalContents();
			
			float currentVolume = m_currentHighlightedContainer.GetCurrentVolume();
			float containerPH = PHManager.CalCulatePH(contents, currentVolume);

			string formattedStr = containerPH.ToString("F3");
			m_phInfoCard.text = $"PH: {formattedStr}";
		
		} else {
			m_phInfoCard.text = "Tidak Ada Objek";

		}



	}

}
