using System.Linq;
using Assets.Scripts;
using UnityEngine;

public class Tile : MonoBehaviour
{
    public GameObject textFab;
    public Manager manager;

    public int tileValue;

    public bool combined;

    private readonly Color[] COLORS = new []{
        0xEEE4DA,
        0xECE0C8,
        0xF2B179,
        0xF59563,
        0xF57C5F,
        0xE95937,
        0xEDCE71,
        0xEDCC63,
        0xEDD071,
        0xE3B814,
        0xEEC22E,
        0xBBADA0
    }.Select(c=>c.ToColor()).ToArray();
    private readonly Color TEXT_COLOR = new Color(0.17f, 0.17f, 0.27f);

    private readonly Vector3 REGULAR_SIZE = new Vector3(150f, 150f, 1f);
    private readonly Vector3 INCREASED_SIZE = new Vector3(187.5f, 187.5f, 1f);

    private Vector2 movePosition;
    private bool combine;
    private Tile cTile;
    private bool grow;

    void Start()
    {
        movePosition = transform.position;
        textFab = Instantiate(textFab, transform.position, Quaternion.Euler(0, 0, 0));
        ChangeTile(tileValue);
    }

    void Update()
    {
        textFab.GetComponent<GUIText>().transform.position = Camera.main.WorldToViewportPoint(transform.position);

        if (transform.position != new Vector3(movePosition.x, movePosition.y, 0f))
        {
            manager.isMoveFinished = false;

            transform.position = Vector3.MoveTowards(transform.position, movePosition, 35 * Time.deltaTime);
        }
        else
        {
            manager.isMoveFinished = true;

            if (combine)
            {
                ChangeTile(tileValue * 2);
                combine = false;
                grow = true;

                Destroy(cTile.textFab);
                Destroy(cTile.gameObject);

                manager.isMoveFinished = true;
            }
        }

        if (transform.localScale.x != 150 && !grow)
            transform.localScale = Vector3.MoveTowards(transform.localScale, REGULAR_SIZE, 1000 * Time.deltaTime);

        if (grow)
        {
            manager.isMoveFinished = false;
            transform.localScale = Vector3.MoveTowards(transform.localScale, INCREASED_SIZE, 500 * Time.deltaTime);

            if (transform.localScale == INCREASED_SIZE)
                grow = false;
        }
        else
            manager.isMoveFinished = true;
    }

    private void ChangeTile(int newValue)
    {
        tileValue = newValue;

        GetComponent<SpriteRenderer>().color = COLORS[Mathf.RoundToInt(Mathf.Log(tileValue, 2) - 1)]; ;
        textFab.GetComponent<GUIText>().text = tileValue.ToString();

        textFab.GetComponent<GUIText>().color = TEXT_COLOR;
    }

    public bool Move(int x, int y)
    {
        movePosition = CoordinatesUtil.FromGridToCoordinates(x, y);

        if (transform.position != (Vector3)movePosition)
        {
            if (manager.grid[x, y] != null)
            {
                combine = true;
                combined = true;
                cTile = manager.grid[x, y];
                manager.grid[x, y] = null;
            }
            manager.grid[x, y] = GetComponent<Tile>();
            manager.grid[Mathf.RoundToInt(CoordinatesUtil.FromCoordinatesToGrid(transform.position.x, transform.position.y).x), Mathf.RoundToInt(CoordinatesUtil.FromCoordinatesToGrid(transform.position.x, transform.position.y).y)] = null;
            return true;
        }

        return false;
    }
}