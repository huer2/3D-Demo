using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;



public class MouseManager : MonoBehaviour
{
    public static MouseManager Instance;

    RaycastHit hitInfo;
    public event Action<Vector3> OnMouseClicked;
    public Texture2D point,doorway,attack,target,arrow;
    void Awake()
    {
        if(Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }
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
            switch(hitInfo.collider.gameObject.tag)
            {
                case "Point":
                    Cursor.SetCursor(point, new Vector2(16,16), CursorMode.Auto); 
                    break;
                case "Door":
                    Cursor.SetCursor(doorway, new Vector2(16,16), CursorMode.Auto);                  
                    break;
                case "Attack":
                    Cursor.SetCursor(attack, new Vector2(16,16), CursorMode.Auto); 
                    break;
                case "Ground":
                    Cursor.SetCursor(target, new Vector2(16,16), CursorMode.Auto);
                    break;
                case "Arrow":
                    Cursor.SetCursor(arrow, new Vector2(16,16), CursorMode.Auto); 
                    break;
                default:
                    Cursor.SetCursor(null, new Vector2(16,16), CursorMode.Auto); 
                    break;
            }
        }
        else
        {
            Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto); // ÐÞÕýÆ´Ð´
        }
    }
    void MouseControl()
    {
        if(Input.GetMouseButtonDown(0) && hitInfo.collider != null) // ÐÞÕýÆ´Ð´
        {
            if(hitInfo.collider.gameObject.CompareTag("Ground")) // ÐÞÕýÆ´Ð´
                OnMouseClicked?.Invoke(hitInfo.point); 
        }
    }
}
