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
    public GameObject Camera;
    #endregion

    #region Private Variables
    private bool _visible = true;
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

    #region Unity Methods
    private void Update()
    {
        // Iterate through this when sending the Meshes (pieces of info)
        List<MeshList> listOfMeshList = new List<MeshList>();
        MeshList sendList = new MeshList();

        // Send upon keydown
        if (Input.GetKeyDown(KeyCode.Space))
        {
            AddToList(sendList, listOfMeshList);
            foreach (MeshList list in listOfMeshList)
            {
                ClientUDP<MeshList>.UDPSend(1234, list);
            }
        }
    }
    #endregion

    #region Private Methods
    private void AddToList(MeshList testList, List<MeshList> listofMeshes)
    {
        List<int> sentList = new List<int>();
        
        // Loop over all the child mesh nodes created by MLSpatialMapper script
        for (int i = 0; i < transform.childCount; i++)
        {
            GameObject gameObject = transform.GetChild(i).gameObject;

            // Size limit --> could limit if it gets too big.
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

    private void AddMeshProto(GameObject meshObject, MeshList meshList, List<int> confirmed)
    {
        MeshProto newMeshProto = new MeshProto();
        Mesh sharedMesh = meshObject.GetComponent<MeshCollider>().sharedMesh;

        List<MeshProto> protoList = new List<MeshProto>();

        // Don't know if null is a problem but... yea temp fix
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
                    newMeshProto.Vertices[triangle] = (newVector);
                }

            }
        }
        meshList.Meshes.Add(newMeshProto);
    }
    #endregion
}