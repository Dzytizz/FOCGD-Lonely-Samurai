using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MultiplayerPlayerController : NetworkBehaviour
{
    private MultiplayerPlayer player;

    [SerializeField] private float swipeRange;
    [SyncVar(hook = nameof(ChangeMaterial))] int materialId = 0;
    [SerializeField] private Material[] materials;
    [SerializeField] private GameObject playerIndicator;
    [SerializeField] private Material indicatorMaterial;
    [SerializeField] private GameObject helmetObject;

    public bool isDragging = false;
    private float colliderOffset;
    private Vector2 touchOffset;

    private void Start()
    {
        if (isLocalPlayer)
        {
            playerIndicator.GetComponent<Renderer>().material = indicatorMaterial;
        }
        else
        {
            playerIndicator.SetActive(false);
        }
        player = GetComponent<MultiplayerPlayer>();
        colliderOffset = GetComponent<CapsuleCollider>().radius;
    }

    private void Update()
    {
        if (!isLocalPlayer) return;
        ManageInput();
    }

    
    private void ManageInput()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            Vector2 touchPosition = touch.position;

            if (touch.phase == TouchPhase.Began && !IsPointerOverUIObject())
            {
                Vector2 playerPositionInScreenSpace = Camera.main.WorldToScreenPoint(transform.position);
                touchOffset = touchPosition - new Vector2(playerPositionInScreenSpace.x, playerPositionInScreenSpace.y);
                isDragging = true;
                SetMaterialId(1);
            }

            if (isDragging && touch.phase == TouchPhase.Moved)
            {
                if (touch.deltaPosition.magnitude > swipeRange) // if finger moved fast, dash
                {
                    player.isDashing = true;
                    SetMaterialId(2);
                }
                else
                {
                    player.isDashing = false;
                    SetMaterialId(1);
                }
                Walk(touchPosition - touchOffset); // walk to finger position
            }

            if (isDragging && (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled))
            {
                isDragging = false;
                player.isDashing = false;
                SetMaterialId(0);
            }
        }
    }

    [Command]
    private void SetMaterialId(int id)
    {
        materialId = id;
    }

    private void ChangeMaterial(int oldMaterialId, int newMaterialId)
    {
        helmetObject.GetComponent<Renderer>().material = materials[newMaterialId];
    }

    [Command]
    private void Walk(Vector2 touchPosition)
    {
        Ray ray = Camera.main.ScreenPointToRay(touchPosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 1000, ~((1 << 6) | (1 << 7)))) // ignore 6th and 7th layers (player/enemy)
        {
            Vector3 currentPos = transform.position;
            Vector3 targetPos = hit.point;
            targetPos.y = transform.position.y;
            Vector3 direction = targetPos - currentPos;
            if (direction.magnitude < 1)
            {
                direction = (direction.normalized);
            }

            Ray dirRay = new Ray(currentPos, direction);
            RaycastHit dirHit;

            if (Physics.Raycast(dirRay, out dirHit, direction.magnitude, ~(1 << 7)))
            {
                transform.position = dirHit.point - dirRay.direction.normalized * colliderOffset * 2;
                DraggingResetIfTooFar();    // reset dragging mode if player got stuck and finger moved far away
            }
            else
            {
                transform.LookAt(targetPos);
                transform.position = targetPos;
            }
        }
    }

    private void DraggingResetIfTooFar()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            Vector2 currentPositionV2 = Camera.main.WorldToScreenPoint(transform.position);
            float distanceMoved = (currentPositionV2 - touch.position + touchOffset).magnitude;
            if (touch.phase == TouchPhase.Moved && distanceMoved > 200)
            {
                isDragging = false;
            }
        }
    }

    private bool IsPointerOverUIObject()
    {
        PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
        eventDataCurrentPosition.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
        return results.Count > 0;
    }
}
