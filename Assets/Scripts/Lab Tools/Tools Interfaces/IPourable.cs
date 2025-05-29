using UnityEngine;
using System.Collections.Generic;
using BrandosLab.Chemical;

namespace BrandosLab.LabTools.Model {

    public interface IPourable<T> where T : ChemicalBaseData {

        public void PourObject(List<ChemicalPortion<T>> chemicals, Vector3 pourLocation);
        public Object GetObjectAttached();

    }

}