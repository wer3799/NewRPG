using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine.Unity;
using static PlayerViewController;

public enum MoveDirection
{
    Left,
    Right,
    Max
};

public class PlayerMoveController : SingletonMono<PlayerMoveController>
{
    [SerializeField] private Rigidbody2D rb;
    public Rigidbody2D Rb => rb;

    [SerializeField] private Transform characterView;

    [SerializeField] private Collider2D collider2D;

    private UiMoveStick uiMoveStick;

    private string horizontal = "Horizontal";
    private string vertical = "Vertical";

    public MoveDirection MoveDirection { get; private set; }

    private void Start()
    {
        Initialize();
    }


    private void Initialize()
    {
        uiMoveStick = UiMoveStick.Instance;
    }

    void Update()
    {
        MovePlayer();
    }

    private void MovePlayer()
    {
        Vector3 moveDirection;

        float moveSpeed = PlayerBalance.moveSpeed;

        moveDirection = new Vector3(GetHorizontalAxis(), GetVerticalAxis());


        rb.velocity = moveDirection * moveSpeed;

        Debug.Log($"{"GetHorizontalAxis()"} {"GetVerticalAxis()"} {rb.velocity}");

        if (rb.velocity.magnitude != 0)
        {
            PlayerViewController.Instance.SetCurrentAnimation(AnimState.run);
        }
        else
        {
            PlayerViewController.Instance.SetCurrentAnimation(AnimState.idle);
        }
    }

    private int GetVerticalAxis()
    {
        int value = 0;
        
#if UNITY_EDITOR
        value = (int)Input.GetAxisRaw(vertical);
#else
        value = uiMoveStick.Vertical;
#endif
        
        return value;
    }

    private int GetHorizontalAxis()
    {
        int value = 0;


#if UNITY_EDITOR
        value = (int)Input.GetAxisRaw(horizontal);
#else
        value = uiMoveStick.Horizontal;
#endif


        if (value == 1)
        {
            SetMoveDirection(MoveDirection.Right);
        }
        else if (value == -1)
        {
            SetMoveDirection(MoveDirection.Left);
        }

        return value;
    }

    public void SetMoveDirection(MoveDirection direction)
    {
        MoveDirection = direction;
        FlipCharacter();
    }

    private void FlipCharacter()
    {
        if (MoveDirection == MoveDirection.Left)
        {
            characterView.transform.localScale = new Vector3(-1f, 1f, 1f);
            characterView.transform.rotation = Quaternion.identity;
        }
        else
        {
            characterView.transform.localScale = Vector3.one;
            characterView.transform.rotation = Quaternion.identity;
        }
    }

    public void AddForce(Vector3 direction, float power)
    {
        rb.velocity = Vector3.zero;
        rb.AddForce(direction * power, ForceMode2D.Impulse);
    }
}