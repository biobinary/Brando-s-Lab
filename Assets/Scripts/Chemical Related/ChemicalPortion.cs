using UnityEngine;

[System.Serializable]
public class ChemicalPortion<T> where T : ScriptableObject {

	public T data;
	public float volume; // In mL

}
