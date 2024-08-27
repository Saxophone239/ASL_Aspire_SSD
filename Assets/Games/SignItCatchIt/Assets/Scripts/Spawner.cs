using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class Spawner : MonoBehaviour
{
    [Header("Prefabs")]
    [SerializeField] private GameObject word; // Word prefab already contains TextMeshPro, or at least it should
    [SerializeField] private GameObject[] powerUps; // Power-up prefabs in array

    [Header("World Bounds and Parameters")]
    // Set bounds for where words spawn
	[SerializeField] private Vector2 xBound;
	[SerializeField] private Vector2 yBound;
    [SerializeField] public float fallingSpeed = 0.2f;
    [SerializeField] private float spawnRate = 1.0f;

	[Header("References")]
	[SerializeField] private SignItGameManager gameManager;
    [SerializeField] private VideoPlayer videoPlayer;
	[SerializeField] private Image imageHolder;
	[SerializeField] private Sprite defaultIconToShow;
    private RawImage rawImage;

    private List<string> currentWordsToSpawn = new List<string>();
    private int currentWordsToSpawnSize = 6;

    // Specific correct word/link chosen at period
    public string CorrectWord = "";
    private bool isSpawnerActive;

	public enum SICIQuestionType
	{
		ASLSignToEnglishWord,
		IconToEnglishWord,
	}

    //private List<string> vidVocabList;

    // Start is called before the first frame update
    private void Start()
    {
        // Make videoplayer transparent
        rawImage = videoPlayer.gameObject.GetComponent<RawImage>();
        rawImage.color = new Color32(255, 255, 255, 0);

        // Handle difficulty setting
        switch (SignItGlobals.difficulty)
		{
			case SignItGlobals.Difficulty.Easy:
                fallingSpeed = 0.2f;
                spawnRate = 1.2f;
				break;
			case SignItGlobals.Difficulty.Medium:
				fallingSpeed = 0.3f;
                spawnRate = 1.0f;
				break;
			case SignItGlobals.Difficulty.Hard:
				fallingSpeed = 0.4f;
                spawnRate = 0.8f;
				break;
		}

		gameManager.OnGameActivated += StartSpawningWords;
    }

    public void StartSpawningWords()
    {
        isSpawnerActive = true;
        ChangeCorrectWord();
        rawImage.color = new Color32(255, 255, 255, 255);
        StartCoroutine(SpawnRandomGameObject());
    }

    public void StopSpawningWords()
    {
        isSpawnerActive = false;
        StopCoroutine(SpawnRandomGameObject());
    }

    public IEnumerator SpawnRandomGameObject()
    {
        if (!isSpawnerActive) yield break;

        yield return new WaitForSeconds(spawnRate);
        int randomVocabWordIndex = Random.Range(0, currentWordsToSpawn.Count);
        if (word.GetComponent<TextMeshPro>() != null)
        {
            word.GetComponent<TextMeshPro>().text = currentWordsToSpawn[randomVocabWordIndex];
            Rigidbody2D wordRigidBody = word.GetComponent<Rigidbody2D>();
            wordRigidBody.gravityScale = fallingSpeed;
            GameObject tmp = Instantiate(word, new Vector2(Random.Range(xBound.x, xBound.y), Random.Range(yBound.x, yBound.y)), Quaternion.identity);
            tmp.transform.SetParent(transform, false);
        }
        else
        {
            Debug.Log("Problem with word! TextMeshPro doesn't exist!");
        }

        if (Random.Range(0, 10) == 0)
        {
            StartCoroutine(SpawnRandomPowerUp());
        }
        StartCoroutine(SpawnRandomGameObject());

        yield return new WaitForSeconds(SignItGlobals.spawnRate);
    }

    IEnumerator SpawnRandomPowerUp()
    {
        yield return new WaitForSeconds(Random.Range(1, 2));

        int randomPowerUp = Random.Range(0, powerUps.Length);
        GameObject tmp = Instantiate(powerUps[randomPowerUp], new Vector2(Random.Range(xBound.x, xBound.y), Random.Range(yBound.x, yBound.y)), Quaternion.identity);
        tmp.transform.SetParent(transform, false);

    }

    // public void ReadFromFileJSON()
    // {
    //     //Debug.Log("about to read file");
    //     // feed in textasset.text, add json file as text asset to a game object (forces load)
    //     Questions questionsjson = JsonUtility.FromJson<Questions>(jsonFile.text);
    //     //Debug.Log("file read");
    //     foreach (Question q in questionsjson.questions)
    //     {
    //         links.Add(q.Link);
    //         words.Add(q.Word);
    //     }
    // }

    public void ChangeCorrectWord()
    {
        // List<string> levelVocabList = LevelOperator.CurrentLevelVocabList; // TODO: set levelVocabList to list of words to show
		List<string> levelVocabList = new List<string>{"mitochondria", "chromosome", "DNA"};
        
        int randomWordIndex = Random.Range(0, levelVocabList.Count);
        string randomWord = levelVocabList[randomWordIndex];
        CorrectWord = randomWord;
        currentWordsToSpawn.Clear();
    
        if (levelVocabList.Count <= currentWordsToSpawnSize)
        {
            currentWordsToSpawn.AddRange(levelVocabList);
        }
        else
        {
            currentWordsToSpawn.Add(CorrectWord);
            
            for (int i = 0; i < currentWordsToSpawnSize; i++)
            {
                int randomVocabWordIndex = Random.Range(0, levelVocabList.Count);
                currentWordsToSpawn.Add(levelVocabList[randomVocabWordIndex]);
            }
        }

		// Choose whether to show video or icon of word
		int choice = Random.Range(0, 2);
		if (choice == 1)
		{
			// Let's show a video
			imageHolder.gameObject.SetActive(false);
			videoPlayer.gameObject.SetActive(true);
			Debug.Log($"About to play video: {CorrectWord}");
			// videoPlayer.url = VideoManager.VocabWordToPathDict[CorrectWord];
			videoPlayer.url = "http://commondatastorage.googleapis.com/gtv-videos-bucket/sample/BigBuckBunny.mp4";
			videoPlayer.Play();
		}
		else
		{
			// Let's show an icon
			videoPlayer.gameObject.SetActive(false);
			imageHolder.gameObject.SetActive(true);
			Debug.Log($"About to show icon: {CorrectWord}");
			imageHolder.sprite = defaultIconToShow; // TODO: add funcionality to show images
		}
    }

	private void OnDestroy()
	{
		gameManager.OnGameActivated -= StartSpawningWords;
	}
}
