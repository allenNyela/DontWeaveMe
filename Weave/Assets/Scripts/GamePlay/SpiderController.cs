using UnityEngine;
using TMPro;
using System.Collections.Generic;
using System;

public class SpiderController : MonoBehaviour
{
    public Rigidbody2D rb;
    public float moveSpeed;
    //public Vector2 input;
    public int stamina;
    public int maxStamina = 100;
    public TMP_Text staminaText;
    public Mode currentMode = Mode.Idle;
    public WebNode currentConnectingNode;
    private LineRenderer currentLine;
    public static SpiderController instance;
    public WebNode highlightNode;
    public List<WebNode> currentChains = new List<WebNode>();
    public List<WebEdge> currentLines = new List<WebEdge>();
    public int currentChainStamina = 0;
    //public GameObject linePrefab;
    public bool inControl;
    public bool isPlayer = true;
    //condition check hardcode
    public bool formingAWeb = false;
    public bool cancelNode = false;

    public bool nearPlatform = false;
    public bool alreadyMoving = false;
    public GameObject nodePlatform;


    public Transform Ik;

    public Web highlightedWeb;

    //create
    public int potentialStaminaUse; //in realtime
    public SilkBar energyBar;

    [Header("Node Dropping")]
    public int numberOfNodesAvailable;
    public bool canDropNodes;

    public int numberOfFliesEaten = 0;

    float rotateSpeed = 10f;

    //mother rotation
    Vector3 lastPos;
    public float smooth = 10f;
    public float angleOffset = 180f; // adjust if sprite not facing right by default

    public Transform visual;
    public Sprite normalSprite;
    public Sprite antennaUpSprite;

    public List<WebNode> currentLoop = new List<WebNode>();

    public PlayerInput input;

    public Animator animator;

    public enum Mode
    {
        Idle,
        Weaving,
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    public void Init(int maxStamina, int numberNodes)
    {
        this.maxStamina = maxStamina;
        this.numberOfNodesAvailable = numberNodes;

        stamina = maxStamina;
        lastPos = transform.position;

        //if (isPlayer)
        //{
        //    energyBar = GameManager.Instance.playerEnergy;
        //}
        //else
        //{
        //    energyBar = GameManager.Instance.motherEnergy;
        //}
        energyBar.RefreshSilkBar(this);

        if (Ik != null)
        {
            Ik.transform.parent = GameManager.Instance.BoardRoot.transform;
        }
    }

    private void FixedUpdate()
    {
        // mother check
        if (!isPlayer)
        {
            Vector3 movement = transform.position - lastPos;

            if (movement.sqrMagnitude > 0.0001f) // moved enough to rotate
            {
                float angle = Mathf.Atan2(movement.y, movement.x) * Mathf.Rad2Deg;
                Quaternion targetRot = Quaternion.Euler(0, 0, angle + angleOffset);
                //visual.rotation = Quaternion.Euler(0, 0, angle + angleOffset);
                visual.rotation = Quaternion.Lerp(visual.rotation, targetRot, Time.deltaTime * smooth);

                animator?.SetBool("walking", true);
            }
            else
            {
                animator?.SetBool("walking", false);
            }

            lastPos = transform.position;
        }

        if (!inControl)
        {
            rb.linearVelocity = Vector2.zero;
            if (isPlayer)
                animator?.SetBool("walking", false);
            return;

        }

        var velocity = input.MovementInput2D() * moveSpeed;
        if (currentMode == Mode.Weaving && currentConnectingNode != null)
        {
            LimitWeaveMovement(ref velocity);
        }

        rb.linearVelocity = velocity;

        if (isPlayer)
        {
            if (rb.linearVelocity.magnitude > 0.01f)
            {
                animator?.SetBool("walking", true);
            }
            else
            {
                animator?.SetBool("walking", false);
            }

        }

        if (alreadyMoving)
        {
            if (nodePlatform.GetComponent<PlatformMove>().movingHorizontal)
            {
                Vector3 currentPosition = transform.position;
                currentPosition.x = Mathf.Clamp(currentPosition.x, nodePlatform.GetComponent<PlatformMove>().patrolLeftBound.x, nodePlatform.GetComponent<PlatformMove>().patrolRightBound.x);
                currentPosition.y = transform.position.y;
                transform.position = currentPosition;
            } else
            {
                Vector3 currentPosition = transform.position;
                currentPosition.x = transform.position.x;
                currentPosition.y = Mathf.Clamp(currentPosition.y, nodePlatform.GetComponent<PlatformMove>().patrolLeftBound.y, nodePlatform.GetComponent<PlatformMove>().patrolRightBound.y);
                transform.position = currentPosition;
            }
            
        }
    }

