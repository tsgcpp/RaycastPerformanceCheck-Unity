using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class ColliderLineGeneratorWizard : ScriptableWizard
{
    [Tooltip("z方向にRaycastするオブジェクトのPrefab")]
    public TestRaycaster raycasterPrefab;

    [Tooltip("配置対象のColliderのPrefab")]
    public Collider colliderPrefab;

    [Tooltip("ColliderのGameObjectのTag")]
    public string tagName = "RaycastTarget";

    [Tooltip("Collider列のそれぞれの配置数")]
    public List<int> counts = new List<int> { 0, 1, 10, 100, 1000 };

    [Tooltip("x, y, z それぞれの軸のマージン")]
    public Vector3 margins = new Vector3(5f, 0f, 5f);

    [Tooltip("Physics.Raycastに使用するMaxDistance")]
    public float maxDistance = 1000000f;

    [Tooltip("配置の中心座標")]
    public Vector3 center = Vector3.zero;


    [MenuItem("ColliderGeneratorWizard/Generate line colliders")]
    static void CloneObject()
    {
        ScriptableWizard.DisplayWizard<ColliderLineGeneratorWizard>(
            "Generate line colliders", "Generate!");
    }

    void OnWizardCreate()
    {
        if (colliderPrefab == null) {
            Debug.LogError("colliderPrefab is null");
            return;
        }

        if (raycasterPrefab != null) {
            for (int i = 0; i < counts.Count; ++i) {
                int count = counts[i];
                var name = string.Format("Raycaster to {0} ({1})", count, i);
                CreateRaycasterObject(
                    name,
                    center + new Vector3(margins.x * i, 0f, -margins.z),
                    center + new Vector3(margins.x * i, 0f, 0f));
            }
        }

        GenerateAllColliderObject();
    }

    private void CreateRaycasterObject(string name, Vector3 position, Vector3 targetPos)
    {
        var rotation = Quaternion.LookRotation((targetPos - position).normalized);
        var raycaster = Object.Instantiate(raycasterPrefab, position, rotation);
        raycaster.gameObject.name = name;
        raycaster.maxDistance = maxDistance;

        var rayLengthSetter = raycaster.gameObject.GetComponent<RayLengthSetter>();
        if (rayLengthSetter != null) {
            rayLengthSetter.Length = maxDistance;
        }
    }

    private void GenerateAllColliderObject()
    {
        for (int i = 0; i < counts.Count; ++i) {
            int count = counts[i];

            float x = margins.x * i;
            for (int j = 0; j < count; ++j) {
                GenerateColliderObject(center + new Vector3(x, 0f, margins.z * j));
            }
        }
    }

    private void GenerateColliderObject(Vector3 position)
    {
        var collider = Object.Instantiate(colliderPrefab, position, Quaternion.identity);
        collider.gameObject.name = colliderPrefab.gameObject.name;
        collider.gameObject.tag = tagName;

        EditorUtility.SetDirty(collider.gameObject);
    }
}