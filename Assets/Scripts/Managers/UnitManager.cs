
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Event;

public class UnitManager : MonoBehaviour
{
    public static UnitManager Instance;
    public GameObject attackerPrefab;
    public GameObject defenderPrefab;
    public Transform unitParent;
    [SerializeField] GameObject test;

    private List<GameObject> _lstEnemies;
    private GameObject _player;
    public Transform _playerTransform;

    void Awake()
    {
        Instance = this;
        _lstEnemies = new List<GameObject>();
        //_units = Resources.LoadAll<ScriptableUnit>("Units").ToList();
        //test.GetComponent<CinemachineVirtualCamera>()
    }

    public void SpawnAttackerAtPos(Vector2 vt)
    {
        GameObject spawnedAttacker = Instantiate(attackerPrefab, unitParent);
        spawnedAttacker.transform.position = vt;
        Tile randomSpawnTile = GridManager.Instance.GetTileAtPosition(vt);
        randomSpawnTile.SetUnit(spawnedAttacker);
    }

    public void SpawnDefenderAtPos(Vector2 vt)
    {
        GameObject spawnedDefender = Instantiate(defenderPrefab, unitParent);
        spawnedDefender.transform.position = vt;
        Tile randomSpawnTile = GridManager.Instance.GetTileAtPosition(vt);
        randomSpawnTile.SetUnit(spawnedDefender);
    }

    public void SpawnDefenderInSquare(Vector2 vt)
    {

    }

    public void SpawnDefenderInRectangle(Vector2 vt)
    {

    }

    public void SpawnDefenderInTriangle(Vector2 vt)
    {

    }

    public void SpawnAttackerRandomPos()
    {
        GameObject spawnedAttacker = Instantiate(attackerPrefab, unitParent);
        List<Vector2> allTilesPosition = GridManager.Instance.GetAllTiles();

        spawnedAttacker.transform.position = allTilesPosition[UnityEngine.Random.Range(0, allTilesPosition.Count)];
        Tile randomSpawnTile = GridManager.Instance.GetTileAtPosition(spawnedAttacker.transform.position);

        spawnedAttacker.transform.position = new Vector3(spawnedAttacker.transform.position.x, spawnedAttacker.transform.position.y, 
                                                          spawnedAttacker.transform.position.y);

        spawnedAttacker.name = "Player";
        randomSpawnTile.SetUnit(spawnedAttacker);
        _player = spawnedAttacker;
        
    }

    public void SpawnDefenderRandomPos()
    {
        GameObject spawnedDenfender = Instantiate(defenderPrefab, unitParent);
        List<Vector2> freeTilesPosition = GridManager.Instance.GetFreeTiles();

        spawnedDenfender.transform.position = freeTilesPosition[UnityEngine.Random.Range(0, freeTilesPosition.Count)];
        Tile randomSpawnTile = GridManager.Instance.GetTileAtPosition(spawnedDenfender.transform.position);

        randomSpawnTile.SetUnit(spawnedDenfender);
        
        if (randomSpawnTile.transform.localScale.x > 1.0f)
        {
            spawnedDenfender.transform.localScale = new Vector3(randomSpawnTile.transform.localScale.x - 0.5f, randomSpawnTile.transform.localScale.y - 0.5f, 0);
        }
        else spawnedDenfender.transform.localScale = randomSpawnTile.transform.localScale;
        spawnedDenfender.transform.position = new Vector3(spawnedDenfender.transform.position.x, spawnedDenfender.transform.position.y, 
                                                          spawnedDenfender.transform.position.y);

        _lstEnemies.Add(spawnedDenfender);
        this.PostEvent(EventID.OnEnemyIncrease);
    }

    public void AdjustUnitsPosition(Dictionary<Vector2, Tile> current, Dictionary<Vector2, Tile> target)
    {
        // if(current.ContainsKey((Vector2)_player.transform.position))
        // {
        //     current.
        //     _player.transform.position = target.ElementAt(i).Key;
        //     current.ElementAt(i).Value.StandingUnit = null;
        //     target.ElementAt(i).Value.StandingUnit = _player;
        //     _player.transform.localScale = target.ElementAt(i).Value.transform.localScale;
        // }

        for (int i = 0; i < current.Keys.Count; i++)
        {
            if (current.ElementAt(i).Key == (Vector2)_player.transform.position)
            {
                _player.transform.position = new Vector3(target.ElementAt(i).Key.x, target.ElementAt(i).Key.y, target.ElementAt(i).Key.y);
                current.ElementAt(i).Value.StandingUnit = null;
                target.ElementAt(i).Value.StandingUnit = _player;
                if(target.ElementAt(i).Value.transform.localScale.x > 1.0f)
                {
                    _player.transform.localScale += new Vector3(0.5f,0.5f,0);
                }
                else _player.transform.localScale = target.ElementAt(i).Value.transform.localScale;
                break;
            }
        }


        foreach (GameObject go in _lstEnemies)
        {
            for (int i = 0; i < current.Keys.Count; i++)
            {
                if (current.ElementAt(i).Key == (Vector2)go.transform.position)
                {
                    go.transform.position = new Vector3(target.ElementAt(i).Key.x, target.ElementAt(i).Key.y, target.ElementAt(i).Key.y);
                    current.ElementAt(i).Value.StandingUnit = null;
                    target.ElementAt(i).Value.StandingUnit = go;
                    if(target.ElementAt(i).Value.transform.localScale.x > 1.0f)
                    {   
                        go.transform.localScale += new Vector3(0.5f,0.5f,0);
                    }
                    else go.transform.localScale = target.ElementAt(i).Value.transform.localScale;
                    break;
                }
            }
        }
    }

    public void RemoveEnemy(GameObject go)
    {
        _lstEnemies.Remove(go);
        this.PostEvent(EventID.OnEnemyDecrease);
    }
}