    void LimitWeaveMovement(ref Vector2 velocity)
    {
        if (velocity.sqrMagnitude <= 0.0001f)
            return;

        float dt = Time.fixedDeltaTime;

        Vector2 curPos = rb.position;
        Vector2 nextPos = curPos + velocity * dt;

        Vector2Int nodeGrid = currentConnectingNode.gridPos;
        Vector2Int curGrid = WeaveBoardManager.instance.WorldToGridPos(curPos);
        Vector2Int nextGrid = WeaveBoardManager.instance.WorldToGridPos(nextPos);

        float curDist = (nodeGrid - curGrid).magnitude;
        float nextDist = (nodeGrid - nextGrid).magnitude;

        int nextCost = Mathf.CeilToInt(nextDist);

        if (nextCost > stamina && nextDist >= curDist)
        {
            velocity = Vector2.zero;
        }
    }

    // Update is called once per frame
    void Update()
    {
        //staminaText.text = $"Stamina: {stamina}";
        input.SampleInput();
        if (currentLine != null)
        {
            Vector3 p0 = this.transform.position;
            Vector3 p1 = currentConnectingNode.transform.position;

            // force line to stay on the visible 2D plane
            p0.z = 0f;
            p1.z = 0f;

            currentLine.SetPosition(0, p0);
            currentLine.SetPosition(1, p1);
        }

        if (currentMode == Mode.Weaving)
        {
            var currentSpiderPos = WeaveBoardManager.instance.WorldToGridPos(this.transform.position);
            var distance = (currentConnectingNode.gridPos - currentSpiderPos).magnitude;
            //Debug.Log($"====> potential distance {distance}");
            potentialStaminaUse = (int)distance;
            energyBar.RefreshSilkBar(this);
        }

        Vector2 spiderVel = rb.linearVelocity;
        if (spiderVel.sqrMagnitude > 0.01f)
        {
            float targetAngle = Mathf.Atan2(spiderVel.y, spiderVel.x) * Mathf.Rad2Deg + 180f; //ÈôÄ¬ÈÏ³¯×ó
            Quaternion targetRot = Quaternion.Euler(0, 0, targetAngle);
            visual.rotation = Quaternion.Lerp(visual.rotation, targetRot, Time.deltaTime * rotateSpeed);
        }

        //CheckAttena
        CheckHotSpot();

        if (!inControl)
            return;

        //float h = Input.GetAxisRaw("Horizontal");
        //float v = Input.GetAxisRaw("Vertical");

        //input = new Vector2(h, v).normalized;

        //if (Input.GetKeyDown(KeyCode.Space))
        if (input.weaveIsPressed)
        {
            if (!alreadyMoving)
            {
                HandleWeave();
            }
            
        }
        //else if (Input.GetKeyDown(KeyCode.C))
        else if (input.cancelWeaveIsPressed)
        {
            
            CancelWeave();
        }

        if (currentMode != Mode.Weaving)
        {
            //if (Input.GetKeyDown(KeyCode.R))
            if (input.undoIsPressed)
            {
                UndoWeb();
                UndoWebNode();
            }


            //if (Input.GetKeyDown(KeyCode.F) && targetFly != null)            
            if (input.eatIsPressed && targetFly != null)
            {
                Eat(targetFly);
            }

        }

        //if (Input.GetKeyDown(KeyCode.X))
        if (input.pushIsPressed)
        {
            if (nearPlatform && !alreadyMoving)
            {
                MovePlatform(nodePlatform);
                if (nodePlatform.GetComponent<PlatformMove>().movingHorizontal)
                {
                    rb.constraints = RigidbodyConstraints2D.FreezePositionY;
                } else
                {
                    rb.constraints = RigidbodyConstraints2D.FreezePositionX;
                }
                
            } else if (alreadyMoving)
            {
                ReleasePlatform(nodePlatform);
                rb.constraints = RigidbodyConstraints2D.None;
                rb.constraints = RigidbodyConstraints2D.FreezeRotation;
            }
        }


        //if (Input.GetKeyDown(KeyCode.E))
        if (input.dropNodeIsPressed)
        {
            if(numberOfNodesAvailable > 0)
            {
                DropNode();

            }
        }

    }

