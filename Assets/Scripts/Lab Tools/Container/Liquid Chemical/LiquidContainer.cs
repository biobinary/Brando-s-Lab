using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using BrandosLab.Chemical;
using BrandosLab.LabTools.Model;

namespace BrandosLab.LabTools.Container.Liquid {

	public class LiquidContainer : MonoBehaviour, IPourable<ChemicalData>, IChemicalContainer<ChemicalData> {

		[Header("Fillable Settings")]
		[SerializeField] private bool m_canFill = false;
		[SerializeField] private bool m_canRefill = false;
		[SerializeField] private float m_refillDelay = 3.0f;
		[SerializeField] private float m_refillRate = 0.8f;

		[Header("Chemical Settings")]
		[SerializeField]
		private List<ChemicalPortion<ChemicalData>> m_initialChemical =
			new List<ChemicalPortion<ChemicalData>>();

		[SerializeField] private float m_maxVolume = 50.0f;
		[SerializeField] private float m_chemicalReactionSpeed = 5.0f;

		[Header("Renderer Settings")]
		[SerializeField] private Vector2 m_rendererLimitBounds = new Vector2(0.0f, 1.0f);
		[SerializeField] private Renderer m_solutionRenderer;

		[Header("Effects Settings")]
		[SerializeField] private ParticleSystem m_pourParticles;

		[Header("Neutralization Checker")]
		[SerializeField] private NeutralizationChecker m_neutralizationChecker;

		private Dictionary<string, ChemicalPortion<ChemicalData>> m_contents =
			new Dictionary<string, ChemicalPortion<ChemicalData>>();

		private float m_currentVolume = 0.0f;

		private float m_currentPH = 7.0f;
		private float m_targetPH = 7.0f;
		private bool m_neutralizationHasBeenCheckedLastFrame = false;

		private PHColorSet m_currentColor = new(Color.white, Color.white, Color.white);
		private PHColorSet m_targetColor = new(Color.white, Color.white, Color.white);

		private Coroutine m_currentRefillCoroutine = null;
		private Coroutine m_currentWaitUntilRefillTimerCoroutine = null;

		private const float WATER_CONSTANT = 1e-14f;

		// Material String Hash
		private int _Fill = Shader.PropertyToID("_Fill");
		private int _Side_Color = Shader.PropertyToID("_Side_Color");
		private int _Top_Color = Shader.PropertyToID("_Top_Color");
		private int _Fresnel_Color = Shader.PropertyToID("_Fresnel_Color");

		private void OnEnable() {
			ResetData();
		}

		private void Update() {

			if (!Mathf.Approximately(m_currentPH, m_targetPH)) {

				m_currentPH = Mathf.Lerp(
					m_currentPH, m_targetPH, Time.deltaTime * m_chemicalReactionSpeed);

				if (m_neutralizationChecker != null) {
					if (m_neutralizationChecker.confirmationDelayIsOnProgress)
						m_neutralizationChecker.StopConfirmationDelayTimeout();
				}

				m_neutralizationHasBeenCheckedLastFrame = false;

			} else {

				if (m_neutralizationChecker != null) {
					if (!m_neutralizationHasBeenCheckedLastFrame)
						m_neutralizationChecker.StartNeutralizationConfirmation(
							m_currentPH, GetChemicalContents());
				}

				m_neutralizationHasBeenCheckedLastFrame = true;

			}

			m_currentColor = PHColorSet.Lerp(
				m_currentColor,
				m_targetColor,
				Time.deltaTime * m_chemicalReactionSpeed);

			UpdateVisual();

		}

		public void AddChemical(ChemicalPortion<ChemicalData> chemical) {

			if (Mathf.Approximately(m_maxVolume, m_currentVolume))
				return;

			float currentAddedVolume = chemical.volume;
			currentAddedVolume = Mathf.Min(chemical.volume, m_maxVolume - m_currentVolume);
			if (currentAddedVolume <= 0) return;

			if (m_contents.TryGetValue(
					chemical.data.name,
					out ChemicalPortion<ChemicalData> chemicalPortion)) {

				chemicalPortion.volume += currentAddedVolume;

			} else {
				m_contents.Add(
					chemical.data.name,
					new ChemicalPortion<ChemicalData> {
						data = chemical.data,
						volume = currentAddedVolume
					}
					);

			}

			m_currentVolume += currentAddedVolume;
			m_targetPH = PHManager.CalCulatePH(GetChemicalContents(), m_currentVolume);
			m_targetColor = PHManager.GetPHColor(m_targetPH);

			m_neutralizationHasBeenCheckedLastFrame = false;

		}

		public float GetCurrentVolume() {
			return m_currentVolume;
		}

