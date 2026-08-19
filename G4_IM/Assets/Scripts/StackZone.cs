using System.Collections.Generic;
using UnityEngine;

public class StackZone : MonoBehaviour
{
    private HashSet<Rigidbody> cubesInZone = new HashSet<Rigidbody>();

    private void OnTriggerEnter(Collider other)
    {
        // Obtiene el Rigidbody del cubo que entra en la zona
        Rigidbody rb = other.attachedRigidbody;
        if (rb != null)
        {
            if (!cubesInZone.Contains(rb))
            {
                cubesInZone.Add(rb);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // Elimina el cubo de la lista si sale de la zona
        Rigidbody rb = other.attachedRigidbody;
        if (rb != null)
        {
            if (cubesInZone.Contains(rb))
            {
                cubesInZone.Remove(rb);
            }
        }
    }

    /// <summary>
    /// Devuelve la cantidad de cubos actualmente dentro de la zona de apilado.
    /// </summary>
    public int GetCubeCount()
    {
        // Limpia referencias por si algún objeto fue destruido
        cubesInZone.RemoveWhere(item => item == null);
        return cubesInZone.Count;
    }
}