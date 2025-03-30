namespace HRProContracts.StoragesContracts
{
    public interface IBackUpInfo
    {
        List<T>? GetList<T>() where T : class, new();
        Type? GetTypeByModelInterface(string modelInterfaceName);
    }
}
