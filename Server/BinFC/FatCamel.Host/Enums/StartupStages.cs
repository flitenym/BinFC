namespace FatCamel.Host.Enums
{
    public enum StartupStages
    {
        PreConfigureServices = 0,
        ConfigureServices = 1,
        Configure = 2,
        ApplicationStart = 3
    }
}