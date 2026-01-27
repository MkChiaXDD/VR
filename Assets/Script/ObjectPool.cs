using UnityEngine;
using System.Collections.Generic;

public class ObjectPool : MonoBehaviour
{
    [Header("Pool Settings")]
    [SerializeField] private GameObject prefab;
    [SerializeField] private int poolSize = 5;

    [Header("Hierarchy")]
    [SerializeField] private Transform poolParent; // ? NEW

    private List<GameObject> pool = new List<GameObject>();

    private void Awake()
    {
        // If no parent assigned, create one automatically
        if (poolParent == null)
        {
            GameObject parentObj = new GameObject($"{prefab.name}_Pool");
            parentObj.transform.SetParent(transform);
            poolParent = parentObj.transform;
        }

        for (int i = 0; i < poolSize; i++)
        {
            CreateNewObject();
        }
    }

    public GameObject GetObject()
    {
        foreach (GameObject obj in pool)
        {
            if (!obj.activeInHierarchy)
            {
                obj.SetActive(true);
                return obj;
            }
        }

        // Expand pool if needed
        return CreateNewObject(true);
    }

    public void ReturnObject(GameObject obj)
    {
        obj.SetActive(false);
        obj.transform.SetParent(poolParent); // keep hierarchy clean
    }

    // ================= INTERNAL =================

    private GameObject CreateNewObject(bool setActive = false)
    {
        GameObject obj = Instantiate(prefab, poolParent);
        obj.SetActive(setActive);
        pool.Add(obj);
        return obj;
    }
}
