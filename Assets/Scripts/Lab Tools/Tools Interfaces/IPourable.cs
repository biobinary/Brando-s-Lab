using System.Collections.Generic;
using UnityEngine;

public interface IPourable<T> where T : ChemicalBaseData {

    public void PourObject(List<ChemicalPortion<T>> chemicals, Vector3 pourLocation);
    public Object GetObjectAttached();

}
