using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Pipette : TriggerBasedTool {

	[Header("Pipette Settings")]
	[SerializeField] private bool m_transferOnly = true;
	[SerializeField] private float m_maxVolume = 8.0f;
	[SerializeField] private List<ChemicalPortion<ChemicalData>> m_initialChemicals = new();

	[Header("Pour Settings")]
	[SerializeField] private Transform m_pourTransform;
	[SerializeField] private float m_pourRadius = 0.003f;
	[SerializeField] private float m_pourRate = 10.0f;
	[SerializeField] private ParticleSystem m_pourParticles;
	[SerializeField] protected LayerMask m_pourLayerMask;

	[Header("Trigger Visual")]
	[SerializeField] private float m_triggerInitalPosition = 0.07f;
	[SerializeField] private float m_triggerPressedPosition = 0.05f;
	[SerializeField] private GameObject m_triggerGameObject;

	[Header("Liquid Visual Settings")]
	[SerializeField] private bool m_dynamicallyChangeLiquidColor = false;
	[SerializeField] private Color m_liquidColor = Color.white;

	protected const float MAX_POUR_DISTANCE = 5.0f;
	protected const float ANGLE_THRESHOLD = 70.0f;

	private Coroutine m_triggerInteractionVisualCoroutine = null;

	private Dictionary<string, ChemicalPortion<ChemicalData>> m_contents =
		new Dictionary<string, ChemicalPortion<ChemicalData>>();
	
	private float m_currentVolume = 0.0f;

	private bool m_isPouring = false;

	private void OnEnable() {
		
		m_contents.Clear();

		if( m_initialChemicals.Any() ) {

			foreach (ChemicalPortion<ChemicalData> chem in m_initialChemicals) {

				ChemicalPortion<ChemicalData> portion = new();
				portion.data = chem.data;
				portion.volume = chem.volume;

				AddChemical(portion);

			}

		}

	}

	protected override void Update() {

		base.Update();

		if (m_pourParticles == null)
			return;

		m_pourParticles.transform.forward = -transform.up;

		if (m_isPouring) {
			PourLiquid();
		
		} else {

			if (m_pourParticles.isPlaying)
				m_pourParticles.Stop();

		}

	}

	protected override void OnHandleTriggerPressed() {

		if( m_triggerInteractionVisualCoroutine != null)
			StopCoroutine(m_triggerInteractionVisualCoroutine);

		m_triggerInteractionVisualCoroutine = StartCoroutine(
			HandleMoveTriggerVisual(m_triggerPressedPosition));

		m_isPouring = true;

	}

	protected override void OnHandleTriggerReleased() {

		if (m_triggerInteractionVisualCoroutine != null)
			StopCoroutine(m_triggerInteractionVisualCoroutine);

		m_triggerInteractionVisualCoroutine = StartCoroutine(
			HandleMoveTriggerVisual(m_triggerInitalPosition));

		m_isPouring = false;

	}

	private void AddChemical(ChemicalPortion<ChemicalData> chemical) {

		if (Mathf.Approximately(m_maxVolume, m_currentVolume))
			return;

		float currentAddedVolume = chemical.volume;
		currentAddedVolume = Mathf.Min(chemical.volume, m_maxVolume - m_currentVolume);
		
		if (currentAddedVolume <= 0) 
			return;

		if (m_contents.TryGetValue(
			chemical.data.name,
			out ChemicalPortion<ChemicalData> chemicalPortion)) {

			chemicalPortion.volume += currentAddedVolume;

		} else {
			m_contents.Add(
				chemical.data.name,
				new ChemicalPortion<ChemicalData> {
					data = chemical.data,
					volume = currentAddedVolume }
				);

		}

		m_currentVolume += currentAddedVolume;

	}

	private List<ChemicalPortion<ChemicalData>> RemoveChemical(float amount) {

		if (m_currentVolume <= 0.0f)
			return null;

		List<ChemicalPortion<ChemicalData>> chemicalsRemovePortion =
			new List<ChemicalPortion<ChemicalData>>();

		List<string> chemicalsToRemove = new List<string>();

		float removalRatio = Mathf.Min(1.0f, amount / m_currentVolume);

		foreach (var kvp in m_contents) {

			float volumeToRemove = kvp.Value.volume * removalRatio;
			
			if( !m_transferOnly ) {
				
				kvp.Value.volume -= volumeToRemove;

				// If chemical volume becomes negligible, mark for removal
				if (Mathf.Approximately(kvp.Value.volume, 0.0f)) {
					chemicalsToRemove.Add(kvp.Key);
				}

			}

			ChemicalPortion<ChemicalData> removedPortion = new();
			removedPortion.data = kvp.Value.data;
			removedPortion.volume = volumeToRemove;
			chemicalsRemovePortion.Add(removedPortion);

		}

		foreach (string chemicalName in chemicalsToRemove) {
			m_contents.Remove(chemicalName);
		}

		if( !m_transferOnly ) {
			m_currentVolume -= amount;
			m_currentVolume = Mathf.Max(0, m_currentVolume);
		}

		return chemicalsRemovePortion;

	}

	private void PourLiquid() {

		bool isShouldPour = Vector3.Dot(transform.up, Vector3.down) <= 0.0f && m_currentVolume > 0.0f;
		
		if (!isShouldPour) {

			if ( m_pourParticles.isPlaying )
				m_pourParticles.Stop();

			return;

		}

		if ( !m_pourParticles.isPlaying )
			m_pourParticles.Play();

		List<ChemicalPortion<ChemicalData>> removedChemical = RemoveChemical(m_pourRate * Time.deltaTime);

		if (Physics.SphereCast(
			m_pourTransform.position, m_pourRadius,
			Vector3.down, out RaycastHit hit,
			MAX_POUR_DISTANCE, m_pourLayerMask,
			QueryTriggerInteraction.Collide)) {

			GameObject foundGameObject = hit.transform.gameObject;
			if (foundGameObject.TryGetComponent(out IPourable<ChemicalData> foundPourableObject)) {

				if (foundPourableObject.GetObjectAttached() is IChemicalContainer<ChemicalData>) {

					bool isAngleCorrect = (Vector3.Angle(Vector3.up, hit.collider.transform.up) <= ANGLE_THRESHOLD);
					if (!isAngleCorrect)
						return;

				}

				foundPourableObject.PourObject(removedChemical, hit.point);

			}
		
		}

	}

	private IEnumerator HandleMoveTriggerVisual(float targetPosition, float duration = 0.1f) {

		float visualChangeDuration = duration;
		float elapsedTime = 0.0f;

		while (elapsedTime < visualChangeDuration) {

			float newPos = Mathf.Lerp( m_triggerGameObject.transform.localPosition.z, 
					targetPosition, elapsedTime / visualChangeDuration);

			m_triggerGameObject.transform.localPosition = new Vector3(
				m_triggerGameObject.transform.localPosition.x,
				m_triggerGameObject.transform.localPosition.y,
				newPos 
			);


			elapsedTime += Time.deltaTime;
			
			yield return null;

		}

		m_triggerGameObject.transform.localPosition = new Vector3(
			m_triggerGameObject.transform.localPosition.x,
			m_triggerGameObject.transform.localPosition.y,
			targetPosition
		);

		m_triggerInteractionVisualCoroutine = null;

	}

}
