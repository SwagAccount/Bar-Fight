using UnityEngine;

[CreateAssetMenu(fileName = "surface", menuName = "Surface", order = 1)]
public class Surface : ScriptableObject
{
    public GameObject ImpactEffect;
    public SoundEvent ImpactSound;

    public static void DoImpact(GameObject gameObject, Vector3 position)
    {
        SurfaceDefinition surfaceDefinition = null;
        if (!gameObject.TryGetComponent(out surfaceDefinition))
        {
            gameObject.transform.root.TryGetComponent(out surfaceDefinition);
        }

        if (surfaceDefinition?.Surface == null)
            return;

        var surface = surfaceDefinition.Surface;

        if (surface.ImpactEffect != null)
            Instantiate(surface.ImpactEffect, position, Quaternion.identity);

        if (surface.ImpactSound != null)
            surface.ImpactSound.Play(position);
    }
}