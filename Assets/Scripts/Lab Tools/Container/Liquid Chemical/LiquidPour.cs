using UnityEngine;
using System.Collections.Generic;
using BrandosLab.Chemical;
using BrandosLab.LabTools.Model;

namespace BrandosLab.LabTools.Container.Liquid {

	public class LiquidPour : PourInteraction<ChemicalData> {

		protected override void HandlePouring() {

			// Set The Pour Particles Position And Aligned it
			m_pourParticles.transform.position = m_newPosition;
			m_pourParticles.transform.forward = transform.up;

			// Set The Radius Based Of The Tilt Strength
			float particlesMaxRadius = Mathf.Min(m_pourParticlesMaxRadius, m_pourRadius);
			float particlesMinRadius = m_pourParticlesMinRadius;
			float newParticlesRadius = Mathf.Lerp(particlesMinRadius, particlesMaxRadius, 1.0f - m_tiltStrength);

			ParticleSystem.ShapeModule shapeModule = m_pourParticles.shape;
			shapeModule.radius = newParticlesRadius;

			float pourAmount = (1.0f - m_tiltStrength) * m_pourRate * Time.deltaTime;
			List<ChemicalPortion<ChemicalData>> removedChemical = m_container.RemoveChemical(pourAmount);

			FindPourable(m_newPosition, newParticlesRadius, removedChemical);

		}

		protected override void PourObject(RaycastHit hit, IPourable<ChemicalData> pourable, List<ChemicalPortion<ChemicalData>> chemicals) {

			if (pourable.GetObjectAttached() is IChemicalContainer<ChemicalData>) {

				bool isAngleCorrect = (Vector3.Angle(Vector3.up, hit.collider.transform.up) <= ANGLE_THRESHOLD);
				if (!isAngleCorrect)
					return;

			}

			pourable.PourObject(chemicals, hit.point);

		}
	}

}