    private bool inHotspot = false;
    public void CheckHotSpot()
    {
        if (inHotspot)
        {
            visual.GetComponent<SpriteRenderer>().sprite = antennaUpSprite;
        }
        else
        {
            visual.GetComponent<SpriteRenderer>().sprite = normalSprite;
        }
    }

    private void DropNode()
    {
        Debug.Log("Dropping Node");
        GameObject newNode = Instantiate(WeaveBoardManager.instance.nodeObject, transform.position, Quaternion.identity, GameManager.Instance.BoardRoot.gameObject.transform);
        var customNode = newNode.GetComponent<CustomWebNode>();
        customNode.customPosition = transform.position;
        customNode.gridPos = WeaveBoardManager.instance.WorldToGridPos(transform.position);
        numberOfNodesAvailable--;
    }


    public void CancelWeave(bool closeLoop = false)
    {
        if (currentMode == Mode.Weaving)
        {
            if (WeaveBoardManager.instance.silkWebOrder != WeaveBoardManager.instance.defaultSilkWebOrder)
            {
                Highlight.Instance.UnHightlightLineRenderer(currentLine);
            }
            currentConnectingNode = null;
            GameObject.Destroy(currentLine);
            currentMode = Mode.Idle;
            currentLine = null;

            potentialStaminaUse = 0;
            energyBar.RefreshSilkBar(this);
            //energyBar.UpdateStaminaConsume();

            if (!closeLoop)
            {
                if (AudioManager.Instance != null)
                {
                    AudioManager.Instance.PlaySFX(SFXTypes.WebCancel, 1.0f);
                }
                foreach(var go in currentLines)
                {
                    GameObject.Destroy(go.gameObject);
                }

                foreach(var node in currentChains)
                {
                    node.ClearNeighbors();
                }

                // return stamina
                stamina += currentChainStamina;
                energyBar.RefreshSilkBar(this);
                //energyBar.UpdateStaminaConsume();
            }
            else
            {

            }
        }

        currentChainStamina = 0;
        cancelNode = true;
        currentChains.Clear();
        currentLines.Clear();
    }

    public void HandleWeave()
    {
        if (highlightNode != null)
        {
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.PlaySFX(SFXTypes.WebBegin, 1.0f);
            }
            if (currentMode == Mode.Idle)
            {
                currentLoop.Clear();
                //first node
                currentMode = Mode.Weaving;
                CreateLineToNode(highlightNode);
            }
            else
            {
                FinishConnectNode(highlightNode);
                CreateLineToNode(highlightNode);
            }
        }
    }

