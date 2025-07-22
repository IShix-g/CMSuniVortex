
namespace CMSuniVortex
{
    public interface ICuvImporterStatus
    {
        string GetName();
        string GetClientName();
        string GetClintClassName();
        string GetOutputClassName();
        string GetBuildPath();
        bool IsLocalization();
    }
}