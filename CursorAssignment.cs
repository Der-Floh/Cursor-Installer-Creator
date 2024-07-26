using CsvHelper;
using CsvHelper.Configuration;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace Cursor_Installer_Creator;

public enum CursorAssignmentType
{
    ID,
    Name,
    DisplayName,
    Windows,
    WindowsReg,
    WindowsInstall,
    Avalonia
}

public sealed class CursorAssignment
{
    public static Dictionary<int, CursorAssignment> CursorAssignments { get; } = ReadCsvString(CursorAssignmentMap.AssignmentCSVMap);

    public int ID { get; set; }
    public string Name { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public string Windows { get; set; } = string.Empty;
    public string WindowsReg { get; set; } = string.Empty;
    public string WindowsInstall { get; set; } = string.Empty;
    public string Avalonia { get; set; } = string.Empty;

    public static Dictionary<int, CursorAssignment> ReadCsvString(string fileContent)
    {
        using var reader = new StringReader(fileContent);
        using var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture));
        csv.Context.RegisterClassMap<CursorAssignmentMap>();
        var records = csv.GetRecords<CursorAssignment>();
        var dictionary = new Dictionary<int, CursorAssignment>();
        foreach (var record in records)
        {
            if (string.IsNullOrEmpty(record.Name))
                continue;
            dictionary[record.ID] = record;
        }
        return dictionary;
    }

    public static CursorAssignment? FromName(string name, CursorAssignmentType type)
    {
        return type switch
        {
            CursorAssignmentType.Name => CursorAssignments.Values.FirstOrDefault(x => x.Name == name),
            CursorAssignmentType.DisplayName => CursorAssignments.Values.FirstOrDefault(x => x.DisplayName == name),
            CursorAssignmentType.Windows => CursorAssignments.Values.FirstOrDefault(x => x.Windows == name),
            CursorAssignmentType.WindowsReg => CursorAssignments.Values.FirstOrDefault(x => x.WindowsReg == name),
            CursorAssignmentType.WindowsInstall => CursorAssignments.Values.FirstOrDefault(x => x.WindowsInstall == name),
            CursorAssignmentType.Avalonia => CursorAssignments.Values.FirstOrDefault(x => x.Avalonia == name),
            _ => null,
        };
    }

    public static CursorAssignment? FromName(string name)
    {
        var assignment = FromName(name, CursorAssignmentType.Name);
        assignment ??= FromName(name, CursorAssignmentType.DisplayName);
        assignment ??= FromName(name, CursorAssignmentType.Windows);
        assignment ??= FromName(name, CursorAssignmentType.WindowsReg);
        assignment ??= FromName(name, CursorAssignmentType.WindowsInstall);
        assignment ??= FromName(name, CursorAssignmentType.Avalonia);
        return assignment;
    }
}

public sealed class CursorAssignmentMap : ClassMap<CursorAssignment>
{
    public CursorAssignmentMap()
    {
        Map(m => m.ID).Name("ID");
        Map(m => m.Name).Name("Name");
        Map(m => m.DisplayName).Name("DisplayName");
        Map(m => m.Windows).Name("Windows");
        Map(m => m.WindowsReg).Name("WindowsReg");
        Map(m => m.WindowsInstall).Name("WindowsInstall");
        Map(m => m.Avalonia).Name("Avalonia");
    }

    public static string AssignmentCSVMap { get; } = @"ID,Name,DisplayName,Windows,WindowsReg,WindowsInstall,Avalonia
32512,IDC_ARROW,Normal Select,arrow_m,Arrow,pointer,Arrow
32513,IDC_IBEAM,Text Select,beam_im,IBeam,text,Ibeam
32514,IDC_WAIT,Busy,busy_m,Wait,busy,Wait
32515,IDC_CROSS,Precision Select,cross_im,Crosshair,cross,Cross
32516,IDC_UPARROW,Alternate Select,up_m,UpArrow,alternate,UpArrow
32642,IDC_SIZENWSE,Diagonal Resize 1,lnwse,SizeNWSE,dgn1,TopLeftCorner
32643,IDC_SIZENESW,Diagonal Resize 2,lnesw,SizeNESW,dgn2,TopRightCorner
32644,IDC_SIZEWE,Horizontal Resize,lwe,SizeWE,horz,SizeWestEast
32645,IDC_SIZENS,Vertical Resize,lns,SizeNS,vert,SizeNorthSouth
32646,IDC_SIZEALL,Move,move_m,SizeAll,move,SizeAll
32648,IDC_NO,Unavailable,no_m,No,unavailiable,No
32649,IDC_HAND,Link Select,aero_link_im,Hand,link,Hand
32650,IDC_APPSTARTING,Working in Background,wait_m,AppStarting,work,AppStarting
32651,IDC_HELP,Help Select,help_m,Help,help,Help
32671,IDC_PIN,Location Select,pin_m,Pin,pin,
32672,IDC_PERSON,Person Select,person_m,Person,person,
32631,OCR_NWPEN,Handwriting,pen_m,NWPen,handwrt,
";
}
