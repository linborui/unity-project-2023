using UnityEngine;
using UnityEngine.UI;

public class ScrollBarController : MonoBehaviour
{
    public Scrollbar scrollbar;

    private void Start()
    {
        // 在開始時設置滾動條的值為最大值（底部）
        scrollbar.value = 1;
    }

    private void Update()
    {
        scrollbar.value = 0;
        /*
        // 檢查滾動條的值是否接近最大值（底部）
        if (scrollbar.value <= 0.01f)
        {
            // 將滾動條的值設置為最大值（底部）
            scrollbar.value = 1;
        }*/
    }
}