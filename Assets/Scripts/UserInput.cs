using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserInput : MonoBehaviour
{
    [SerializeField] private LayerMask cardsLayer;

    public event System.Action<Collider2D> OnCardClick;
    public bool InputEnabled 
    { 
        get
        {
            return inputEnabled;
        }
        set
        {
            inputEnabled = value;
        }
    }
    private bool inputEnabled;
    private Camera mainCam;
    private RaycastHit2D raycastHit;
    void Start()
    {
        mainCam = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        if (!inputEnabled) return;
        if (Input.GetMouseButtonDown(0))
        {
            if(FireRaycast(Input.mousePosition, out Collider2D collider))
            {
                OnCardClick?.Invoke(collider);
            }
        }
    }
    private bool FireRaycast(Vector2 screenPos,out Collider2D collider)
    {
        Vector2 worldPos = mainCam.ScreenToWorldPoint(screenPos);

        raycastHit = Physics2D.Raycast(worldPos, Vector2.down, Mathf.Infinity, cardsLayer.value);

        collider = raycastHit.collider;
        return raycastHit.collider != null;
    }
}
