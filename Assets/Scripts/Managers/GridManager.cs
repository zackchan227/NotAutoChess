using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class GridManager : MonoBehaviour
{
    public static GridManager Instance;
    [SerializeField] private int _width, _height;

    [SerializeField] private Tile _tilePrefab;
    [SerializeField] private Tile _isometricTilePrefab;
    [SerializeField] private Tile _isometricBlockPrefab;

    //[SerializeField] private Transform _cam;
    [SerializeField] private Transform parentTile;

    private Dictionary<Vector2, Tile> _tiles, _normalTiles, _isometricTiles;
    private Transform isometricTransform, normalTransform;
    private Sprite _normalTileSprite, _isometricTileSprite;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        _normalTileSprite = _tilePrefab.GetComponent<SpriteRenderer>().sprite;
        _isometricTileSprite = _tilePrefab.GetComponent<SpriteRenderer>().sprite;
        _normalTiles = new Dictionary<Vector2, Tile>();
        _isometricTiles = new Dictionary<Vector2, Tile>();
    }

    void Start()
    {
        isometricTransform = _isometricTilePrefab.gameObject.GetComponent<Transform>();
        normalTransform = _tilePrefab.gameObject.GetComponent<Transform>();
    }

    public void GenerateIsometricBlockGrid()
    {
        int i = -1, j;
        _tiles = new Dictionary<Vector2, Tile>();
        _isometricTiles = new Dictionary<Vector2, Tile>();
        for (int x = _width / 2 - 1; x >=  -_width / 2; x--)
        {
            i++;
            j = 0;
            float depth = _width;
            for (int y = _height / 2 - 1; y >= -_height / 2; y--)
            {
                j++;
                depth--;
                // float isometricPosX = x * 0.25f * isometricTransform.localScale.x + y * (-0.25f) * isometricTransform.localScale.y;
                // float isometricPosY = x * 0.25f * isometricTransform.localScale.x + y * 0.25f * isometricTransform.localScale.y;
                float isometricPosX = x * 0.5f * 1.0f + y * (-0.5f) * 1.0f;
                float isometricPosY = x * 0.25f * 1.0f + y * 0.25f * 1.0f;
                Vector3 isometricVector3 = new Vector3(isometricPosX,isometricPosY,depth);
                Tile spawnedTile = Instantiate(_isometricBlockPrefab, isometricVector3, Quaternion.identity, parentTile);
                spawnedTile.name = $"Tile {i} {j}";

                bool isOffset = (x % 2 == 0 && y % 2 != 0) || (x % 2 != 0 && y % 2 == 0);
                spawnedTile.Init(isOffset);
                if(!_isometricTiles.ContainsKey(isometricVector3)) 
                {
                    _isometricTiles[isometricVector3] = spawnedTile;
                }
            }
        }
        // for (int x = -_width / 2; x < _width / 2; x++)
        // {
        //     i++;
        //     j = 0;
        //     float depth = _width;
        //     for (int y = -_height / 2; y < _height / 2; y++)
        //     {
        //         j++;
        //         depth--;
        //         // float isometricPosX = x * 0.25f * isometricTransform.localScale.x + y * (-0.25f) * isometricTransform.localScale.y;
        //         // float isometricPosY = x * 0.25f * isometricTransform.localScale.x + y * 0.25f * isometricTransform.localScale.y;
        //         float isometricPosX = x * 0.5f * 1.0f + y * (-0.5f) * 1.0f;
        //         float isometricPosY = x * 0.25f * 1.0f + y * 0.25f * 1.0f;
        //         Vector3 isometricVector3 = new Vector3(isometricPosX,isometricPosY,depth);
        //         Tile spawnedTile = Instantiate(_isometricBlockPrefab, isometricVector3, Quaternion.identity, parentTile);
        //         spawnedTile.name = $"Tile {i} {j}";

        //         bool isOffset = (x % 2 == 0 && y % 2 != 0) || (x % 2 != 0 && y % 2 == 0);
        //         spawnedTile.Init(isOffset);
        //         if(!_isometricTiles.ContainsKey(isometricVector3)) 
        //         {
        //             _isometricTiles[isometricVector3] = spawnedTile;
        //         }
        //     }
        // }
        _tiles = _isometricTiles;
        //Camera.main.transform.position = new Vector3(-0.5f,-0.25f,-10);
        if(_normalTiles.Keys.Count > 0) 
        {
            UnitManager.Instance.AdjustUnitsPosition(_normalTiles, _isometricTiles);
            GameManager.Instance.ChangeState(GameManager.GameState.AttackerTurn);
        }
    }

    public void GenerateIsometricGrid()
    {
        int i = -1, j;
        _tiles = new Dictionary<Vector2, Tile>();
        _isometricTiles = new Dictionary<Vector2, Tile>();
        for (int x = -_width / 2; x < _width / 2; x++)
        {
            i++;
            j = 0;
            for (int y = -_height / 2; y < _height / 2; y++)
            {
                j++;
                float isometricPosX = x * 0.5f * isometricTransform.localScale.x + y * (-0.5f) * isometricTransform.localScale.y;
                float isometricPosY = x * 0.25f * isometricTransform.localScale.x + y * 0.25f * isometricTransform.localScale.y;
                Vector2 isometricVector2 = new Vector2(isometricPosX,isometricPosY);
                Tile spawnedTile = Instantiate(_isometricTilePrefab, isometricVector2, Quaternion.identity, parentTile);
                spawnedTile.name = $"Tile {i} {j}";

                bool isOffset = (x % 2 == 0 && y % 2 != 0) || (x % 2 != 0 && y % 2 == 0);
                spawnedTile.Init(isOffset);
                if(!_isometricTiles.ContainsKey(isometricVector2)) 
                {
                    _isometricTiles[isometricVector2] = spawnedTile;
                }
            }
        }
        _tiles = _isometricTiles;
        //Camera.main.transform.position = new Vector3(-0.5f,-0.25f,-10);
        if(_normalTiles.Keys.Count > 0) 
        {
            UnitManager.Instance.AdjustUnitsPosition(_normalTiles, _isometricTiles);
            GameManager.Instance.ChangeState(GameManager.GameState.AttackerTurn);
        }
    }

    public void GenerateNormalGrid()
    {
        int i = -1, j;
        _tiles = new Dictionary<Vector2, Tile>();
        _normalTiles = new Dictionary<Vector2, Tile>();
        for (int x = -_width / 2; x < _width / 2; x++)
        {
            i++;
            j = 0;
            for (int y = -_height / 2; y < _height / 2; y++)
            {
                j++;
                Vector2 normalVector2 = new Vector2(x,y);
                Tile spawnedTile = Instantiate(_tilePrefab, normalVector2, Quaternion.identity, parentTile);
                spawnedTile.name = $"Tile {i} {j}";

                bool isOffset = (x % 2 == 0 && y % 2 != 0) || (x % 2 != 0 && y % 2 == 0);
                spawnedTile.Init(isOffset);
                if(!_normalTiles.ContainsKey(normalVector2)) 
                {
                    _normalTiles[normalVector2] = spawnedTile;
                }
            }
        }
         _tiles = _normalTiles;
        if(_isometricTiles.Keys.Count > 0) 
        {
            UnitManager.Instance.AdjustUnitsPosition(_isometricTiles, _normalTiles);
            GameManager.Instance.ChangeState(GameManager.GameState.AttackerTurn);
        }
    }

    public bool DestroyAllCurrentGrid()
    {
        for(int i = 0; i < _tiles.Keys.Count; i++)
        {
            Destroy(_tiles[_tiles.ElementAt(i).Key].gameObject);
        }
        return true;
    }

    public Tile GetAttackerSpawnTile()
    {
        //return _tiles.Where(t => t.Key.x < _width / 2).OrderBy(t => UnityEngine.Random.value).FirstOrDefault().Value;
        //Debug.Log(_tiles);
        return _tiles.OrderBy(t => UnityEngine.Random.value).FirstOrDefault().Value;
    }

    public Tile GetDefenderSpawnTile()
    {
        //return _tiles.Where(t => t.Key.x > _width / 2).OrderBy(t => UnityEngine.Random.value).FirstOrDefault().Value;
        return _tiles.OrderBy(t => UnityEngine.Random.value).FirstOrDefault().Value;
        //return _tiles
    }

    public Tile GetTileAtPosition(Vector2 pos)
    {
        if (_tiles.TryGetValue(pos, out var tile)) return tile;
        return null;
    }

    public int getWidth()
    {
        return _width;
    }

    public int getHeight()
    {
        return _height;
    }

    public List<Vector2> GetFreeTiles()
    {
        List<Vector2> results = new List<Vector2>();
        foreach(Vector2 v in _tiles.Keys)
        {
            if(GetTileAtPosition(v).StandingUnit == null)
            {
                results.Add(v);
            }
        }
        return results;
    }

    public List<Vector2> GetAllTiles()
    {
        List<Vector2> results = new List<Vector2>();
        results.AddRange(_tiles.Keys);
        return results;
    }

    public IEnumerator ConvertToNormalTileWithDestroy()
    {
        yield return new WaitUntil(DestroyAllCurrentGrid);
        GenerateNormalGrid();
    }

    public void ConvertToNormalTileWithoutDestroy()
    {
        foreach(Vector2 v in _tiles.Keys)
        {
            float normalPosX = v.x/0.5f/isometricTransform.localScale.x - v.y/(-0.5f)/isometricTransform.localScale.y;
            float normalPosY = v.x/0.25f/isometricTransform.localScale.x - v.y/0.25f/isometricTransform.localScale.y;
            Vector2 normalVector2 = new Vector2(normalPosX, normalPosY);
            Tile spawnedTile = _tiles[v];
            
            _tiles[normalVector2] = spawnedTile;
        }
    }

    public IEnumerator ConvertToIsometricTileWithDestroy()
    {
        yield return new WaitUntil(DestroyAllCurrentGrid);
        GenerateIsometricGrid();
    }

    private List<Vector2> getKnightMoveableNormalTiles(Vector2 playerPos)
    {
        List<Vector2> results = new List<Vector2>();
        results.Add(new Vector2(playerPos.x-(2*normalTransform.localScale.x),playerPos.y-(1*normalTransform.localScale.y)));
        results.Add(new Vector2(playerPos.x-(1*normalTransform.localScale.x),playerPos.y-(2*normalTransform.localScale.y)));
        results.Add(new Vector2(playerPos.x+(1*normalTransform.localScale.x),playerPos.y+(2*normalTransform.localScale.y)));
        results.Add(new Vector2(playerPos.x+(2*normalTransform.localScale.x),playerPos.y+(1*normalTransform.localScale.y)));
        results.Add(new Vector2(playerPos.x+(1*normalTransform.localScale.x),playerPos.y-(2*normalTransform.localScale.y)));
        results.Add(new Vector2(playerPos.x+(2*normalTransform.localScale.x),playerPos.y-(1*normalTransform.localScale.y)));
        results.Add(new Vector2(playerPos.x-(1*normalTransform.localScale.x),playerPos.y+(2*normalTransform.localScale.y)));
        results.Add(new Vector2(playerPos.x-(2*normalTransform.localScale.x),playerPos.y+(1*normalTransform.localScale.y)));
        return results;
    }

    private List<Vector2> getKnightMoveableIsometricTiles(Vector2 playerPos)
    {
        List<Vector2> results = new List<Vector2>();  
        results.Add(new Vector2(playerPos.x+(0.5f*isometricTransform.localScale.x), playerPos.y-(0.75f*isometricTransform.localScale.y)));
        results.Add(new Vector2(playerPos.x+(1.5f*isometricTransform.localScale.x),playerPos.y-(0.25f*isometricTransform.localScale.y)));
        results.Add(new Vector2(playerPos.x+(1.5f*isometricTransform.localScale.x),playerPos.y+(0.25f*isometricTransform.localScale.y))); 
        results.Add(new Vector2(playerPos.x+(0.5f*isometricTransform.localScale.x),playerPos.y+(0.75f*isometricTransform.localScale.y)));
        results.Add(new Vector2(playerPos.x-(0.5f*isometricTransform.localScale.x),playerPos.y+(0.75f*isometricTransform.localScale.y)));
        results.Add(new Vector2(playerPos.x-(1.5f*isometricTransform.localScale.x),playerPos.y+(0.25f*isometricTransform.localScale.y)));
        results.Add(new Vector2(playerPos.x-(1.5f*isometricTransform.localScale.x),playerPos.y-(0.25f*isometricTransform.localScale.y)));
        results.Add(new Vector2(playerPos.x-(0.5f*isometricTransform.localScale.x),playerPos.y-(0.75f*isometricTransform.localScale.y)));
        return results;
    }

    private List<Vector2> getKnightMoveableTiles(bool isIsometric, Vector2 playerPos)
    {
        if(isIsometric) return getKnightMoveableIsometricTiles(playerPos);
        else return getKnightMoveableNormalTiles(playerPos);
    }

    private List<Vector2> getPawnMoveableNormalTiles(Vector2 playerPos)
    {
        List<Vector2> results = new List<Vector2>();
        results.Add(new Vector2(playerPos.x-(2*normalTransform.localScale.x),playerPos.y-(1*normalTransform.localScale.y)));
        results.Add(new Vector2(playerPos.x-(1*normalTransform.localScale.x),playerPos.y-(2*normalTransform.localScale.y)));
        results.Add(new Vector2(playerPos.x+(1*normalTransform.localScale.x),playerPos.y+(2*normalTransform.localScale.y)));
        results.Add(new Vector2(playerPos.x+(2*normalTransform.localScale.x),playerPos.y+(1*normalTransform.localScale.y)));
        results.Add(new Vector2(playerPos.x+(1*normalTransform.localScale.x),playerPos.y-(2*normalTransform.localScale.y)));
        results.Add(new Vector2(playerPos.x+(2*normalTransform.localScale.x),playerPos.y-(1*normalTransform.localScale.y)));
        results.Add(new Vector2(playerPos.x-(1*normalTransform.localScale.x),playerPos.y+(2*normalTransform.localScale.y)));
        results.Add(new Vector2(playerPos.x-(2*normalTransform.localScale.x),playerPos.y+(1*normalTransform.localScale.y)));
        return results;
    }

    private List<Vector2> getPawnMoveableIsometricTiles(Vector2 playerPos)
    {
        List<Vector2> results = new List<Vector2>();  
        results.Add(new Vector2(playerPos.x+(0.5f*isometricTransform.localScale.x), playerPos.y-(0.75f*isometricTransform.localScale.y)));
        results.Add(new Vector2(playerPos.x+(1.5f*isometricTransform.localScale.x),playerPos.y-(0.25f*isometricTransform.localScale.y)));
        results.Add(new Vector2(playerPos.x+(1.5f*isometricTransform.localScale.x),playerPos.y+(0.25f*isometricTransform.localScale.y))); 
        results.Add(new Vector2(playerPos.x+(0.5f*isometricTransform.localScale.x),playerPos.y+(0.75f*isometricTransform.localScale.y)));
        results.Add(new Vector2(playerPos.x-(0.5f*isometricTransform.localScale.x),playerPos.y+(0.75f*isometricTransform.localScale.y)));
        results.Add(new Vector2(playerPos.x-(1.5f*isometricTransform.localScale.x),playerPos.y+(0.25f*isometricTransform.localScale.y)));
        results.Add(new Vector2(playerPos.x-(1.5f*isometricTransform.localScale.x),playerPos.y-(0.25f*isometricTransform.localScale.y)));
        results.Add(new Vector2(playerPos.x-(0.5f*isometricTransform.localScale.x),playerPos.y-(0.75f*isometricTransform.localScale.y)));
        return results;
    }

    private List<Vector2> getPawnMoveableTiles(bool isIsometric, Vector2 playerPos)
    {
        if(isIsometric) return getKnightMoveableIsometricTiles(playerPos);
        else return getKnightMoveableNormalTiles(playerPos);
    }

    private List<Vector2> getBishopMoveableTiles(bool isIsometric, Vector2 playerPos)
    {
        if(isIsometric) return getKnightMoveableIsometricTiles(playerPos);
        else return getKnightMoveableNormalTiles(playerPos);
    }

    private List<Vector2> getRookMoveableTiles(bool isIsometric, Vector2 playerPos)
    {
        if(isIsometric) return getKnightMoveableIsometricTiles(playerPos);
        else return getKnightMoveableNormalTiles(playerPos);
    }

    private List<Vector2> getQueenMoveableTiles(bool isIsometric, Vector2 playerPos)
    {
        if(isIsometric) return getKnightMoveableIsometricTiles(playerPos);
        else return getKnightMoveableNormalTiles(playerPos);
    }

     private List<Vector2> getKingMoveableTiles(bool isIsometric, Vector2 playerPos)
    {
        if(isIsometric) return getKnightMoveableIsometricTiles(playerPos);
        else return getKnightMoveableNormalTiles(playerPos);
    }

    public List<Vector2> GetPlayerMoveableTiles(bool isIsometric, Vector2 playerPos, GameManager.MoveType moveType)
    {
        List<Vector2> results = new List<Vector2>(); 
        switch (moveType) {
        case GameManager.MoveType.Pawn:
            results = getPawnMoveableTiles(isIsometric, playerPos);
            break;
        case GameManager.MoveType.Knight:
            results = getKnightMoveableTiles(isIsometric, playerPos);
            break;
        case GameManager.MoveType.Bishop:
            results = getBishopMoveableTiles(isIsometric, playerPos);
            break;
        case GameManager.MoveType.Rook:
             results = getRookMoveableTiles(isIsometric, playerPos);
            break;
        case GameManager.MoveType.Queen:
            results = getQueenMoveableTiles(isIsometric, playerPos);
            break;
        case GameManager.MoveType.King:
            results = getKingMoveableTiles(isIsometric, playerPos);
            break;
        default :
            break;
       }
       return results;
    }

}