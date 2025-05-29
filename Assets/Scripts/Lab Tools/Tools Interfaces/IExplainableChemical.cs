using UnityEngine;

namespace BrandosLab.LabTools.Model {

	public interface IExplainableChemical {

		public AudioClip audioDescription { get; }
		public bool hasBeenExplained { get; set; }

	}

}