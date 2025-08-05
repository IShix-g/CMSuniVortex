
namespace CMSuniVortex.Excel
{
    public abstract class ExcelCuvReference<T, TS>
        : CuvReference<T, TS>
        where T : ExcelModel
        where TS : ExcelCuvModelList<T> {}
}