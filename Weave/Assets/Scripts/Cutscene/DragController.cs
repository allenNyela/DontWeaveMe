using DG.Tweening;
using UnityEngine;

public class DragController : MonoBehaviour
{
    public float maxRadius = 2f;

    private Camera cam;
    private Vector3 startPos;    // 初始位置（圆心）
    private Vector3 grabOffset;  // 鼠标点击点与物体中心的偏移
    private float zDistance;     // 物体到相机的深度，用来 ScreenToWorldPoint

    public Transform targetZone;      // 指定区域的中心
    public float targetRadius = 1f;   // 区域判定范围

    public GameObject hint;

    public bool isInRange = false;
    private bool isBreak = false;
    public float breakTime = 0.5f;
    private float currentDt = 0;
    public DOTweenAnimation shakeAnim;

    public DOTweenAnimation breakBloodAnim;

    public CutsceneManager cutSceneManager;

    void Awake()
    {
        cam = Camera.main;
        startPos = transform.position;
    }

    private void Update()
    {
        if (IsInsideTarget())
        {
            currentDt += Time.deltaTime;
            if (!isInRange)
            {
                isInRange = true;
                OnDragEnterZone();
            }
        }
        else
        {
            if (isInRange)
            {
                isInRange = false;
                OnDragExitZone();
            }
        }

        if (isInRange && currentDt >= breakTime && !isBreak)
        {
            BreakLeg();
        }
    }

    private void BreakLeg()
    {
        isBreak = true;
        shakeAnim.DOPause();
        breakBloodAnim.DOPlay();
        cutSceneManager?.PlayDirector(TimelineWaitType.Custom);
        if (pullSfx != null)
        {
            pullSfx.Stop();
            pullSfx = null;
        }
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlaySFX(SFXTypes.Break, 1.5f);
        }
    }

    AudioSource pullSfx;
    private void OnDragEnterZone()
    {
        currentDt = 0;
        if (!isBreak)
            shakeAnim.DOPlay();

        if (!isBreak)
        {
            if (AudioManager.Instance != null)
            {
                pullSfx = AudioManager.Instance.PlaySFX(SFXTypes.Pull, 1.0f);
            }
        }

    }

    private void OnDragExitZone()
    {
        if (!isBreak)
        {
            shakeAnim.DOPause();
            if (pullSfx != null)
            {
                pullSfx.Stop();
                pullSfx = null;
            }
        }

    }

    void OnMouseDown()
    {
        if (!cam) cam = Camera.main;

        // 记录物体当前深度
        var screenPos = cam.WorldToScreenPoint(transform.position);
        zDistance = screenPos.z;

        Vector3 mouseWorld = ScreenToWorldAtZ(Input.mousePosition, zDistance);
        grabOffset = transform.position - mouseWorld;
    }

    private bool IsInsideTarget()
    {
        if (targetZone == null) return false;

        float dist = Vector3.Distance(transform.position, targetZone.position);
        return dist <= targetRadius;
    }

    void OnMouseDrag()
    {
        if (!cam) return;

        if (isBreak) return;

        hint.gameObject.SetActive(false);

        Vector3 mouseWorld = ScreenToWorldAtZ(Input.mousePosition, zDistance);

        // 理论上的目标位置（未约束）
        Vector3 target = mouseWorld + grabOffset;

        // 从初始位置出发的位移
        Vector3 delta = target - startPos;

        // 超过半径就 clamp 在圆周上
        if (delta.magnitude > maxRadius)
        {
            delta = delta.normalized * maxRadius;
            target = startPos + delta;
        }

        // 如果是 2D 游戏，锁定 z 不动
        target.z = startPos.z;

        transform.position = target;
    }

    private Vector3 ScreenToWorldAtZ(Vector3 screenPos, float z)
    {
        screenPos.z = z;
        return cam.ScreenToWorldPoint(screenPos);
    }

    // 在 Scene 里画出可拖动范围，方便调试
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Vector3 center = Application.isPlaying ? startPos : transform.position;
        Gizmos.DrawWireSphere(center, maxRadius);

        if (targetZone != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(targetZone.position, targetRadius);
        }
    }
}
