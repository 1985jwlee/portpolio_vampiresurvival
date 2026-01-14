using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

namespace Game.ECS
{
    
    public static class ExtensionFunction
    {
        public static void DisposeCancellationTokenSource(CancellationTokenSource source)
        {
            if (source == null) return;

            if (source.IsCancellationRequested == false)
            {
                source.Cancel();
                source.Dispose();
            }
        }

        public static LinkedListNode<T> CircularNextOrFirst<T>(this LinkedListNode<T> current)
        {
            if (current.Next != null)
            {
                return current.Next;
            }
            return current.List.First;
        }
        
        public static LinkedListNode<T> CircularPreviousOrLast<T>(this LinkedListNode<T> current)
        {
            if (current.Previous != null)
            {
                return current.Previous;
            }
            return current.List.Last;
        }

        public static Vector3 RandomPointInBounds(Bounds bounds)
        {
            return new Vector3(
                UnityEngine.Random.Range(bounds.min.x, bounds.max.x),
                UnityEngine.Random.Range(bounds.min.y, bounds.max.y),
                UnityEngine.Random.Range(bounds.min.z, bounds.max.z)
            );
        }

        public static bool IsSameSide(Vector3 a, Vector3 b, Vector3 p, Vector3 q)
        {
            Vector3 c1 = Vector3.Cross(b - a, p - a);
            Vector3 c2 = Vector3.Cross(b - a, q - a);
            return Vector3.Dot(c1, c2) >= 0f;
        }

        public static bool IsInsideTriangle(Vector3 a, Vector3 b, Vector3 c, Vector3 position)
        {
            return IsSameSide(a, b, c, position) && IsSameSide(b, c, a, position) && IsSameSide(c, a, b, position);
        }

        public static bool TryGetEnumArray<T>(out T[] output) where T : System.Enum
        {
            output = default;
            if (typeof(T).IsEnum == false) return false;

            output = (T[])System.Enum.GetValues(typeof(T));
            return true;
        }

        public static T Random<T>(this IEnumerable<T> enumerable)
        {
            var r = new System.Random();
            var enumerable1 = enumerable as T[] ?? enumerable.ToArray();
            return enumerable1.ElementAt(r.Next(0, enumerable1.Count()));
        }

        public static T Random<T>(this IEnumerable<T> enumerable, System.Func<T, bool> option)
        {
            var r = new System.Random();
            var enumer = enumerable.Where(option.Invoke).ToList();
            if (enumer.Count > 0 == false)
            {
                return default;
            }
            var enumerable1 = enumer as T[] ?? enumer.ToArray();
            var rndValue = r.Next(0, enumerable1.Length);
            return enumerable1.ElementAt(rndValue);
        }

        public static int WeightedRandomIndex(this IEnumerable<float> weights)
        {
            var enumerable = weights as float[] ?? weights.ToArray();
            float random = UnityEngine.Random.value * enumerable.Sum();
            int len = enumerable.Count();
            for (int i = 0; i < len; i++)
            {
                random -= enumerable.ElementAt(i);
                if (random <= 0)
                    return i;
            }
            return len;
        }

        public static int PerTenThousandRandomIndex(this IEnumerable<int> data)
        {
            int random = UnityEngine.Random.Range(1, 10001);
            int len = data.Count();
            for (int i = 0; i < len; i++)
            {
                random -= data.ElementAt(i);
                if (random <= 0)
                    return i;
            }
            return -1;
        }
        
        public static string IntToRoman(int value) => value switch
        {
            1 => "Ⅰ",
            2 => "Ⅱ",
            3 => "Ⅲ",
            4 => "Ⅳ",
            5 => "Ⅴ",
            6 => "Ⅵ",
            7 => "Ⅶ",
            8 => "Ⅷ",
            9 => "Ⅸ",
            10 => "Ⅹ",
            _ => throw new System.NotImplementedException(),
        };

        public static void AddFlag<T>(ref T src, int flag) where T : struct, System.IConvertible
        {
            var castInt = src.ToInt32(null);
            castInt |= flag;
            src = (T)System.Enum.ToObject(typeof(T), castInt);
        }
        
        public static void RemoveFlag<T>(ref T src, int flag) where T : struct, System.IConvertible
        {
            var castInt = src.ToInt32(null);
            castInt &= ~flag;
            src = (T)System.Enum.ToObject(typeof(T), castInt);
        }

