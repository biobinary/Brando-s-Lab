using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using YourNamespace;
using Oculus.Interaction;

public class PetriDishContainer : MetalSaltContainer, IPourable<MetalSaltData>, IBurnable {

	[Header("Container Settings")]
	[SerializeField] private GameObject m_interactableGameObject;
	[SerializeField] private float m_maxVolume = 10.0f;
	[SerializeField] private float m_maxBurnTime = 1.0f;

	[Header("Salt Visual Settings")]
	[SerializeField] private GameObject m_saltVisual;
	[SerializeField] private float m_saltEmptyScale = 0.0f;
	[SerializeField] private float m_saltFullScale = 0.06f;

	[Header("Fire Visual Settings")]
	[SerializeField] private GameObject m_fireEffect;
	[SerializeField] private float m_maxFireTimeout = 8.0f;

	[Header("Sound Effect Settings")]
	[SerializeField] private AudioTrigger m_fireTriggerSFX;

	[Header("Objectives")]
	public PlaygroundObjective m_playgroundObjective;

	private bool m_isOnFire = false;
	private Coroutine m_onFireCoroutine = null;

	private float m_burnTimer = 0f;
	private bool m_isBurning = false;

	public bool IsBurning => m_isBurning && !m_isOnFire;

	protected override void OnEnable() {
		base.OnEnable();
		UpdateVisual();

	}

	protected override void OnDisable() {
		
		base.OnDisable();

		if ( m_isBurning )
			StopBurning();

		if( m_onFireCoroutine != null )
			HandleFireTimeout();

	}

	public override void AddChemical(ChemicalPortion<MetalSaltData> salt) {

		if (salt == null)
			return;

		ChemicalPortion<MetalSaltData> currentSalt = m_salts?.Count > 0 ? m_salts[0] : null;

		if (currentSalt != null) {

			if (currentSalt.data.name == salt.data.name) {

				float currentAddedVolume = salt.volume;
				currentAddedVolume = Mathf.Min(salt.volume, m_maxVolume - currentSalt.volume);
				if (currentAddedVolume <= 0) return;

				currentSalt.volume += currentAddedVolume;

			}

		} else {

			float currentAddedVolume = salt.volume;
			currentAddedVolume = Mathf.Min(salt.volume, m_maxVolume);
			if (currentAddedVolume <= 0) return;

			m_salts.Add(new ChemicalPortion<MetalSaltData> {
				data = salt.data,
				volume = currentAddedVolume
			});

		}

		UpdateVisual();

	}

	public override List<ChemicalPortion<MetalSaltData>> RemoveChemical(float volume) {

		ChemicalPortion<MetalSaltData> currentSalt = m_salts?.Count > 0 ? m_salts[0] : null;

		if (currentSalt == null) return null;

		List<ChemicalPortion<MetalSaltData>> removedSalts = new() {
			new ChemicalPortion<MetalSaltData> { 
				data = currentSalt.data, 
				volume = currentSalt.volume 
			}
		};

		m_salts.Clear();
		UpdateVisual();

		return removedSalts;

	}

	public void PourObject(List<ChemicalPortion<MetalSaltData>> salts, Vector3 pourLocation) {

		ChemicalPortion<MetalSaltData> saltData = salts?.Count > 0 ? salts[0] : null;

		if( saltData != null )
			AddChemical( saltData );

	}

	public Object GetObjectAttached() {
		return this;

	}

	private void UpdateVisual() {

		ChemicalPortion<MetalSaltData> currentSalt = m_salts?.Count > 0 ? m_salts[0] : null;

		if (currentSalt != null) {

			float newScaleFactor = Mathf.Lerp(
				m_saltEmptyScale, m_saltFullScale, currentSalt.volume / m_maxVolume);

			m_saltVisual.transform.localScale = Vector3.one * newScaleFactor;

		} else {
			m_saltVisual.transform.localScale = Vector3.zero;

		}

	}

	public void StartBurning() {

		if (m_isOnFire) return;

		if (!m_isBurning) {
			m_isBurning = true;
			m_burnTimer = 0.0f;

		}

	}

	public void StopBurning() {

		if (m_isBurning) {
			m_isBurning = false;
			m_burnTimer = 0.0f;

		}

	}

	private IEnumerator StartFire() {

		ChemicalPortion<MetalSaltData> currentSalt = m_salts?.Count > 0 ? m_salts[0] : null;

		if (currentSalt != null) {

			m_isOnFire = true;

			m_fireTriggerSFX.PlayAudio();

			m_fireEffect.SetActive(true);
			m_interactableGameObject.SetActive(false);

			float fireStrength = currentSalt.volume / m_maxVolume;

			VFX_FireController fireController = m_fireEffect.GetComponent<VFX_FireController>();
			fireController.SetFireColor(currentSalt.data.flameColor);
			fireController.SetFireIntensity(fireStrength);

			CheckColorObjectives(currentSalt.data.GetColorName());

			yield return new WaitForSeconds(m_maxFireTimeout * fireStrength);

			HandleFireTimeout();

		} else {
			m_onFireCoroutine = null;

		}

	}

	private void CheckColorObjectives(string colorName) {

		switch(colorName) {
			
			case "Merah":
				m_playgroundObjective.SetCompletion("Buat Api yang Berwarna Merah");
				break;
			
			case "Kuning":
				m_playgroundObjective.SetCompletion("Buat Api yang Berwarna Kuning");
				break;
			
			case "Oren":
				m_playgroundObjective.SetCompletion("Buat Api yang Berwarna Oren");
				break;

			case "Hijau":
				m_playgroundObjective.SetCompletion("Buat Api yang Berwarna Hijau");
				break;

			case "Cyan":
				m_playgroundObjective.SetCompletion("Buat Api yang Berwarna Cyan");
				break;

			case "Biru":
				m_playgroundObjective.SetCompletion("Buat Api yang Berwarna Biru");
				break;

			case "Ungu":
				m_playgroundObjective.SetCompletion("Buat Api yang Berwarna Ungu");
				break;

			case "Magenta":
				m_playgroundObjective.SetCompletion("Buat Api yang Berwarna Magenta");
				break;


		}

	}

	private void HandleFireTimeout() {

		m_isOnFire = false;

		m_fireEffect.SetActive(false);
		m_interactableGameObject.SetActive(true);

		m_salts.Clear();
		UpdateVisual();

		m_onFireCoroutine = null;

	}

	private void Update() {
		
		if( m_isBurning ) {
			m_burnTimer += Time.deltaTime;
			if( m_burnTimer > m_maxBurnTime ) {
				StopBurning();
				m_onFireCoroutine = StartCoroutine(StartFire());
			}
		}

	}

}
