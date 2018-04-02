using System;
using UnityEngine;

/// <summary>
/// Handles turning a DisplayReadySlice into something that can be displayed
/// </summary>
public class MapDisplayData {

    private const int lowLod = 20;
    public DisplayReadySlice mapData;
	private TerrainType[] regions;

	public Texture2D texture;
	public Mesh mesh;
	public Mesh lowLodMesh;
	public MapDisplayStatus status;
    private AreaDisplay areaDisplay;

    public MapDisplayData(){}

    public MapDisplayData(DisplayReadySlice mapData) {
        this.SetMapData(mapData);
    }

    public void SetMapData(DisplayReadySlice mapData) {
        this.mapData    = mapData;
		int originalLod = mapData.lod;
		mapData.lod     = lowLod;
		lowLodMesh      = GenerateMesh();
		mapData.lod     = originalLod;
    }

    private Color[] CalculateColourMap(MapData mapData) {
		int width  = mapData.GetWidth();
		int height = mapData.GetHeight();
        if (areaDisplay == null) {
            areaDisplay = GameObject.FindObjectOfType<AreaDisplay>();
        }
		Color[] colourMap = new Color[width * height];
        MapDataSlice slice = (MapDataSlice)mapData;
		
        for (int y = 0; y < height; y++) {
			for (int x = 0; x < width; x++) {
				float currentHeight = mapData.GetSquished(x, y);
                float scaledPosX = (slice.GetX() + x);
                float scaledPosY = (slice.GetY() + y);
                Color areaColor = areaDisplay.GetAreaColor(scaledPosX, scaledPosY);
				Color regionColor = GetRegionColour(currentHeight);

				if (areaColor != Color.black) {
					regionColor = areaColor - regionColor;
				}

                colourMap[y * width + x] = regionColor;
			}
		}
		return colourMap;
	}

    public Color GetRegionColour(float currentHeight) {
        for (int i = 0; i < regions.Length; i++) {
            if (currentHeight <= regions[i].height) {
                return regions[i].colour;
            }
        }
        return Color.white;
    }

    public void SetRegions(TerrainType[] regions) {
		this.regions = regions;
	}

	public void SetStatus(MapDisplayStatus newStatus) {
		this.status = newStatus;
	}

    private Mesh GenerateMesh() {
		return FixNormals(MeshGenerator.GenerateTerrainMesh(mapData).CreateMesh());
	}

    private void FillInNormals(Vector3[] normals, int coord, int inc, 
        Func<int, int> indexFunc, Vector3 normal) {
        
        if(inc > 1 && coord > 0) {
            for(int i = 1; i < inc; i++) {
                normals[indexFunc(coord - i)] = normal;
            }
        }

    }

    private void FixNormalEdge(
        Vector3[] first, int firstDimension, Func<int, int> firstIndexFunc,
        Vector3[] second, int secondDimension, Func<int, int> secondIndexFunc
        ) {
        
        int firstCoord = 0, secondCoord = 0;
        int firstInc = secondDimension / firstDimension > 0 ? secondDimension / firstDimension : 1;
        int secondInc = firstDimension / secondDimension > 0 ? firstDimension / secondDimension : 1;
        while(firstCoord < firstDimension && secondCoord < secondDimension) {
            int firstIndex = firstIndexFunc(firstCoord);
            int secondIndex = secondIndexFunc(secondCoord);
            Vector3 firstNormal = first[firstIndex];
            Vector3 secondNormal = second[secondIndex];
            Vector3 consensus = (firstNormal + secondNormal);
            consensus.Normalize();
            first[firstIndex] = consensus;
            second[secondIndex] = consensus;
            FillInNormals(first, firstCoord, firstInc, firstIndexFunc, consensus);
            FillInNormals(second, secondCoord, secondInc, secondIndexFunc, consensus);
            firstCoord += firstInc;
            secondCoord += secondInc;
        }

    }

    private void FixNormals(
        Vector3[] first, int firstWidth, int firstHeight,
        Vector3[] second, int secondWidth, int secondHeight,
        NeighborType relation) {
        
        if(relation == NeighborType.TopBottom) {
            FixNormalEdge(
                first, firstWidth, (x) => firstHeight*(firstWidth - 1) + x,
                second, secondWidth, (x) => x
            );
        } else if (relation == NeighborType.LeftRight) {
            FixNormalEdge(
                first, firstHeight, (y) => y * firstWidth + (firstWidth - 1),
                second, secondHeight, (y) => y * secondWidth
            );
        } else throw new System.ArgumentException("Unsupported NeighborType " + relation);

    }

    private Mesh FixNormals(Mesh mesh) {
        Vector3[] normals = mesh.normals;
        int width = MeshGenerator.GetVerticesPerDimension(mapData.GetWidth(), GetActualLOD());
        int height = MeshGenerator.GetVerticesPerDimension(mapData.GetHeight(), GetActualLOD());
        foreach(DisplayNeighborRelation relation in mapData.GetDisplayNeighbors()) {
            MapDisplay other = relation.GetOtherDisplay(mapData);
            if(other == null || other.GetStatus() == MapDisplayStatus.HIDDEN) continue;
            Mesh otherMesh = relation.GetOtherMesh(mapData);
            DisplayReadySlice otherSlice = relation.GetOtherDRSlice(mapData);
            Vector3[] otherNormals = otherMesh.normals;
            int otherWidth =  MeshGenerator.GetVerticesPerDimension(otherSlice.GetWidth(), other.GetActualLOD());
            int otherHeight = MeshGenerator.GetVerticesPerDimension(otherSlice.GetHeight(), other.GetActualLOD());
            if(relation.IsFirstMember(mapData)) {
                FixNormals(normals, width, height,
                              otherNormals, otherWidth, otherHeight,
                              relation.neighborType);
            } else {
                FixNormals(otherNormals, otherWidth, otherHeight,
                              normals, width, height,
                              relation.neighborType);
            }
            otherMesh.normals = otherNormals;
        }
        mesh.normals = normals;
        return mesh;
    }

    public Texture2D GenerateTexture() {
		if (regions != null)
			return TextureGenerator.TextureFromColourMap(CalculateColourMap(mapData), mapData.GetWidth(), mapData.GetHeight());
		else
			return TextureGenerator.TextureFromHeightMap(mapData);
	}

    public void UpdateLOD(int lod) {
		if(mapData.lod != lod) {
			mapData.lod = lod;
			mesh = GenerateMesh();
		}
	}

    public int GetActualLOD() {
        return status == MapDisplayStatus.LOW_LOD ? lowLod * 2 : mapData.GetActualLOD();
    }

    public MapDisplayStatus PrepareDraw() {
        if(texture == null) texture = GenerateTexture();
		switch(this.status) {
			case MapDisplayStatus.VISIBLE:
				if(mesh == null) mesh = GenerateMesh();
				break;
			case MapDisplayStatus.LOW_LOD:
				break;
			case MapDisplayStatus.HIDDEN:
				break;
		}
        return this.status;
    }

    public Mesh GetMesh() {
        return (this.status == MapDisplayStatus.VISIBLE && mesh != null) ? mesh : lowLodMesh;
    }

    public Texture2D GetTexture() {
        return texture;
    }

    public float GetScale() {
        return mapData.GetScale();
    }
    
}
