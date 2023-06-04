using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]

public class CollectUI : MonoBehaviour
{
    public string itemName;
    public int max;
    Text text;

    void Start()
    {
        text = GetComponent<Text>();
    }

    // Update is called once per frame
    void Update()
    {
        text.text = Collector.count[itemName].ToString() + " / " + max.ToString();
    }
}
