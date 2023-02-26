using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SluggaManager.Services
{

    public sealed class SluggaService : APIServiceProxy, ISluggaService
    {
        // hard-coded.... for the now.
        public string Wallet { get; set; } = "[Your Wallet Here]";
        public string Version { get; set; } = "v1";

        public string GetShards()
        {
            return ServiceCall(SluggaServiceEndpoints.V1.ParseUrl(SluggaServiceEndpoints.V1.GetShardAmount, Wallet));
        }

        public string GetState(string id)
        {
            return ServiceCall(SluggaServiceEndpoints.V1.ParseUrl(SluggaServiceEndpoints.V1.GetSluggaState, Wallet, id));
        }

        public string Sleep(string id)
        {
            return ServiceCall(SluggaServiceEndpoints.V1.ParseUrl(SluggaServiceEndpoints.V1.SluggaSleepAction, Wallet, id));
        }

        public string Feed(string id)
        {
            return ServiceCall(SluggaServiceEndpoints.V1.ParseUrl(SluggaServiceEndpoints.V1.SluggaFeedAction, Wallet, id));
        }

        public string Pet(string id)
        {
            return ServiceCall(SluggaServiceEndpoints.V1.ParseUrl(SluggaServiceEndpoints.V1.SluggaPetAction, Wallet, id));
        }
    }
}
