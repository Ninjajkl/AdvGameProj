﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Constants;

public class Chunk : MonoBehaviour
{
    [Tooltip("Active gizmos that represent the area of the chunk")]
    public bool debug = false;
    private byte[] data;
    private int Xpos;
    private int Zpos;
    private Region fatherRegion;
    private bool modified = false;
    private bool changesUnsaved;

    /// <summary>
    /// Create a Chunk using a byte[] that contain all the data of the chunk.
    /// </summary>
    /// <param name="b"> data of the chunk</param>
    public Chunk ChunkInit(byte[] b, int x, int z, Region region, bool save)
    {
        data = b;
        Xpos = x;
        Zpos = z;
        fatherRegion = region;
        changesUnsaved = save;

        Mesh myMesh = MeshBuilder.Instance.BuildChunk(b);
        GetComponent<MeshFilter>().mesh = myMesh;

        //Assign random color, new material each chunk.
        //mat mymaterial = new mat(Shader.Find("Custom/Geometry/FlatShading"));//Custom/DoubleFaceShader  |   Specular
        //mymat.color = new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f));
        GetComponent<MeshRenderer>().material = ChunkManager.Instance.terrainMaterial;
        gameObject.AddComponent<MeshCollider>();

        return this;
    }

    public void Update()
    {
        if(modified)
        {
            modified = false;
            changesUnsaved = true;

            Mesh myMesh = MeshBuilder.Instance.BuildChunk(data);
            GetComponent<MeshFilter>().mesh = myMesh;
            GetComponent<MeshCollider>().sharedMesh = myMesh;
        }
    }

    /// <summary>
    /// Call depending of the type of modification to removeTerrain or addTerrain
    /// </summary>
    /// <param name="vertexPoint"></param>
    /// <param name="modification"></param>
    /// <param name="mat"></param>
    public void modifyTerrain(Vector3 vertexPoint, int modification, int mat = 0, int hardness = 9)
    {
        if (modification > 0)
            addTerrain(vertexPoint,modification, mat);//A little more costly
        else
            removeTerrain(vertexPoint,modification, hardness);//Less operations
    }

    /// <summary>
    /// Remove terrain in the chunk,
    /// </summary>
    public void removeTerrain(Vector3 vertexPoint, int modification, int hardness = 9)
    {
        int byteIndex = ((int)vertexPoint.x + (int)vertexPoint.z * TerrainConstants.CHUNK_VERTEX_SIZE + (int)vertexPoint.y * TerrainConstants.CHUNK_VERTEX_AREA) * TerrainConstants.CHUNK_POINT_BYTE;

        int value = data[byteIndex];
        int matVal = data[byteIndex + 1];
        int newValue = Mathf.Clamp(value + modification, 0, 255);

        if (value == newValue)
            return;

        // If trying to break something harder than the drill, don't
        if (matVal > hardness)
        {
            return;
        }

        if (ChunkManager.Instance.editorMode == true)
        {
            // Can break bedrock, does not go to inventory
        }
        else
        {
            // Add material to inventory otherwise
            GameManager.Instance.Player.Inventory[matVal] += 1;
        }

        data[byteIndex] = (byte)newValue;
        modified = true; //Don't direct change because some vertex are modifier in the same editions, wait to next frame
    }

    /// <summary>
    /// Similar to the removeTerrain, but when we add terrain we need indicate a color.
    /// </summary>
    public void addTerrain(Vector3 vertexPoint,int modification, int mat)
    {
        int isoSurface = MeshBuilder.Instance.isoLevel;
        int byteIndex = ((int)vertexPoint.x + (int)vertexPoint.z * TerrainConstants.CHUNK_VERTEX_SIZE + (int)vertexPoint.y * TerrainConstants.CHUNK_VERTEX_AREA) * TerrainConstants.CHUNK_POINT_BYTE ;

        int value = data[byteIndex];
        int newValue = Mathf.Clamp(value + modification, 0, 255);

        if (value == newValue)
            return;
        if (value < isoSurface && newValue >= isoSurface)
            data[byteIndex + 1] = (byte)mat;


        data[byteIndex] = (byte)newValue;
        modified = true; //Don't direct change because some vertex are modifier in the same editions, wait to next frame
    }

    /// <summary>
    /// Get the material(byte) from a specific point in the chunk
    /// </summary>
    public byte GetMaterial(Vector3 vertexPoint)
    {
        int byteIndex = ((int)vertexPoint.x + (int)vertexPoint.z * TerrainConstants.CHUNK_VERTEX_SIZE + (int)vertexPoint.y * TerrainConstants.CHUNK_VERTEX_AREA) * TerrainConstants.CHUNK_POINT_BYTE;
        return data[byteIndex + 1];
    }

    /// <summary>
    /// Save the chunk data in the region if the chunk get some changes.
    /// </summary>
    public void saveChunkInRegion()
    {
        if(changesUnsaved)
            fatherRegion.saveChunkData(data, Xpos, Zpos);
    }

#if UNITY_EDITOR
    //Used for visual debug
    void OnDrawGizmos()
    {
        if (debug)
        {
            //Gizmos.color = new Color(1f,0.28f,0f);
            Gizmos.color = Color.Lerp(Color.red, Color.magenta, ((transform.position.x + transform.position.z) % 100) / 100);


            Gizmos.DrawWireCube(transform.position,new Vector3(TerrainConstants.CHUNK_SIDE, TerrainConstants.MAX_HEIGHT * TerrainConstants.VOXEL_SIDE, TerrainConstants.CHUNK_SIDE));
        }
    }
#endif
}


