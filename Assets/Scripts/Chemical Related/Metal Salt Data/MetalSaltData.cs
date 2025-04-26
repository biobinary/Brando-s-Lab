using UnityEngine;

[CreateAssetMenu(fileName = "MetalSaltData", menuName = "Scriptable Objects/Metal Salt Data")]
public class MetalSaltData : ScriptableObject, IExplainableChemical {

	public bool hasBeenExplained { get; set; } = false;
	AudioClip IExplainableChemical.audioDescription { get => audioDescription; }

	[Header("Chemical Identity")]
	public string metalSaltName;
	public string chemicalFormula;

	[Header("Flame‑Test Data")]
	public Color flameColor = Color.white;

	[Header("Description")]
	[TextArea(1, 10)] public string description;
	public AudioClip audioDescription;

}
