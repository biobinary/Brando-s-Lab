using UnityEngine;

[CreateAssetMenu(fileName = "ChemicalData", menuName = "Scriptable Objects/Chemical Data")]
public class ChemicalData : ChemicalBaseData {

	public enum Type {
        ACID, 
        BASE, 
        NEUTRAL
    }

    public enum Strength {
        WEAK, 
        STRONG,
        NONE
    }

	[Header("Chemical Properties")]
	public Type type;
	public Strength strength;
    [Range(0f, 1f)] public float concentration = 1.0f;
	[Tooltip("Molar mass in g/mol")] public float molarMass;

	[Header("Dissociation Constants")]
	[Tooltip("Ka value for acids (e.g. 1.8e-5 for acetic acid)")]
	public float Ka = 0f;
	[Tooltip("Kb value for bases (e.g. 1.8e-5 for ammonia)")]
	public float Kb = 0f;

	private void OnEnable() {
		hasBeenExplained = false;
	}

	public float GetDissociationConstant() {
		
		switch (type) {
			case Type.ACID:
				return Ka;
			case Type.BASE:
				return Kb;
			default:
				return 0f;
		}

	}

	public float GetDissociationFactor(float dilutionFactor = 1f) {

		switch (strength) {
			
			case Strength.STRONG:
				return 1.0f;

			case Strength.WEAK:

				if (type == Type.ACID && Ka > 0f) {
					
					float effectiveConcentration = concentration * dilutionFactor;
					float alpha = Mathf.Sqrt(Ka / Mathf.Max(effectiveConcentration, 1e-6f));
					return Mathf.Min(alpha, 1.0f);
				
				} else if (type == Type.BASE && Kb > 0f) {

					float effectiveConcentration = concentration * dilutionFactor;
					float alpha = Mathf.Sqrt(Kb / Mathf.Max(effectiveConcentration, 1e-6f));
					return Mathf.Min(alpha, 1.0f);
				
				} else {
					return 0.1f;
				}

			case Strength.NONE:
			default:
				return 0.0f;
		
		}

	}

}

[System.Serializable]
public struct PHColorSet {

	public Color main;
	public Color top;
	public Color fresnel;

	public PHColorSet(Color main, Color top, Color fresnel) {
		this.main = main;
		this.top = top;
		this.fresnel = fresnel;
	}

	public static PHColorSet Lerp(PHColorSet from, PHColorSet to, float t) {

		return new PHColorSet(
			Color.Lerp(from.main, to.main, t),
			Color.Lerp(from.top, to.top, t),
			Color.Lerp(from.fresnel, to.fresnel, t));

	}

}
