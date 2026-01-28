using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemRespawner : MonoBehaviour
{
    public GameObject Item;
    public GameObject Prefab;

    private Vector3 pos;
    private Quaternion rot;

    private void Start()
    {
        pos = Item.transform.position;
        rot = Item.transform.rotation;
    }

    void Update()
    {
        if (Item != null)
            return;

        Item = Instantiate(Prefab);

        Item.transform.position = pos;
        Item.transform.rotation = rot;
    }
}
