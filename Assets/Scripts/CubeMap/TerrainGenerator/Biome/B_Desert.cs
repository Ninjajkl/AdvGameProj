﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class B_Desert : Biome
{

	[Tooltip("The max deep and height of the snow dunes, low values")][Range(0, TerrainConstants.MAX_HEIGHT - 1)]
	public int maxHeightDifference = TerrainConstants.MAX_HEIGHT / 5;

	[Header("Texture generation")]
	[Tooltip("Number vertex (y), where the sand end and the rock start")][Range(0, TerrainConstants.MAX_HEIGHT - 1)]
	public int sandDeep = TerrainConstants.MAX_HEIGHT / 5;
	public override byte[] GenerateChunkData(Vector2Int vecPos, float[] biomeMerge)
	{
		byte[] chunkData = new byte[TerrainConstants.CHUNK_BYTES];
		float[] noise = NoiseManager.GenerateNoiseMap(scale, octaves, persistance, lacunarity, vecPos);
		for (int z = 0; z < TerrainConstants.CHUNK_VERTEX_SIZE; z++)
		{
			for (int x = 0; x < TerrainConstants.CHUNK_VERTEX_SIZE; x++)
			{
				// Get surface height of the x,z position 
				float height = Mathf.Lerp(
					NoiseManager.Instance.worldConfig.surfaceLevel,//Biome merge height
					(((terrainHeightCurve.Evaluate(noise[x + z * TerrainConstants.CHUNK_VERTEX_SIZE]) * 2 - 1) * maxHeightDifference) + NoiseManager.Instance.worldConfig.surfaceLevel),//Desired biome height
					biomeMerge[x + z * TerrainConstants.CHUNK_VERTEX_SIZE]);//Merge value,0 = full merge, 1 = no merge

				int heightY = Mathf.CeilToInt(height);//Vertex Y where surface start
				int lastVertexWeigh = (int)((255 - isoLevel) * (height % 1) + isoLevel);//Weigh of the last vertex

				for (int y = 0; y < TerrainConstants.CHUNK_VERTEX_HEIGHT; y++)
				{
					int index = (x + z * TerrainConstants.CHUNK_VERTEX_SIZE + y * TerrainConstants.CHUNK_VERTEX_AREA) * TerrainConstants.CHUNK_POINT_BYTE;
					if (y < heightY - sandDeep)
					{
						chunkData[index] = 255;
						chunkData[index + 1] = 4;//Rock
					}
					else if (y < heightY)
					{
						chunkData[index] = 255;
						chunkData[index + 1] = 6;//sand
					}
					else if (y == heightY)
					{
						chunkData[index] = (byte)lastVertexWeigh;
						chunkData[index + 1] = 6;//sand

					}
					else
					{
						chunkData[index] = 0;
						chunkData[index + 1] = TerrainConstants.NUMBER_MATERIALS;
					}
				}
			}
		}
		return chunkData;
	}
}
