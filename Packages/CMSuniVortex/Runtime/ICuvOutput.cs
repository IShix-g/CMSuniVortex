
namespace CMSuniVortex
{
    /// <summary>
    /// Represents an output object used in the CMSuniVortex framework.
    /// </summary>
    public interface ICuvOutput
    {
        void Select(string assetPath);
        void Deselect();
        void Generate(string buildPath, ICuvClient client, string[] listGuids);
        void Release();
    }
}