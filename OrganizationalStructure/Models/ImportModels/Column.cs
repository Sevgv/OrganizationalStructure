namespace OrganizationalStructure.Models.ImportModels;

[AttributeUsage(AttributeTargets.All)]
public class Column : Attribute
{
    public int ColumnIndex { get; set; }

    public Column(int column)
    {
        ColumnIndex = column;
    }
}
