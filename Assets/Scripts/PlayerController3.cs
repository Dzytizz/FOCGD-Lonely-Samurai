using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerController3 : MonoBehaviour
{
    private Player player;

    [SerializeField] private float swipeRange;

    [SerializeField] private GameObject slowMotionEffectPanel;
    [SerializeField] Material standingMaterial;
    [SerializeField] Material walkingMaterial;
    [SerializeField] Material dashingMaterial;
    [SerializeField] GameObject helmetObject;

    public bool isDragging = false;
    private float colliderOffset;
    private Vector2 touchOffset;

    private void Start()
    {
        player = GetComponent<Player>();
        colliderOffset = GetComponent<CapsuleCollider>().radius;
    }

    private void Update()
    {
        ManageInput();
    }

    private void ManageInput()
    {
        if (!UIManager.Instance.isPaused && Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            Vector2 touchPosition = touch.position;

            if (touch.phase == TouchPhase.Began && !IsPointerOverUIObject())
            {
                //Ray ray = Camera.main.ScreenPointToRay(touchPosition);
                //RaycastHit hit;

                //if (Physics.Raycast(ray, out hit, (1<<6) )) // if player is touched (layer 6), start dragging mode
                //{
                //    isDragging = true;
                //}
                Vector2 playerPositionInScreenSpace = Camera.main.WorldToScreenPoint(transform.position);
                touchOffset = touchPosition - new Vector2(playerPositionInScreenSpace.x, playerPositionInScreenSpace.y);
                isDragging = true;
                helmetObject.GetComponent<Renderer>().material = walkingMaterial;
            }

            if(isDragging && touch.phase == TouchPhase.Moved)
            {
                if (touch.deltaPosition.magnitude > swipeRange) // if finger moved fast, dash
                {
                    player.isDashing = true;
                    helmetObject.GetComponent<Renderer>().material = dashingMaterial;
                }
                else
                {
                    player.isDashing = false;
                    helmetObject.GetComponent<Renderer>().material = walkingMaterial;
                }
                Walk(touchPosition - touchOffset); // walk to finger position
            }

            if (isDragging && (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled))
            {
                isDragging = false;
                player.isDashing = false;
                helmetObject.GetComponent<Renderer>().material = standingMaterial;
            }
        }
    }

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
            if(direction.magnitude < 1) 
            {
                direction = (direction.normalized);
            }

            Ray dirRay = new Ray(currentPos, direction);
            RaycastHit dirHit;

            if (Physics.Raycast(dirRay, out dirHit, direction.magnitude, ~(1<<7)))
            {
                transform.position = dirHit.point - dirRay.direction.normalized * colliderOffset*2;
                DraggingResetIfTooFar();    // reset dragging mode if player got stuck and finger moved far away
            }
            else
            {
                transform.LookAt(targetPos);
                transform.position = targetPos;
            }
        }
    }

    public void StartUltimate()
    {
        if (player.isUltimateReady) StartCoroutine(Ultimate());
    }

    private IEnumerator Ultimate()
    {
        Time.timeScale = 0.05f;
        Time.fixedDeltaTime = Time.timeScale * 0.02f;
        player.PutUltimateOnCoolDown();
        GameObject.Find("AudioManager")?.GetComponent<AudioManager>()?.PlaySound("startSlowMotion");
        player.inUltimate = true;
        slowMotionEffectPanel.SetActive(true);
        yield return new WaitForSecondsRealtime(player.ultimateDuration);

        GameObject.Find("AudioManager")?.GetComponent<AudioManager>()?.PlaySound("endSlowMotion");
        Time.timeScale = 1;
        player.inUltimate = false;
        slowMotionEffectPanel.SetActive(false);
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
