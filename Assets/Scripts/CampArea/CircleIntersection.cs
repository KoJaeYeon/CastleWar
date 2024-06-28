using UnityEngine;
public static class CircleIntersection
{
    public static bool FindCircleIntersections(Vector2 center1, float radius1, Vector2 center2, float radius2, out Vector2 intersection1, out Vector2 intersection2)
    {
        float dx = center2.x - center1.x;
        float dy = center2.y - center1.y;
        float distance = Mathf.Sqrt(dx * dx + dy * dy);

        if (distance > radius1 + radius2 || distance < Mathf.Abs(radius1 - radius2))
        {
            intersection1 = Vector2.zero;
            intersection2 = Vector2.zero;
            return false;
        }

        float a = (radius1 * radius1 - radius2 * radius2 + distance * distance) / (2 * distance);
        float h = Mathf.Sqrt(radius1 * radius1 - a * a);

        Vector2 midpoint = center1 + a * new Vector2(dx / distance, dy / distance);

        intersection1 = midpoint + h * new Vector2(-dy / distance, dx / distance);
        intersection2 = midpoint - h * new Vector2(-dy / distance, dx / distance);

        return true;
    }
}
