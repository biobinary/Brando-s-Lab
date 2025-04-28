using System.Collections.Generic;
using UnityEngine;

public interface IChemicalContainer<T> where T : ChemicalBaseData {

	public void AddChemical(ChemicalPortion<T> chemical);
	public List<ChemicalPortion<T>> RemoveChemical(float volume);
	public List<ChemicalPortion<T>> GetChemicalContents();
	public float GetCurrentVolume();

}