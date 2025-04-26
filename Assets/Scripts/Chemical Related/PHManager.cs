using System.Collections.Generic;
using UnityEngine;

public static class PHManager {

	private const float WATER_CONSTANT = 1e-14f;

	public static float CalCulatePH(List<ChemicalPortion<ChemicalData>> chemicals, float currentVolume) {

		float totalMolesH = 0f;
		float totalMolesOH = 0f;

		float totalVolumeL = currentVolume / 1000f;

		foreach (ChemicalPortion<ChemicalData> chem in chemicals) {

			float volumeL = chem.volume / 1000.0f;

			float dilutionFactor = volumeL / totalVolumeL;

			float molarity = chem.data.concentration;
			float moles = molarity * volumeL;

			if (chem.data.type == ChemicalData.Type.ACID) {
				float dissociationFactor = chem.data.GetDissociationFactor(dilutionFactor);
				totalMolesH += moles * dissociationFactor;

			} else if (chem.data.type == ChemicalData.Type.BASE) {
				float dissociationFactor = chem.data.GetDissociationFactor(dilutionFactor);
				totalMolesOH += moles * dissociationFactor;

			}

		}

		float hConcentration = totalMolesH / totalVolumeL;
		float ohConcentration = totalMolesOH / totalVolumeL;

		if (hConcentration > 0 && ohConcentration > 0) {

			if (hConcentration > ohConcentration) {
				hConcentration -= ohConcentration;
				ohConcentration = 0;

			} else if (ohConcentration > hConcentration) {
				ohConcentration -= hConcentration;
				hConcentration = 0;

			} else {
				hConcentration = 0;
				ohConcentration = 0;

			}

		}

		// Hitung [H⁺] dan [OH⁻] akhir berdasarkan Kw
		if (hConcentration > 0) {
			// [H⁺] = sisa H⁺ setelah netralisasi, [OH⁻] = Kw / [H⁺]
			ohConcentration = WATER_CONSTANT / hConcentration;

		} else if (ohConcentration > 0) {
			// [OH⁻] = sisa OH⁻ setelah netralisasi, [H⁺] = Kw / [OH⁻]
			hConcentration = WATER_CONSTANT / ohConcentration;

		} else {
			// Larutan netral (H⁺ = OH⁻ = √Kw)
			hConcentration = Mathf.Sqrt(WATER_CONSTANT);
			ohConcentration = hConcentration;

		}

		hConcentration = Mathf.Max(hConcentration, 1e-14f);

		float targetPH = -Mathf.Log10(hConcentration);
		return Mathf.Clamp(targetPH, 0f, 14f);

	}

	public static PHColorSet GetPHColor(float ph) {

		PHColorSet targetColor = new();

		if (ph <= 2.0f) {
			targetColor.main = Color.red;
			targetColor.top = new Color(1.0f, 0.2f, 0.2f);
			targetColor.fresnel = new Color(1.0f, 0.2f, 0.2f);

		} else if (ph <= 4.0f) {
			float t = (ph - 2.0f) / 2.0f;

			targetColor.main = Color.Lerp(
				new Color(0.8f, 0.0f, 0.0f),
				new Color(0.9f, 0.4f, 0.0f), t);

			targetColor.top = Color.Lerp(
				new Color(1.0f, 0.2f, 0.2f),
				new Color(1.0f, 0.6f, 0.2f), t);

			targetColor.fresnel = Color.Lerp(
				new Color(1.0f, 0.6f, 0.4f),
				new Color(1.0f, 0.8f, 0.4f), t);

		} else if (ph <= 6.0f) {

			float t = (ph - 4.0f) / 2.0f;

			targetColor.main = Color.Lerp(
				new Color(0.9f, 0.4f, 0.0f),
				new Color(0.9f, 0.9f, 0.0f), t);

			targetColor.top = Color.Lerp(
				new Color(1.0f, 0.6f, 0.2f),
				new Color(1.0f, 1.0f, 0.4f), t);

			targetColor.fresnel = Color.Lerp(
				new Color(1.0f, 0.8f, 0.4f),
				new Color(0.8f, 1.0f, 0.4f), t);

		} else if (ph < 8.0f) {

			float t = (ph - 6.0f) / 2.0f;

			targetColor.main = Color.Lerp(
				new Color(0.9f, 0.9f, 0.0f),
				new Color(0.2f, 0.7f, 0.2f), t);

			targetColor.top = Color.Lerp(
				new Color(1.0f, 1.0f, 0.4f),
				new Color(0.4f, 0.9f, 0.4f), t);

			targetColor.fresnel = Color.Lerp(
				new Color(0.8f, 1.0f, 0.4f),
				new Color(0.6f, 1.0f, 0.8f), t);

		} else if (ph <= 10.0f) {

			float t = (ph - 8.0f) / 2.0f;

			targetColor.main = Color.Lerp(
				new Color(0.2f, 0.7f, 0.2f),
				new Color(0.0f, 0.4f, 0.8f), t);

			targetColor.top = Color.Lerp(
				new Color(0.4f, 0.9f, 0.4f),
				new Color(0.2f, 0.6f, 1.0f), t);

			targetColor.fresnel = Color.Lerp(
				new Color(0.6f, 1.0f, 0.8f),
				new Color(0.4f, 0.8f, 1.0f), t);

		} else if (ph <= 12.0f) {
			float t = (ph - 10.0f) / 2.0f;

			targetColor.main = Color.Lerp(
				new Color(0.0f, 0.4f, 0.8f),
				new Color(0.4f, 0.0f, 0.8f), t);

			targetColor.top = Color.Lerp(
				new Color(0.2f, 0.6f, 1.0f),
				new Color(0.6f, 0.2f, 1.0f), t);

			targetColor.fresnel = Color.Lerp(
				new Color(0.4f, 0.8f, 1.0f),
				new Color(0.8f, 0.4f, 1.0f), t);

		} else {
			targetColor.main = new Color(0.4f, 0.0f, 0.8f);
			targetColor.top = new Color(0.6f, 0.2f, 1.0f);
			targetColor.fresnel = new Color(0.8f, 0.6f, 1.0f);

		}

		return targetColor;

	}

}
