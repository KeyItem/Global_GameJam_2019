using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    [Header("Game State")] public bool isWaitingToStart = true;
    public bool isPlaying = false;
    public bool isGameOver = false;
    
    [Header("Input Attributes")] 
    public EntityInput input;

    [Space(10)] 
    public InputInfo inputValues;

    [Space(10)] 
    public bool isCapturingInput = true;

    [Header("Player Attributes")] 
    public EntityResetAttributes resetAttributes;
    
    [Header("Active Player Objects")] 
    public EntityController player;
    
    [Space(10)]
    public List<EntityMovement> playerRoots;    
    public List<EntityMovement> alivePlayerRoots;

    [Header("Player Prefabs")]
    public GameObject newRootObject;

    [Header("Player Stats")] public float playerDepth;
    private float nextDepthMarker;

    [Header("Progression Markers")] 
    public float[] depthMarkers;

    [Header("Obstacles")] public EntityObstacle[] obstacles;

    [Header("UI Attributes")] public UIController UIController;
    
    [Header("Score Attributes")]
    public ScoreController scoreController;

    private void Start()
    {
        InitializeGameController();
    }

    private void Update()
    {
        ManageGame();
    }

    private void InitializeGameController()
    {
        InitializeInput();
        InitializeDepthMarkers();
        InitializeEntityController();
        InitializeObstacles();
        InitializeUIController();
        InitializeScoreController();
    }

    private void InitializeInput()
    {
        input = GetComponent<EntityInput>();
    }

    private void InitializeEntityController()
    {
        player.InitializeController();
    }

    private void InitializeObstacles()
    {
        obstacles = GameObject.FindObjectsOfType<EntityObstacle>();
    }

    private void InitializeUIController()
    {
        UIController = GameObject.FindGameObjectWithTag("UI").GetComponent<UIController>();
    }

    private void InitializeScoreController()
    {
        scoreController = GetComponent<ScoreController>();
    }
   
    private void ManageGame()
    {
        ManageInput();
        ManageGameStates(inputValues);
    }

    private void ManageInput()
    {
        input.GetInput();

        inputValues = input.ReturnInput();
    }
    
    private void ManageGameStates(InputInfo input)
    {
        if (isWaitingToStart)
        {
            if (CheckForStartGameInput(input))
            {
                ResetObstacles();
                
                isWaitingToStart = false;

                player.StopPlayerTrailRenderer();
                
                player.StopCamera();
                player.StartCamera();
                
                player.StartPlayerMovement();
                
                isPlaying = true;
            }
        }
        else if (isPlaying)
        {            
            ManageActivePlayerObjects();

            if (CheckForGameOver(alivePlayerRoots))
            {
                player.StopPlayerMovement();
                
                scoreController.RegisterScore();

                isPlaying = false;

                isGameOver = true;
            }
            
            UpdateDepth(alivePlayerRoots);
            
            scoreController.ManageScore(playerDepth, alivePlayerRoots.Count);
            
            player.UpdateController(input, alivePlayerRoots);
        }
        else if (isGameOver)
        {
            if (CheckForStartGameInput(input))
            {
                ResetPlayerRoots();

                isGameOver = false;

                isWaitingToStart = true;
            }
        }
    }
    private void ManageActivePlayerObjects()
    {
        alivePlayerRoots = ReturnActivePlayerObjects(playerRoots);       
    }

    private void ResetPlayerRoots()
    {
        player.Reset(resetAttributes);
    }

    private void ResetObstacles()
    {
        for (int i = 0; i < obstacles.Length; i++)
        {
            obstacles[i].Reset();
        }
    }

    private void UpdateDepth(List<EntityMovement> activeRoots)
    {
        playerDepth = ReturnPlayerDepth(activeRoots);
        
        UIController.UpdateDepthText(playerDepth);
        UIController.UpdateScoreText(scoreController.ReturnScore());

        CheckForNextDepthMarker(playerDepth);
    }
    
    private void InitializeDepthMarkers()
    {
        nextDepthMarker = depthMarkers[0];
    }

    private void AddNewRoot()
    {
        if (alivePlayerRoots.Count < 3)
        {
            EntityMovement latestRoot = ReturnLatestRoot(alivePlayerRoots);

            Vector2 spawnRootPosition = ReturnSafeSpawnPosition(latestRoot.transform.position);

            if (spawnRootPosition == Vector2.zero)
            {
                Debug.Log("Not Safe to Spawn");
            
                return;
            }

            GameObject newRoot = Instantiate(newRootObject, spawnRootPosition, Quaternion.identity, transform);
        }
    }

    private Vector2 ReturnSafeSpawnPosition(Vector2 basePosition)
    {
        return basePosition + Vector2.right;
    }
    
    private EntityMovement ReturnLatestRoot(List<EntityMovement> activeRoots)
    {
        return activeRoots[activeRoots.Count - 1];
    }
    
    private List<EntityMovement> ReturnActivePlayerObjects(List<EntityMovement> playerRoots)
    {
        List<EntityMovement> activeObjects = new List<EntityMovement>();

        for (int i = 0; i < playerRoots.Count; i++)
        {
            if (playerRoots[i].isActive)
            {
                activeObjects.Add(playerRoots[i]);
            }
        }

        return activeObjects;
    }

    private float ReturnPlayerDepth(List<EntityMovement> activePlayerObjects)
    {
        float furthestDepth = 0f;

        for (int i = 0; i < activePlayerObjects.Count; i++)
        {
            if (activePlayerObjects[i].transform.position.y < furthestDepth)
            {
                furthestDepth = activePlayerObjects[i].transform.position.y;
            }
        }

        return furthestDepth;
    }

    private bool CheckForStartGameInput(InputInfo input)
    {
        if (input.ReturnCurrentButtonState("StartButton"))
        {
            return true;
        }

        return false;
    }
    
    private bool CheckForGameOver(List<EntityMovement> activePlayerRoots)
    {
        if (activePlayerRoots.Count > 0)
        {
            return false;
        }

        return true;
    }

    private bool CheckForNextDepthMarker(float currentDepth)
    {
        if (currentDepth < nextDepthMarker)
        {
            nextDepthMarker = ReturnNextDepthMarker(currentDepth);
            
            return true;
        }

        return false;
    }

    private float ReturnNextDepthMarker(float currentDepth)
    {
        for (int i = 0; i < depthMarkers.Length; i++)
        {
            if (currentDepth < depthMarkers[i])
            {
                continue;
            }

            return depthMarkers[i];
        }

        return Mathf.NegativeInfinity;
    }
}

[System.Serializable]
public struct EntityResetAttributes
{
    [Header("Entity Reset Attributes")] public Vector2[] resetPosition;

    public EntityResetAttributes(Vector2[] newResetPosition)
    {
        this.resetPosition = newResetPosition;
    }
}
