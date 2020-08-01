using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class MaxDistancesToColliderLineGeneratorWizard : ScriptableWizard
{
    [Tooltip("z方向にRaycastするオブジェクトのPrefab")]
    public TestRaycaster raycasterPrefab;

    [Tooltip("配置対象のColliderのPrefab")]
    public Collider colliderPrefab;

    [Tooltip("ColliderのGameObjectのTag")]
    public string tagName = "RaycastTarget";

    [Tooltip("Collider列の配置数(固定)")]
    public int count = 10000;

    [Tooltip("x, y, z それぞれの軸のマージン")]
    public Vector3 margins = new Vector3(5f, 0f, 5f);

    [Tooltip("検証ごとのPhysics.Raycastに使用するMaxDistance")]
    public List<float> maxDistances = new List<float> { 0f, 1f, 10f, 100f, 1000f };

    [Tooltip("配置の中心座標")]
    public Vector3 center = Vector3.zero;

    [MenuItem("ColliderGeneratorWizard/Generate line with max distance to colliders")]
    static void CloneObject()
    {
        ScriptableWizard.DisplayWizard<MaxDistancesToColliderLineGeneratorWizard>(
            "Generate line with max distance to colliders", "Generate!");
    }

    void OnWizardCreate()
    {
        if (colliderPrefab == null) {
            Debug.LogError("colliderPrefab is null");
            return;
        }

        if (raycasterPrefab != null) {
            for (int i = 0; i < maxDistances.Count; ++i) {
                float maxDistance = maxDistances[i];
                var name = string.Format("Raycaster maxDistance {0} ({1})", maxDistance.ToString("F1"), i);
                CreateRaycasterObject(
                    name,
                    center + new Vector3(margins.x * i, 0f, -margins.z),
                    center + new Vector3(margins.x * i, 0f, 0f),
                    maxDistance);
            }
        }

        GenerateAllColliderObject();
    }

    private void CreateRaycasterObject(
        string name,
        Vector3 position,
        Vector3 targetPos,
        float maxDistance)
    {
        var rotation = Quaternion.LookRotation((targetPos - position).normalized);
        var raycaster = Object.Instantiate(raycasterPrefab, position, rotation);
        raycaster.maxDistance = maxDistance;
        raycaster.gameObject.name = name;
        raycaster.maxDistance = maxDistance;

        var rayLengthSetter = raycaster.gameObject.GetComponent<RayLengthSetter>();
        if (rayLengthSetter != null) {
            rayLengthSetter.Length = maxDistance;
        }
    }

    private void GenerateAllColliderObject()
    {
        for (int i = 0; i < maxDistances.Count; ++i) {
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