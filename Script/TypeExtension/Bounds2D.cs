using UnityEngine;

namespace Game.ECS
{
    [System.Serializable]
    public struct Bounds2D : System.IEquatable<Bounds2D>
    {
        public Vector2 center
        {
            get => _center;
            set => _center = value;
        }

        public Vector2 size
        {
            get => _size;
            set => _size = value;
        }

        public Vector2 extents
        {
            get => _extents;
            set => _extents = value;
        }

        public float rightExtents => center.x + extents.x;
        public float leftExtents => center.x - extents.x;
        public float upExtents => center.y + extents.y;
        public float downExtents => center.y - extents.y;


        [SerializeField] private Vector2 _center;
        [SerializeField] private Vector2 _size;
        [SerializeField] private Vector2 _extents;
        
        public Bounds2D(Vector2 center, Vector2 size) {
            if (size.x < 0) size.x = -size.x;
            if (size.y < 0) size.y = -size.y;
            _center = center;
            _size = size;
            _extents = size * 0.5f;
        }
        
        public Bounds2D (Vector2[] verts) {
            Vector2 max = verts[0];
            Vector2 min = verts[0];
            for (int i = 0; i < verts.Length; i++) {
                if (verts[i].x > max.x) max.x = verts[i].x;
                if (verts[i].y > max.y) max.y = verts[i].y;
                if (verts[i].x < min.x) min.x = verts[i].x;
                if (verts[i].y < min.y) min.x = verts[i].y;
            }
            _center = (min + max) * 0.5f;
            _size = new Vector2(max.x - min.x, max.y - min.y);
            _extents = _size * 0.5f;
        }
        
        public Bounds2D(BoundsInt boundsInt) : this(new Vector2(boundsInt.center.x, boundsInt.center.y), new Vector2(boundsInt.size.x, boundsInt.size.y)) {}
        
        public bool Intersects(Bounds2D bounds) {
            Vector2 maxDelta = extents + bounds.extents;
            Vector2 delta = center - bounds.center;
            return (Mathf.Abs(delta.x) <= Mathf.Abs(maxDelta.x) && Mathf.Abs(delta.y) <= Mathf.Abs(maxDelta.y));
        }
        
        public bool Contains(Vector2 point) {
            point -= center;
            return Mathf.Abs(point.x) <= extents.x && Mathf.Abs(point.y) <= extents.y;
        }
        
        public void Expand(Vector2 amount) => _extents += amount * 0.5f;
        
        public override string ToString() {
            return "Bounds2D[" + center +", " + size + "]";
        }

        public bool Equals(Bounds2D other) => center.Equals(other.center) && extents.Equals(other.extents);

        public override bool Equals(System.Object obj) {
            return obj is Bounds2D && this == (Bounds2D)obj;
        }
        
        public Vector2 GetRandomPoint()
        {
            return new Vector2(Random.Range(center.x - extents.x, center.x + extents.x), Random.Range(center.y - extents.y, center.y + extents.y));
        }
        
        public override int GetHashCode() {
            return center.GetHashCode() ^ size.GetHashCode();
        }

        public static bool operator ==(Bounds2D a, Bounds2D b) {
            return (a.center == b.center) && (a.extents == b.extents);
        }

        public static bool operator !=(Bounds2D a, Bounds2D b) {
            return !(a == b);
        }
    }
}
