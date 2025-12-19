public interface IProximityHighlightable
{
    /// <summary>
    /// Called when this object becomes (or stops being) the closest interactable in range.
    /// </summary>
    void SetIsClosest(bool isClosest);
}