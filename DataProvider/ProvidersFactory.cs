using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using DataProvider.Amex;
using DataProvider.Cal;
using DataProvider.Hapoalim;
using DataProvider.Interfaces;
using log4net;

namespace DataProvider
{
    public static class ProvidersFactory
    {
        private static readonly ILog _log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public static IObservable<IDataProvider> CreateProvider(IProviderDescriptor descriptor)
        {
            IObservable<IDataProvider> result = Observable.Create<IDataProvider>((observer) =>
            {
                var task = new Task(() =>
                {
                    IDataProvider provider = null;
                    try
                    {
                        provider = CreateVendorProvider(descriptor);
                    }
                    catch (Exception exp)
                    {
                        observer.OnError(new DataProviderException(descriptor, exp.Message));
                    }
                      
                    if (provider == null || !provider.IsReady)
                    {
                        var error = new DataProviderException(descriptor, "Cannot access");
                        observer.OnError(error);
                    }

                    observer.OnNext(provider);
                });
                task.Start();

                return () => { };
            });

            return result;
        }

        public static IList<IObservable<IDataProvider>> CreateProviders(IEnumerable<IProviderDescriptor> descriptors)
        {
            var providers = new List<IObservable<IDataProvider>>();

            foreach (var descriptor in descriptors)
            {
                providers.Add(CreateProvider(descriptor));
            }

            _log.InfoFormat("{0} providers were initialized", providers.Count);

            return providers;
        }

        private static IDataProvider CreateVendorProvider(IProviderDescriptor accountDescriptor)
        {
            IDataProvider result = null;
            var vendorId = GetVendorIdByName(accountDescriptor.Name);

            if (vendorId != VendorId.None)
            {
                switch (vendorId)
                {
                    case VendorId.Hapoalim:
                        //var hapoalimApi = new HapoalimApi(accountDescriptor);
                        var hapoalimApi = new MockHapoalimApi(accountDescriptor);
                        result = new HapoalimDataProvider(hapoalimApi);
                        break;
                    case VendorId.Amex:
                        //var amexApi = new AmexApi(accountDescriptor);
                        var amexApi = new MockAmexApi(accountDescriptor);
                        result = new AmexDataProvider(amexApi);
                        break;
                    case VendorId.Cal:
                        //var calApi = new CalApi(accountDescriptor);
                        var calApi = new MockCalApi();
                        result = new CalDataProvider(calApi);
                        break;
                    default: break;
                }
            }

            return result;
        }

        public static VendorId GetVendorIdByName(string vendorName)
        {
            if (string.IsNullOrEmpty(vendorName))
            {
                _log.ErrorFormat("Vendor doesn't exist - {0}", vendorName);
                return VendorId.None;
            }

            switch (vendorName)
            {
                case "Bank Hapoalim": return VendorId.Hapoalim;
                case "American Express": return VendorId.Amex;
                case "Visa-Cal": return VendorId.Cal;
                case "Umtb": return VendorId.Umtb;
                default: return VendorId.None;
            }
        }
    }
}
