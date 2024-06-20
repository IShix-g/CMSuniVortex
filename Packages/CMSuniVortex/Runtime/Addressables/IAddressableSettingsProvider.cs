#if ENABLE_ADDRESSABLES
namespace CMSuniVortex.Addressable
{
    public interface IAddressableSettingsProvider
    {
        AddressableCuvSettings GetSetting();
        void SetSetting(AddressableCuvSettings settings);
    }
}
#endif