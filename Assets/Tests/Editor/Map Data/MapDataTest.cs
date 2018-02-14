﻿using UnityEngine;
using UnityEditor;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;

public class MapDataTest {
    public MapData mapdata;
    private double precision = 0.00001;

    [SetUp]
    public void Setup() {
        MapMetadata metadata = new MapMetadata();
        metadata.Set("cellsize", "2");
        metadata.Set("minheight", "1");
        metadata.Set("maxheight", "6");
        float[,] data = new float[2, 3] { { 1.0F, 2.0F, 3.0F }, { 4.0F, 5.0F, 6.0F } };
        this.mapdata = new MapData(data, metadata);
    }

    [Test]
    public void SetWorks() {
        mapdata.Set(0, 2, -1.0F);
        Assert.True(mapdata.GetRaw(0, 2) == -1.0F);
    }

    [Test]
    public void GetHeightWorks() {
        Assert.True(mapdata.GetHeight() == 3, "GetHeight() returns incorrect value.");
    }

    [Test]
    public void GetWidthWorks() {
        Assert.True(mapdata.GetWidth() == 2, "GetWidth() returns incorrect value.");
    }

    [Test]
    public void GetScaleWorks() {
        Assert.True(mapdata.GetScale() - 0.333333 < precision, "GetScale() returns incorrect value.");
    }

    [Test]
    public void GetTopLeftWorks() {
        Assert.True(mapdata.GetTopLeft().x == -0.5F, "GetTopLeft() returns incorrect vector x coordinate.");
        Assert.True(mapdata.GetTopLeft().y == 1F, "GetTopLeft() returns incorrect vector y coordinate.");
    }

    [Test]
    public void GetRawWorks() {
        Assert.True(mapdata.GetRaw(1,0) == 4.0F, "GetRaw(1,0) returns incorrect value.");
    }

    [Test]
    public void GetHeightMultiplierWorks() {
        Assert.True(mapdata.GetHeightMultiplier() - 0.16666666F < precision , "GetHeightMultiplier() returns incorrect value.");
    }

    [Test]
    public void GetNormalizedWorks() {
        Assert.True(mapdata.GetNormalized(1,2) - 0.83333333F< precision, "GetNormalized(1,2) returns incorrect value.");
    }

    [Test]
    public void GetSquishedWorks() {
        Assert.True(mapdata.GetSquished(1, 2) == 1F, "GetSquished(1,2) returns incorrect value.");
    }

    [Test]
    public void GetSlices_SliceHeightCorrect() {
        List<MapData> slices = mapdata.GetSlices(2);
        MapData slice = slices.ElementAt(0);
        int sliceHeight = slice.GetHeight();
        Assert.True(sliceHeight == 2, "Slice GetHeight() returns incorrect value.");
    }

    [Test]
    public void GetSlices_SliceWidthCorrect() {
        List<MapData> slices = mapdata.GetSlices(2);
        MapData slice = slices.ElementAt(0);
        int sliceWidth = slice.GetWidth();
        Assert.True(sliceWidth == 2, "Slice GetWidth() returns incorrect value.");
    }

    [Test]
    public void GetSlices_GetTopLeftCorrect() {
        List<MapData> slices = mapdata.GetSlices(2);
        MapData slice = slices.ElementAt(2);
        Vector2 sliceTopLeft = slice.GetTopLeft();
        Assert.True(Mathf.Approximately(sliceTopLeft.x, -0.5F), "Slice GetTopLeft() returns incorrect x value:" + sliceTopLeft.x);
        Assert.True(Mathf.Approximately(sliceTopLeft.y, 0F), "Slice GetTopLeft() returns incorrect y value:" + sliceTopLeft.y);
    }

    [Test]
    public void GetSlices_GetRawCorrect() {
        List<MapData> slices = mapdata.GetSlices(2);
        MapData slice = slices.ElementAt(0);
        float altitude = slice.GetRaw(1,1);
        Assert.True(altitude == 5F, "Slice GetRaw(1,1) returns incorrect value.");
    }

    [Test]
    public void GetSlices_WithOffsetCorrect() {
        List<MapDataSlice> slices = mapdata.GetSlices(1, 2, 2, 3, 2, 2);
        Assert.True(Mathf.Approximately(6.0F, slices[0].GetRaw(0, 0)), 
            "Slice with offset 0 GetRaw(0, 0) == " + slices[0].GetRaw(0, 0) + "; should be 6.0");
    }

    [Test]
    public void GetDisplayReadySlices_CountAndLODsCorrect() {
        // Other display ready slicing functionality will be tested in further tasks when it will actually be used
        int[,] lodMatrix = new int[2,2] {
            {1,2}, {3,4}
        };
        List<DisplayReadySlice> slices = mapdata.GetDisplayReadySlices(2, 3, 0, 0, lodMatrix);
        Assert.True(slices.Count == 4, "Incorrect number of slices after GetDisplayReadySlices (was " + slices.Count + ", should be 4)");
        for(int y = 0; y < lodMatrix.GetLength(1); y++) {
            for(int x = 0; x < lodMatrix.GetLength(0); x++) {
                Assert.True(lodMatrix[x, y] == slices[y * lodMatrix.GetLength(0) + x].lod, "LOD was incorrect for piece at " + x + ", " + y 
                + " (was " + slices[y * lodMatrix.GetLength(0) + x].lod + ", should be " + lodMatrix[x, y] + ")");
            }
        }
    }

}
