using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class EnemyUI : MonoBehaviour
{

    // Start is called before the first frame update
    public Text friendlyValueText;
    public Text nonFriendlyValueText;

    private List<Tile> _enemyTiles;

    public float updateSpan = 1.0f;

    private float _timer = 0;

    void Start()
    {
        this.gameObject.name = "EnemyUI";
        this.transform.parent = GameObject.Find("Canvas").transform;
        this.GetComponent<RectTransform>().anchoredPosition = new Vector2(37, -26);
        this.transform.localScale = Vector3.one;
        _enemyTiles = new List<Tile>();
        Invoke("SetValue", 0.1f);
    }

    private void SetValue()
    {
        friendlyValueText.text = "0";
        var o = GameObject.FindGameObjectsWithTag("Enemy");
        foreach(GameObject obj in o)
        {
            _enemyTiles.Add(obj.GetComponent<Tile>());
        }

        nonFriendlyValueText.text = o.Length.ToString();
    }

    // Update is called once per frame
    void Update()
    {
        _timer += Time.deltaTime;
        if(_timer > updateSpan)
        {
            _timer = 0;
            int friendCount = 0;
            int nonFriendCount = 0;
            foreach(Tile t in _enemyTiles)
            {
                if(t != null)
                {
                    if (t.hasTag(TileTags.Friendly))
                        friendCount++;
                    else
                        nonFriendCount++;
                }
            }
            friendlyValueText.text = friendCount.ToString();
            nonFriendlyValueText.text = nonFriendCount.ToString();
        }   
    }
}
