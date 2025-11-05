using UnityEngine;

public class TransferMeshesToNewArmature : MonoBehaviour
{
    [Header("Assign these in Inspector")]
    public Transform oldArmature;
    public Transform newArmature;

    [ContextMenu("Transfer Children")]
    public void TransferChildren()
    {
        if (!oldArmature || !newArmature)
        {
            Debug.LogError("Please assign both old and new armatures.");
            return;
        }

        // Loop through all transforms in the old armature
        foreach (Transform oldBone in oldArmature.GetComponentsInChildren<Transform>(true))
        {
            // Skip the root itself
            if (oldBone == oldArmature)
                continue;

            // Find matching bone in the new armature by name
            Transform newBone = FindChildByName(newArmature, oldBone.name);
            if (!newBone)
                continue;

            // Move any meshes (or other objects) parented to the old bone
            for (int i = oldBone.childCount - 1; i >= 0; i--)
            {
                Transform child = oldBone.GetChild(i);
                child.SetParent(newBone, true); // Keep world position
            }
        }

        Debug.Log("Transfer complete!");
    }

    Transform FindChildByName(Transform root, string name)
    {
        foreach (Transform t in root.GetComponentsInChildren<Transform>(true))
        {
            if (t.name == name)
                return t;
        }
        return null;
    }
}
