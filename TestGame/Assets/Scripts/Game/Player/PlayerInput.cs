using System.Collections;
using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    private const float k_AttackInputDuration = 0.03f;

    protected static PlayerInput s_Instance;

    [HideInInspector] public bool playerControllerInputBlocked;

    protected bool m_Attack;

    private WaitForSeconds m_AttackInputWait;
    private Coroutine m_AttackWaitCoroutine;
    protected Vector2 m_Camera;
    protected bool m_ExternalInputBlocked;
    protected bool m_Jump;

    protected Vector2 m_Movement;
    protected bool m_Pause;

    public static PlayerInput Instance => s_Instance;

    public Vector2 MoveInput
    {
        get
        {
            if (playerControllerInputBlocked || m_ExternalInputBlocked)
                return Vector2.zero;
            return m_Movement;
        }
    }

    public Vector2 CameraInput
    {
        get
        {
            if (playerControllerInputBlocked || m_ExternalInputBlocked)
                return Vector2.zero;
            return m_Camera;
        }
    }

    public bool JumpInput => m_Jump && !playerControllerInputBlocked && !m_ExternalInputBlocked;

    public bool Attack => m_Attack && !playerControllerInputBlocked && !m_ExternalInputBlocked;

    public bool Pause => m_Pause;

    private void Awake()
    {
        m_AttackInputWait = new WaitForSeconds(k_AttackInputDuration);

        if (s_Instance == null)
            s_Instance = this;
        else if (s_Instance != this)
            throw new UnityException("There cannot be more than one PlayerInput script.  The instances are " +
                                     s_Instance.name + " and " + name + ".");
    }


    private void Update()
    {
        m_Movement.Set(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        m_Camera.Set(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
        m_Jump = Input.GetButton("Jump");

        if (Input.GetButtonDown("Fire1"))
        {
            if (m_AttackWaitCoroutine != null)
                StopCoroutine(m_AttackWaitCoroutine);

            m_AttackWaitCoroutine = StartCoroutine(AttackWait());
        }

        m_Pause = Input.GetButtonDown("Pause");
    }

    private IEnumerator AttackWait()
    {
        m_Attack = true;

        yield return m_AttackInputWait;

        m_Attack = false;
    }

    public bool HaveControl()
    {
        return !m_ExternalInputBlocked;
    }

    public void ReleaseControl()
    {
        m_ExternalInputBlocked = true;
    }

    public void GainControl()
    {
        m_ExternalInputBlocked = false;
    }
}