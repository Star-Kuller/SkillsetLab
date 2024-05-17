using System.Transactions;

namespace Application.Infrastructure
{
    public static class Transaction
    {
        /// <summary>
        /// Do action in transacion async flow scope
        /// </summary>
        /// <param name="action"></param>
        public static async Task Do(Func<Task> action)
        {
            using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    await action();
                    scope.Complete();
                }
                catch (Exception ex)
                {
                    scope.Dispose();
                    throw ex;
                }
            }
        }
    }
}
