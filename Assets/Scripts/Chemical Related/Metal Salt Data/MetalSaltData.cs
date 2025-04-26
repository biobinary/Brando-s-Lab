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

	public string GetColorName() {

		float hue;
		Color.RGBToHSV(flameColor, out hue, out _, out _);

		hue *= 360f;

		if (hue >= 0 && hue < 9)
			return "Merah";

		else if (hue >= 9 && hue < 32)
			return "Oren";
		
		else if (hue >= 32 && hue < 80)
			return "Kuning";
		
		else if (hue >= 80 && hue < 160)
			return "Hijau";
		
		else if (hue >= 160 && hue < 200)
			return "Cyan";
		
		else if (hue >= 200 && hue < 250)
			return "Biru";
		
		else if (hue >= 250 && hue < 290)
			return "Ungu";
		
		else if (hue >= 290 && hue < 330)
			return "Magenta";
		
		else if (hue >= 330 && hue <= 360)
			return "Merah";

		return "Tidak Diketahui";

	}

}
