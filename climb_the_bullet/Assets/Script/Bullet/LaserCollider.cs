using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserCollider : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        MeshCrate();

    }

    private void MeshCrate()
    {
        //TrailRendererの頂点情報からメッシュを生成する
        TrailRenderer paintObjectTrailRenderer = this.GetComponent<TrailRenderer>();
        //子にコライダーだけ持つオブジェクトを作成する
        GameObject colliderContainer = new GameObject("Collider Container");
        colliderContainer.transform.SetParent(this.transform);
        MeshCollider meshCollider = colliderContainer.AddComponent<MeshCollider>();
        Mesh mesh = new Mesh();
        paintObjectTrailRenderer.BakeMesh(mesh);
        meshCollider.sharedMesh = mesh;
    }
}
