using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using  NaughtyAttributes.Editor;
using NaughtyAttributes;
[ExecuteInEditMode]
public class GroundGenerator : MonoBehaviour
{
    public GameObject objectPrefab; // The prefab you want to spawn
    public int gridWidth = 5; // Number of objects in the x-axis
    public int gridHeight = 5; // Number of objects in the y-axis
    public float spacing = 2f; // Spacing between objects

    void Start()
    {
        SpawnGrid();
    }
    [Button("Make Grid")]
    void SpawnGrid()
    {
        float aspectRatio = objectPrefab.transform.localScale.x / objectPrefab.transform.localScale.z;

        for (int x = 0; x < gridWidth; x++)
        {
            for (int y = 0; y < gridHeight; y++)
            {
                float offsetX = x * spacing * aspectRatio;
                float offsetY = y * spacing;

                Vector3 spawnPosition = new Vector3(offsetX, 0f, offsetY);
                Instantiate(objectPrefab, spawnPosition, Quaternion.identity, transform);
            }
        }
    }
    [ContextMenu("DestroyGround)")]
    void DeleteGround()
    {
        foreach(Transform child in this.transform)
{
    Destroy(child.gameObject);
}
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
