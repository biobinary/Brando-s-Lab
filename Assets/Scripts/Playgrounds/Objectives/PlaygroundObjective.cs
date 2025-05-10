using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PlaygroundObjective", menuName = "Scriptable Objects/Playground Objective")]
public class PlaygroundObjective : ScriptableObject {

	public event System.Action OnObjectiveCompleted;

	[System.Serializable]
	public class ObjectiveInstruction {

		public string instructions;
		public AudioClip objectiveAudioHint;
		public bool isComplete = false;

	}

	[SerializeField] private string m_objectiveIntroductionMonologueName;
	[SerializeField] private List<ObjectiveInstruction> m_instructionList;

	public string GetIntroductionMonologueName() => m_objectiveIntroductionMonologueName;
	public IReadOnlyList<ObjectiveInstruction> GetObjectives() => m_instructionList;

	public void ResetAllObjectives() {

		if (m_instructionList == null || !(m_instructionList.Count > 0))
			return;

		foreach (ObjectiveInstruction instruction in m_instructionList) {
			instruction.isComplete = false;
		}

	}

	public void SetCompletion(string instruction) {

		if (string.IsNullOrEmpty(instruction))
			return;

		if (HasCompleted(instruction))
			return;

		ObjectiveInstruction currentObjective = null;

		foreach (ObjectiveInstruction objective in m_instructionList) { 
			if( string.Equals(objective.instructions, instruction)) {
				currentObjective = objective;
				break;
			}
		}

		if( currentObjective != null ) {
			currentObjective.isComplete = true;
			OnObjectiveCompleted.Invoke();
		}
			
	}

	public void SetCompletion(int idx) {

		if (idx < 0 || idx >= m_instructionList.Count)
			return;

		if (HasCompleted(idx))
			return;

		ObjectiveInstruction currentObjective = m_instructionList[idx];

		if (currentObjective != null) {
			currentObjective.isComplete = true;
			OnObjectiveCompleted.Invoke();
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
		
		if (idx < 0 || idx >= m_instructionList.Count)
			return null;

		ObjectiveInstruction currentObjective = m_instructionList[idx];

		if (currentObjective != null) {
			return currentObjective.objectiveAudioHint;
		}

		return null;

	}

	public bool HasCompleted(string instruction) {

		if (string.IsNullOrEmpty(instruction))
			return false;

		ObjectiveInstruction currentObjective = null;

		foreach (ObjectiveInstruction objective in m_instructionList) {
			if (string.Equals(objective.instructions, instruction)) {
				currentObjective = objective;
				break;
			}
		}

		if (currentObjective != null)
			return currentObjective.isComplete;

		return false;

	}

	public bool HasCompleted(int idx) {

		if (idx < 0 || idx >= m_instructionList.Count)
			return false;

		ObjectiveInstruction currentObjective = m_instructionList[idx];

		if (currentObjective != null)
			return currentObjective.isComplete;

		return false;

	}

}
