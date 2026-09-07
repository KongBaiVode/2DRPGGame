using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//背景移动，形成视差
public class ParallaxBackground : MonoBehaviour
{   
    private GameObject cam;

    [SerializeField] private float parallaxEffect;

    //记录背景初始的 X 轴位置
    private float xPosition;

    // 获取背景图片在游戏世界中的实际宽度
    //可以用于跑酷游戏，使背景图片可以无限使用，实现无限循环视差滚动背景
    private float length;

    void Start()
    {
        cam = GameObject.Find("Main Camera");

        xPosition = this.transform.position.x;

        length = GetComponent<SpriteRenderer>().bounds.size.x;
    }


    void Update()
    {
        // 1. 计算背景应该移动的距离
        float distanceToMove = cam.transform.position.x * parallaxEffect;

        // 2. 更新背景的实际位置
        this.transform.position =  new Vector3(xPosition + distanceToMove, this.transform.position.y);

        // 计算相机相对于背景“走过”的相对位移
        float distanceMoved = cam.transform.position.x * (1 - parallaxEffect);
        // 如果相机向右走出了边界，把背景起始点往前（右）移一张图的距离
        if(distanceMoved > xPosition + length)
            xPosition = xPosition + length;
        // 如果相机向左走出了边界，把背景起始点往后（左）移一张图的距离
        if(distanceMoved < xPosition - length)
            xPosition = xPosition - length;
    }
}
