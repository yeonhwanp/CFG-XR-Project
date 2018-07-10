using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.MagicLeap;
using UnityEngine.Experimental.XR;

public class Meshing : MonoBehaviour
{
    #region Public Variables
    public Material BlackMaterial;
    public Material GroundMaterial;
    public Material InactiveMaterial;
    public MLSpatialMapper _mapper;
    public MeshList testList;
    #endregion

    #region Private Variables
    private bool _visible = true;
    #endregion

    #region Unity Methods
    private void Start()
    {
        MeshList testList = new MeshList();
    }

    private void Update()
    {
        // First, grab all of the meshes into a new MeshList. Next, send them over via UDP.
        // Seems as if meshes don't change if nothing happens... but what happens when something does happen?
        // How do we know if the environment changed or not? Is there a way? We can check transform.childcount... but is that the best way?
        UpdateMeshMaterial();

    }
    #endregion

    #region Public Methods
    public void ToggleMeshVisibility()
    {
        _visible = _visible ? false : true;
    }
    public void ToggleMeshScanning()
    {
        _mapper.enabled = _mapper.enabled ? false : true;
    }
    #endregion

    #region Private Methods
    /// Switch mesh material based on whether meshing is active and mesh is visible
    /// visible & active = ground material
    /// visible & inactive = meshing off material
    /// invisible = black mesh
    private void UpdateMeshMaterial()
    {
        // Loop over all the child mesh nodes created by MLSpatialMapper script
        // Aka just looping through all of them. Cool.
        // Going to assume that in the real verison only scans what it sees?
        // So just loop through them, get vertices and trianges for each, put into protobuf, send over server.
        // See what happens? How big are the files...
        // Data structure coming later.
        for (int i = 0; i < transform.childCount; i++)
        {
            // Get the child gameObject then add the mesh to the list
            GameObject gameObject = transform.GetChild(i).gameObject;
            AddMeshProto(gameObject, testList);
        }
    }

    // For each Mesh, creates a new MeshProto and adds it to the provided meshList
    private void AddMeshProto(GameObject meshObject, MeshList meshList)
    {
        MeshProto newMeshProto = new MeshProto();

        foreach (int triangle in meshObject.GetComponent<Mesh>().triangles)
        {
            newMeshProto.Triangles.Add(triangle);
        }

        foreach (Vector3 vertex in meshObject.GetComponent<Mesh>().vertices)
        {
            ProtoVector3 newVector = new ProtoVector3();
            newVector.X = vertex.x;
            newVector.Y = vertex.y;
            newVector.Z = vertex.z;
            newMeshProto.Vertices.Add(newVector);
        }

        meshList.Meshes.Add(newMeshProto);
    }
    #endregion
}