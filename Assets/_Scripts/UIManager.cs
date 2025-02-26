using UnityEngine;
using System.Collections.Generic;
using TMPro;
public class UIManager : MonoBehaviour
{
    public TMP_Text timeText;
    public TMP_Text scoreText;

    public GameObject roundOverScreen;

    public TMP_Text winScore;
    public TMP_Text winScoreText;
    public GameObject winStars1, winStars2, winStars3;

    

    void Start()
    {
        winStars1.SetActive(false);
        winStars2.SetActive(false);
        winStars3.SetActive(false); 
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
