using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PHMeterGun : TriggerBasedTool {

	[SerializeField] private TextMeshProUGUI m_phInfoCard;
	[SerializeField] private GameObject m_beamDetector;

	private IChemicalContainer<ChemicalData> m_currentContainer;

	private void Awake() {
		m_beamDetector.SetActive(false);
	}

	protected override void OnHandleTriggerPressed() {
		m_beamDetector.SetActive(true);
	}

	protected override void OnHandleTriggerReleased() {
		m_beamDetector.SetActive(false);
		m_currentContainer = null;
	}

	private void OnTriggerEnter(Collider other) {
		
		if( TryGetContainer(other, out IChemicalContainer<ChemicalData> container) ) {
			
			if (m_currentContainer == null)
				m_currentContainer = container;

		}

	}

	private void OnTriggerExit(Collider other) {
		if (TryGetContainer(other, out IChemicalContainer<ChemicalData> container)) {
			if (m_currentContainer == container)
				m_currentContainer = null;
		}
	}

	private bool TryGetContainer(
		Collider other, 
		out IChemicalContainer<ChemicalData> container) {

		container = null;

		Rigidbody currentObjectRB = other.attachedRigidbody;
		if (currentObjectRB == null)
			return false;

		if (currentObjectRB.TryGetComponent(out container))
			return true;

		return false;
	
	}

	protected override void Update() {
		
		base.Update();

		if (m_currentContainer != null) {

			List<ChemicalPortion<ChemicalData>> contents = m_currentContainer.GetChemicalContents();
			float currentVolume = m_currentContainer.GetCurrentVolume();
			float containerPH = PHManager.CalCulatePH(contents, currentVolume);

			if( m_phInfoCard != null ) {
				string formattedStr = containerPH.ToString("F3");
				m_phInfoCard.text = formattedStr;
			}

		} else {

			if (m_phInfoCard != null)
				m_phInfoCard.text = "No object detected";

		}

	}

}
