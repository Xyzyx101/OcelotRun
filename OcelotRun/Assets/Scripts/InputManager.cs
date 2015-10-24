using UnityEngine;
using System.Collections;

public enum Action
{
    FORWARD,
    BACKWARD,
    JUMP,
    _ACTION_LENGTH
}

public class InputManager : MonoBehaviour
{
    public bool[] held = new bool[(int)Action._ACTION_LENGTH];
    public bool[] pressed = new bool[(int)Action._ACTION_LENGTH];
    private bool[] canPress = new bool[(int)Action._ACTION_LENGTH];

    void Start()
    {
        for (int i = 0; i < (int)Action._ACTION_LENGTH; ++i)
        {
            canPress[i] = true;
        }
    }

    void Update()
    {
        held[(int)Action.FORWARD] = Input.GetKey(KeyCode.RightArrow);
        held[(int)Action.BACKWARD] = Input.GetKey(KeyCode.LeftArrow);
        held[(int)Action.JUMP] = Input.GetKey(KeyCode.Space);

        { // Forward
            if (canPress[(int)Action.FORWARD] && Input.GetKeyDown(KeyCode.RightArrow))
            {
                pressed[(int)Action.FORWARD] = true;
            }

            if (Input.GetKeyUp(KeyCode.RightArrow))
            {
                canPress[(int)Action.FORWARD] = true;
                pressed[(int)Action.FORWARD] = false;
            }
            if (canPress[(int)Action.FORWARD] && Input.GetKeyDown(KeyCode.RightArrow))
            {
                pressed[(int)Action.FORWARD] = true;
            }
        }

        { // Backward
            if (canPress[(int)Action.BACKWARD] && Input.GetKeyDown(KeyCode.LeftArrow))
            {
                pressed[(int)Action.BACKWARD] = true;
            }

            if (Input.GetKeyUp(KeyCode.LeftArrow))
            {
                canPress[(int)Action.BACKWARD] = true;
                pressed[(int)Action.BACKWARD] = false;
            }
            if (canPress[(int)Action.BACKWARD] && Input.GetKeyDown(KeyCode.LeftArrow))
            {
                pressed[(int)Action.BACKWARD] = true;
            }
        }

        { // Jump
            if (canPress[(int)Action.JUMP] && Input.GetKeyDown(KeyCode.Space))
            {
                pressed[(int)Action.JUMP] = true;
            }

            if (Input.GetKeyUp(KeyCode.Space))
            {
                canPress[(int)Action.JUMP] = true;
                pressed[(int)Action.JUMP] = false;
            }
            if (canPress[(int)Action.JUMP] && Input.GetKeyDown(KeyCode.Space))
            {
                pressed[(int)Action.JUMP] = true;
            }
        }
    }

    public bool Held(Action action)
    {
        return held[(int)action];
    }

    public bool Pressed(Action action)
    {
        bool val = pressed[(int)action];
        pressed[(int)action] = false;
        return val;
    }
}
