
namespace CMSuniVortex.Excel
{
    public abstract class ExcelCuvModelList<T> : CuvModelList<T> where T : ExcelModel
    {
        public string ModifiedDate;
    }
}