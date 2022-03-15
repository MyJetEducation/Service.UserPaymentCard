using MyJetWallet.Sdk.Service;
using MyYamlParser;

namespace Service.UserPaymentCard.Settings
{
    public class SettingsModel
    {
        [YamlProperty("UserPaymentCard.SeqServiceUrl")]
        public string SeqServiceUrl { get; set; }

        [YamlProperty("UserPaymentCard.ZipkinUrl")]
        public string ZipkinUrl { get; set; }

        [YamlProperty("UserPaymentCard.ElkLogs")]
        public LogElkSettings ElkLogs { get; set; }

        [YamlProperty("UserPaymentCard.ServerKeyValueServiceUrl")]
        public string ServerKeyValueServiceUrl { get; set; }

        [YamlProperty("UserPaymentCard.ServiceBusReader")]
        public string ServiceBusReader { get; set; }

        [YamlProperty("UserPaymentCard.KeyUserPaymentCard")]
        public string KeyUserPaymentCard { get; set; }
    }
}
