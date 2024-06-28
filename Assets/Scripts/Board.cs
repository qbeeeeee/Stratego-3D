using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Board : MonoBehaviour
{
    [SerializeField] private Material tileMat;
    
    [SerializeField] private GameObject[] prefabs;
    [SerializeField] private Material[] teamMaterials;
    private List<Vector2Int> threeMovesDraw = new List<Vector2Int>();
    private List<Vector2Int> threeMovesDrawBlue = new List<Vector2Int>();
 
    private Pieces[,] pieces;
    private Pieces currentlyDragging;
    private List<Vector2Int> availableMoves = new List<Vector2Int>();
    private bool spawnBluePieces = false;
    private const int TILE_COUNT_X = 10;
    private const int TILE_COUNT_Y = 10;
    public static GameObject[,] tiles;
    private Camera currentCamera;
    private Vector2Int currentHover;
    private List<Vector2> occupiedPositions = new List<Vector2>{
        new Vector2(0, 6), new Vector2(1, 6), new Vector2(2, 6), new Vector2(3, 6), new Vector2(4, 6),
        new Vector2(5, 6), new Vector2(6, 6), new Vector2(7, 6), new Vector2(8, 6), new Vector2(9, 6),
        new Vector2(0, 7), new Vector2(1, 7), new Vector2(2, 7), new Vector2(3, 7), new Vector2(4, 7),
        new Vector2(5, 7), new Vector2(6, 7), new Vector2(7, 7), new Vector2(8, 7), new Vector2(9, 7),
        new Vector2(0, 8), new Vector2(1, 8), new Vector2(2, 8), new Vector2(3, 8), new Vector2(4, 8),
        new Vector2(5, 8), new Vector2(6, 8), new Vector2(7, 8), new Vector2(8, 8), new Vector2(9, 8),
        new Vector2(0, 9), new Vector2(1, 9), new Vector2(2, 9), new Vector2(3, 9), new Vector2(4, 9),
        new Vector2(5, 9), new Vector2(6, 9), new Vector2(7, 9), new Vector2(8, 9), new Vector2(9, 9)
    };

    public static bool preGame = false;
    public static int turn = 0;
    private List<Vector4> availableMovesAI = new List<Vector4>();
    private float turnTimer = 1;
    private List<Vector2Int> outOfMoves = new List<Vector2Int>();
    int ofm = 0;

    public GameObject menuDeath;
    public GameObject deathPanel;
    [SerializeField] TextMeshProUGUI deathText;

    [SerializeField] TextMeshProUGUI pawnMoveText;
    [SerializeField] TextMeshProUGUI pawnMoveTextBlue;
    private List<string> redList = new List<string>();
    private List<string> blueList = new List<string>();
    private bool attack = false;

    private void Awake()
    {
        GenerateTiles(1f, TILE_COUNT_X, TILE_COUNT_Y);
        SpawnAllPieces();
        PositionAllPieces();
        spawnBluePieces = true;
        
        menuDeath.SetActive(false);
        deathPanel.SetActive(false);
        pawnMoveText.color = Color.red;
        pawnMoveTextBlue.color = Color.blue;
    }

    private void Update()
    {
        if(!Interface.isPaused)
        {    
            if (!currentCamera)
            {
                currentCamera = Camera.main;
                return;
            }

            RaycastHit info;
            Ray ray = currentCamera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out info, 100, LayerMask.GetMask("Tile", "Hover" , "Highlight")))
            {   
                //Get the indexes of the tiles i hit
                Vector2Int hitPosition = LookUpTileIndex(info.transform.gameObject);
                
                //If we are hovering a tile after not hovering any tile
                if (currentHover == -Vector2Int.one)
                {
                    currentHover = hitPosition;
                    tiles[hitPosition.x, hitPosition.y].layer = LayerMask.NameToLayer("Hover");
                }

                //If we were already hovering a tile, change the previous one
                if (currentHover != hitPosition)
                {
                    if(ContainsValidMoves(ref availableMoves, currentHover) && preGame)
                    {
                        tiles[currentHover.x, currentHover.y].layer = LayerMask.NameToLayer("Highlight");
                    }
                    else{
                        tiles[currentHover.x, currentHover.y].layer = LayerMask.NameToLayer("Tile");
                    }
                    currentHover = hitPosition;
                    tiles[hitPosition.x, hitPosition.y].layer = LayerMask.NameToLayer("Hover");
                }

                //If we press M1
                if (Input.GetMouseButtonDown(0))
                {
                    if(pieces[hitPosition.x,hitPosition.y] != null && pieces[hitPosition.x,hitPosition.y].team != 1)
                    {
                        //If is our turn
                        if(turn == 0)
                        {
                            currentlyDragging = pieces[hitPosition.x,hitPosition.y];

                            //Get Available moves
                            availableMoves = currentlyDragging.GetMoves(ref pieces, TILE_COUNT_X, TILE_COUNT_Y);
                            HighlightTiles();
                        }
                    }
                }

                //If we release M1
                if (currentlyDragging != null && Input.GetMouseButtonUp(0) && preGame)
                {
                    Vector2Int prevPosition = new Vector2Int(currentlyDragging.currentX,currentlyDragging.currentY);

                    bool validMove = MoveTo(currentlyDragging, hitPosition.x, hitPosition.y);
                    if (!validMove)
                    {
                        currentlyDragging.SetPosition(GetTileCenter(prevPosition.x, prevPosition.y));
                    }

                    currentlyDragging = null;
                    RemoveHighlightTiles();

                }
                else if(currentlyDragging != null && Input.GetMouseButtonUp(0))
                {
                    Vector2Int prevPosition = new Vector2Int(currentlyDragging.currentX,currentlyDragging.currentY);

                    bool validSwap = Swap(currentlyDragging, hitPosition.x, hitPosition.y);
                    if (!validSwap)
                    {
                        currentlyDragging.SetPosition(GetTileCenter(prevPosition.x, prevPosition.y));
                    }

                    currentlyDragging = null;
                    RemoveHighlightTiles();
                }

            }
            else //When the Mouse is out of the board
            {
                if (currentHover != -Vector2Int.one)
                {
                    if(ContainsValidMoves(ref availableMoves, currentHover))
                    {
                        tiles[currentHover.x, currentHover.y].layer = LayerMask.NameToLayer("Highlight");
                    }
                    else{
                        tiles[currentHover.x, currentHover.y].layer = LayerMask.NameToLayer("Tile");
                    }
                    currentHover = -Vector2Int.one;
                }

                if(currentlyDragging && Input.GetMouseButtonUp(0))
                {
                    currentlyDragging.SetPosition(GetTileCenter(currentlyDragging.currentX, currentlyDragging.currentY));
                    currentlyDragging = null;
                    RemoveHighlightTiles();
                }
            }

            //If we have a piece
            if(currentlyDragging)
            {
                Plane plane = new Plane(Vector3.up, Vector3.up * 0.5f);
                float distance = 0.0f;
                if(plane.Raycast(ray, out distance))
                {
                    currentlyDragging.SetPosition(ray.GetPoint(distance));
                }
            }


            //AI PROGRAMMING
            if(turn == 1 && turnTimer >= 1)
            {
                AIPlaying();    
            }

            turnTimer += 1 * Time.deltaTime;
        }
        else
        {
            if (currentHover != -Vector2Int.one)
                {
                    if(ContainsValidMoves(ref availableMoves, currentHover))
                    {
                        tiles[currentHover.x, currentHover.y].layer = LayerMask.NameToLayer("Highlight");
                    }
                    else{
                        tiles[currentHover.x, currentHover.y].layer = LayerMask.NameToLayer("Tile");
                    }
                    currentHover = -Vector2Int.one;
                }

                if(currentlyDragging && Input.GetMouseButtonUp(0))
                {
                    currentlyDragging.SetPosition(GetTileCenter(currentlyDragging.currentX, currentlyDragging.currentY));
                    currentlyDragging = null;
                    RemoveHighlightTiles();
                }
        }
        
    }

    //Generate Board Tiles
    private void GenerateTiles(float tileSize, int tileCountX, int tileCountY)
    {

        tiles = new GameObject[tileCountX, tileCountY];
        for (int x = 0; x < tileCountX; x++)
        {
            for (int y = 0; y < tileCountY; y++)
            {
                tiles[x,y] = GenerateTile(tileSize, x, y);
            }
        }

        //Destroy Water Tiles
        tiles[2,5].layer = LayerMask.NameToLayer("Water");
        tiles[2,4].layer = LayerMask.NameToLayer("Water");
        tiles[3,5].layer = LayerMask.NameToLayer("Water");
        tiles[3,4].layer = LayerMask.NameToLayer("Water");
        tiles[6,4].layer = LayerMask.NameToLayer("Water");
        tiles[6,5].layer = LayerMask.NameToLayer("Water");
        tiles[7,4].layer = LayerMask.NameToLayer("Water");
        tiles[7,5].layer = LayerMask.NameToLayer("Water");
        // Destroy(tiles[2,5]);
        // Destroy(tiles[2,4]);
        // Destroy(tiles[3,5]);
        // Destroy(tiles[3,4]);
        // Destroy(tiles[6,4]);
        // Destroy(tiles[6,5]);
        // Destroy(tiles[7,5]);
        // Destroy(tiles[7,4]);

    }

    private GameObject GenerateTile(float tileSize, int x, int y)
    {
        GameObject tileObject = new GameObject(string.Format($"X:{x}, Y:{y}"));
        tileObject.transform.parent = transform;

        Mesh mesh = new Mesh();
        tileObject.AddComponent<MeshFilter>().mesh = mesh;
        tileObject.AddComponent<MeshRenderer>().material = tileMat;

        Vector3[] vertices = new Vector3[4];
        vertices[0] = new Vector3(x * tileSize, 0.1f, y * tileSize);
        vertices[1] = new Vector3(x * tileSize, 0.1f, (y+1) * tileSize);
        vertices[2] = new Vector3((x+1) * tileSize, 0.1f, y * tileSize);
        vertices[3] = new Vector3((x+1) * tileSize, 0.1f, (y+1) * tileSize);
        
        int[] tris = new int[] { 0, 1, 2 ,1 ,3 ,2};
        
        mesh.vertices = vertices;
        mesh.triangles = tris;

        mesh.RecalculateNormals();

        tileObject.layer = LayerMask.NameToLayer("Tile");
        tileObject.AddComponent<BoxCollider>();

        return tileObject;
    }

    private Vector2Int LookUpTileIndex(GameObject hitInfo)
    {
        for (int x = 0; x < TILE_COUNT_X; x++)
        {
            for(int y = 0; y < TILE_COUNT_Y; y++)
            {
                if(tiles[x,y] == hitInfo)
                {
                    return new Vector2Int(x,y);
                }
            }
        }
        return -Vector2Int.one; 
    }  

    //Spawn the Pieces
    private void SpawnAllPieces()
    {
        pieces = new Pieces[TILE_COUNT_X,TILE_COUNT_Y];

        int redTeam = 0; 
        int blueTeam = 1;

        //Spawn the Red pieces in order
        pieces[0,3] = SpawnSinglePiece(PieceType.Spy, redTeam);

        for(int j = 1; j < 9; j++)
        {
            pieces[j,3] = SpawnSinglePiece(PieceType.Scout, redTeam);
        }
        pieces[9,3] = SpawnSinglePiece(PieceType.Miner, redTeam);

        for(int j = 0; j < 4; j++)
        {
            pieces[j,2] = SpawnSinglePiece(PieceType.Miner, redTeam);
        }
        for(int j = 4; j < 8; j++)
        {
            pieces[j,2] = SpawnSinglePiece(PieceType.Sergeant, redTeam);
        }
        pieces[8,2] = SpawnSinglePiece(PieceType.Lieutenant, redTeam);
        pieces[9,2] = SpawnSinglePiece(PieceType.Lieutenant, redTeam);
        pieces[0,1] = SpawnSinglePiece(PieceType.Lieutenant, redTeam);
        pieces[1,1] = SpawnSinglePiece(PieceType.Lieutenant, redTeam);
        
        for(int j = 2; j < 6; j++)
        {
            pieces[j,1] = SpawnSinglePiece(PieceType.Captain, redTeam);
        }
        for(int j = 6; j < 9; j++)
        {
            pieces[j,1] = SpawnSinglePiece(PieceType.Major, redTeam);
        }
        pieces[9,1] = SpawnSinglePiece(PieceType.Colonel, redTeam);
        pieces[0,0] = SpawnSinglePiece(PieceType.Colonel, redTeam);
        pieces[1,0] = SpawnSinglePiece(PieceType.General, redTeam);
        pieces[2,0] = SpawnSinglePiece(PieceType.Marshall, redTeam);
        for(int j = 3; j < 9; j++)
        {
            pieces[j,0] = SpawnSinglePiece(PieceType.Bomb, redTeam);
        }
        pieces[9,0] = SpawnSinglePiece(PieceType.Flag, redTeam);


        //Spawn the Blue pieces random
        //Spawn Flag only on the last line
        int randomIndex = Random.Range(29, occupiedPositions.Count);
        Vector2 randomPosition = occupiedPositions[randomIndex];
        occupiedPositions.RemoveAt(randomIndex);
        pieces[(int)randomPosition[0],(int)randomPosition[1]] = SpawnSinglePiece(PieceType.Flag, blueTeam);
        
        //Spawn Spy only on the last 2 Lines
        randomIndex = Random.Range(19, occupiedPositions.Count);
        randomPosition = occupiedPositions[randomIndex];
        occupiedPositions.RemoveAt(randomIndex);
        pieces[(int)randomPosition[0],(int)randomPosition[1]] = SpawnSinglePiece(PieceType.Spy, blueTeam);
        
        //Spawn 8 Scouts
        for(int j = 0; j < 8; j++)
        {
            randomIndex = Random.Range(0, occupiedPositions.Count);
            randomPosition = occupiedPositions[randomIndex];
            occupiedPositions.RemoveAt(randomIndex);
            pieces[(int)randomPosition[0],(int)randomPosition[1]] = SpawnSinglePiece(PieceType.Scout, blueTeam);
        }

        //Spawn 5 Miners
        for(int j = 0; j < 5; j++)
        {
            randomIndex = Random.Range(0, occupiedPositions.Count);
            randomPosition = occupiedPositions[randomIndex];
            occupiedPositions.RemoveAt(randomIndex);
            pieces[(int)randomPosition[0],(int)randomPosition[1]] = SpawnSinglePiece(PieceType.Miner, blueTeam);
        }

        //Spawn 4 Sergeants
        for(int j = 0; j < 4; j++)
        {
            randomIndex = Random.Range(0, occupiedPositions.Count);
            randomPosition = occupiedPositions[randomIndex];
            occupiedPositions.RemoveAt(randomIndex);
            pieces[(int)randomPosition[0],(int)randomPosition[1]] = SpawnSinglePiece(PieceType.Sergeant, blueTeam);
        }

        //Spawn 4 Lieutentants
        for(int j = 0; j < 4; j++)
        {
            randomIndex = Random.Range(0, occupiedPositions.Count);
            randomPosition = occupiedPositions[randomIndex];
            occupiedPositions.RemoveAt(randomIndex);
            pieces[(int)randomPosition[0],(int)randomPosition[1]] = SpawnSinglePiece(PieceType.Lieutenant, blueTeam);
        }

        //Spawn 4 Captains
        for(int j = 0; j < 4; j++)
        {
            randomIndex = Random.Range(0, occupiedPositions.Count);
            randomPosition = occupiedPositions[randomIndex];
            occupiedPositions.RemoveAt(randomIndex);
            pieces[(int)randomPosition[0],(int)randomPosition[1]] = SpawnSinglePiece(PieceType.Captain, blueTeam);
        }

        //Spawn 3 Majors
        for(int j = 0; j < 3; j++)
        {
            randomIndex = Random.Range(0, occupiedPositions.Count);
            randomPosition = occupiedPositions[randomIndex];
            occupiedPositions.RemoveAt(randomIndex);
            pieces[(int)randomPosition[0],(int)randomPosition[1]] = SpawnSinglePiece(PieceType.Major, blueTeam);
        }

        //Spawn 2 Colonels
        for(int j = 0; j < 2; j++)
        {
            randomIndex = Random.Range(0, occupiedPositions.Count);
            randomPosition = occupiedPositions[randomIndex];
            occupiedPositions.RemoveAt(randomIndex);
            pieces[(int)randomPosition[0],(int)randomPosition[1]] = SpawnSinglePiece(PieceType.Colonel, blueTeam);
        }

        //Spawn 1 General
        randomIndex = Random.Range(0, occupiedPositions.Count);
        randomPosition = occupiedPositions[randomIndex];
        occupiedPositions.RemoveAt(randomIndex);
        pieces[(int)randomPosition[0],(int)randomPosition[1]] = SpawnSinglePiece(PieceType.General, blueTeam);

        //Spawn 1 Marshal
        randomIndex = Random.Range(0, occupiedPositions.Count);
        randomPosition = occupiedPositions[randomIndex];
        occupiedPositions.RemoveAt(randomIndex);
        pieces[(int)randomPosition[0],(int)randomPosition[1]] = SpawnSinglePiece(PieceType.Marshall, blueTeam);

        //Spawn 6 Bombs
        for(int j = 0; j < 6; j++)
        {
            randomIndex = Random.Range(0, occupiedPositions.Count);
            randomPosition = occupiedPositions[randomIndex];
            occupiedPositions.RemoveAt(randomIndex);
            pieces[(int)randomPosition[0],(int)randomPosition[1]] = SpawnSinglePiece(PieceType.Bomb, blueTeam);
        }
        Debug.Log(occupiedPositions.Count);

    } 

    private Pieces SpawnSinglePiece(PieceType type, int team)
    {

        Pieces p = Instantiate(prefabs[(int)type - 1], transform).GetComponent<Pieces>();
        p.type = type;
        p.team = team;
        p.GetComponent<MeshRenderer>().material = teamMaterials[team];
        p.transform.GetChild(0).GetComponent<MeshRenderer>().material = teamMaterials[team];
        
        return p;
    }

    //Position the Pieces
    private void PositionAllPieces()
    {

        for(int x = 0; x < TILE_COUNT_X; x++)
        {
            for(int y = 0; y < TILE_COUNT_Y; y++)
            {
                if(pieces[x,y] != null)
                {
                    PositionSinglePiece(x,y,true);
                }
            }
        }
    }

    private void PositionSinglePiece(int x, int y, bool force = false)
    {
        pieces[x,y].currentX = x;
        pieces[x,y].currentY = y;
        pieces[x,y].SetPosition(GetTileCenter(x,y), force);
        if(pieces[x,y].team == 1 && spawnBluePieces == false)
        {
            pieces[x,y].transform.Rotate(0,0,180);
        }
    }

    private Vector3 GetTileCenter(int x, int y)
    {
        return new Vector3(x + 0.5f ,0.1f ,y + +0.5f);
    }

    private bool MoveTo(Pieces cd, int x, int y)
    {
        if(turn == 0){
            if(!ContainsValidMoves(ref availableMoves, new Vector2(x,y)))
            {
                return false;
            }
        }

        Vector2Int prevPosition = new Vector2Int(cd.currentX,cd.currentY);

        Pieces p = pieces[x,y];

        //Checking if there is another piece
        if(pieces[x,y] != null)
        {

            if(p.team == cd.team)
            {
                return false;
            }
        }

        //Checking if flag or bomb, then dont move
        if(pieces[prevPosition.x,prevPosition.y].type == PieceType.Flag || pieces[prevPosition.x,prevPosition.y].type == PieceType.Bomb)
        {
            Debug.Log("are");
            return false;
        }

        //Checking if attacking the flag then its game over
        if(p != null && p.type == PieceType.Flag)
        {
            SpawnBrokenTowerAttacking(cd.currentX ,cd.currentY ,x ,y);
            p.gameObject.SetActive(false);
            p = null;
            MovePawnLog(cd,x,y,true);
            DeathPanel();
            deathText.text = "You Won!!!";
            return false;
        }

        //Checking if Spy attacks Marshall
        if(p != null && pieces[prevPosition.x,prevPosition.y].type == PieceType.Spy && p.type == PieceType.Marshall)
        {
            SpawnBrokenTowerAttacking(cd.currentX ,cd.currentY ,x ,y);
            p.gameObject.SetActive(false);
            p = null;
            attack = true;
        }

        //Checking if Miner attacks Bomb
        if(p != null && pieces[prevPosition.x,prevPosition.y].type == PieceType.Miner && p.type == PieceType.Bomb)
        {
            SpawnBrokenTowerAttacking(cd.currentX ,cd.currentY ,x ,y);
            p.gameObject.SetActive(false);
            p = null;
            attack = true;
        }

        //Checking the hierarchy of the pieces
        if(p != null && pieces[prevPosition.x,prevPosition.y].type > p.type)
        {
            SpawnBrokenTowerAttacking(cd.currentX ,cd.currentY ,x ,y);
            p.gameObject.SetActive(false);
            p = null;
            attack = true;
        }
        else if(p != null && pieces[prevPosition.x,prevPosition.y].type < p.type)
        {
            MovePawnLog(cd,x,y,true);

            cd.SetPosition(GetTileCenter(x,y));
            StartCoroutine(WaitToRemoveBothPieces(x ,y ,cd.currentX ,cd.currentY , false));
            SpawnBrokenTowers(x,y,cd.currentX,cd.currentY,false);

            QueueThreeMovesDraw(x,y,prevPosition.x, prevPosition.y);
            if(turn == 1){
                turn = 0;
            }else if(turn == 0){
                turn = 1;
            }
            turnTimer = 0;
            OutOfMoves();

            return true;
        } //Checking if the pieces have the same value
        else if(p != null && pieces[prevPosition.x,prevPosition.y].type == p.type)
        {
            MovePawnLog(cd,x,y,true);

            cd.SetPosition(GetTileCenter(x,y));
            StartCoroutine(WaitToRemoveBothPieces(cd.currentX ,cd.currentY ,x ,y , true));
            SpawnBrokenTowers(x,y,cd.currentX,cd.currentY,true);
            
            QueueThreeMovesDraw(x,y,prevPosition.x, prevPosition.y);
            if(turn == 1){
                turn = 0;
            }else if(turn == 0){
                turn = 1;
            }
            turnTimer = 0;
            OutOfMoves();

            return true;
        }
        MovePawnLog(cd,x,y,attack);

        QueueThreeMovesDraw(x,y,prevPosition.x, prevPosition.y);

        pieces[x,y] = cd;
        pieces[prevPosition.x, prevPosition.y] = null;
        
        if(turn == 1){
            turn = 0;
        }else if(turn == 0){
            turn = 1;
        }
        turnTimer = 0;
        OutOfMoves();
        
        PositionSinglePiece(x,y);
        attack = false;
        
        return true;
    }

    //Highlights the tiles of the available moves
    private void HighlightTiles()
    {
        if(preGame){
            for(int i = 0; i < availableMoves.Count; i++)
            {
                tiles[availableMoves[i].x, availableMoves[i].y].layer = LayerMask.NameToLayer("Highlight");
            }
        }
    }

    //Removes the highlighted tiles
    private void RemoveHighlightTiles()
    {
        for(int i = 0; i < availableMoves.Count; i++)
        {
            tiles[availableMoves[i].x, availableMoves[i].y].layer = LayerMask.NameToLayer("Tile");
        }

        availableMoves.Clear();
    }

    //Checking for Valid Moves(to xrisimopioume gia na allazei apo Highlight se Hover)
    private bool ContainsValidMoves(ref List<Vector2Int> moves, Vector2 position)
    {
        for(int i = 0; i < moves.Count; i++)
        {
            if(moves[i].x == position.x && moves[i].y == position.y)
            {
                return true;
            }
        }
        return false;
    }     

    IEnumerator WaitToRemoveBothPieces(int x ,int y ,int cx ,int cy ,bool removePieces)
    {
        yield return new WaitForSeconds(0.2f);
        pieces[cx,cy].gameObject.SetActive(false);
        pieces[cx,cy] = null;

        if(removePieces){
            pieces[x,y].gameObject.SetActive(false);
            pieces[x,y] = null;
        }
    }

    void SpawnBrokenTowers(int x, int y, int cx, int cy, bool brokenTowers)
    {   
        Pieces p = Instantiate(prefabs[12], pieces[x,y].transform.position, pieces[x,y].transform.rotation).GetComponent<Pieces>();
        p.SetPosition(GetTileCenter(x,y),false);
        int teamBroken = 0;
        if(pieces[x,y].team == 1)
        {
            teamBroken = 0;
        }
        else if(pieces[x,y].team == 0)
        {
            teamBroken = 1;
        }
        p.team = teamBroken;
        p.GetComponent<MeshRenderer>().material = teamMaterials[teamBroken];
        for(int i = 0; i < 7; i++){
            p.transform.GetChild(i).GetComponent<MeshRenderer>().material = teamMaterials[teamBroken];
        }
        StartCoroutine(RemoveBrokenTower(p));

        if(brokenTowers){
            Pieces p2 = Instantiate(prefabs[12], pieces[x,y].transform.position, pieces[x,y].transform.rotation).GetComponent<Pieces>();
            p2.SetPosition(GetTileCenter(x,y),false);
            if(pieces[cx,cy].team == 1)
            {
                teamBroken = 0;
            }
            else if(pieces[cx,cy].team == 0)
            {
                teamBroken = 1;
            }
            p2.team = teamBroken;
            p2.GetComponent<MeshRenderer>().material = teamMaterials[teamBroken];
            for(int i = 0; i < 7; i++){
                p2.transform.GetChild(i).GetComponent<MeshRenderer>().material = teamMaterials[teamBroken];
            }
            StartCoroutine(RemoveBrokenTower(p2));
        }
    }

    void SpawnBrokenTowerAttacking(int cx, int cy, int x, int y)
    {   
        Pieces p = Instantiate(prefabs[12], pieces[cx,cy].transform.position, pieces[cx,cy].transform.rotation).GetComponent<Pieces>();
        p.SetPosition(GetTileCenter(x,y),false);
        int teamBroken = 0;
        if(pieces[cx,cy].team == 1)
        {
            teamBroken = 0;
        }
        else if(pieces[cx,cy].team == 0)
        {
            teamBroken = 1;
        }
        p.team = teamBroken;
        p.GetComponent<MeshRenderer>().material = teamMaterials[teamBroken];
        for(int i = 0; i < 7; i++){
            p.transform.GetChild(i).GetComponent<MeshRenderer>().material = teamMaterials[teamBroken];
        }
        StartCoroutine(RemoveBrokenTower(p));
    }

    IEnumerator RemoveBrokenTower(Pieces p)
    {
        yield return new WaitForSeconds(2f);
        p.gameObject.SetActive(false);
        for(int i = 0; i < 7; i++){
            p.transform.GetChild(i).gameObject.SetActive(false);
        }
    }

    void QueueThreeMovesDraw(int x, int y, int cx, int cy)
    {   
        if(pieces[cx,cy].team == 0 && pieces[cx,cy] != null){
            threeMovesDraw.Add(new Vector2Int(cx,cy));
            threeMovesDraw.Add(new Vector2Int(x,y));
            if(threeMovesDraw.Count >= 6){
                if(threeMovesDraw[1] == threeMovesDraw[2] && threeMovesDraw[2] == threeMovesDraw[5])
                {
                    if(threeMovesDraw[0] == threeMovesDraw[3] && threeMovesDraw[3] == threeMovesDraw[4]){
                        DeathPanel();
                        deathText.text = "You Lost!!!";
                        Debug.Log("GAME ENDS RED LOSES");
                    }
                }
            }
            if(threeMovesDraw.Count >= 6)
            {
                threeMovesDraw.RemoveAt(0);
                threeMovesDraw.RemoveAt(0);
            }
        }
        // else if(pieces[cx,cy].team == 1 && pieces[cx,cy] != null)
        // {
        //     threeMovesDrawBlue.Add(new Vector2Int(cx,cy));
        //     threeMovesDrawBlue.Add(new Vector2Int(x,y));
        //     if(threeMovesDrawBlue.Count >= 6){
        //         if(threeMovesDrawBlue[1] == threeMovesDrawBlue[2] && threeMovesDrawBlue[2] == threeMovesDrawBlue[5])
        //         {
        //             if(threeMovesDrawBlue[0] == threeMovesDrawBlue[3] && threeMovesDrawBlue[3] == threeMovesDrawBlue[4]){
        //                 DeathPanel();
        //                 deathText.text = "You Won!!!";
        //                 Debug.Log("GAME ENDS BLUE LOSES");
        //             }
        //         }
        //     }
        //     if(threeMovesDrawBlue.Count >= 6)
        //     {
        //         threeMovesDrawBlue.RemoveAt(0);
        //         threeMovesDrawBlue.RemoveAt(0);
        //     }
        // }

    }
 
    void OutOfMoves()
    {
            for(int i = 0; i < TILE_COUNT_X; i++) 
            {
                for(int j = 0; j < TILE_COUNT_Y; j++)
                {
                    if(pieces[i,j] != null && pieces[i,j].team == 0)
                    {
                        outOfMoves = pieces[i,j].GetMoves(ref pieces, TILE_COUNT_X, TILE_COUNT_Y);
                        if(outOfMoves.Count > 0)
                        {
                            ofm += 1;
                        }
                    }
                }
            }
            if(ofm == 0)
            {
                DeathPanel();
                deathText.text = "You Lost!!!";
                Debug.Log("LITOURGEIII RED TEAM LOSES");
            }
            ofm = 0;
    }

    bool Swap(Pieces cd, int x, int y)
    {
        Vector2Int prevPosition = new Vector2Int(cd.currentX,cd.currentY);

        if(pieces[x,y] != null)
        {
            if(pieces[x,y].team != cd.team)
            {
                return false;
            }
        }
        else if(pieces[x,y] == null)
        {
            return false;
        }

        pieces[2,4] = pieces[x,y];
        pieces[x,y] = pieces[prevPosition.x,prevPosition.y];
        pieces[prevPosition.x,prevPosition.y] = pieces[2,4];

        pieces[2,4] = null;

        PositionSinglePiece(x,y);
        PositionSinglePiece(prevPosition.x,prevPosition.y);

        return true;

    }

    void AIPlaying()
    {
        for(int i = 0; i < 10; i++)
            {
                for(int j = 0; j < 10; j++)
                {
                    if(pieces[i,j] != null && pieces[i,j].team == 1)
                    {
                        availableMovesAI.AddRange(pieces[i,j].GetMovesAI(ref pieces, TILE_COUNT_X, TILE_COUNT_Y));
                    }
                }
            }

            if (availableMovesAI.Count > 0)
            {
                int randomMove = Random.Range(0, availableMovesAI.Count);
                Vector4 randomPos = availableMovesAI[randomMove];
                bool validMoveAI = MoveTo(pieces[(int)randomPos[2],(int)randomPos[3]], (int)randomPos[0], (int)randomPos[1]);
                if (!validMoveAI)
                {
                    pieces[(int)randomPos[2],(int)randomPos[3]].SetPosition(GetTileCenter((int)randomPos[2],(int)randomPos[3]));
                }
         
            }
            else
            {
                DeathPanel();
                deathText.text = "You Won!!!";
                Debug.Log("Red Wins");
            }
            availableMovesAI.Clear();                
    }

    public void DeathPanel()
    {
        Interface.isPaused = true;
        menuDeath.SetActive(true);
        deathPanel.SetActive(true);
        turn = 1;
    }

    public void MovePawnLog(Pieces cd,int x,int y, bool attack)
    {
        if(turn == 0)
        {
            if(attack)
            {
                redList.Add($"X:{cd.currentX}, Y:{cd.currentY} to X:{x}, Y:{y}, {cd.type} attacking " + $"<color=blue>{pieces[x,y].type}</color>");
            }
            else
            {
                redList.Add($"X:{cd.currentX}, Y:{cd.currentY} to X:{x}, Y:{y}");
            }
            pawnMoveText.text = "";
            if(blueList.Count >= 6)
            {
                blueList.RemoveAt(0);
                redList.RemoveAt(0);
                pawnMoveTextBlue.text = "";
                foreach (string i in blueList)
                {
                    pawnMoveTextBlue.text += i + "\n\n";
                }
            }
            foreach (string i in redList)
            {
                pawnMoveText.text += i + "\n\n";
            }
        }
        else
        {
            if(attack)
            {
                blueList.Add($"X:{cd.currentX}, Y:{cd.currentY} to X:{x}, Y:{y}, {cd.type} attacking " + $"<color=red>{pieces[x,y].type}</color>");
            }
            else
            {
                blueList.Add($"X:{cd.currentX}, Y:{cd.currentY} to X:{x}, Y:{y}");
            }
            pawnMoveTextBlue.text = "";
            foreach (string i in blueList)
            {
                pawnMoveTextBlue.text += i + "\n\n";
            }
        }
    }
}
