using UnityEngine;

public interface IHoverInfoUI
{
    // Called whenever hover state changes for this object.
    // isHovered: cursor over this object
    // inRange: player close enough to interact
    void SetHoverState(bool isHovered, bool inRange);
}