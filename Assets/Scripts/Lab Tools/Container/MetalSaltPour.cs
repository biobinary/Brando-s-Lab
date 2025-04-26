using System.Collections.Generic;
using UnityEngine;

public class MetalSaltPour : PourInteraction<MetalSaltData> {

	protected override void HandlePouring() {

		m_pourParticles.transform.position = m_newPosition;
		m_pourParticles.transform.forward = transform.up;

		float particlesMaxRadius = Mathf.Min(m_pourParticlesMaxRadius, m_pourRadius);
		float particlesMinRadius = m_pourParticlesMinRadius;
		float newParticlesRadius = Mathf.Lerp(particlesMinRadius, particlesMaxRadius, 1.0f - m_tiltStrength);

		ParticleSystem.ShapeModule shapeModule = m_pourParticles.shape;
		shapeModule.radius = newParticlesRadius;

		float pourAmount = (1.0f - m_tiltStrength) * m_pourRate * Time.deltaTime;
		List<ChemicalPortion<MetalSaltData>> removedChemical = m_container.RemoveChemical(pourAmount);

		FindPourable(m_newPosition, newParticlesRadius, removedChemical);

	}

	protected override void PourObject(RaycastHit hit, IPourable<MetalSaltData> pourable, List<ChemicalPortion<MetalSaltData>> chemicals) {

		if (pourable.GetObjectAttached() is IChemicalContainer<MetalSaltData>) {

			bool isAngleCorrect = (Vector3.Angle(Vector3.up, hit.collider.transform.up) <= ANGLE_THRESHOLD);
			if (!isAngleCorrect)
				return;

		}

		pourable.PourObject(chemicals, hit.point);

	}

}
