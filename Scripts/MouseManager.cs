using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events; // 添加此行

[System.Serializable] // 修正拼写

public class EventVector3 : UnityEvent<Vector3> {}

public class MouseManager : MonoBehaviour
{
    public EventVector3 OnMouseClicked;
}
    