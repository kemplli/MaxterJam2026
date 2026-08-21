using Unity.IO.LowLevel.Unsafe;

public class Moveable : MoveableBase
{
    private DraggableSmoothDamp draggable;

    protected override void ManagedInitialize()
    {
        draggable = GetComponent<DraggableSmoothDamp>();
    }

    public override void ManagedUpdate()
    {
        if(DraggableSmoothDamp.CurrentDraggable == draggable)
           transform.position = targetPosition;
    }
}
