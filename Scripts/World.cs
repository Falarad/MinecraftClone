using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class World : MonoBehaviour {

    public Material material;
    public BlockType[] blocktypes;

    Chunk[,] chunks = new Chunk[VoxelData.WorldSizeInChunks,VoxelData.WorldSizeInChunks];

    private void Start() {
      GenerateWorld();
    }

    void GenerateWorld () {
      for(int x = 0; x < VoxelData.WorldSizeInChunks; x++){
        for(int z = 0; z < VoxelData.WorldSizeInChunks; z++){

          CreateNewChunk(x,z);

        }
      }
    }

    public byte GetVoxel(Vector3 pos){

      if(!IsVoxelInWorld(pos))
        return 0;
      if(pos.y < 3)
        return 3;
      else if (pos.y == VoxelData.ChunkHeight - 1)
        return 1;
      else if (pos.y < VoxelData.ChunkHeight - 3)
        return 0;
      else
        return 2;

    }

    void CreateNewChunk(int x, int z){
      chunks[x,z] = new Chunk(new ChunkCoord(x,z), this);
    }

    bool IsVoxelInChunk(ChunkCoord coord){

      if(coord.x > 0 && coord.x < VoxelData.WorldSizeInChunks  && coord.z > 0 && coord.z < VoxelData.WorldSizeInChunks)
        return true;
      else
        return false;

    }

    bool IsVoxelInWorld(Vector3 pos){

      if(pos.x >= 0 && pos.x < VoxelData.WorldSizeInVoxels && pos.z >= 0 && pos.z < VoxelData.WorldSizeInVoxels && pos.y >= 0 && pos.y < VoxelData.ChunkHeight)
        return true;
      else
        return false;

    }

}

[System.Serializable]
public class BlockType {

  public string blockName;
  public bool isSolid;

  [Header ("Texture Values")]
  public int backFaceTexture;
  public int frontFaceTexture;
  public int topFaceTexture;
  public int bottomFaceTexture;
  public int leftFaceTexture;
  public int rightFaceTexture;

  //Back, Front, Top, Bottom, Left, Right

  public int GetTextureID (int faceIndex) {
    switch (faceIndex) {
      case 0:
        return backFaceTexture;
      case 1:
        return frontFaceTexture;
      case 2:
        return topFaceTexture;
      case 3:
        return bottomFaceTexture;
      case 4:
        return leftFaceTexture;
      case 5:
        return rightFaceTexture;
      default:
        Debug.Log("Error in GetTextureID. Invalid face index.");
        return 0;
    }
  }

}
