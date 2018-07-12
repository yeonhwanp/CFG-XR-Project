using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.MagicLeap;
using UnityEngine.Experimental.XR;
using System.IO;
using Google.Protobuf;
using System.Runtime.InteropServices;

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
    private void Update()
    {
        // First, grab all of the meshes into a new MeshList. Next, send them over via UDP.
        // Seems as if meshes don't change if nothing happens... but what happens when something does happen?
        // How do we know if the environment changed or not? Is there a way? We can check transform.childcount... but is that the best way?
        // Also if this works, then how do we priority queue the stuff in case of a bigger room?
        List<MeshList> listOfMeshList = new List<MeshList>();
        MeshList testList = new MeshList();

        if (Input.GetKeyDown(KeyCode.Space))
        {
            UpdateMeshMaterial(testList, listOfMeshList);
            foreach (MeshList list in listOfMeshList)
            {
                ClientUDP<MeshList>.UDPSend(1234, list);
            }
        }
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
    private void UpdateMeshMaterial(MeshList testList, List<MeshList> listofMeshes)
    {
        List<int> sentList = new List<int>();
        
        // Loop over all the child mesh nodes created by MLSpatialMapper script
        for (int i = 0; i < transform.childCount; i++)
        {
            // Get the child gameObject then add the mesh to the list
            GameObject gameObject = transform.GetChild(i).gameObject;

            if (testList.Meshes.Count <= 40)
            {
                AddMeshProto(gameObject, testList, sentList);
            }
            else if (i == transform.childCount - 1)
            {
                listofMeshes.Add(testList);
                testList = new MeshList();
            }
            else
            {
                listofMeshes.Add(testList);
                testList = new MeshList();
            }
        }
    }

    // For each Mesh, creates a new MeshProto and adds it to the provided meshList
    private void AddMeshProto(GameObject meshObject, MeshList meshList, List<int> confirmed)
    {
        MeshProto newMeshProto = new MeshProto();
        Mesh sharedMesh = meshObject.GetComponent<MeshCollider>().sharedMesh;

        List<MeshProto> protoList = new List<MeshProto>();

        // Don't know if null is a problem but... yea temp fix
        // Ok, need to put the newVectors into Dictionaries so it can actually find the value
        if (sharedMesh != null)
        {
            foreach (int triangle in sharedMesh.triangles)
            {
                newMeshProto.Triangles.Add(triangle);
                confirmed.Add(triangle);
                if (confirmed.Contains(triangle) == false)
                {
                    ProtoVector3 newVector = new ProtoVector3();
                    newVector.X = sharedMesh.vertices[triangle].x;
                    newVector.Y = sharedMesh.vertices[triangle].y;
                    newVector.Z = sharedMesh.vertices[triangle].z;
                    newMeshProto.Vertices.Add(newVector);
                }

            }
        }
        meshList.Meshes.Add(newMeshProto);
    }

    //private int CheckMeshProtoSize(MeshList proto)
    //{
    //    using (MemoryStream ms = new MemoryStream())
    //    {
    //        proto.WriteTo(ms);
    //        ms.Position = 0;
    //        byte[] array = ms.ToArray();
    //        return array.Length;
    //    }
    //}
    #endregion
}