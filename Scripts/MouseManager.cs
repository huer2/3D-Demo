using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events; // ��Ӵ���

[System.Serializable] // ����ƴд

public class EventVector3 : UnityEvent<Vector3> {}

public class MouseManager : MonoBehaviour
{
    public EventVector3 OnMouseClicked;
}
    