﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chunk : MonoBehaviour
{
    [Tooltip("Active gizmos that represent the area of the chunk")]
    public bool debug = false;
    [Tooltip("Active gizmos that represent all vertex of terrain, very expensive")]
    public bool advancedDebug = false;
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

        //Assign random color
        //mat mymaterial = new mat(Shader.Find("Custom/Geometry/FlatShading"));//Custom/DoubleFaceShader  |   Specular
        //mymat.color = new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f));
        GetComponent<MeshRenderer>().material = ChunkManager.Instance.terrainMaterial;
        gameObject.AddComponent<MeshCollider>();

        return this;
    }


    /// <summary>
    /// TEMPORAL CODE. Generate plain chunk with sample data.
    /// </summary>
    public static byte[] GenerateSampleData()
    {
        int offset = 289;
        byte[] chunkData = new byte[Constants.CHUNK_TOTAL_VERTEX * Constants.CHUNK_POINT_BYTE];
        for (int j = 0; j < Constants.CHUNK_BYTES / 2 + offset; j += 2)
        {
            chunkData[j] = 255;//terrain
            if (j < Constants.CHUNK_BYTES * 2 / 5)
                chunkData[j + 1] = 2;//stone
            else if (j < Constants.CHUNK_BYTES * 2.438f / 5)
                chunkData[j + 1] = 1;//dirt
            else
                chunkData[j + 1] = 0;//grass
        }

        for (int j = Constants.CHUNK_BYTES / 2 + offset; j < Constants.CHUNK_BYTES; j += 2)
        {
            chunkData[j] = 0;//air
            chunkData[j + 1] = 255;//type material, now empty
        }

        return chunkData;
    }

    public void Update()
    {
        if (modified)
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
    public void modifyTerrain(Vector3 vertexPoint, int modification, int mat = 0)
    {
        if (modification > 0)
            addTerrain(vertexPoint, modification, mat);//A little more costly
        else
            removeTerrain(vertexPoint, modification);//Less operations
    }

    /// <summary>
    /// Remove terrain in the chunk,
    /// </summary>
    public void removeTerrain(Vector3 vertexPoint, int modification)
    {
        int byteIndex = ((int)vertexPoint.x + (int)vertexPoint.z * Constants.CHUNK_VERTEX_SIZE + (int)vertexPoint.y * Constants.CHUNK_VERTEX_AREA) * Constants.CHUNK_POINT_BYTE;

        int value = data[byteIndex];
        int newValue = Mathf.Clamp(value + modification, 0, 255);

        if (value == newValue)
            return;

        data[byteIndex] = (byte)newValue;
        modified = true; //Don't direct change because some vertex are modifier in the same editions, wait to next frame
    }

    /// <summary>
    /// Similar to the removeTerrain, but when we add terrain we need indicate a color.
    /// </summary>
    public void addTerrain(Vector3 vertexPoint, int modification, int mat)
    {
        int isoSurface = MeshBuilder.Instance.isoLevel;
        int byteIndex = ((int)vertexPoint.x + (int)vertexPoint.z * Constants.CHUNK_VERTEX_SIZE + (int)vertexPoint.y * Constants.CHUNK_VERTEX_AREA) * Constants.CHUNK_POINT_BYTE;

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
    /// Save the chunk data in the region if the chunk get some changes.
    /// </summary>
    public void saveChunkInRegion()
    {
        if (changesUnsaved)
            fatherRegion.saveChunkData(data, Xpos, Zpos);
    }



    //Used for visual debug
    void OnDrawGizmos()
    {
        if (debug)
        {
            //Gizmos.color = new Color(1f,0.28f,0f);
            Gizmos.color = Color.Lerp(Color.red, Color.magenta, ((transform.position.x + transform.position.z) % 100) / 100);


            Gizmos.DrawWireCube(transform.position, new Vector3(Constants.CHUNK_SIDE, Constants.MAX_HEIGHT * Constants.VOXEL_SIDE, Constants.CHUNK_SIDE));
        }
        if (advancedDebug)
        {

            Gizmos.color = Color.Lerp(Color.red, Color.magenta, ((transform.position.x + transform.position.z) % 100) / 100);

            Gizmos.matrix = transform.localToWorldMatrix;

            float isoLevel = MeshBuilder.Instance.isoLevel;
            for (int y = 0; y < Constants.CHUNK_VERTEX_HEIGHT; y++)//height
            {
                for (int z = 0; z < Constants.CHUNK_VERTEX_SIZE; z++)//column
                {
                    for (int x = 0; x < Constants.CHUNK_VERTEX_SIZE; x++)//line 
                    {

                        if (data[(x + z * Constants.CHUNK_VERTEX_SIZE + y * Constants.CHUNK_VERTEX_AREA) * Constants.CHUNK_POINT_BYTE] > isoLevel)
                        {
                            Gizmos.DrawSphere(new Vector3((x - Constants.CHUNK_SIZE / 2) * Constants.VOXEL_SIDE,
                                (y - Constants.MAX_HEIGHT / 2) * Constants.VOXEL_SIDE,
                                (z - Constants.CHUNK_SIZE / 2) * Constants.VOXEL_SIDE)
                                , 0.2f);
                        }
                    }
                }

            }
        }
    }
}


