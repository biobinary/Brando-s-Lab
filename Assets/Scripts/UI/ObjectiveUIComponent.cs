using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ObjectiveUIComponent : MonoBehaviour {

	[Header("Component References")]
	[SerializeField] private TextMeshProUGUI m_instructionLabel;
	[SerializeField] private Toggle m_checkBox;

	public void SetInstructionLabel(string instructionLabel) { 
		m_instructionLabel.text = instructionLabel;
	}

	public void SetCheckBox(bool isOn) { 
		m_checkBox.isOn = isOn;
	}

}
