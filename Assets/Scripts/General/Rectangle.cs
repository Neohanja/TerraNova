using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rectangle
{
    int startX;
    int startY;
    int endX;
    int endY;

    public Rectangle(Vector2Int startPoint, Vector2Int recSize)
    {
        startX = startPoint.x;
        startY = startPoint.y;
        endX = startX + recSize.x;
        endY = startY + recSize.y;
    }

    public bool RectangleOverlaps(Rectangle other, int buffer = 0)
    {
        return
            PointWithinBox(other.UpperLeft, buffer) ||
            PointWithinBox(other.UpperRight, buffer) ||
            PointWithinBox(other.LowerLeft, buffer) ||
            PointWithinBox(other.LowerRight, buffer) ||
            PointWithinBox(other.Center, buffer) ||
            other.PointWithinBox(UpperLeft, buffer) ||
            other.PointWithinBox(UpperRight, buffer) ||
            other.PointWithinBox(LowerLeft, buffer) ||
            other.PointWithinBox(LowerRight, buffer) ||
            other.PointWithinBox(Center, buffer);

    }

    public bool PointWithinBox(Vector2Int point, int buffer = 0)
    {
        return 
            point.x >= startX - buffer && point.x <= endX + buffer &&
            point.y >= startY - buffer && point.y <= endY + buffer;
    }


    public Vector2Int UpperLeft
    {
        get
        {
            return new Vector2Int(startX, startY);
        }
    }

    public Vector2Int UpperRight
    {
        get
        {
            return new Vector2Int(endX, startY);
        }
    }

    public Vector2Int LowerLeft
    {
        get
        {
            return new Vector2Int(startX, endY);
        }
    }

    public Vector2Int LowerRight
    {
        get
        {
            return new Vector2Int(endX, endY);
        }
    }

    public Vector2Int Center
    {
        get
        {
            int cX = (endX - startX) / 2 + startX;
            int cY = (endY - startY) / 2 + startY;

            return new Vector2Int(cX, cY);
        }
    }
}