    //not by controls but by script
    public void ConnectNodes(WebNode nodeA, WebNode nodeB)
    {
        WeaveBoardManager.instance.Connect(nodeA, nodeB, this, false);

        Vector3 p0 = nodeA.transform.position;
        Vector3 p1 = nodeB.transform.position;

        // force line to stay on the visible 2D plane
        p0.z = 0f;
        p1.z = 0f;

        currentLine = Instantiate(WeaveBoardManager.instance.linePrefab).GetComponent<LineRenderer>();
        if (WeaveBoardManager.instance.silkWebOrder != WeaveBoardManager.instance.defaultSilkWebOrder)
        {
            //middle of highlighting
            Highlight.Instance.HighlightSingleLineRenderer(currentLine);
        }
        currentLine.gameObject.SetActive(true);
        currentLine.gameObject.name = $"WebEdge_{WeaveBoardManager.instance.edgeCounter++}";
        currentLine.transform.parent = GameManager.Instance.BoardRoot.gameObject.transform;
        var edge = currentLine.gameObject.AddComponent<WebEdge>();
        edge.owner = this;
        currentLines.Add(edge);


        currentLine.SetPosition(0, p0);
        currentLine.SetPosition(1, p1);

        currentLine.GetComponent<WebEdge>().nodeA = nodeA;
        currentLine.GetComponent<WebEdge>().nodeB = nodeB;

        currentLine.GetComponent<WebEdge>().energyCost = potentialStaminaUse;
        currentLine.GetComponent<WebEdge>().line = currentLine;

        currentLine.GetComponent<WebEdge>().GenerateWebPhysics();

        currentLine = null;
    }

    void FinishConnectNode(WebNode node) 
    {
        if (node != currentConnectingNode)
        {
            WeaveBoardManager.instance.Connect(node, currentConnectingNode, this);

            Vector3 p0 = node.transform.position;
            Vector3 p1 = currentConnectingNode.transform.position;

            // force line to stay on the visible 2D plane
            p0.z = 0f;
            p1.z = 0f;

            currentLine.SetPosition(0, p0);
            currentLine.SetPosition(1, p1);

            currentLine.GetComponent<WebEdge>().nodeA = currentConnectingNode;
            currentLine.GetComponent<WebEdge>().nodeB = node;

            currentLine.GetComponent<WebEdge>().energyCost = potentialStaminaUse;
            currentLine.GetComponent<WebEdge>().line = currentLine;

            currentLine.GetComponent<WebEdge>().GenerateWebPhysics();

            potentialStaminaUse = 0;

            energyBar.RefreshSilkBar(this);
            //energyBar.UpdateStaminaConsume();
        }

    }

    void CreateLineToNode(WebNode node)
    {
        currentLine = Instantiate(WeaveBoardManager.instance.linePrefab).GetComponent<LineRenderer>();
        currentLine.gameObject.SetActive(true);
        if (WeaveBoardManager.instance.silkWebOrder != WeaveBoardManager.instance.defaultSilkWebOrder)
        {
            //middle of highlighting
            Highlight.Instance.HighlightSingleLineRenderer(currentLine);
        }
        currentLine.gameObject.name = $"WebEdge_{WeaveBoardManager.instance.edgeCounter++}";
        currentLine.transform.parent = GameManager.Instance.BoardRoot.gameObject.transform;
        var edge = currentLine.gameObject.AddComponent<WebEdge>();
        edge.owner = this;
        currentConnectingNode = node;

        highlightNode = null;
        currentChains.Add(node);
        currentLines.Add(edge);

        WeaveBoardManager.instance.OnPlayerConnectToNode(currentConnectingNode, this);
    }

    private void Awake()
    {
        instance = this;
    }

    void LateUpdate()
    {
        float halfHeight = Camera.main.orthographicSize;
        float halfWidth = halfHeight * Camera.main.aspect;

        Vector3 pos = transform.position;

        pos.x = Mathf.Clamp(pos.x, -halfWidth, halfWidth);
        pos.y = Mathf.Clamp(pos.y, -halfHeight, halfHeight);

        transform.position = pos;


    }   

    public void ConsumeStamina(int cost)
    {
        this.stamina -= cost;
        this.stamina = Mathf.Max(this.stamina, 0);
        energyBar.RefreshSilkBar(this);
        //energyBar.UpdateStaminaConsume();
    }

    public void GetStamina(int stam)
    {
        this.stamina += stam;
        this.stamina = Mathf.Min(this.stamina, this.maxStamina);
        energyBar.RefreshSilkBar(this);
        //energyBar.UpdateStaminaConsume();
    }

    public void ResetStamina()
    {
        this.stamina = maxStamina;
        energyBar.RefreshSilkBar(this);
        //energyBar.UpdateStaminaConsume();
    }

    public void TurnOnControl()
    {
        inControl = true;
    }

