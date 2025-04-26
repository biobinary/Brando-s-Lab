using UnityEngine;

public interface IExplainableChemical {
	
	public AudioClip audioDescription { get; }
	public bool hasBeenExplained { get; set; }

}