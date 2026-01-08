using System.Collections;
using TMPro;
using UnityEngine;

[DefaultExecutionOrder(-100)] // run before typical player scripts so Space can be "consumed" cleanly
[RequireComponent(typeof(Collider2D))]
public class DraggableObject : MonoBehaviour
{
    public enum DragDirection { Horizontal, Vertical }

    [Header("Drag Settings")]
    public DragDirection direction = DragDirection.Horizontal;

    [Tooltip("Min world position the object can be dragged to (clamps x/y as relevant).")]
    public Vector2 minWorldPos = new Vector2(-5f, -5f);

    [Tooltip("Max world position the object can be dragged to (clamps x/y as relevant).")]
    public Vector2 maxWorldPos = new Vector2(5f, 5f);

    [Tooltip("Units per second while dragging. If 0 or less, will try to use player.moveSpeed.")]
    public float dragSpeed = 0f;

    [Header("Prompt UI (Optional)")]
    [Tooltip("Enable/disable this object when the player is in range (e.g., 'Press Space').")]
    public GameObject promptGO;

    [Tooltip("If assigned, will set this text when in range.")]
    public TMP_Text promptText;

    [TextArea] public string promptMessage = "Press Space to drag";

    [Header("Physics (Optional)")]
    [Tooltip("If true and a Rigidbody2D exists, movement uses MovePosition for smoother physics interaction.")]
    public bool useRigidbodyMovePosition = true;

    private Collider2D _col;
    private Rigidbody2D _rb;

    private SpiderController _playerInRange;
    private SpiderController _draggingPlayer;
    private bool _isDragging;

    // When dragging, we keep the player "attached" to the object using this offset.
    private Vector2 _playerOffsetFromObject;

    // Cache the fixed axis value so it doesn't drift.
    private float _lockedY;
    private float _lockedX;

    // input captured in Update, applied in FixedUpdate
    private Vector2 _dragInput;

    private Coroutine _restoreControlRoutine;

    private static DraggableObject _activeDrag; // only one draggable at a time

    private void Awake()
    {
        _col = GetComponent<Collider2D>();
        _rb = GetComponent<Rigidbody2D>();

        // Clamp initial position into bounds
        Vector2 p = transform.position;
        p.x = Mathf.Clamp(p.x, minWorldPos.x, maxWorldPos.x);
        p.y = Mathf.Clamp(p.y, minWorldPos.y, maxWorldPos.y);
        SetObjectPosition(p);

        SetPrompt(false);
    }

    private void OnEnable()
    {
        SetPrompt(false);
    }

    private void Update()
    {
        // Show prompt if player is nearby and nothing is being dragged
        bool canPrompt = !_isDragging && _playerInRange != null && _activeDrag == null;
        SetPrompt(canPrompt);

        if (canPrompt && Input.GetKeyDown(KeyCode.Space))
        {
            BeginDrag(_playerInRange);
            return;
        }

        if (_isDragging && _draggingPlayer != null)
        {
            // Space releases
            if (Input.GetKeyDown(KeyCode.Space))
            {
                EndDrag();
                return;
            }

            // Capture input for FixedUpdate movement
            float h = Input.GetAxisRaw("Horizontal");
            float v = Input.GetAxisRaw("Vertical");
            _dragInput = new Vector2(h, v).normalized;

            // Restrict input to the selected axis
            if (direction == DragDirection.Horizontal) _dragInput.y = 0f;
            else _dragInput.x = 0f;
        }
    }

