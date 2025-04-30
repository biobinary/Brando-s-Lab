using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NeutralizationChecker : MonoBehaviour {

	[SerializeField] private float m_phTolerance = 0.25f;
	[SerializeField] private float m_confirmationCheckTimer = 0.25f;

	[Header("Objectives")]
	[SerializeField] private PlaygroundObjective m_currentObjectives;

	public bool confirmationDelayIsOnProgress { get; private set; } = false;
	private Coroutine m_delayCheckerCoroutine = null;

	[System.Serializable]
	public struct NeutralizationPair {

		public string objectiveMessage;
		public ChemicalData acid;
		public ChemicalData baseCompound;

		public bool Matches(ChemicalData a, ChemicalData b) {
			return (a == acid && b == baseCompound) || (a == baseCompound && b == acid);
		
		}

	}

	[Header("Valid Neutralization Pairs")]
	[SerializeField] private List<NeutralizationPair> m_validPairs;

	public void StartNeutralizationConfirmation(float currentPH, List<ChemicalPortion<ChemicalData>> contents) {

		bool isNeutral = Mathf.Abs(currentPH - 7.0f) <= m_phTolerance;
		if ( !isNeutral )
			return;

		ChemicalData acid = null;
		ChemicalData baseCompound = null;

		foreach (ChemicalPortion<ChemicalData> chem in contents) {

			switch (chem.data.type) {

				case ChemicalData.Type.ACID:

					if (acid != null)
						return;

					acid = chem.data;
					break;

				case ChemicalData.Type.BASE:

					if (baseCompound != null)
						return;

					baseCompound = chem.data;
					break;

			}

		}

		foreach (NeutralizationPair pair in m_validPairs) {
			if (pair.Matches(acid, baseCompound))
				m_delayCheckerCoroutine = StartCoroutine(StartConfirmationTimer(pair.objectiveMessage));
		}

		return;

	}

	public void StopConfirmationDelayTimeout() {
		
		if( m_delayCheckerCoroutine != null )
			StopCoroutine( m_delayCheckerCoroutine );

		m_delayCheckerCoroutine = null;
		confirmationDelayIsOnProgress = false;

	}

	private IEnumerator StartConfirmationTimer(string objectiveMessage) {
		confirmationDelayIsOnProgress = true;
		yield return new WaitForSeconds(m_confirmationCheckTimer);
		confirmationDelayIsOnProgress = false;
		m_currentObjectives.SetCompletion(objectiveMessage);
	}

}
