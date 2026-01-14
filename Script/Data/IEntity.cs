using System.Collections.Generic;

namespace Client
{
    public interface IEntity
    {
        string MainKey { get; }
    }

    public interface IEntityMainKey : IEntity
    {
    }

    public interface IEntitySubKey : IEntity
    {
        string SubKey { get; }
    }

    public interface IEntityDualKey : IEntitySubKey
    {
        (string, string) DualKey { get; }
    }

    public interface IGroupEntity<T> : IEntity where T:IEntity
    {
        IDictionary<string, T> GroupEntity { get; }
    }

    public interface IDualGroupEntity<T> : IEntitySubKey where T : IEntityDualKey
    {
        IDictionary<string, T> GroupByMainKey { get; }
        IDictionary<string, T> GroupBySubKey { get; }
    }

    public class MainKeyEntityImpl : IEntityMainKey
    {
        public virtual string MainKey { get; }

        private MainKeyEntityImpl(string _key)
        {
            MainKey = _key;
        }
        public MainKeyEntityImpl(IMainKeyData src) : this(src.KeyField){}
    }

    [System.Serializable]
    public class SerializableMainKeyEntityImpl : IEntityMainKey
    {
        public virtual string MainKey { get; }

        private SerializableMainKeyEntityImpl(string _key)
        {
            MainKey = _key;
        }
        public SerializableMainKeyEntityImpl(IMainKeyData src) : this(src.KeyField) { }
    }

    public class SubKeyEntityImpl : IEntitySubKey
    {
        public string MainKey { get; }
        public string SubKey { get; }
        public SubKeyEntityImpl(ISubKeyData src)
        {
            MainKey = src.KeyField;
            SubKey = src.SubKeyField;
        }
    }

    public class DualKeyEntityImpl : IEntityDualKey
    {
        public string MainKey { get; }
        public string SubKey { get; }

        public (string, string) DualKey => (MainKey, SubKey);

        public DualKeyEntityImpl(IDualKeyData src)
        {
            MainKey = src.KeyFields.Item1;
            SubKey = src.KeyFields.Item2;
        }
    }

    public class GroupEntityImpl<T> : IGroupEntity<T> where T : IEntity
    {
        public string MainKey { get; }
        public IDictionary<string, T> GroupEntity { get; }

        protected GroupEntityImpl(string _key)
        {
            MainKey = _key;
            GroupEntity = new Dictionary<string, T>();
        }
    }

    public class DualGroupEntityImpl<T> : IDualGroupEntity<T> where T : IEntityDualKey
    {
        public string MainKey { get; }
        public string SubKey { get; }
        public IDictionary<string, T> GroupByMainKey { get; }
        public IDictionary<string, T> GroupBySubKey { get; }

        protected DualGroupEntityImpl(string _key, string _subKey)
        {
            MainKey = _key;
            SubKey = _subKey;
            GroupByMainKey = new Dictionary<string, T>();
            GroupBySubKey = new Dictionary<string, T>();
        }
    }
}
