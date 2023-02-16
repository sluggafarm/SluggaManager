namespace SluggaManager.Services
{
    public interface ISluggaService
    {
        public string Wallet { get; set; }
        public string Version { get; set; }
        public string GetShards();

        public string GetState(string id);

        public string Sleep(string id);

        public string Feed(string id);

        public string Pet(string id);
    }
}