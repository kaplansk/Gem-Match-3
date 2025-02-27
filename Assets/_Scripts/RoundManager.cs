using UnityEngine;
using UnityEngine.SceneManagement;

public class RoundManager : MonoBehaviour
{

    public float roundTime = 0;

    private UIManager UIMan;

    private bool endingRound = false;

    private Board board;

    public int currentScore;

    public float displayScore;
    public float scoreSpeed;

    public int scoreTarget1, scoreTarget2, scoreTarget3;
    void Awake()
    {
        UIMan = FindObjectOfType<UIManager>();
        board = FindObjectOfType<Board>();
    }

    // Update is called once per frame
    void Update()
    {
        if(roundTime > 0)
        {
            roundTime -= Time.deltaTime;

            if(roundTime <= 0)
            {
                roundTime = 0;

                endingRound = true;
            }
        }
        if(endingRound && board.currentState == Board.BoardState.move)
        {
            WinCheck();
            endingRound=false;
        }

        UIMan.timeText.text = roundTime.ToString("0.0") + "s";

        displayScore =Mathf.Lerp(displayScore, currentScore, scoreSpeed *Time.deltaTime);
        UIMan.scoreText.text = displayScore.ToString("0");
    }

    private void WinCheck()
    {
        UIMan.roundOverScreen.SetActive(true);

        UIMan.winScore.text = currentScore.ToString();

        if(currentScore >= scoreTarget3)
        {
            UIMan.winScoreText.text = "Congratulations! You earned 3 stars!";
            UIMan.winStars3.SetActive(true);

            PlayerPrefs.SetInt(SceneManager.GetActiveScene().name + "_Star1", 1);
            PlayerPrefs.SetInt(SceneManager.GetActiveScene().name + "_Star2", 1);
            PlayerPrefs.SetInt(SceneManager.GetActiveScene().name + "_Star3", 1);

        }else if(currentScore >= scoreTarget2)
        {
            UIMan.winScoreText.text = "Congratulations! You earned 2 stars!";
            UIMan.winStars2.SetActive(true);


            PlayerPrefs.SetInt(SceneManager.GetActiveScene().name + "_Star1", 1);
            PlayerPrefs.SetInt(SceneManager.GetActiveScene().name + "_Star2", 1);
        }
        else if (currentScore >= scoreTarget1)
        {
            UIMan.winScoreText.text = "Congratulations! You earned 1 stars!";
            UIMan.winStars1.SetActive(true);

            PlayerPrefs.SetInt(SceneManager.GetActiveScene().name + "_Star1", 1);
        }
        else
        {
            UIMan.winScoreText.text = "Oh no! No stars for you. Try Again?";
        }

    }
}
