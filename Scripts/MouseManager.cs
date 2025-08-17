using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class EventVector3 : UnityEvent<Vector3> {}

public class MouseManager : MonoBehaviour
{
    RaycastHit hitInfo;
    public EventVector3 OnMouseClicked;

    void Update()
    {
        SetCursorTexture();
        MouseControl();
    }
    void SetCursorTexture() // ÐÞÕý·½·¨Ãû
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition); // ÐÞÕýÆ´Ð´
        if(Physics.Raycast(ray, out hitInfo)) // ÐÞÕýÆ´Ð´
        {
            //ÇÐ»»Êó±êÌùÍ¼
        }
    }
    void MouseControl()
    {
        if(Input.GetMouseButtonDown(0) && hitInfo.collider != null) // ÐÞÕýÆ´Ð´
        {
            if(hitInfo.collider.gameObject.CompareTag("Ground")) // ÐÞÕýÆ´Ð´
                OnMouseClicked?.Invoke(hitInfo.point); // ÐÞÕýÆ´Ð´
        }
    }
}
