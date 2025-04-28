using System.Collections.Generic;

public class MetalSaltInfoCard : ContainerInfoCard<MetalSaltData> {
	
	protected override void SetupLabel() {
		
		List<ChemicalPortion<MetalSaltData>> salts = m_mainContainer.GetChemicalContents();
		MetalSaltData salt = salts?.Count > 0 ? salts[0].data : null;

		if (salt != null && salt.hasBeenExplained) {

			if (m_primaryLabel != null)
				m_primaryLabel.text = salt.formula;

			if( m_secondaryLabel != null )
				m_secondaryLabel.text = salt.GetColorName();

		} else {

			if (m_primaryLabel != null)
				m_primaryLabel.text = "?";

			if (m_secondaryLabel != null)
				m_secondaryLabel.text = string.Empty;

		}

	}

}
