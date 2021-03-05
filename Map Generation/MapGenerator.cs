using System;
using System.Collections;
using System.Collections.Generic;
using Script.GameManager;
using Script.Player;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Script.Map_Generation
{
    public class MapGenerator : MonoBehaviour
    {
        [SerializeField] private int NPlayers;
        [SerializeField] private Color[] AvailableColors;
        [SerializeField] private int Seed;
        [SerializeField] private bool RandomSeed;
        [SerializeField] private int ChunkSize;
        [SerializeField] private Vector2Int GridSize;

        [SerializeField] private int MaxTries;
        [SerializeField] private Vector2Int GoldChunks;
        [SerializeField] private Vector2Int IronChunks;
        [SerializeField] private Vector2Int TreesChunks;
        [SerializeField] private Vector2Int NonWalkableChunks;

        [SerializeField] private GameObject[] AvailableGoldTerrains;
        [SerializeField] private GameObject[] AvailableIronTerrains;
        [SerializeField] private GameObject[] AvailableTreesTerrains;
        [SerializeField] private GameObject[] AvailableNonWalkableTerrains;
        [SerializeField] private GameObject FreeTerrain;
        
        [SerializeField] private bool GenerateGameObjects;
        [SerializeField] private bool GizmoVisible;

        [SerializeField] private GameObject[] MapGoldPositions;
        [SerializeField] private GameObject[] MapIronPositions;
        [SerializeField] private GameObject[] MapTreesPositions;
        [SerializeField] private GameObject[] MapNonWalkablePositions;
        
        private Chunk[,] mapCoords;
        private List<Chunk> availableChunks;
        private List<Chunk> walkableChunksList;
        private List<Chunk> ironChunksSpawnedList;
        private List<Chunk> goldChunksSpawnedList;
        private List<Chunk> treesChunksSpawnedList;
        private List<Chunk> nonWalkableChunksSpawnedList;

        private int numberGoldChunks, numberIronChunks, numberTreesChunks, numberNonWalkableChunks;
        private bool isPlaying;
        
        private void Start()
        {
            isPlaying = false;
            GeneratePlayers();
            GenerateTerrain();
            GenerateStartPoint();
        }

        private void GeneratePlayers()
        {
            List<Color> availableColors = new List<Color>();
            for (int i = 0; i < AvailableColors.Length; i++)
            {
                availableColors.Add(AvailableColors[i]);
            }
            
            GameManagerUtility.PlayersList = new List<Player.Player>(NPlayers);
            for (int i = 0; i < NPlayers; i++)
            {
                int randomIndex = Random.Range(0, availableColors.Count);
                Debug.Log("Creating player: " + i);
                Color randomColor = availableColors[randomIndex];
                availableColors.Remove(randomColor);
                GameManagerUtility.PlayersList.Add(new Player.Player(i, randomColor)); 
            }
        }

        private void Update()
        {
            if (Input.GetKey(KeyCode.R))
            {
                GenerateTerrain();
                DestroyUnits();
                GenerateStartPoint();
            }
            
            if (!isPlaying)
                isPlaying = true;
        }

        private void DestroyUnits()
        {
            for (int i = 0; i < GameManagerUtility.PlayersList.Count; i++)
            {
                List<GameObject> playerUnits = GameManagerUtility.PlayersList[i].GetUnits();
                for(int j = 0; j < playerUnits.Count; j++)
                    Destroy(playerUnits[j]);
            }
        }

        private void GenerateTerrain()
        {
            InitializeSeed();
            ClearTerrain();
            CreateArray();
            GenerateChunks();
        }

        private void InitializeSeed()
        {
            if (RandomSeed)
            {
                Seed = Random.Range(int.MinValue, int.MaxValue);
            }
            Random.InitState(Seed);
        }
        
        private void ClearTerrain()
        {
            if (!isPlaying) 
                return;
            for (int i = 0; i < GridSize.x; i++)
            {
                for (int j = 0; j < GridSize.y; j++)
                {
                    if(mapCoords[i,j].GetParentGameObject() != null)
                        Destroy(mapCoords[i,j].GetParentGameObject());
                } 
            }
            
        }
        
        private void CreateArray()
        {
            availableChunks = new List<Chunk>();
            numberGoldChunks = Random.Range(GoldChunks.x, GoldChunks.y);
            goldChunksSpawnedList = new List<Chunk>(numberGoldChunks);
            MapGoldPositions = new GameObject[numberGoldChunks];
            
            numberIronChunks = Random.Range(IronChunks.x, IronChunks.y);
            ironChunksSpawnedList = new List<Chunk>(numberIronChunks);
            MapIronPositions = new GameObject[numberIronChunks];
            
            numberTreesChunks = Random.Range(TreesChunks.x, TreesChunks.y);
            treesChunksSpawnedList = new List<Chunk>(numberTreesChunks);
            MapTreesPositions = new GameObject[numberTreesChunks];
            
            numberNonWalkableChunks = Random.Range(NonWalkableChunks.x, NonWalkableChunks.y);
            nonWalkableChunksSpawnedList = new List<Chunk>(numberNonWalkableChunks);
            MapNonWalkablePositions = new GameObject[numberNonWalkableChunks];

            walkableChunksList = new List<Chunk>(GridSize.x * GridSize.y -
                                                 (numberGoldChunks + numberIronChunks + numberTreesChunks +
                                                  numberNonWalkableChunks));
            
            mapCoords = new Chunk[GridSize.x, GridSize.y];
            
            for (int i = 0; i < GridSize.x; i++)
            {
                for (int j = 0; j < GridSize.y; j++)
                {
                    mapCoords[i, j] = new Chunk(new Vector2(i * ChunkSize, j * ChunkSize), new Vector2Int(i, j));
                    availableChunks.Add(mapCoords[i,j]);
                }
            }
        }

        private void GenerateChunks()
        {
            GenerateResourceChunk(ref goldChunksSpawnedList, numberGoldChunks, ChunkType.Gold);
            GenerateResourceChunk(ref ironChunksSpawnedList, numberIronChunks, ChunkType.Iron);
            GenerateResourceChunk(ref treesChunksSpawnedList, numberTreesChunks, ChunkType.Trees);
            GenerateNonWalkableChunk(nonWalkableChunksSpawnedList, numberNonWalkableChunks);
            GenerateFreeChunks();
        }


        private void GenerateResourceChunk(ref List<Chunk> resourceSpawnedList, int maxResourceChunks, ChunkType chunkType)
        {
            GameObject[] resourceArray = chunkType switch
            {
                ChunkType.Gold => AvailableGoldTerrains,
                ChunkType.Iron => AvailableIronTerrains,
                ChunkType.Trees => AvailableTreesTerrains,
                _ => AvailableGoldTerrains
            };

            GameObject[] finalArray = chunkType switch
            {
                ChunkType.Gold => MapGoldPositions,
                ChunkType.Iron => MapIronPositions,
                ChunkType.Trees => MapTreesPositions,
                _ => MapGoldPositions
            };

            int actualTries = 0;
            int counter = 0;
            
            while(availableChunks.Count > 0 && resourceSpawnedList.Count < maxResourceChunks && actualTries < MaxTries)
            {
                actualTries++;
                int index = Random.Range(0, availableChunks.Count-1);

                Vector2Int indexes = availableChunks[index].GetIndexes();
                bool canSpawn = true;
                
                if (resourceSpawnedList.Count > 0)
                {
                    foreach (Chunk tmpChunk in resourceSpawnedList)
                    {
                        Vector2 pos = tmpChunk.GetPosV2();
                        Vector2 actualPos = mapCoords[indexes.x, indexes.y].GetPosV2();

                        if(Mathf.Abs(actualPos.x - pos.x) <= ChunkSize && Mathf.Abs(actualPos.y - pos.y) <= ChunkSize)
                        {
                            canSpawn = false;
                        }
                    }
                }
                
                if (canSpawn)
                {
                    mapCoords[indexes.x, indexes.y].SetChunkType(chunkType);
                    
                    Vector3 finalPos = mapCoords[indexes.x, indexes.y].GetPos() -
                                       new Vector3(ChunkSize / 2f, 0, ChunkSize / 2f);
                    if (GenerateGameObjects)
                    {
                        int randomIndexTerrain = Random.Range(0, resourceArray.Length-1);
                        GameObject resource = Instantiate(resourceArray[randomIndexTerrain], finalPos, Quaternion.identity, transform);
                        resource.gameObject.name = chunkType.ToString() + (counter+1);
                        mapCoords[indexes.x, indexes.y].SetParentGameObject(resource);
                        finalArray[counter] = resource;
                        counter++;
                    }
                    resourceSpawnedList.Add(mapCoords[indexes.x, indexes.y]);
                    availableChunks.Remove(availableChunks[index]);
                    actualTries = 0;
                }
            }
        }

        private void GenerateNonWalkableChunk(ICollection<Chunk> resourceSpawnedList, int maxResourceChunks)
        {
            int counter = 0;
            while(resourceSpawnedList.Count < maxResourceChunks)
            {
                int index = Random.Range(0, availableChunks.Count-1);
                Vector2Int indexes = availableChunks[index].GetIndexes();

                mapCoords[indexes.x, indexes.y].SetChunkType(ChunkType.NonWalkable);
                if (GenerateGameObjects)
                {
                    Vector3 finalPos = mapCoords[indexes.x, indexes.y].GetPos() -
                                       new Vector3(ChunkSize / 2f, 0, ChunkSize / 2f);
                    GameObject nonWalkableFloor = Instantiate(FreeTerrain, finalPos, Quaternion.identity, transform);
                    mapCoords[indexes.x, indexes.y].SetParentGameObject(nonWalkableFloor);
                    finalPos = mapCoords[indexes.x, indexes.y].GetPos();
                    float randomHeight = Random.Range(0.045f, 0.09f);
                    MapNonWalkablePositions[counter] = nonWalkableFloor;
                    counter++;
                    int randomIndexTerrain = Random.Range(0, AvailableNonWalkableTerrains.Length-1);
                    GameObject nonWalkableObject = Instantiate(AvailableNonWalkableTerrains[randomIndexTerrain], finalPos, Quaternion.identity, transform);
                    Vector3 lastScale = nonWalkableObject.transform.localScale;
                    nonWalkableObject.transform.localScale = new Vector3(lastScale.x, randomHeight, lastScale.z);
                    Vector3 lastRotation = nonWalkableObject.transform.eulerAngles;
                    lastRotation.y = Random.Range(0, 359);
                    nonWalkableObject.transform.Rotate(lastRotation, Space.Self);
                    nonWalkableObject.transform.parent = nonWalkableFloor.transform;
                }
                resourceSpawnedList.Add(mapCoords[indexes.x, indexes.y]);
                availableChunks.Remove(availableChunks[index]);
            }
        }

        private void GenerateFreeChunks()
        {
            while (availableChunks.Count > 0)
            {
                int index = Random.Range(0, availableChunks.Count-1);
                Vector2Int indexes = availableChunks[index].GetIndexes();
                mapCoords[indexes.x, indexes.y].SetChunkType(ChunkType.Free);
                if (GenerateGameObjects)
                {
                    Vector3 finalPos = mapCoords[indexes.x, indexes.y].GetPos() -
                                       new Vector3(ChunkSize / 2f, 0, ChunkSize / 2f);
                    GameObject freeChunkTerrain = Instantiate(FreeTerrain, finalPos, Quaternion.identity, transform);
                    mapCoords[indexes.x, indexes.y].SetParentGameObject(freeChunkTerrain);
                }
                walkableChunksList.Add(availableChunks[index]);
                availableChunks.Remove(availableChunks[index]);
            }
        }

        private void GenerateStartPoint()
        {
            int[] indexes = new int[NPlayers];
            do
            {
                for (int i = 0; i < NPlayers; i++)
                    indexes[i] = Random.Range(0, walkableChunksList.Count);
                
                Debug.Log(Vector3.Distance(walkableChunksList[indexes[0]].GetPos(), walkableChunksList[indexes[1]].GetPos()));
            } while (Vector3.Distance(walkableChunksList[indexes[0]].GetPos(), walkableChunksList[indexes[1]].GetPos()) < (GridSize.x + GridSize.y)/2.0/ 4);
            
            for (int i = 0; i < NPlayers; i++)
            {
                GameObject startUnit = GameObject.CreatePrimitive(PrimitiveType.Cube);
                startUnit.GetComponent<Transform>().position = walkableChunksList[indexes[i]].GetPos() + new Vector3(0, 0.5f, 0);
                startUnit.AddComponent<Entity>();
                startUnit.GetComponent<Entity>().SetPlayerOwner(i);
                startUnit.GetComponent<Entity>().SetEntityPos(walkableChunksList[indexes[i]].GetPos());
                startUnit.GetComponent<MeshRenderer>().material.color = GameManagerUtility.PlayersList[i].GetColor();
                GameManagerUtility.PlayersList[i].AddUnit(startUnit);
            }
        }
        
        private void OnDrawGizmos() 
        {
            if (!isPlaying || !GizmoVisible) 
                return;
            
            for (int i = 0; i < GridSize.x; i++)
            {
                for (int j = 0; j < GridSize.y; j++)
                {
                    Color color;
                    switch (mapCoords[i,j].GetChunkType())
                    {
                        case ChunkType.Gold:
                            color = Color.yellow;
                            break;
                        case ChunkType.Iron:
                            color = Color.red;
                            break;
                        case ChunkType.Trees:
                            color = Color.green;
                            break;
                        case ChunkType.NonWalkable:
                            color = Color.black;
                            break;
                        default:
                            color = Color.white;
                            break;
                    }

                    Gizmos.color = color;
                    Gizmos.DrawWireCube(mapCoords[i, j].GetPos(),
                        new Vector3(1, 0, 1) * ChunkSize);
                    //Gizmos.DrawSphere(new Vector3(MapCoords[i,j].x, 0, MapCoords[i,j].y), 0.02f);
                }
            }
        }
    }
}
