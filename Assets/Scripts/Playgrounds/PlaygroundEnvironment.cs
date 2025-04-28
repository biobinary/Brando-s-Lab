using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public class PlaygroundEnvironment : MonoBehaviour {

	[SerializeField] private float m_spawnRate = 0.2f;
	[SerializeField] private GameObject m_spawnEffect;
	[SerializeField] private PlaygroundObjective m_objective;

	public PlaygroundObjective GetObjective() => m_objective;

	private struct EnvironmentObjectTransformData {
		public Vector3 initialPosition;
		public Quaternion initialRotation;
	}

	private Dictionary<GameObject, EnvironmentObjectTransformData> m_environmentObjects = 
		new Dictionary<GameObject, EnvironmentObjectTransformData>();

	private void Awake() {
		
		if (m_objective != null)
			m_objective.ResetAllObjectives();

	}

	private void Start() {

		foreach (Transform child in transform) {

			EnvironmentObjectTransformData data = new EnvironmentObjectTransformData();
			data.initialPosition = child.localPosition;
			data.initialRotation = child.localRotation;
			
			GameObject childGameObject = child.gameObject;
			m_environmentObjects.Add(childGameObject, data);

			childGameObject.SetActive(false);

		}

	}

	public void Load(System.Action onFinishedAction = null) {
		StartCoroutine(StartLoadProgress(onFinishedAction));
	}

	private IEnumerator StartLoadProgress(System.Action onFinishedAction = null) {

		foreach(Transform child in transform) {

			EnvironmentObjectTransformData childData = m_environmentObjects[child.gameObject];
			child.position = childData.initialPosition;
			child.rotation = childData.initialRotation;

			GameObject childGameObject = child.gameObject;
			childGameObject.SetActive( true );

			Instantiate(m_spawnEffect, child.position, Quaternion.identity );

			yield return new WaitForSeconds(m_spawnRate);

		}
		
		onFinishedAction?.Invoke();

	}

	public void Reset(System.Action onFinishedAction = null) {
		StartCoroutine(StartResetProgress(onFinishedAction));
	}

	private IEnumerator StartResetProgress(System.Action onFinishedAction = null) {

		foreach (Transform child in transform) {

			child.gameObject.SetActive( false );

			EnvironmentObjectTransformData childData = m_environmentObjects[child.gameObject];
			child.position = childData.initialPosition;
			child.rotation = childData.initialRotation;

			child.gameObject.SetActive( true );

			Instantiate(m_spawnEffect, child.position, Quaternion.identity);

			yield return new WaitForSeconds(m_spawnRate);

		}

		onFinishedAction?.Invoke();

	}

	public void Clear(System.Action onFinishedAction = null) { 
		StartCoroutine(StartClearProgress(onFinishedAction));
	}

	private IEnumerator StartClearProgress(System.Action onFinishedAction = null) {

		int childCount = transform.childCount;

		for (int i = transform.childCount - 1; i >= 0; i--) {

			Instantiate(m_spawnEffect, transform.GetChild(i).position, Quaternion.identity);

			GameObject childGameObject = transform.GetChild(i).gameObject;
			childGameObject.SetActive(false);

			yield return new WaitForSeconds(m_spawnRate);

		}

		onFinishedAction?.Invoke();

	}

}