    private void FixedUpdate()
    {
        if (!_isDragging || _draggingPlayer == null)
            return;

        float speed = dragSpeed;
        if (speed <= 0f)
        {
            // Try to use the player's moveSpeed if available
            speed = (_draggingPlayer != null) ? _draggingPlayer.moveSpeed : 3f;
            if (speed <= 0f) speed = 3f;
        }

        Vector2 curObjPos = GetObjectPosition();
        Vector2 delta = _dragInput * speed * Time.fixedDeltaTime;
        Vector2 targetObjPos = curObjPos + delta;

        // Lock the non-moving axis so the object doesn't drift.
        if (direction == DragDirection.Horizontal)
        {
            targetObjPos.y = _lockedY;
            targetObjPos.x = Mathf.Clamp(targetObjPos.x, minWorldPos.x, maxWorldPos.x);
            // still clamp y just in case
            targetObjPos.y = Mathf.Clamp(targetObjPos.y, minWorldPos.y, maxWorldPos.y);
        }
        else // Vertical
        {
            targetObjPos.x = _lockedX;
            targetObjPos.y = Mathf.Clamp(targetObjPos.y, minWorldPos.y, maxWorldPos.y);
            targetObjPos.x = Mathf.Clamp(targetObjPos.x, minWorldPos.x, maxWorldPos.x);
        }

        SetObjectPosition(targetObjPos);

        // Move player with the object so their movement is effectively "locked" to this axis.
        if (_draggingPlayer != null && _draggingPlayer.rb != null)
        {
            Vector2 targetPlayerPos = targetObjPos + _playerOffsetFromObject;

            if (useRigidbodyMovePosition)
                _draggingPlayer.rb.MovePosition(targetPlayerPos);
            else
                _draggingPlayer.transform.position = targetPlayerPos;
        }
    }

    private void BeginDrag(SpiderController player)
    {
        if (player == null) return;
        if (_activeDrag != null) return;

        _activeDrag = this;
        _isDragging = true;
        _draggingPlayer = player;

        // Lock player control (so SpiderController won't also respond to WASD/Space)
        _draggingPlayer.TurnOffControl();

        // Attach player to object at their current relative offset
        Vector2 objPos = GetObjectPosition();
        Vector2 playerPos = _draggingPlayer.transform.position;
        _playerOffsetFromObject = playerPos - objPos;

        // Cache the axis lock values
        _lockedX = objPos.x;
        _lockedY = objPos.y;

        SetPrompt(false);
    }

    private void EndDrag()
    {
        if (!_isDragging) return;

        _isDragging = false;
        _dragInput = Vector2.zero;

        // Keep control OFF for the rest of this frame so Space doesn't trigger other actions,
        // then restore next frame.
        if (_draggingPlayer != null)
        {
            if (_restoreControlRoutine != null)
                StopCoroutine(_restoreControlRoutine);

            _restoreControlRoutine = StartCoroutine(RestoreControlNextFrame(_draggingPlayer));
        }

        _draggingPlayer = null;
        _activeDrag = null;

        // If the player is still inside the collider, the prompt can show again next Update
    }

    private IEnumerator RestoreControlNextFrame(SpiderController player)
    {
        yield return null;
        if (player != null)
            player.TurnOnControl();

        _restoreControlRoutine = null;
    }

    private void SetPrompt(bool on)
    {
        if (promptGO != null)
            promptGO.SetActive(on);

        if (promptText != null)
        {
            promptText.gameObject.SetActive(on);
            if (on) promptText.text = promptMessage;
        }
    }

    private Vector2 GetObjectPosition()
    {
        if (_rb != null) return _rb.position;
        return transform.position;
    }

    private void SetObjectPosition(Vector2 pos)
    {
        if (_rb != null && useRigidbodyMovePosition)
            _rb.MovePosition(pos);
        else if (_rb != null)
            _rb.position = pos;
        else
            transform.position = pos;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (_isDragging) return;

        var player = other.GetComponent<SpiderController>();
        if (player != null && player.isPlayer)
            _playerInRange = player;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        var player = other.GetComponent<SpiderController>();
        if (player != null && player == _playerInRange)
            _playerInRange = null;
    }

    private void OnDrawGizmosSelected()
    {
        // Visualize bounds in scene view
        Gizmos.color = Color.yellow;
        Vector3 center = (minWorldPos + maxWorldPos) * 0.5f;
        Vector3 size = new Vector3(Mathf.Abs(maxWorldPos.x - minWorldPos.x), Mathf.Abs(maxWorldPos.y - minWorldPos.y), 0.1f);
        Gizmos.DrawWireCube(center, size);
    }
}
