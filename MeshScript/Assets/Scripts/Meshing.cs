using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.MagicLeap;
using UnityEngine.Experimental.XR;
using System.IO;
using Google.Protobuf;
using System.Runtime.InteropServices;
using Priority_Queue;

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
    private float _remainingTime = 3; // use this to minimize "lag" -- will need to test (idea is to ignore unsent packets after this amount of time)
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

    /// <summary>
    /// Uses a priority queue to sort based on distance.
    /// 1. Puts all of the meshes into the priority queue.
    /// 2. Pops out based on least distance, adding to the MeshList. Once it gets too big, it gets added to listofMeshes.
    /// 3. Above, in update, goes through the listofMeshes which should also be in order.
    /// </summary>
    #region Private Methods
    private void AddToList(MeshList testList, List<MeshList> listofMeshes)
    {
        List<int> sentList = new List<int>();
        SimplePriorityQueue<GameObject, float> priorityq = new SimplePriorityQueue<GameObject, float>(); 

        for (int i = 0; i < transform.childCount; i++)
        {
            priorityq.Enqueue(transform.GetChild(i).gameObject, GetDistance(transform.GetChild(i).gameObject, Camera));
        }

        int counter = 0;
        while (priorityq.Count != 0)
        {
            GameObject gameObject = priorityq.Dequeue();

            if (testList.Meshes.Count <= 40)
            {
                AddMeshProto(gameObject, testList, sentList);
            }
            else if (counter == transform.childCount - 1)
            {
                listofMeshes.Add(testList);
                testList = new MeshList();
            }
            else
            {
                listofMeshes.Add(testList);
                testList = new MeshList();
            }

            counter++;
        }
    }

    /// <summary>
    /// Gets distance between camera and gameObject.
    /// </summary>
    private float GetDistance(GameObject other, GameObject Camera)
    {
        float distance = Mathf.Pow(Mathf.Pow(other.transform.position.x, 2) + Mathf.Pow(other.transform.position.y, 2) + Mathf.Pow(other.transform.position.z, 2), .5f);
        return distance;
    }
    
    /// <summary>
    /// Adds a MeshProto into a MeshList for sending over.
    /// </summary>
    private void AddMeshProto(GameObject meshObject, MeshList meshList, List<int> confirmed)
    {
        MeshProto newMeshProto = new MeshProto();
        Mesh sharedMesh = meshObject.GetComponent<MeshCollider>().sharedMesh;

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