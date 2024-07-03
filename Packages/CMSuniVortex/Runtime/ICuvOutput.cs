
namespace CMSuniVortex
{
    /// <summary>
    /// Represents an output object used in the CMSuniVortex framework.
    /// </summary>
    public interface ICuvOutput
    {
        bool IsCompleted();
        void Select(string buildPath);
        void Deselect();
        void Generate(string buildPath, ICuvClient client, string[] listGuids);
        void Release();
        void ReloadReference(string buildPath);
    }
}