		public List<ChemicalPortion<ChemicalData>> RemoveChemical(float volume) {

			if (m_currentVolume <= 0.0f)
				return null;

			OnHandleRefillCallback(false);

			List<ChemicalPortion<ChemicalData>> chemicalsRemovePortion =
				new List<ChemicalPortion<ChemicalData>>();

			List<string> chemicalsToRemove = new List<string>();

			float removalRatio = Mathf.Min(1.0f, volume / m_currentVolume);

			foreach (var kvp in m_contents) {

				float volumeToRemove = kvp.Value.volume * removalRatio;
				kvp.Value.volume -= volumeToRemove;

				// If chemical volume becomes negligible, mark for removal
				if (Mathf.Approximately(kvp.Value.volume, 0.0f) && !m_canRefill) {
					chemicalsToRemove.Add(kvp.Key);
				}

				ChemicalPortion<ChemicalData> removedPortion = new();
				removedPortion.data = kvp.Value.data;
				removedPortion.volume = volumeToRemove;
				chemicalsRemovePortion.Add(removedPortion);

			}

			foreach (string chemicalName in chemicalsToRemove) {
				m_contents.Remove(chemicalName);
			}

			m_currentVolume -= volume;
			m_currentVolume = Mathf.Max(0, m_currentVolume);

			m_targetPH = PHManager.CalCulatePH(GetChemicalContents(), m_currentVolume);
			m_targetColor = PHManager.GetPHColor(m_targetPH);

			m_neutralizationHasBeenCheckedLastFrame = false;

			OnHandleRefillCallback(true);
			return chemicalsRemovePortion;

		}

		public void UpdateVisual() {

			if (m_solutionRenderer == null)
				return;

			float fillLevel = Mathf.Clamp01(m_currentVolume / m_maxVolume);
			float mappedFill = Mathf.Lerp(m_rendererLimitBounds.x, m_rendererLimitBounds.y, fillLevel);

			m_solutionRenderer.material.SetFloat(_Fill, mappedFill);

			m_solutionRenderer.material.SetColor(_Side_Color, m_currentColor.main);
			m_solutionRenderer.material.SetColor(_Top_Color, m_currentColor.top);
			m_solutionRenderer.material.SetColor(_Fresnel_Color, m_currentColor.fresnel);

			ParticleSystem.MainModule mainModule = m_pourParticles.main;
			mainModule.startColor = m_currentColor.main;

		}

		private IEnumerator RefillContainer() {

			while (!Mathf.Approximately(m_currentVolume, m_maxVolume)) {

				float remainingVolume = m_maxVolume - m_currentVolume;
				float volumePerChemical = Mathf.Min(
					(m_refillRate * Time.deltaTime) / m_contents.Count,
					remainingVolume / m_contents.Count
				);

				foreach (var kvp in m_contents) {

					ChemicalPortion<ChemicalData> portion = new();
					portion.data = kvp.Value.data;
					portion.volume = volumePerChemical;

					AddChemical(portion);

				}

				yield return null;

			}

			m_currentRefillCoroutine = null;

		}

		private IEnumerator StartRefillTimer() {

			yield return new WaitForSeconds(m_refillDelay);
			m_currentWaitUntilRefillTimerCoroutine = null;

			if (m_currentRefillCoroutine == null)
				m_currentRefillCoroutine = StartCoroutine(RefillContainer());

		}

		private void OnHandleRefillCallback(bool isRefilled) {

			if (!isRefilled) {

				if (m_currentWaitUntilRefillTimerCoroutine != null) {
					StopCoroutine(m_currentWaitUntilRefillTimerCoroutine);
					m_currentWaitUntilRefillTimerCoroutine = null;
				}

				if (m_currentRefillCoroutine != null) {
					StopCoroutine(m_currentRefillCoroutine);
					m_currentRefillCoroutine = null;
				}

			} else {

				if (!m_canRefill)
					return;

				if (m_currentWaitUntilRefillTimerCoroutine == null)
					m_currentWaitUntilRefillTimerCoroutine = StartCoroutine(StartRefillTimer());

			}

		}

		public void PourObject(List<ChemicalPortion<ChemicalData>> chemicals, Vector3 pourLocation) {

			if (!m_canFill || chemicals.Count == 0)
				return;

			OnHandleRefillCallback(false);

			foreach (ChemicalPortion<ChemicalData> chem in chemicals) {

				ChemicalPortion<ChemicalData> portion = new();
				portion.data = chem.data;
				portion.volume = chem.volume;
				AddChemical(portion);

			}

			OnHandleRefillCallback(true);

		}

		public Object GetObjectAttached() {
			return this;

		}

		public List<ChemicalPortion<ChemicalData>> GetChemicalContents() {
			return m_contents.Values.ToList();
		}

		private void ResetData() {

			m_contents.Clear();

			m_currentVolume = 0.0f;
			m_currentPH = 7.0f;
			m_targetPH = 7.0f;

			m_currentColor = new(Color.white, Color.white, Color.white);
			m_targetColor = new(Color.white, Color.white, Color.white);

			OnHandleRefillCallback(false);

			if (m_initialChemical.Count > 0) {

				foreach (ChemicalPortion<ChemicalData> chem in m_initialChemical) {

					ChemicalPortion<ChemicalData> portion = new();
					portion.data = chem.data;
					portion.volume = chem.volume;

					AddChemical(portion);

				}

				m_currentPH = m_targetPH;
				m_currentColor = m_targetColor;

			}

		}

	}

}
