using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameplayController : MonoBehaviour {
    public static GameplayController instance;

    public GameObject fruit_PickUp, frog_PickUp, bomb_PickUp, nest_Pickup;

    private float min_X = -4.25f, max_X = 4.25f, min_Y = -2.26f, max_Y = 2.26f;
    private float z_Pos = 5.891f;

    private Text score_Text;
    private Text life_Text;

    private int scoreCount;
    private int livesCount;

    void Awake() {
        MakeInstance();
    }

    void Start() {
        score_Text = GameObject.Find("Score").GetComponent<Text>();
        life_Text = GameObject.Find("Lives").GetComponent<Text>();

        Invoke("StartSpawning", 0.5f);
    }

    void MakeInstance() {
        if (instance == null) {
            instance = this;
        }
    }

    void StartSpawning() {
        StartCoroutine(SpawnPickUps());
    }

    public void CancelSpawning() {
        CancelInvoke("StartSpawning");
    }

    IEnumerator SpawnPickUps() {
        yield return new WaitForSeconds(Random.Range(1f, 1.5f));
        int random = Random.Range(0, 10);

        if (random < 2) {
            Instantiate(bomb_PickUp, new Vector3(Random.Range(min_X, max_X), Random.Range(min_Y, max_Y), z_Pos), Quaternion.identity);
        } else if (random < 3) {
            Instantiate(nest_Pickup, new Vector3(Random.Range(min_X, max_X), Random.Range(min_Y, max_Y), z_Pos), Quaternion.identity);
        } else {
            int anotherRandom = Random.Range(0, 10);

            if (anotherRandom >= 5) {
                Instantiate(frog_PickUp, new Vector3(Random.Range(min_X, max_X), Random.Range(min_Y, max_Y), z_Pos), Quaternion.identity);
            } else {
                Instantiate(fruit_PickUp, new Vector3(Random.Range(min_X, max_X), Random.Range(min_Y, max_Y), z_Pos), Quaternion.identity);
            }
        }

        Invoke("StartSpawning", 0f);
    }

    public void IncreaseScore() {
        scoreCount++;
        score_Text.text = "Score: " + scoreCount;
    }

    public void IncreaseLives() {
        livesCount++;
        life_Text.text = "Lives: " + livesCount;
    }
}