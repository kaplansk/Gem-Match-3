
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Mono.Cecil;

public class Board : MonoBehaviour
{

    public int width;
    public int height;

    public GameObject bgTilePrefab;

    public Gem[] gems;
    public Gem[,] allGems;

    public float gemSpeed;
    [HideInInspector]
    public MatchFinder matchFind;

    public enum BoardState { wait, move};
    public BoardState currentState = BoardState.move;

    public Gem bomb;
    public float bombChance = 2f;

    private void Awake()
    {
        matchFind = FindFirstObjectByType<MatchFinder>();
    }

    void Start()

    {
        allGems = new Gem[width, height];
        Setup();

    }

    private void Update()
    {
           
          //  matchFind.FindAllMatches();
        
    }



    private void Setup()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {

                Vector2 pos = new Vector2(x, y);
                GameObject bgTile = Instantiate(bgTilePrefab, pos, Quaternion.identity);
                bgTile.transform.parent = transform;
                bgTile.name = ("BG Tile- " + x + "," + y);

                int gemToUse = Random.Range(0, gems.Length);

                int iterations = 0;
                while (MatchesAt(new Vector2Int(x, y), gems[gemToUse]) && iterations < 100)
                {
                    gemToUse = Random.Range(0, gems.Length);
                    iterations++;

                }


                SpawnGem(new Vector2Int(x, y), gems[gemToUse]);


            }
        }
    }



    private void SpawnGem(Vector2Int pos, Gem gemToSpawn)
    {
        if(Random.Range(0f,100f) <bombChance)
        {
            gemToSpawn = bomb;
        }

        Gem gem = Instantiate(gemToSpawn, new Vector3(pos.x, pos.y +height, 0f), Quaternion.identity);
        gem.transform.parent = transform;
        gem.name = ("Gem - " + pos.x + "," + pos.y);
        allGems[pos.x, pos.y] = gem;
        gem.SetupGem(pos, this);

    }

    bool MatchesAt(Vector2Int posToCheck, Gem gemToCheck) // oyun basinda taslarin eslesmemesi icin.
    {
        if (posToCheck.x > 1)
        {
            if (allGems[posToCheck.x - 1, posToCheck.y].type == gemToCheck.type && allGems[posToCheck.x - 2, posToCheck.y].type == gemToCheck.type)
            {
                return true;
            }
        }


        if (posToCheck.y > 1)
        {
            if (allGems[posToCheck.x, posToCheck.y - 1].type == gemToCheck.type && allGems[posToCheck.x, posToCheck.y - 2].type == gemToCheck.type)
            {
                return true;
            }
        }
        return false;

    }

    private void DestroyMatchedGemAt(Vector2Int pos) // eslesen bloklari yok etmek icin
    {
        if (allGems[pos.x, pos.y] != null)
        {
            if (allGems[pos.x, pos.y].isMatched)
            {
                Instantiate(allGems[pos.x,pos.y].destroyEffect, new Vector2(pos.x, pos.y), Quaternion.identity);
                
                Destroy(allGems[pos.x, pos.y].gameObject);
                allGems[pos.x, pos.y] = null;
            }
        }
    }

    public void DestroyMatches() // gemde eger eslesme varsa destroy etmesi icin
    {
        for (int i = 0; i < matchFind.currentMatches.Count; i++)
        {
            if (matchFind.currentMatches[i] != null)
            {
                DestroyMatchedGemAt(matchFind.currentMatches[i].posIndex);
            }
        }
        StartCoroutine(DescreaseRowCo());
    }


    private IEnumerator DescreaseRowCo() // yok olan bloklarin yerine usttekilerin gelmesi 
    {
        yield return new WaitForSeconds(.2f);

        int nullCounter = 0;

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (allGems[x, y] == null)
                {
                    nullCounter++;
                }
                else if (nullCounter > 0)
                {
                    allGems[x, y].posIndex.y -= nullCounter;
                    allGems[x, y - nullCounter] = allGems[x, y];
                    allGems[x, y] = null;
                }
            }

            nullCounter = 0;
        }

        StartCoroutine(FillBoardCo());
    }


    private IEnumerator FillBoardCo()
    {
        yield return new WaitForSeconds(.2f);
        RefillBoard();

        yield return new WaitForSeconds(.2f); // buradan sonra eslesme sonrasi baska eslesenler varsa sirayla yok et
        matchFind.FindAllMatches();

        if (matchFind.currentMatches.Count > 0)
        {
            yield return new WaitForSeconds(.5f);
            DestroyMatches();
        }
        else
        {
            yield return new WaitForSeconds(.5f);
            currentState = BoardState.move;
        }
           
        
    }


    private void RefillBoard() // yukaridaki bloklar asagi indiginde bos yerleri doldur
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (allGems[x, y] == null)
                {
                    int gemToUse = Random.Range(0, gems.Length);
                    SpawnGem(new Vector2Int(x, y), gems[gemToUse]);
                }
                
            }
        }
        CheckMisplacedGems();
    }


    private void CheckMisplacedGems()
    {
        List<Gem> foundGems = new List<Gem>();

        foundGems.AddRange(FindObjectsByType<Gem>(FindObjectsSortMode.InstanceID));

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if(foundGems.Contains(allGems[x, y]))
                {
                    foundGems.Remove(allGems[x, y]);
                }

            }
        }

        foreach(Gem gem in foundGems)
        {
            Destroy(gem.gameObject);
        }
    }





}