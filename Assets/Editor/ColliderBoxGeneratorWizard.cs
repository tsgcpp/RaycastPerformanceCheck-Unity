using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class ColliderBoxGeneratorWizard : ScriptableWizard
{
    [Tooltip("z方向にRaycastするオブジェクトのPrefab")]
    public TestRaycaster raycasterPrefab;

    [Tooltip("配置対象のColliderのPrefab")]
    public Collider colliderPrefab;

    [Tooltip("ColliderのGameObjectのTag")]
    public string tagName = "RaycastTarget";

    [Tooltip("x, y, z それぞれの軸の配置数")]
    public Vector3 counts = new Vector3(10f, 10f, 10f);

    [Tooltip("x, y, z それぞれの軸のマージン")]
    public Vector3 margins = new Vector3(5f, 5f, 5f);

    [Tooltip("Physics.Raycastに使用するMaxDistance")]
    public float maxDistance = 10000f;

    [Tooltip("配置の中心座標")]
    public Vector3 center = Vector3.zero;

    [MenuItem("ColliderGeneratorWizard/Generate box colliders")]
    static void CloneObject()
    {
        ScriptableWizard.DisplayWizard<ColliderBoxGeneratorWizard>(
            "Generate box colliders", "Generate!");
    }

    void OnWizardCreate()
    {
        if (colliderPrefab == null) {
            Debug.LogError("colliderPrefab is null");
            return;
        }

        Vector3 starts = new Vector3(
            x: -(margins.x * (counts.x - 1)) / 2.0f + center.x,
            y: -(margins.y * (counts.y - 1)) / 2.0f + center.y,
            z: -(margins.z * (counts.z - 1)) / 2.0f + center.z);

        Vector3 middles = new Vector3(
            x: margins.x * ((counts.x - 1) % 2) + center.x,
            y: margins.y + ((counts.y - 1) % 2) + center.y,
            z: margins.z + ((counts.z - 1) % 2) + center.z);

        Vector3 ends = new Vector3(
            x: (margins.x * (counts.x - 1)) / 2.0f + center.x,
            y: (margins.y * (counts.y - 1)) / 2.0f + center.y,
            z: (margins.z * (counts.z - 1)) / 2.0f + center.z);

        // 奇数個は非対応

        if (raycasterPrefab != null) {
            CreateRaycasterObject("Raycaster to Empty", center, Vector3.forward);
            CreateRaycasterObject("Raycaster to Middle Object", center, middles);

            for (int z = 0; z < 2; ++z) {
                for (int y = 0; y < 2; ++y) {
                    for (int x = 0; x < 2; ++x) {
                        Vector3 targetPos = new Vector3(
                            x: (x == 0) ? starts.x : ends.x,
                            y: (y == 0) ? starts.y : ends.y,
                            z: (z == 0) ? starts.z : ends.z);
                        Vector3 offset = new Vector3(
                            x: (x == 0) ? -margins.x : margins.x,
                            y: (y == 0) ? -margins.y : margins.y,
                            z: (z == 0) ? -margins.z : margins.z);
                        string name = "Raycaster to " +
                            ((x == 0) ? "MinX" : "MaxX") + ", " +
                            ((y == 0) ? "MinY" : "MaxY") + ", " +
                            ((z == 0) ? "MinZ" : "MaxZ");

                        CreateRaycasterObject(name, targetPos + offset, targetPos);
                    }
                }
            }
        }

        GenerateAllColliderObject(starts);

        Debug.Log("Objects count: " + (counts.x * counts.y * counts.z));
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

    private void GenerateAllColliderObject(Vector3 starts)
    {
        (int xCount, int yCount, int zCount) = ((int)counts.x, (int)counts.y, (int)counts.z);

        for (int z = 0; z < zCount; ++z) {
            for (int y = 0; y < yCount; ++y) {
                for (int x = 0; x < xCount; ++x) {
                    GenerateColliderObject(
                        position: new Vector3(
                            x: starts.x + x * margins.x, 
                            y: starts.y + y * margins.y, 
                            z: starts.z + z * margins.z));
                }
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