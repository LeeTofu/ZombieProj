using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IObserver
{
    void ObsUpdate();
    void BeginDragUpdate();
    void EndDragUpdate();
    void DragUpdate();
}
