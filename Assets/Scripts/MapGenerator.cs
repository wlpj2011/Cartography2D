using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class MapGenerator : MonoBehaviour
{
    public int width;
    public int height;

    [Range(0,100)]
    public int randomFillPercent;

    public int smoothingLevels;
    public int fillingLevels;
    public int minNeighboursToFill;
    public int clearingLevels;
    public int maxNeighboursToClear;

    public string seed;
    public bool useRandomSeed;

    int[,] map;

    void Start() {

        #if UNITY_EDITOR
        UnityEditor.SceneView.FocusWindowIfItsOpen(typeof(UnityEditor.SceneView));
        #endif

        GenerateMap();
    }

    void Update() {
        if (Input.GetMouseButtonDown(0)) {
            GenerateMap();
        }
    }

    void GenerateMap() {
        map = new int[width, height];
        RandomFillMap();

        for (int i = 0; i < smoothingLevels; i++) {
            SmoothMap();
        }

        CleanMap();
        
        int borderSize = 5;
        int[,] borderedMap = new int[width + borderSize * 2, height + borderSize * 2];

        for (int x = 0; x < borderedMap.GetLength(0); x++) {
            for (int y = 0; y < borderedMap.GetLength(1); y++) {
                if ( x >= borderSize && x < width + borderSize && y >= borderSize && y < height +borderSize){
                    borderedMap[x, y] = map[x - borderSize, y - borderSize];
                }
                else {
                    borderedMap[x, y] = 1;
                    Debug.Log("Bordering");
                }
            }
        }

        MeshGenerator meshGen = GetComponent<MeshGenerator>();
        meshGen.GenerateMesh(borderedMap, 1);
    }

    void RandomFillMap(){
        if(useRandomSeed) {
            seed = Time.time.ToString();
        }

        System.Random pseudoRandom = new System.Random(seed.GetHashCode());

        for (int x = 0; x < width; x++) {
            for (int y = 0; y < height; y++) {
                if(x == 0 || x == width - 1|| y == 0 || y== height - 1) {
                    map[x, y] = 1;
                }
                else {
                    map[x, y] = (pseudoRandom.Next(0, 100) < randomFillPercent) ? 1 : 0;
                }
            }
        }
    }

    void SmoothMap() {
        int[,] newMap = new int[width, height];
        for (int x = 0; x < width; x++) {
            for (int y = 0; y < height; y++){
                int neighbourWallTiles = GetSurroundingWallCount(x, y);

                if (map[x,y] == 1 && neighbourWallTiles >= 4) {
                    newMap[x, y] = 1;
                }
                else if ( map[x,y] ==1 && neighbourWallTiles < 4) {
                    newMap[x, y] = 0;
                }
                else if (map[x,y]==0 && neighbourWallTiles == 5) {
                    newMap[x, y] = 1;
                }
                else {
                    newMap[x, y] = 0;
                }
            }
        }
        map = newMap;
        Debug.Log("Map Smoothed");
    }

    void FillMapHoles() {
        for (int x = 0; x < width; x++){ 
            for (int y = 0; y < height; y++) {
                int neighbourWallTiles = GetSurroundingWallCount(x, y);
                if(map[x,y] == 0 && neighbourWallTiles >= minNeighboursToFill){
                    map[x, y] = 1;
                }
            }
        }
    }
    void ClearMapSpots() {
        for (int x = 0; x < width; x++){ 
            for (int y = 0; y < height; y++) {
                int neighbourWallTiles = GetSurroundingWallCount(x, y);
                if(map[x,y] == 1 && neighbourWallTiles <= maxNeighboursToClear){
                    map[x, y] = 0;
                }
            }
        }
    }

    void CleanMap() {
        for (int i = 0; i < fillingLevels; i++) {
            FillMapHoles();
        }
        for (int i = 0; i < clearingLevels; i++) {
            ClearMapSpots();
        }

    }

    int GetSurroundingWallCount(int gridX, int gridY) {
        int wallCount = 0;
        for (int neighbourX = gridX - 1; neighbourX <= gridX + 1; neighbourX++) {
            for (int neighbourY = gridY - 1; neighbourY <= gridY + 1; neighbourY++) {
                if ( neighbourX >= 0 && neighbourX < width && neighbourY >= 0 && neighbourY < height){
                    if ( neighbourX != gridX || neighbourY != gridY) {
                        wallCount += map[neighbourX, neighbourY];
                        //Debug.Log("Checked a wall");
                    }
                }
                else {
                    wallCount += 1;
                    //Debug.Log("Skipped an edge");
                }
                
            }
        }
        //Debug.Log("The tile (" + gridX.ToString() + ", " + gridY.ToString() + ") has "+ wallCount.ToString() + "walls nearby.");
        return wallCount;
    }

    // void OnDrawGizmos() {
    //     if (map != null) {
    //         for (int x = 0; x < width; x++) {
    //             for (int y = 0; y < height; y++) {
    //                 Gizmos.color = (map[x, y] == 1) ? Color.black : Color.white;
    //                 Vector3 pos = new Vector3(-width / 2 + x + 0.5f, 0, -height / 2 + y + 0.5f);
    //                 Gizmos.DrawCube(pos, Vector3.one);
    //             }
    //         }
    //     }
    // }

}
