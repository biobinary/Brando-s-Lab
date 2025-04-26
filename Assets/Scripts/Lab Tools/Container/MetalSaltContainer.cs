using System.Collections.Generic;
using UnityEngine;

public class MetalSaltContainer : MonoBehaviour, IChemicalContainer<MetalSaltData> {

	[Header("Container Settings")]
	[SerializeField] protected ChemicalPortion<MetalSaltData> m_initialSalt;

	protected List<ChemicalPortion<MetalSaltData>> m_salts = new();

	protected virtual void OnEnable() {
		AddChemical(m_initialSalt);
	
	}

	protected virtual void OnDisable() {
		m_salts.Clear();

	}

	public virtual void AddChemical(ChemicalPortion<MetalSaltData> chemical) {
		m_salts.Add(chemical);
	}

	public List<ChemicalPortion<MetalSaltData>> GetChemicalContents() {
		return m_salts;
	}

	public float GetCurrentVolume() {
		
		float sumCurrentVolume = 0;

		foreach (ChemicalPortion<MetalSaltData> salt in m_salts) {
			sumCurrentVolume += salt.volume;
		}

		return sumCurrentVolume;

	}

	public virtual List<ChemicalPortion<MetalSaltData>> RemoveChemical(float volume) {

		List<ChemicalPortion<MetalSaltData>> removedSalts = new();
        foreach (ChemicalPortion<MetalSaltData> salt in m_salts) {
			ChemicalPortion<MetalSaltData> newPortion = salt;
			newPortion.volume = volume;
			removedSalts.Add(newPortion);
        }

        return removedSalts;

	}
}
