﻿using System;

// Custom awaiter for Cloaca scheduler to have this coroutine pause for one tick.
// Simply yield a new instance of this when you want to pause this script for at least a tick. No
// extra handling necessary.
public class YieldTick : System.Runtime.CompilerServices.INotifyCompletion, ISubscheduledContinuation
{
    private Action continuation;
    MockInterpreter interpreter;

    public bool IsCompleted
    {
        get
        {
            return false;
        }
    }

    public string Text
    {
        get; protected set;
    }

    public YieldTick(MockInterpreter interpreterToSubschedule)
    {
        interpreter = interpreterToSubschedule;
    }

    public void Continue()
    {
        continuation?.Invoke();
    }

    public void OnCompleted(Action continuation)
    {
        this.continuation = continuation;
        interpreter.SetYielded(this);
    }

    public YieldTick GetAwaiter()
    {
        return this;
    }

    public void GetResult()
    {
        // Empty -- just needed to satisfy the rules for how custom awaiters work.
    }
}
