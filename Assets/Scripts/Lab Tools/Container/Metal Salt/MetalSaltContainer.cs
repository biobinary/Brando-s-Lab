using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using BrandosLab.Chemical;
using BrandosLab.LabTools.Model;

public class MetalSaltContainer : MonoBehaviour, IChemicalContainer<MetalSaltData> {

	[Header("Container Settings")]
	[SerializeField] protected ChemicalPortion<MetalSaltData> m_initialSalt;

	[Header("Visual Settings")]
	[SerializeField] protected ParticleSystem m_saltParticles;

	protected List<ChemicalPortion<MetalSaltData>> m_salts = new();
	protected ParticleSystemRenderer m_saltParticlesRenderer;

	private void Awake() {
		m_saltParticlesRenderer = m_saltParticles.GetComponent<ParticleSystemRenderer>();
	}

	protected virtual void OnEnable() {
		AddChemical(m_initialSalt);
	
	}

	protected virtual void OnDisable() {
		m_salts.Clear();

	}

	public virtual void AddChemical(ChemicalPortion<MetalSaltData> chemical) {
		m_salts.Add(chemical);
		m_saltParticlesRenderer.material.SetColor("_BaseColor", GetNewSaltBlendedColor());
	}

	public List<ChemicalPortion<MetalSaltData>> GetChemicalContents() {
		return m_salts;
	}

	public float GetCurrentVolume() {
		
		float sumCurrentVolume = 0;

		foreach (ChemicalPortion<MetalSaltData> salt in m_salts) {
			sumCurrentVolume += salt.volume;
		}

		return sumCurrentVolume;

	}

	public virtual List<ChemicalPortion<MetalSaltData>> RemoveChemical(float volume) {

		List<ChemicalPortion<MetalSaltData>> removedSalts = new();
        foreach (ChemicalPortion<MetalSaltData> salt in m_salts) {
			ChemicalPortion<MetalSaltData> newPortion = new();
			newPortion.data = salt.data;
			newPortion.volume = volume;
			removedSalts.Add(newPortion);
        }

        return removedSalts;

	}

	protected Color GetNewSaltBlendedColor() {
		
		if (!m_salts.Any())
			return Color.white;

		List<float> saltColorWeights = new();
		float currentContainerVolume = GetCurrentVolume();

		foreach( ChemicalPortion<MetalSaltData> salt in m_salts ) {
			float saltWeight = salt.volume / currentContainerVolume;
			saltColorWeights.Add(saltWeight);
		}

		float r = 0f, g = 0f, b = 0f;
		float totalWeight = 0f;

		for (int i = 0; i < m_salts.Count; i++) {
			r += m_salts[i].data.saltColor.r * saltColorWeights[i];
			g += m_salts[i].data.saltColor.g * saltColorWeights[i];
			b += m_salts[i].data.saltColor.b * saltColorWeights[i];
			totalWeight += saltColorWeights[i];
		}

		if (totalWeight != 0f && totalWeight != 1f) {
			r /= totalWeight;
			g /= totalWeight;
			b /= totalWeight;
		}

		r = Mathf.Clamp01(r);
		g = Mathf.Clamp01(g);
		b = Mathf.Clamp01(b);

		Color newColor = m_saltParticlesRenderer.material.GetColor("_BaseColor");
		newColor.r = r;
		newColor.g = g;
		newColor.b = b;

		return newColor;

	}

}