        public static void ForceRebuildLayout(this LayoutGroup layoutGroup)
        {
            LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)layoutGroup.transform);
        }

        public static Vector2 RectTransformOffsetCenterPivot(this RectTransform rectTransform)
        {
            return (Vector2.one * 0.5f - rectTransform.pivot) * rectTransform.rect.size;
        }

        public static bool IsWithin<T>(this T value, T minimum, T maximum) where T : System.IComparable<T>
        {
            return value.CompareTo(minimum) >= 0 && value.CompareTo(maximum) <= 0;
        }

        public static Vector2 WorldPostionToCanvasPosition(Camera camera, RectTransform rootRectTr, Vector3 worldPosition)
        {
            var worldToScreenPoint = camera.WorldToScreenPoint(worldPosition);
            RectTransformUtility.ScreenPointToLocalPointInRectangle(rootRectTr, worldToScreenPoint, camera, out var canvasPosition);
            return canvasPosition;
        }

        public static T GetOrAddComponent<T>(this Component component) where T : Component
        {
            return component.TryGetComponent(out T t) ? t : component.gameObject.AddComponent<T>();
        }

        public static bool TryGetOrAddComponentInChild<T>(this Component component, out T t) where T : Component
        {
            var tr = component.transform;
            var childCount = tr.childCount;
            for (int i = 0; i < childCount; ++i)
            {
                if (tr.GetChild(i).TryGetComponent(out t))
                {
                    return true;
                }
            }

            t = null;
            return false;
        }
        
        

        public static Vector3 RandomScreenPositionToWorld(Camera viewCamera, float offset)
        {
            var randomScreenPos = new Vector2(UnityEngine.Random.Range(0f, 1f), UnityEngine.Random.Range(0f, 1f));
            var viewportToWorldPos = viewCamera.ViewportToWorldPoint(randomScreenPos);
            viewportToWorldPos.z = 0f;
            return viewportToWorldPos;
        }

        public static Vector2 RandomCirclePosition(Vector2 center, float radius)
        {
            var range = radius * 0.5f;
            var dist = UnityEngine.Random.value * range;
            var radianAngle = UnityEngine.Random.value * 360f * Mathf.Deg2Rad;
            var cos = Mathf.Cos(radianAngle);
            var sin = Mathf.Sin(radianAngle);
            return center + new Vector2(cos, sin) * dist;
        }

        public static void GetLinearSeriesFunction(in Vector2 point, in float slopeAngle, out float a, out float b)
        {
            a = Mathf.Tan(slopeAngle * Mathf.Deg2Rad);
            b = point.y - a * point.x;
        }

        public static string GetPrefixStringByArcaneDeviceType(ArcaneDeviceType deviceType)
        {
            switch (deviceType)
            {
                case ArcaneDeviceType.ACTIVE:
                    return "Active";
                case ArcaneDeviceType.PASSIVE:
                    return "Passive";
                case ArcaneDeviceType.ADVANCED_ACTIVE:
                    return "AdvActive";
                case ArcaneDeviceType.CHARACTER_OWNED_ACTIVE:
                    return "OwnActive";
            }

            return string.Empty;
        }

        public static bool IsInScreen(this Camera viewCamera, Vector3 position)
        {
            var viewport = viewCamera.WorldToViewportPoint(position);
            return viewport.x.IsWithin(0f, 1f) && viewport.y.IsWithin(0f, 1f);
        }
        
        public static bool RandomViewportEdgeToWorldPos(Camera viewCamera, Vector3 position, float angle, out Vector3 viewportToWorld)
        {
            var characterViewport = viewCamera.WorldToViewportPoint(position);
            characterViewport.z = 0f;
            var quadrant = angle / 90;
            GetLinearSeriesFunction(characterViewport, angle, out var lean, out var b);
            viewportToWorld = Vector3.zero;
            switch ((int)quadrant)
            {
                case 0:
                    {
                        //X = 1, Y < 1, lean + b = y < 1
                        if (lean + b < 1f)
                        {
                            //y = lean * 1 + b
                            //(float)(1 - b) / lean = x;
                            viewportToWorld = viewCamera.ViewportToWorldPoint(new Vector3(1f, lean + b));
                            return true;
                        }
                        //Y = 1, X < 1,   x +  = (1 - b) / lean
                        if ((1 - b) / lean < 1f)
                        {
                            viewportToWorld = viewCamera.ViewportToWorldPoint(new Vector3((1 - b) / lean, 1f));
                            return true;
                        }
                    }
                    break;
                case 1:
                    {
                        //X = 0, Y < 1, b = y < 1
                        if (b < 1f)
                        {
                            viewportToWorld = viewCamera.ViewportToWorldPoint(new Vector3(0f, b));
                            return true;
                        }
                        //Y = 1, X > 0,  x = (1 - b) / lean > 0
                        if ((1 - b) / lean > 0f)
                        {
                            viewportToWorld = viewCamera.ViewportToWorldPoint(new Vector3((1 - b) / lean, 1f));
                            return true;
                        }
                    }
                    break;
                case 2:
                    {
                        //X = 0, Y > 0, 0 < b  b = y > 0
                        if (b > 0f)
                        {
                            viewportToWorld = viewCamera.ViewportToWorldPoint(new Vector3(0f, b));
                            return true;
                        }
                        //Y = 0, X > 0, x = b / lean < 0
                        if (b / lean < 0f)
                        {
                            viewportToWorld = viewCamera.ViewportToWorldPoint(new Vector3(-1 * b / lean, 0));
                            return true;
                        }
                    }
                    break;
                case 3:
                    {
                        //X = 1, Y > 0, y = lean + b > 0
                        if (lean + b > 0f)
                        {
                            viewportToWorld = viewCamera.ViewportToWorldPoint(new Vector3(1f, lean + b));
                            return true;
                        }
                        // Y = 0, X < 1, b / lean = x > -1
                        if (b / lean > -1f)
                        {
                            viewportToWorld = viewCamera.ViewportToWorldPoint(new Vector3(-1 * b / lean, 0f));
                            return true;
                        }
                    }
                    break;
            }
            
            return false;
        }
    }
}
