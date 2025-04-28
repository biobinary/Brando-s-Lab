using UnityEngine;

[System.Serializable]
public class ChemicalPortion<T> where T : ChemicalBaseData {

	public T data;
	public float volume; // In mL

}
