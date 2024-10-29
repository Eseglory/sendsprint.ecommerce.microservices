using sendsprint.ecommerce.Common.PaymentGateway.Model;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sendsprint.ecommerce.Common.PaymentGateway
{
    public class GatewayManager : IGatewayManager
    {
        private static readonly ConcurrentDictionary<int, ITransactionProvider> TransactionProviders =
            new ConcurrentDictionary<int, ITransactionProvider>();

        class GatewayTypeComparer : IEqualityComparer<ITransactionProvider>
        {
            public bool Equals(ITransactionProvider p1, ITransactionProvider p2)
            {
                if (p1 == null && p2 == null)
                    return true;
                if (p1 == null || p2 == null)
                    return false;
                return p1.GatewayType == p2.GatewayType;
            }

            public int GetHashCode(ITransactionProvider p)
            {
                return p.GatewayType.GetHashCode();
            }
        }

        private static readonly GatewayTypeComparer gatewayTypeComparer = new GatewayTypeComparer();

        public bool AddTransactionProvider(ITransactionProvider gateway)
        {
            //remove then add
            RemoveTransactionProvider(gateway.GatewayId);
            var res = TransactionProviders.TryAdd(gateway.GatewayId, gateway);
            return res;
        }

        public bool RemoveTransactionProvider(int gatewayId)
        {
            return TransactionProviders.TryRemove(gatewayId, out var _);
        }

        public void ClearTransactionProviders()
        {
            TransactionProviders.Clear();
        }

        /// <summary>
        /// Get a gateway for a specific use case 
        /// </summary>
        /// <param name="gatewayUseCase"></param>
        /// <param name="amount"></param>
        /// <param name="exclusions"></param>
        /// <returns></returns>
        private ITransactionProvider GetProvider(GatewayUseCase gatewayUseCase, double amount, IEnumerable<int> exclusions = null)
        {
            var rand = new Random();
            var gw = TransactionProviders
                .Values.OrderBy(p => gatewayUseCase.PriorityField(p) + rand.NextDouble())
                .FirstOrDefault(p =>
                    gatewayUseCase.ClassType().IsInstanceOfType(p)
                    && p.Status == GatewayStatus.Online
                    && (exclusions == null || !exclusions.Contains(p.GatewayId))
                    && (p.MaxAmount == 0 || amount <= p.MaxAmount));

            if (gw == null)
            {
                if (exclusions == null)
                    throw new InvalidOperationException($"No Provider for {nameof(gatewayUseCase)}");

                //retry with no exclusions
                return GetProvider(gatewayUseCase, amount);
            }

            return gw;
        }


        /// <summary>
        /// Get an active IBankTransactionProvider to receive currency payment ordered by priority and rand
        /// </summary>
        /// <param name="amount"></param>
        /// <param name="exclusions"></param>
        /// <returns></returns>
        public IBankTransactionProvider GetCurrencyReceiveProvider(double amount, IEnumerable<int> exclusions = null)
        {
            return GetProvider(GatewayUseCase.CurrencyReceive, amount, exclusions) as IBankTransactionProvider;
        }

        /// <summary>
        /// Get a list of active and distinct GatewayType IBankTransactionProviders to receive currency payment for a specific priority and ordered by priority and rand
        /// </summary>
        /// <param name="amount"></param>
        /// <param name="exclusions"></param>
        /// <returns></returns>
        public List<IBankTransactionProvider> GetCurrencyReceiveProviderList(decimal amount, IEnumerable<int> exclusions = null)
        {
            var rand = new Random();
            var gw = TransactionProviders.Values
                .OrderBy(p => p.CurrencyReceivePriority + rand.NextDouble()).Where(p =>
                p is IBankTransactionProvider
                && p.Status == GatewayStatus.Online
                && (exclusions == null || !exclusions.Contains(p.GatewayId))
                && (p.MaxAmount == 0 || amount <= p.MaxAmount)).Distinct(gatewayTypeComparer).Cast<IBankTransactionProvider>().ToList();
            if (gw == null || gw.Count == 0)
                return null;

            return gw;
        }

        /// <summary>
        /// Get an active IBankTransactionProvider to send currency payment ordered by priority and rand
        /// </summary>
        /// <param name="amount"></param>
        /// <param name="exclusions"></param>
        /// <returns></returns>
        public IBankTransactionProvider GetCurrencySendProvider(double amount, IEnumerable<int> exclusions = null)
        {
            return GetProvider(GatewayUseCase.CurrencySend, amount, exclusions) as IBankTransactionProvider;
        }


        public IEnumerable<ITransactionProvider> GetAllGateways()
        {
            return TransactionProviders.Values;
        }

        /// <summary>
        /// Get all gateways by provider type. Return only active gateways by default.
        /// </summary>
        /// <param name="gatewayType"></param>
        /// <param name="getAll">Set to true to ignore Status and return all</param>
        /// <returns></returns>
        public IEnumerable<ITransactionProvider> GetGatewaysByType(GatewayType gatewayType, bool getAll = false)
        {
            return TransactionProviders.Values.Where(p =>
                p.GatewayType == gatewayType
                && (getAll || p.Status == GatewayStatus.Online || p.Status == GatewayStatus.Seeding));
        }

        /// <summary>
        /// Get a gateway from the cache.
        /// </summary>
        /// <param name="gatewayId"></param>
        /// <returns>An instance of a ITransactionProvider or Null if the gateway does not exist</returns>
        public ITransactionProvider GetGatewayById(int gatewayId)
        {
            return TransactionProviders.TryGetValue(gatewayId, out var gateway) ? gateway : null;
        }
    }
}
