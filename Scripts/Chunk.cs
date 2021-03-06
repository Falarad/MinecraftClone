using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chunk {
    public ChunkCoord coord;
    GameObject chunkObject;
    MeshRenderer meshRenderer;
    MeshFilter meshFilter;

    int vertexIndex = 0;
    List<Vector3> vertices = new List<Vector3>();
    List<int> triangles = new List<int>();
    List<Vector2> uvs = new List<Vector2>();

    byte [,,] voxelMap = new byte[VoxelData.ChunkWidth, VoxelData.ChunkHeight, VoxelData.ChunkWidth];

    World world;
    // Start is called before the first frame update
    public Chunk (ChunkCoord _coord, World _world){
      coord = _coord;
      world = _world;

      chunkObject = new GameObject();
      meshFilter = chunkObject.AddComponent<MeshFilter>();
      meshRenderer = chunkObject.AddComponent<MeshRenderer>();

      meshRenderer.material = world.material;
      chunkObject.transform.SetParent(world.transform);
      chunkObject.transform.position = new Vector3(coord.x * VoxelData.ChunkWidth, 0f, coord.z * VoxelData.ChunkWidth);
      chunkObject.name = "Chunk " + coord.x + ", " + coord.z;

      PopVoxelMap();
      CreateChunk();
      CreateMesh();

    }

    void CreateChunk(){

      for(int y = 0; y < VoxelData.ChunkHeight; y++){
        for(int x = 0; x < VoxelData.ChunkWidth; x++){
          for(int z = 0; z < VoxelData.ChunkWidth; z++){

            AddVoxelDataToChunk(new Vector3(x,y,z));

          }
        }
      }

    }

    bool CheckVoxel(Vector3 pos){

      int x = Mathf.FloorToInt(pos.x);
      int y = Mathf.FloorToInt(pos.y);
      int z = Mathf.FloorToInt(pos.z);
      if(!IsVoxelInChunk(x, y, z))
        return !world.blocktypes[world.GetVoxel(pos + position)].isSolid;
      else
        return world.blocktypes[voxelMap[x, y, z]].isSolid;
    }

    void PopVoxelMap (){
      for(int y = 0; y < VoxelData.ChunkHeight; y++){
        for(int x = 0; x < VoxelData.ChunkWidth; x++){
          for(int z = 0; z < VoxelData.ChunkWidth; z++){
            voxelMap[x,y,z] = world.GetVoxel(new Vector3(x,y,z) + position);
          }
        }
      }
    }

    void AddVoxelDataToChunk (Vector3 pos){

      for(int p = 0; p < 6; p++){

        if(!CheckVoxel(pos + VoxelData.faceChecks[p])){
          byte blockID = voxelMap[(int)pos.x, (int)pos.y, (int)pos.z];
          for(int i = 0; i < 4; i++){
            vertices.Add(pos + VoxelData.voxelVerts[VoxelData.voxelTris[p,i]]);
          }

          AddTexture(world.blocktypes[blockID].GetTextureID(p));

          triangles.Add(vertexIndex);
          triangles.Add(vertexIndex+1);
          triangles.Add(vertexIndex+2);
          triangles.Add(vertexIndex+2);
          triangles.Add(vertexIndex+1);
          triangles.Add(vertexIndex+3);
          vertexIndex += 4;
        }
      }

    }

    void CreateMesh(){
      Mesh mesh = new Mesh();
      mesh.vertices = vertices.ToArray();
      mesh.triangles = triangles.ToArray();

      mesh.uv = uvs.ToArray();

      mesh.RecalculateNormals();
      meshFilter.mesh = mesh;
    }

    public bool isActive{
      get { return chunkObject.activeSelf; }
      set { chunkObject.SetActive(value); }
    }

    public Vector3 position {
      get { return chunkObject.transform.position; }
    }

    void AddTexture (int textureID){

      float y = textureID / VoxelData.TextureAtlasSizeInBlocks;
      float x = textureID - (y * VoxelData.TextureAtlasSizeInBlocks);

      x = x * VoxelData.NormalizedBlockTextureSize;
      y = 1f - (y * VoxelData.NormalizedBlockTextureSize) - VoxelData.NormalizedBlockTextureSize;

      uvs.Add(new Vector2(x,y));
      uvs.Add(new Vector2(x,y + VoxelData.NormalizedBlockTextureSize));
      uvs.Add(new Vector2(x + VoxelData.NormalizedBlockTextureSize,y));
      uvs.Add(new Vector2(x + VoxelData.NormalizedBlockTextureSize,y + VoxelData.NormalizedBlockTextureSize));
    }

    bool IsVoxelInChunk(int x, int y, int z){

      if(x < 0 || x > VoxelData.ChunkWidth - 1 || y < 0 || y > VoxelData.ChunkHeight - 1 || z < 0 || z > VoxelData.ChunkWidth - 1)
        return false;
      return true;

    }

}

public class ChunkCoord {
  public int x;
  public int z;

  public ChunkCoord(int _x, int _z){
    x = _x;
    z = _z;
  }
}
