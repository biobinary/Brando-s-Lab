using System.Collections.Generic;

public class LiquidContainerInfoCard : ContainerInfoCard<ChemicalData> {

	protected override void SetupLabel() {		

		List<ChemicalPortion<ChemicalData>> chems = m_mainContainer.GetChemicalContents();
		ChemicalData chemicalData = chems?.Count > 0 ? chems[0].data : null;

		if (chemicalData != null && chemicalData.hasBeenExplained) {

			if (m_primaryLabel != null)
				m_primaryLabel.text = chemicalData.name;

			if (m_secondaryLabel != null)
				m_secondaryLabel.text = GetChemicalDescription(chemicalData);

		} else {

			if (m_primaryLabel != null)
				m_primaryLabel.text = "?";

			if (m_secondaryLabel != null)
				m_secondaryLabel.text = string.Empty;

		}

	}

	private string GetChemicalDescription(ChemicalData chemicalData) {

		if (chemicalData == null)
			return "Unknown";

		string chemicalType = chemicalData.type switch {
			ChemicalData.Type.ACID => "Asam",
			ChemicalData.Type.BASE => "Basa",
			ChemicalData.Type.NEUTRAL => "Netral",
			_ => "Unknown"
		};

		if ( chemicalType == "Netral" || chemicalType == "Unknown" )
			return chemicalType;

		string strengthType = chemicalData.strength switch {
			ChemicalData.Strength.STRONG => "Kuat",
			ChemicalData.Strength.WEAK => "Lemah",
			_ => ""
		};

		return $"{chemicalType} {strengthType}";

	}

}
