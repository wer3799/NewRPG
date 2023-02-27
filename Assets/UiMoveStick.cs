using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UiMoveStick : SingletonMono<UiMoveStick>
{
    public enum InputType
    {
        Top, Down, Left, Right, Common
    }
    [SerializeField]
    private List<GameObject> arrowSprites;

    public int Horizontal { get; private set; }
    public int Vertical { get; private set; }

    private Transform playerTr;

    private float quickMoveRange_Common = 8f;
    private float quickMoveRange_Down = 5f;

    private readonly WaitForSeconds doubleInputDelay = new WaitForSeconds(0.5f);
    private readonly WaitForSeconds doubleInputDelay_Awake = new WaitForSeconds(0.25f);
    private const float quickMoveDelaySec = 0.5f;
    private const float quickMoveDelaySec_Awake = 0.25f;
    private const float quickMoveDelaySec_New_Weapon = 0.16f;
    private const float quickMoveDelaySec_ChunAwake = 0.11f;

    [SerializeField]
    private Image quickMoveDelayGauge;

    public bool nowTouching { get; private set; } = false;

    //[SerializeField]
    //private GameObject autoObject;

    //[SerializeField]
    //private GameObject autoToggleObject;

    private void Start()
    {
        playerTr = PlayerMoveController.Instance.transform;

        StartCoroutine(TouchCountCheckRoutine());
    }

    private IEnumerator TouchCountCheckRoutine()
    {
        var ws = new WaitForSeconds(1.0f);

        while (true)
        {
#if !UNITY_EDITOR
            if (Input.touchCount == 0)
            {
                EndTouch();
            }
#endif
            yield return ws;
        }
    }


    private void WhenAutoModeChanged(bool auto)
    {
        return;
        //autoObject.SetActive(auto);

        if (auto)
        {
            OffArrowSprites();
            EndTouch();
        }
    }

    private Dictionary<InputType, bool> downInputContainter = new Dictionary<InputType, bool>()
    {
        { InputType.Top,false},
        { InputType.Down,false},
        { InputType.Left,false},
        { InputType.Right,false}
    };

    private Dictionary<InputType, bool> QuickMoveDelayContainter = new Dictionary<InputType, bool>()
    {
        { InputType.Common,false},
    };

    private IEnumerator InputDelay(InputType type)
    {
        downInputContainter[type] = true;

        yield return doubleInputDelay;

        downInputContainter[type] = false;
    }

    private IEnumerator QuickMoveDelay(InputType type)
    {
        float tick = 0f;

        QuickMoveDelayContainter[type] = true;

        float delay = 0f;

        delay = quickMoveDelaySec;

        while (tick < delay)
        {
            tick += Time.deltaTime;
            quickMoveDelayGauge.fillAmount = tick / delay;
            yield return null;
        }

        quickMoveDelayGauge.fillAmount = 1f;
        QuickMoveDelayContainter[type] = false;
    }

    private void ReceiveDownEvent(InputType type)
    {
        if (QuickMoveDelayContainter[InputType.Common] == true) return;

        if (downInputContainter[type] == true)
        {
            StartCoroutine(QuickMoveDelay(InputType.Common));

            //QuickMove(type);
        }
        else
        {
            StartCoroutine(InputDelay(type));
        }
    }


    public void TeleportToTop()
    {
        Top_downEvent();
        Top_downEvent();
    }
    public void TeleportToBottom()
    {
        Down_downEvent();
        Down_downEvent();
    }

    public void Top_downEvent()
    {
        ReceiveDownEvent(InputType.Top);
    }
    public void Down_downEvent()
    {
        ReceiveDownEvent(InputType.Down);
    }

    public void Left_downEvent()
    {
        ReceiveDownEvent(InputType.Left);
    }

    public void Right_downEvent()
    {
        ReceiveDownEvent(InputType.Right);
    }

    public void Top()
    {
        nowTouching = true;
        Vertical = 1;
        Horizontal = 0;

        SetArrowSprites(0);
    }
    public void Down()
    {
        nowTouching = true;
        Vertical = -1;
        Horizontal = 0;

        SetArrowSprites(1);
    }
    public void Left()
    {
        nowTouching = true;
        Horizontal = -1;
        Vertical = 0;

        SetArrowSprites(2);
    }
    public void Right()
    {
        nowTouching = true;
        Horizontal = 1;
        Vertical = 0;

        SetArrowSprites(3);
    }

    public void EndTouch()
    {
        Vertical = 0;
        Horizontal = 0;
        nowTouching = false;
        OffArrowSprites();
    }

    public void SetHorizontalAxsis(int horizontal)
    {
        Horizontal = horizontal;
    }
    public void SetVerticalAxsis(int vertical)
    {
        Vertical = vertical;
    }

    private void SetArrowSprites(int idx)
    {
        for (int i = 0; i < arrowSprites.Count; i++)
        {
            arrowSprites[i].SetActive(i == idx);
        }
    }
    private void OffArrowSprites()
    {
        for (int i = 0; i < arrowSprites.Count; i++)
        {
            arrowSprites[i].SetActive(false);
        }
    }

#if UNITY_EDITOR
    private void Update()
    {
        // if (Input.GetKeyDown(KeyCode.UpArrow))
        // {
        //     Top_downEvent();
        // }
        // if (Input.GetKeyDown(KeyCode.DownArrow))
        // {
        //     Down_downEvent();
        // }
        // if (Input.GetKeyDown(KeyCode.LeftArrow))
        // {
        //     Left_downEvent();
        // }
        // if (Input.GetKeyDown(KeyCode.RightArrow))
        // {
        //     Right_downEvent();
        // }

        if (Input.GetKey(KeyCode.UpArrow))
        {
            Top();
        }
        if (Input.GetKey(KeyCode.DownArrow))
        {
            Down();
        }
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            Left();
        }
        if (Input.GetKey(KeyCode.RightArrow))
        {
            Right();
        }

        if (Input.GetKeyUp(KeyCode.UpArrow))
        {
            EndTouch();
        }
        if (Input.GetKeyUp(KeyCode.DownArrow))
        {
            EndTouch();
        }
        if (Input.GetKeyUp(KeyCode.LeftArrow))
        {
            EndTouch();
        }
        if (Input.GetKeyUp(KeyCode.RightArrow))
        {
            EndTouch();
        }
    }
#endif


}
