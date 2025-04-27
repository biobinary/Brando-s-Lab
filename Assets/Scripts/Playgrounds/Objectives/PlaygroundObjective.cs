using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PlaygroundObjective", menuName = "Scriptable Objects/Playground Objective")]
public class PlaygroundObjective : ScriptableObject {

	public event System.Action OnObjectiveCompletionUpdated;

	[System.Serializable]
	public class ObjectiveInstruction {

		public string instructions;
		public AudioClip objectiveAudioHint;
		public bool isComplete = false;

	}

	[SerializeField] private List<ObjectiveInstruction> m_instructionList;

	public List<ObjectiveInstruction> GetObjectives() => m_instructionList;

	private void OnEnable() {
		
		foreach (ObjectiveInstruction instruction in m_instructionList) {
			instruction.isComplete = false;
		}

	}

	public void SetCompletion(string instruction) {

		if (string.IsNullOrEmpty(instruction)) return;

		ObjectiveInstruction currentObjective = null;

		foreach (ObjectiveInstruction objective in m_instructionList) { 
			if( string.Equals(objective.instructions, instruction)) {
				currentObjective = objective;
				break;
			}
		}

		if( currentObjective != null ) {
			currentObjective.isComplete = true;
			OnObjectiveCompletionUpdated.Invoke();
		}
			
	}

	public void SetCompletion(int idx) {

		if (idx < m_instructionList.Count || idx >= m_instructionList.Count)
			return;

		ObjectiveInstruction currentObjective = m_instructionList[idx];

		if (currentObjective != null) {
			currentObjective.isComplete = true;
			OnObjectiveCompletionUpdated.Invoke();
		}

	}

	public AudioClip GetAudioHint(string instruction) {
		
		if (string.IsNullOrEmpty(instruction)) 
			return null;

		ObjectiveInstruction currentObjective = null;

		foreach (ObjectiveInstruction objective in m_instructionList) {
			if (string.Equals(objective.instructions, instruction)) {
				currentObjective = objective;
				break;
			}
		}

		if (currentObjective != null) {
			return currentObjective.objectiveAudioHint;	
		}

		return null;

	}

	public AudioClip GetAudioHint(int idx) {
		
		if (idx < m_instructionList.Count || idx >= m_instructionList.Count)
			return null;

		ObjectiveInstruction currentObjective = m_instructionList[idx];

		if (currentObjective != null) {
			return currentObjective.objectiveAudioHint;
		}

		return null;

	}

}
