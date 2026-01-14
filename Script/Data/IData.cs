namespace Client
{
    public interface IKeyData
    {
        void OnReadData(CSVReader reader);
    }
    
    public interface IMainKeyData : IKeyData
    {
        string KeyField { get; }
    }

    public interface ISubKeyData : IMainKeyData
    {
        string SubKeyField { get; }
    }

    public interface IDualKeyData : IKeyData
    {
        (string, string) KeyFields { get; }
    }
}