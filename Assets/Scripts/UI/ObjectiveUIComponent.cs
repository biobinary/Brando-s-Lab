using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ObjectiveUIComponent : MonoBehaviour {

	public PlaygroundObjective.ObjectiveInstruction instruction;

	[Header("Component References")]
	[SerializeField] private TextMeshProUGUI m_instructionLabel;
	[SerializeField] private Toggle m_checkBox;

	[Header("Button Helper")]
	[SerializeField] private Toggle m_toggleHelper;

	private void Awake() {

		if ( m_toggleHelper == null )
			return;

		m_toggleHelper.onValueChanged.AddListener((value) => 
			NPCProfessor.Instance.PlayMonologue(instruction.objectiveAudioHint));

	}

	private void Start() {
		m_instructionLabel.text = instruction.instructions;
		m_checkBox.isOn = instruction.isComplete;
	}

	public void SetNewInstruction(PlaygroundObjective.ObjectiveInstruction instruction) {
		this.instruction = instruction;
		m_instructionLabel.text = this.instruction.instructions;
		m_checkBox.isOn = this.instruction.isComplete;
	}

	private void Update() {
		if (instruction != null) {
			m_checkBox.isOn = instruction.isComplete;
		}
	}

}
