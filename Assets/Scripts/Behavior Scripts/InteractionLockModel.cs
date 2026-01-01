using System;

public class InteractionLockModel
{
    public bool IsLocked => lockCount > 0;
    public event Action<bool> OnLockChanged;

    private int lockCount = 0;

    public void Acquire()
    {
        int prev = lockCount;
        lockCount++;
        if (prev == 0) OnLockChanged?.Invoke(true);
    }

    public void Release()
    {
        if (lockCount <= 0) return;

        lockCount--;
        if (lockCount == 0) OnLockChanged?.Invoke(false);
    }

    public void ForceClear()
    {
        bool wasLocked = IsLocked;
        lockCount = 0;
        if (wasLocked) OnLockChanged?.Invoke(false);
    }
}