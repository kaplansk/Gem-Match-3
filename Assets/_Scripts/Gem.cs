using System.Collections;
using UnityEngine;

public class Gem : MonoBehaviour
{
    [HideInInspector]
    public Vector2Int posIndex;
    //[HideInInspector] 
    public Board board;

    private Vector2 firstTouchPosition;
    private Vector2 finalTouchPosition;

    private bool mousePressed;
    private float swipeAngle = 0;

    private Gem otherGem;

    public enum GemType
    {
        blue,
        green,
        red,
        yellow,
        purple,
        bomb
        
    }

    public GemType type;

    public bool isMatched;

    private Vector2Int previousPos;

    public GameObject destroyEffect;

    public int blastSize = 2;

    public int scoreValue = 10;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Vector2.Distance(transform.position, posIndex) >0.01f)

        {
            transform.position = Vector2.Lerp(transform.position, posIndex, board.gemSpeed * Time.deltaTime);
        }
        else
        {
            transform.position = new Vector3(posIndex.x, posIndex.y, 0f);
            board.allGems[posIndex.x, posIndex.y] = this;
        }
        
        if (mousePressed && Input.GetMouseButtonUp(0))
        {
            mousePressed = false;

            if(board.currentState == Board.BoardState.move && board.roundMan.roundTime > 0)
            {
                finalTouchPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                CalculateAngle();
            }
            
        } 
    }

     public void SetupGem(Vector2Int pos, Board theBoard)
    {
        posIndex = pos;
        board = theBoard;
    }

    private void OnMouseDown()
    {
       if(board.currentState ==Board.BoardState.move && board.roundMan.roundTime > 0)
        {
            firstTouchPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePressed = true;
        }
      
        
    }

    private void CalculateAngle()
    {
        swipeAngle =Mathf.Atan2(finalTouchPosition.y - firstTouchPosition.y, finalTouchPosition.x - firstTouchPosition.x);
        swipeAngle = swipeAngle * 180 / Mathf.PI;
        Debug.Log(swipeAngle);

        if(Vector3.Distance(firstTouchPosition, finalTouchPosition) > .5f)
        {
            MovePieces();
        }
       
    }

    private void MovePieces()
    {
        previousPos = posIndex; // onceki tasi Index posizyonuna esledik

        if(swipeAngle< 45 && swipeAngle > -45 &&posIndex.x <board.width -1) // saga kaydirma acisi
        {
            otherGem = board.allGems[posIndex.x +1, posIndex.y];
            otherGem.posIndex.x--;
            posIndex.x++;
        }
        else if (swipeAngle > 45 && swipeAngle <= 135&& posIndex.y < board.height - 1) // yukari kaydirma acisi
        {
            otherGem = board.allGems[posIndex.x , posIndex.y +1];
            otherGem.posIndex.y--;
            posIndex.y++;
        }
        else if (swipeAngle < -45 && swipeAngle >= -135 && posIndex.y > 0) // asagi kaydirma acisi 
        {
            otherGem = board.allGems[posIndex.x , posIndex.y - 1];
            otherGem.posIndex.y++;
            posIndex.y--;
        }
        else if (swipeAngle > 135 || swipeAngle <-135 && posIndex.x > 0)  // sola kaydirma acisi 
        {
            otherGem = board.allGems[posIndex.x - 1, posIndex.y];
            otherGem.posIndex.x++;
            posIndex.x--;
        }

        board.allGems[posIndex.x , posIndex.y] = this;
        board.allGems[otherGem.posIndex.x, otherGem.posIndex.y] = otherGem;

        StartCoroutine(CheckMoveCo());
    }


    public IEnumerator CheckMoveCo() //eslesmediyse geri islem alan Coroutine
    {
        board.currentState = Board.BoardState.wait;

        yield return new WaitForSeconds(.2f);

        board.matchFind.FindAllMatches();
        
        if(otherGem != null)
        {
            if (!isMatched && !otherGem.isMatched)
            {
                otherGem.posIndex = posIndex;
                posIndex = previousPos;

                board.allGems[posIndex.x, posIndex.y] = this;
                board.allGems[otherGem.posIndex.x, otherGem.posIndex.y] = otherGem;

                yield return new WaitForSeconds(.5f);
                board.currentState = Board.BoardState.move;
            }
            else
            {
                board.DestroyMatches();
            }
        }
       
    }

}
