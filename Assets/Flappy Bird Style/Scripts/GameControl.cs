using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

using BackEnd;

public class GameControl : MonoBehaviour 
{
	public static GameControl instance;			//A reference to our game control script so we can access it statically.
	public Text scoreText;						//A reference to the UI text component that displays the player's score.
	public GameObject gameOvertext;				//A reference to the object that displays the text which appears when the player dies.

	private int score = 0;						//The player's score.
	public bool gameOver = false;				//Is the game over?
	public float scrollSpeed = -1.5f;

    private int bestScore = 0;

    // 임시로 싱글톤 안 만들고 이렇게
    BackEndEngine backendEng = null;

	void Awake()
	{
        if (backendEng == null)
        {
            backendEng = FindObjectOfType<BackEndEngine>();
        }

        BackendReturnObject userPlayData = Backend.GameInfo.GetContentsByIndate("PlayData", backendEng.userIndate);
        //BackendReturnObject userPlayData = Backend.GameInfo.GetPublicContentsByGamerIndate("PlayData", backendEng.userIndate);
        Debug.Log(userPlayData.GetStatusCode());
        if (userPlayData.GetStatusCode() != "404")
        {
            // 플레이 데이터가 있을 경우
            Debug.Log(userPlayData.GetReturnValue());
        }
        else
        {
            bestScore = 0;

            Param scoreParam = new Param();
            scoreParam.Add("score", 0);

            Backend.GameInfo.Insert("PlayData", scoreParam);
        }

		//If we don't currently have a game control...
		if (instance == null)
			//...set this one to be it...
			instance = this;
		//...otherwise...
		else if(instance != this)
			//...destroy this one because it is a duplicate.
			Destroy (gameObject);
	}

	void Update()
	{
		//If the game is over and the player has pressed some input...
		if (gameOver && Input.GetMouseButtonDown(0)) 
		{
			//...reload the current scene.
			SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
		}
	}

    void SaveData()
    {
        if (score > bestScore)
        {
            Param scoreParam = new Param();
            scoreParam.Add("score", this.score);

            Backend.GameInfo.Update("PlayData", backendEng.userIndate, scoreParam);
        }
    }

	public void BirdScored()
	{
		//The bird can't score if the game is over.
		if (gameOver)	
			return;
		//If the game is not over, increase the score...
		score++;
		//...and adjust the score text.
		scoreText.text = "Score: " + score.ToString();
	}

	public void BirdDied()
	{
		//Activate the game over text.
		gameOvertext.SetActive (true);
		//Set the game to be over.
		gameOver = true;

        SaveData();
    }
}
