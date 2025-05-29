using UnityEngine;
using BrandosLab.LabTools.Model;

namespace BrandosLab.Chemical {

	public abstract class ChemicalBaseData : ScriptableObject, IExplainableChemical {

		public bool hasBeenExplained { get; set; } = false;
		AudioClip IExplainableChemical.audioDescription => audioDescription;

		[Header("Chemical Identity")]
		public string chemicalName;
		public string formula;

		[Header("Description")]
		[TextArea(1, 10)]
		public string description;
		public AudioClip audioDescription;

		private void OnEnable() {
			hasBeenExplained = false;
		}

	}

}