    public void TurnOffControl()
    {
        inControl = false;
    }

    public void UndoWeb()
    {
        if (this.highlightedWeb != null) 
        {
            WeaveBoardManager.instance.UndoWeb(this.highlightedWeb);
            this.highlightedWeb.UndoWeb();
            this.highlightedWeb = null;
        }
    }

    public void UndoWebNode()
    {
        if (this.highlightNode != null && this.highlightNode is CustomWebNode customNode && customNode.GetNeighborCount() == 0)
        {
            Destroy(customNode.gameObject);
            numberOfNodesAvailable++;
        }
    }

    public void Eat(Fly fly)
    {
        Destroy(fly.gameObject);
        GetStamina(fly.staminaValue);
        if (isPlayer)
        {
            GameManager.Instance.OnMeal();
            if (fly.transform.CompareTag("Mother"))
            {
                GameManager.Instance.stateMachine.Trigger<StateChangeTrigger>();
            }
        }

        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlaySFX(SFXTypes.Eat, 1.0f);
        }

        targetFly = null;
    }

    public void MovePlatform(GameObject platform)
    {
        
        alreadyMoving = true;
        if (!platform.GetComponent<PlatformMove>().playerinitPickup)
        {
            platform.GetComponent<PlatformMove>().playerinitPickup = true;
            platform.GetComponent<PlatformMove>().playerStartedPickUpPos = this.gameObject.transform.localPosition;
        }
        platform.transform.parent = this.gameObject.transform;
    }

    public void ReleasePlatform(GameObject platform)
    {
        platform.transform.parent = null;
        alreadyMoving = false;
    }

    private Fly targetFly;

    void OnTriggerEnter2D(Collider2D interactable)
    {
        if (interactable.CompareTag("Fly") || interactable.CompareTag("Mother"))
        {
            var flyComp = interactable.GetComponent<Fly>();
            if (flyComp != null)
            {
                targetFly = flyComp;
            }

            if (interactable.gameObject.GetComponent<WeightedObject>() != null)
            {
                inHotspot = true;
            }
        } else if (interactable.CompareTag("Platform") && !alreadyMoving)
        {
            nearPlatform = true;
            nodePlatform = interactable.gameObject;
        }

        
    }

    void OnTriggerStay2D(Collider2D interactable)
    {
        if (interactable.CompareTag("Fly") || interactable.CompareTag("Mother"))
        {
            var flyComp = interactable.GetComponent<Fly>();
            if (flyComp != null)
            {
                targetFly = flyComp;
            }

            if (interactable.gameObject.GetComponent<WeightedObject>() != null)
            {
                inHotspot = true;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D interactable)
    {
        if (interactable.CompareTag("Fly") || interactable.CompareTag("Mother"))
        {
            targetFly = null;

            if (interactable.gameObject.GetComponent<WeightedObject>() != null)
            {
                inHotspot = false;
            }
        } else if (interactable.CompareTag("Platform") && !alreadyMoving)
        {
            nearPlatform = false;
            //nodePlatform = null;
        }
    }

    [Serializable]
    public class PlayerInput
    {
        public Vector2 movementInput;
        public bool weaveIsPressed;
        public bool eatIsPressed;
        public bool cancelWeaveIsPressed;
        public bool dropNodeIsPressed;
        public bool undoIsPressed;
        public bool pushIsPressed;
        public Vector2 MovementInput2D()
        {
            return movementInput;
        }

        public void SampleInput()
        {
            this.movementInput = default(Vector2);
            this.movementInput = WeaveInputSystem.Move.GetValue();
            this.weaveIsPressed = WeaveInputSystem.Weave.GetKey();
            this.eatIsPressed = WeaveInputSystem.Eat.GetKey();
            this.cancelWeaveIsPressed = WeaveInputSystem.CancelWeave.GetKey();
            this.dropNodeIsPressed = WeaveInputSystem.DropNode.GetKey();
            this.undoIsPressed = WeaveInputSystem.Undo.GetKey();
            this.pushIsPressed = WeaveInputSystem.Push.GetKey();
        }
    }
}
