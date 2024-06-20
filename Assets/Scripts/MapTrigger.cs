using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MapCornerPoint
{
    NoCorner,
    BottomLeft,
    BottomLeftCenter,
    BottomRight,
    BottomRightCenter,
    TopLeft,
    TopLeftCenter,
    TopRight,
    TopRightCenter    
}
public class MapTrigger : MonoBehaviour
{
    [SerializeField] MapCornerPoint MapCornerPoint;
    private void OnTriggerEnter(Collider other)
    {
        if(other.TryGetComponent<Unit>(out var unit))
        {
            unit.MapCornerPoint = MapCornerPoint;
        }
    }
}
