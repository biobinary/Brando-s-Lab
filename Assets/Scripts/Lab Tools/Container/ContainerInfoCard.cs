using Oculus.Interaction;
using TMPro;
using UnityEngine;

public abstract class ContainerInfoCard<T> : MonoBehaviour where T : ScriptableObject {

	[SerializeField, Interface(typeof(IPointable))]
	protected UnityEngine.Object _pointable;
	protected IPointable Pointable;

	[SerializeField] private MonoBehaviour m_containerMonoBehaviour;
	protected IChemicalContainer<T> m_mainContainer;

	[Header("UI Settings")]
	[SerializeField] protected GameObject m_canvasGameObject;
	[SerializeField] protected TextMeshProUGUI m_primaryLabel;
	[SerializeField] protected TextMeshProUGUI m_secondaryLabel;

	protected Transform m_mainCameraTransform;
	protected bool m_isPicked = false;

	private void Awake() {
		
		Pointable = _pointable as IPointable;
		if (m_containerMonoBehaviour != null) {
			if (m_containerMonoBehaviour is IChemicalContainer<T>)
				m_mainContainer = m_containerMonoBehaviour as IChemicalContainer<T>;
		}

	}

	private void Start() {

		OVRCameraRig cameraRig = FindAnyObjectByType<OVRCameraRig>();
		if (cameraRig != null) {
			m_mainCameraTransform = cameraRig.centerEyeAnchor;
		}

		m_canvasGameObject.SetActive(false);

	}

	public void OnEnable() {

		if (Pointable != null)
			Pointable.WhenPointerEventRaised += OnHandlePointerEventRaised;

	}

	private void OnDisable() {

		if (Pointable != null)
			Pointable.WhenPointerEventRaised -= OnHandlePointerEventRaised;

	}

	private void OnHandlePointerEventRaised(PointerEvent pointerEvent) {

		switch (pointerEvent.Type) {

			case PointerEventType.Select:
				m_isPicked = true;
				m_canvasGameObject.SetActive(false);
				break;

			case PointerEventType.Unselect:
				m_isPicked = false;
				m_canvasGameObject.SetActive(true);
				SetupLabel();
				break;

			case PointerEventType.Hover:

				if (!m_isPicked) {
					m_canvasGameObject.SetActive(true);
					SetupLabel();
				}

				break;

			case PointerEventType.Unhover:
				m_canvasGameObject.SetActive(false);
				break;

		}

	}

	protected abstract void SetupLabel();

	private void Update() {
		Vector3 direction = (m_mainCameraTransform.position - transform.position).normalized;
		transform.rotation = Quaternion.LookRotation(-direction);
	}

}
