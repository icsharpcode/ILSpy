using System.Threading.Tasks;

public interface IDapperAsyncImplementor
{
	Task<T> GetAsync<T>(IDbConnection connection, object id, IDbTransaction transaction, int? commandTimeout) where T : class;
}
public interface IDbConnection
{
}
public interface IDbTransaction
{
}
public static class Issue3320
{
	public static IDapperAsyncImplementor Instance => null;

	public static async Task<T> GetAsync<T>(this IDbConnection connection, dynamic id, IDbTransaction transaction = null, int? commandTimeout = null) where T : class
	{
		dynamic async = Instance.GetAsync<T>(connection, id, transaction, commandTimeout);
		return await async;
	}
}
