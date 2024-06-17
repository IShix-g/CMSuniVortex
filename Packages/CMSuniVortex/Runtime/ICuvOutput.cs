
namespace CMSuniVortex
{
    public interface ICuvOutput
    {
        void Select(string assetPath);
        void Deselect();
        void Generate(string buildPath, ICuvClient client, string[] listGuids);
        void Release();
    }
}