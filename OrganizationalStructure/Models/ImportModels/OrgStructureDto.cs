using System.ComponentModel.DataAnnotations;

namespace OrganizationalStructure.Models.ImportModels;

/// <summary>
/// Data transfer object for import data from file
/// </summary>
public class OrgStructureDto
{
    [Column(1)]
    [Required]
    public string Department { get; set; } = string.Empty;
    [Column(2)]
    [Required]
    public string ParentDepartment { get; set; } = string.Empty;
    [Column(3)]
    [Required]
    public string Position { get; set; } = string.Empty;
    [Column(4)]
    [Required]
    public string User { get; set; } = string.Empty;
}
