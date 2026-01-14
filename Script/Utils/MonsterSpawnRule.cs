using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public static class MonsterSpawnRule
{
    public static List<Vector2> GetCircleSpawnPoints(int num, float radius, float angleOffset, float colliderRadius)
    {
        float angleInterval = 360 / num;

        float maximumColliderRadius = Mathf.Sin(angleInterval / 2 * Mathf.Deg2Rad) * radius;
        bool isColliderOverlapped = maximumColliderRadius < colliderRadius;

        if(num < 2)
        {
            return new List<Vector2>() { Quaternion.AngleAxis(angleOffset, Vector3.forward) * (Vector2.right * radius) };
        }
        else if(isColliderOverlapped)
        {
            return GetCircleSpawnPoints(num - 1, radius, angleOffset, colliderRadius);
        }
        else
        {
            var vectorToRotate = Vector2.right * radius;

            List<float> angles = new List<float>(num);
            for (int i = 0; i < num; i++)
                angles.Add(angleOffset + (angleInterval * i));

            return (from angle in angles
                    select Quaternion.AngleAxis(angle, Vector3.forward).Rotate(vectorToRotate)).ToList();
        }
    }

    public static List<Vector2> GetLinearSpawnPoints(int num, float distance, float length, float angle, float colliderRadius)
    {
        List<Vector2> points = new List<Vector2>(num);

        float lengthInterval = length / (num - 1);
        bool isColliderOverlapped = lengthInterval / 2 < colliderRadius;

        var quaternion = Quaternion.AngleAxis(angle, Vector3.forward);
        var moveVector = quaternion.Rotate(Vector2.right * distance);

        if (num < 2)
        {
            return new List<Vector2>() { new Vector2(0, 0) + moveVector };
        }
        else if(isColliderOverlapped)
        {
            return GetLinearSpawnPoints(num - 1, distance, length, angle, colliderRadius);
        }
        else
        {
            for (int i = 0; i < num; i++)
                points.Add(new Vector2(0, lengthInterval * i * -1 + (length / 2)));

            return (from point in points
                    select quaternion.Rotate(point) + moveVector).ToList();
        }

    }

    public static List<Vector2> GetAreaSpawnPoints(int num, Rect rect, float colliderRadius)
    {
        int widthCapacity = Mathf.FloorToInt(rect.width / colliderRadius);
        int heightCapacity = Mathf.FloorToInt(rect.height / colliderRadius);

        int capacity = widthCapacity * heightCapacity;
        bool isColliderOverlapped = capacity < num;

        if (num < 2)
        {
            return new List<Vector2>() { rect.center };
        }
        else if(isColliderOverlapped)
        {
            return GetAreaSpawnPoints(capacity, rect, colliderRadius);
        }
        else
        {
            List<Vector2> points = new List<Vector2>(num);
            int spawnedNum = 0;
            for(int i = 0; i < widthCapacity; i++)
            {
                for(int j = 0; j < heightCapacity; j++)
                {
                    points.Add(new Vector2(rect.xMin + colliderRadius * i + colliderRadius / 2, rect.yMin + colliderRadius * j + colliderRadius / 2));
                    spawnedNum++;
                    if (spawnedNum == num)
                        break;
                }
                if (spawnedNum == num)
                    break;
            }
            return points;
        }

    }

    public static Vector2 Rotate(this Quaternion rotation, Vector2 point)
    {
        float num = rotation.x * 2f;
        float num2 = rotation.y * 2f;
        float num3 = rotation.z * 2f;
        float num4 = rotation.x * num;
        float num5 = rotation.y * num2;
        float num6 = rotation.z * num3;
        float num7 = rotation.x * num2;
        float num12 = rotation.w * num3;
        Vector2 result = default(Vector2);
        result.x = (1f - (num5 + num6)) * point.x + (num7 - num12) * point.y;
        result.y = (num7 + num12) * point.x + (1f - (num4 + num6)) * point.y;
        return result;
    }
}
