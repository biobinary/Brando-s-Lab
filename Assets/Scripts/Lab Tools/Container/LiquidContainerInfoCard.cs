using Oculus.Interaction;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LiquidContainerInfoCard : MonoBehaviour {

	[SerializeField, Interface(typeof(IPointable))]
	private UnityEngine.Object _pointable;
	private IPointable Pointable;

	[SerializeField] private LiquidContainer m_mainContainer;

	[Header("UI Settings")]
	[SerializeField] private GameObject m_canvasGameObject;
	[SerializeField] private TextMeshProUGUI m_primaryLabel;
	[SerializeField] private TextMeshProUGUI m_secondaryLabel;

	private Transform m_mainCameraTransform;
	private bool m_isPicked = false;

	private void Awake() {
		Pointable = _pointable as IPointable;
	}

	private void Start() {

		OVRCameraRig cameraRig = FindAnyObjectByType<OVRCameraRig>();
		if (cameraRig != null) {
			m_mainCameraTransform = cameraRig.centerEyeAnchor;
		}

		m_canvasGameObject.SetActive( false );

	}

	public void OnEnable() {

		if( Pointable != null )
			Pointable.WhenPointerEventRaised += OnHandlePointerEventRaised;

	}

	private void OnDisable() {

		if (Pointable != null)
			Pointable.WhenPointerEventRaised -= OnHandlePointerEventRaised;

	}

	private void OnHandlePointerEventRaised(PointerEvent pointerEvent) {

		switch(pointerEvent.Type) {

			case PointerEventType.Select:
				m_isPicked = true;
				m_canvasGameObject.SetActive(false);
				break;

			case PointerEventType.Unselect:
				m_isPicked = false;
				m_canvasGameObject.SetActive(true);
				ResetupLabel();
				break;

			case PointerEventType.Hover:
				
				if (!m_isPicked ) {
					m_canvasGameObject.SetActive(true);
					ResetupLabel();
				}

				break;

			case PointerEventType.Unhover:
				m_canvasGameObject.SetActive(false);
				break;

		}

	}

	private void ResetupLabel() {

		List<ChemicalPortion<ChemicalData>> chems = m_mainContainer.GetChemicalContents();
		ChemicalData chemicalData = chems?.Count > 0 ? chems[0].data : null;

		if (chemicalData != null && chemicalData.hasBeenExplained) {

			if (m_primaryLabel != null)
				m_primaryLabel.text = chemicalData.name;

			if (m_secondaryLabel != null)
				m_secondaryLabel.text = GetChemicalDescription(chemicalData);

		} else {

			if (m_primaryLabel != null)
				m_primaryLabel.text = "?";

			if (m_secondaryLabel != null)
				m_secondaryLabel.text = string.Empty;

		}

	}

	private string GetChemicalDescription(ChemicalData chemicalData) {

		if (chemicalData == null)
			return "Unknown";

		string chemicalType = chemicalData.type switch {
			ChemicalData.Type.ACID => "Asam",
			ChemicalData.Type.BASE => "Basa",
			ChemicalData.Type.NEUTRAL => "Neutral",
			_ => "Unknown"
		};

		if ( chemicalType == "Neutral" || chemicalType == "Unknown" )
			return chemicalType;

		string strengthType = chemicalData.strength switch {
			ChemicalData.Strength.STRONG => "Kuat",
			ChemicalData.Strength.WEAK => "Lemah",
			_ => ""
		};

		return $"{chemicalType} {strengthType}";

	}

	private void Update() {
		Vector3 direction = (m_mainCameraTransform.position - transform.position).normalized;
		transform.rotation = Quaternion.LookRotation(-direction);
	}

}
