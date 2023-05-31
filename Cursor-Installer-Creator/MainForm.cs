using System.Diagnostics;
using System.Security.Principal;
using System.Text.Json;

namespace Cursor_Installer_Creator;

public sealed partial class MainForm : Form
{
    private Dictionary<string, string> BoxCursorAssignment { get; set; } = new Dictionary<string, string>();
    private List<CCursor>? CCursors { get; set; }

    public MainForm(List<CCursor>? cCursors = null)
    {
        InitializeComponent();
        AdminLabel.Visible = !IsRunningWithAdminPrivileges();
        SetUpBoxCursorAssignment();
        FillCursors(cCursors);
    }

    private void SetUpBoxCursorAssignment()
    {
        BoxCursorAssignment.Add(CursorPictureBox1.Name, "Arrow");
        BoxCursorAssignment.Add(CursorPictureBox2.Name, "Help");
        BoxCursorAssignment.Add(CursorPictureBox3.Name, "AppStarting");
        BoxCursorAssignment.Add(CursorPictureBox4.Name, "Wait");
        BoxCursorAssignment.Add(CursorPictureBox5.Name, "Crosshair");
        BoxCursorAssignment.Add(CursorPictureBox6.Name, "IBeam");
        BoxCursorAssignment.Add(CursorPictureBox7.Name, "NWPen");
        BoxCursorAssignment.Add(CursorPictureBox8.Name, "No");
        BoxCursorAssignment.Add(CursorPictureBox9.Name, "SizeNS");
        BoxCursorAssignment.Add(CursorPictureBox10.Name, "SizeWE");
        BoxCursorAssignment.Add(CursorPictureBox11.Name, "SizeNWSE");
        BoxCursorAssignment.Add(CursorPictureBox12.Name, "SizeNESW");
        BoxCursorAssignment.Add(CursorPictureBox13.Name, "SizeAll");
        BoxCursorAssignment.Add(CursorPictureBox14.Name, "UpArrow");
        BoxCursorAssignment.Add(CursorPictureBox15.Name, "Hand");
        BoxCursorAssignment.Add(CursorPictureBox16.Name, "Pin");
        BoxCursorAssignment.Add(CursorPictureBox17.Name, "Person");
    }

    private void FillCursors(List<CCursor>? cCursors = null)
    {
        if (cCursors is null)
            CCursors = CursorHelper.GetSelectedCursors();
        else
            CCursors = cCursors;


        foreach (KeyValuePair<string, string> kvp in BoxCursorAssignment)
        {
            string pictureBoxName = kvp.Key;
            string cursorName = kvp.Value;

            PictureBox? pictureBox = CursorsTableLayoutPanel.Controls.Find(pictureBoxName, true).FirstOrDefault() as PictureBox;
            Label? label = CursorsTableLayoutPanel.Controls.Find(pictureBoxName.Replace("CursorPictureBox", "CursorNameLabel"), true).FirstOrDefault() as Label;

            if (pictureBox is null || label is null)
                continue;

            label.Text = cursorName;

            CCursor? cursor = CCursors.FirstOrDefault(c => c.Name?.ToLower() == cursorName.ToLower());
            if (cursor is null || !File.Exists(cursor.ImagePath))
                continue;

            pictureBox.Image = Image.FromFile(cursor.ImagePath);
            label.Text = cursor.Name;
        }
    }

    private void FillCursor(string boxname)
    {
        if (CCursors is null || CCursors.Count == 0)
            return;

        string pictureBoxName = boxname;
        string cursorName = BoxCursorAssignment[boxname];

        PictureBox? pictureBox = CursorsTableLayoutPanel.Controls.Find(pictureBoxName, true).FirstOrDefault() as PictureBox;
        Label? label = CursorsTableLayoutPanel.Controls.Find(pictureBoxName.Replace("CursorPictureBox", "CursorNameLabel"), true).FirstOrDefault() as Label;

        if (pictureBox is null || label is null)
            return;

        CCursor? cursor = CCursors.FirstOrDefault(c => c.Name?.ToLower() == cursorName.ToLower());
        if (cursor is null || !File.Exists(cursor.ImagePath))
            return;

        pictureBox.Image = Image.FromFile(cursor.ImagePath);
        label.Text = cursor.Name;
    }

    private void UpdateCCursor(string filepath, string boxname)
    {
        (CursorsTableLayoutPanel.Controls.Find(boxname, true).FirstOrDefault() as PictureBox)?.Image.Dispose();
        if (!CursorHelper.ConvertCursorFile(filepath, BoxCursorAssignment[boxname], true))
            MessageBox.Show("An error occured while importing", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

        if (CCursors is null)
            CCursors = new List<CCursor>();

        CCursor? cursor = CCursors.FirstOrDefault(c => c.Name?.ToLower() == BoxCursorAssignment[boxname].ToLower());
        if (cursor is null)
        {
            cursor = new CCursor()
            {
                Name = BoxCursorAssignment[boxname],
                CursorName = Path.GetFileName(filepath),
                ImagePath = Path.Combine(Program.TempPath, $"{BoxCursorAssignment[boxname]}.png"),
                CursorPath = filepath,
            };
            CCursors.Add(cursor);
        }
        else
        {
            cursor.CursorName = Path.GetFileName(filepath);
            cursor.ImagePath = Path.Combine(Program.TempPath, $"{BoxCursorAssignment[boxname]}.png");
            cursor.CursorPath = filepath;
        }

    }

    private string? GetCursorFile()
    {
        using OpenFileDialog openFileDialog = new OpenFileDialog();
        openFileDialog.Filter = "Cursor Files|*.cur;*.ani";
        openFileDialog.FilterIndex = 0;
        openFileDialog.RestoreDirectory = true;
        openFileDialog.AddToRecent = true;
        openFileDialog.AutoUpgradeEnabled = true;

        if (openFileDialog.ShowDialog() == DialogResult.OK)
        {
            string selectedFilePath = openFileDialog.FileName;
            return selectedFilePath;
        }

        return null;
    }

    private bool IsCursorFile(string filepath)
    {
        string? extension = Path.GetExtension(filepath)?.ToLower();
        return extension == ".cur" || extension == ".ani";
    }

    private void HandleDragEffects(DragEventArgs e)
    {
        if (e.Data.GetDataPresent(DataFormats.FileDrop))
        {
            string[] filepaths = (string[])e.Data.GetData(DataFormats.FileDrop);

            if (filepaths.Any(IsCursorFile))
                e.Effect = DragDropEffects.Copy;
            else
                e.Effect = DragDropEffects.None;
        }
        else
        {
            e.Effect = DragDropEffects.None;
        }
    }
    private void HandleDragDrop(DragEventArgs e, string boxname)
    {
        string[]? filepaths = (string[]?)e.Data.GetData(DataFormats.FileDrop);

        if (filepaths is null || filepaths.Length == 0)
            return;

        UpdateCCursor(filepaths[0], boxname);
        FillCursor(boxname);
    }
    private void HandleFilePick(string boxname)
    {
        string? file = GetCursorFile();
        if (string.IsNullOrEmpty(file))
            return;

        UpdateCCursor(file, boxname);
        FillCursor(boxname);
    }

    private bool IsRunningWithAdminPrivileges()
    {
        WindowsIdentity identity = WindowsIdentity.GetCurrent();
        WindowsPrincipal principal = new WindowsPrincipal(identity);
        return principal.IsInRole(WindowsBuiltInRole.Administrator);
    }

    private void RestartWithAdminPrivileges()
    {
        if (IsRunningWithAdminPrivileges())
            return;
        else if (MessageBox.Show("Installing a cursor requires Admin privileges.\n\nWhen running as Admin Drag & Drop only accepts files from other programs that run as Admin.\n\nRestart program as Admin?", "Missing privileges", MessageBoxButtons.YesNo, MessageBoxIcon.Error) == DialogResult.No)
            return;

        string json = JsonSerializer.Serialize(CCursors);
        json = json.Replace("\"", "\\\"");
        ProcessStartInfo startInfo = new ProcessStartInfo()
        {
            FileName = Application.ExecutablePath,
            UseShellExecute = true,
            Verb = "runas",
            Arguments = $"\"{json}\"",
        };

        Process.Start(startInfo);
        Close();
    }

    #region EventListeners

    private void PackageNameTextBox_TextChanged(object sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(PackageNameTextBox.Text))
            PackageNameEmptyLabel.Visible = true;
        else
            PackageNameEmptyLabel.Visible = false;
    }

    private void CancelButton_Click(object sender, EventArgs e)
    {
        Close();
    }

    private void OkButton_Click(object sender, EventArgs e)
    {
        if (PackageNameEmptyLabel.Visible)
        {
            MessageBox.Show("Package Name must be filled!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            PackageNameTextBox.Text = "Cursor-Installer";
            return;
        }
        string folderPath = "";
        using (FolderBrowserDialog dialog = new FolderBrowserDialog() { AddToRecent = true, AutoUpgradeEnabled = true })
        {
            dialog.Description = "Select a folder to save the files";

            if (dialog.ShowDialog() == DialogResult.OK)
                folderPath = dialog.SelectedPath;
        }
        if (string.IsNullOrEmpty(folderPath))
            return;
        CursorHelper.CreateInstaller(PackageNameTextBox.Text, folderPath, CCursors, ZipCheckBox.Checked);
    }

    private void InstallButton_Click(object sender, EventArgs e)
    {
        CursorHelper.CreateInstaller(PackageNameTextBox.Text, Program.TempPath, CCursors, false);
        string installerPath = Path.Combine(Program.TempPath, PackageNameTextBox.Text, "installer.inf");
        CursorHelper.InstallCursor(installerPath);
    }

    private void CursorPanel1_DragEnter(object sender, DragEventArgs e)
    {
        HandleDragEffects(e);
    }
    private void CursorPanel1_DragOver(object sender, DragEventArgs e)
    {
        HandleDragEffects(e);
    }
    private void CursorPanel1_DragDrop(object sender, DragEventArgs e)
    {
        HandleDragDrop(e, CursorPictureBox1.Name);
    }
    private void CursorSelectButton1_Click(object sender, EventArgs e)
    {
        HandleFilePick(CursorPictureBox1.Name);
    }

    private void CursorPanel2_DragEnter(object sender, DragEventArgs e)
    {
        HandleDragEffects(e);
    }
    private void CursorPanel2_DragOver(object sender, DragEventArgs e)
    {
        HandleDragEffects(e);
    }
    private void CursorPanel2_DragDrop(object sender, DragEventArgs e)
    {
        HandleDragDrop(e, CursorPictureBox2.Name);
    }
    private void CursorSelectButton2_Click(object sender, EventArgs e)
    {
        HandleFilePick(CursorPictureBox2.Name);
    }

    private void CursorPanel3_DragEnter(object sender, DragEventArgs e)
    {
        HandleDragEffects(e);
    }
    private void CursorPanel3_DragOver(object sender, DragEventArgs e)
    {
        HandleDragEffects(e);
    }
    private void CursorPanel3_DragDrop(object sender, DragEventArgs e)
    {
        HandleDragDrop(e, CursorPictureBox3.Name);
    }
    private void CursorSelectButton3_Click(object sender, EventArgs e)
    {
        HandleFilePick(CursorPictureBox3.Name);
    }

    private void CursorPanel4_DragEnter(object sender, DragEventArgs e)
    {
        HandleDragEffects(e);
    }
    private void CursorPanel4_DragOver(object sender, DragEventArgs e)
    {
        HandleDragEffects(e);
    }
    private void CursorPanel4_DragDrop(object sender, DragEventArgs e)
    {
        HandleDragDrop(e, CursorPictureBox4.Name);
    }
    private void CursorSelectButton4_Click(object sender, EventArgs e)
    {
        HandleFilePick(CursorPictureBox4.Name);
    }

    private void CursorPanel5_DragEnter(object sender, DragEventArgs e)
    {
        HandleDragEffects(e);
    }
    private void CursorPanel5_DragOver(object sender, DragEventArgs e)
    {
        HandleDragEffects(e);
    }
    private void CursorPanel5_DragDrop(object sender, DragEventArgs e)
    {
        HandleDragDrop(e, CursorPictureBox5.Name);
    }
    private void CursorSelectButton5_Click(object sender, EventArgs e)
    {
        HandleFilePick(CursorPictureBox5.Name);
    }

    private void CursorPanel6_DragEnter(object sender, DragEventArgs e)
    {
        HandleDragEffects(e);
    }
    private void CursorPanel6_DragOver(object sender, DragEventArgs e)
    {
        HandleDragEffects(e);
    }
    private void CursorPanel6_DragDrop(object sender, DragEventArgs e)
    {
        HandleDragDrop(e, CursorPictureBox6.Name);
    }
    private void CursorSelectButton6_Click(object sender, EventArgs e)
    {
        HandleFilePick(CursorPictureBox6.Name);
    }

    private void CursorPanel7_DragEnter(object sender, DragEventArgs e)
    {
        HandleDragEffects(e);
    }
    private void CursorPanel7_DragOver(object sender, DragEventArgs e)
    {
        HandleDragEffects(e);
    }
    private void CursorPanel7_DragDrop(object sender, DragEventArgs e)
    {
        HandleDragDrop(e, CursorPictureBox7.Name);
    }
    private void CursorSelectButton7_Click(object sender, EventArgs e)
    {
        HandleFilePick(CursorPictureBox7.Name);
    }

    private void CursorPanel8_DragEnter(object sender, DragEventArgs e)
    {
        HandleDragEffects(e);
    }
    private void CursorPanel8_DragOver(object sender, DragEventArgs e)
    {
        HandleDragEffects(e);
    }
    private void CursorPanel8_DragDrop(object sender, DragEventArgs e)
    {
        HandleDragDrop(e, CursorPictureBox8.Name);
    }
    private void CursorSelectButton8_Click(object sender, EventArgs e)
    {
        HandleFilePick(CursorPictureBox8.Name);
    }

    private void CursorPanel9_DragEnter(object sender, DragEventArgs e)
    {
        HandleDragEffects(e);
    }
    private void CursorPanel9_DragOver(object sender, DragEventArgs e)
    {
        HandleDragEffects(e);
    }
    private void CursorPanel9_DragDrop(object sender, DragEventArgs e)
    {
        HandleDragDrop(e, CursorPictureBox9.Name);
    }
    private void CursorSelectButton9_Click(object sender, EventArgs e)
    {
        HandleFilePick(CursorPictureBox9.Name);
    }

    private void CursorPanel10_DragEnter(object sender, DragEventArgs e)
    {
        HandleDragEffects(e);
    }
    private void CursorPanel10_DragOver(object sender, DragEventArgs e)
    {
        HandleDragEffects(e);
    }
    private void CursorPanel10_DragDrop(object sender, DragEventArgs e)
    {
        HandleDragDrop(e, CursorPictureBox10.Name);
    }
    private void CursorSelectButton10_Click(object sender, EventArgs e)
    {
        HandleFilePick(CursorPictureBox10.Name);
    }

    private void CursorPanel11_DragEnter(object sender, DragEventArgs e)
    {
        HandleDragEffects(e);
    }
    private void CursorPanel11_DragOver(object sender, DragEventArgs e)
    {
        HandleDragEffects(e);
    }
    private void CursorPanel11_DragDrop(object sender, DragEventArgs e)
    {
        HandleDragDrop(e, CursorPictureBox11.Name);
    }
    private void CursorSelectButton11_Click(object sender, EventArgs e)
    {
        HandleFilePick(CursorPictureBox11.Name);
    }

    private void CursorPanel12_DragEnter(object sender, DragEventArgs e)
    {
        HandleDragEffects(e);
    }
    private void CursorPanel12_DragOver(object sender, DragEventArgs e)
    {
        HandleDragEffects(e);
    }
    private void CursorPanel12_DragDrop(object sender, DragEventArgs e)
    {
        HandleDragDrop(e, CursorPictureBox12.Name);
    }
    private void CursorSelectButton12_Click(object sender, EventArgs e)
    {
        HandleFilePick(CursorPictureBox12.Name);
    }

    private void CursorPanel13_DragEnter(object sender, DragEventArgs e)
    {
        HandleDragEffects(e);
    }
    private void CursorPanel13_DragOver(object sender, DragEventArgs e)
    {
        HandleDragEffects(e);
    }
    private void CursorPanel13_DragDrop(object sender, DragEventArgs e)
    {
        HandleDragDrop(e, CursorPictureBox13.Name);
    }
    private void CursorSelectButton13_Click(object sender, EventArgs e)
    {
        HandleFilePick(CursorPictureBox13.Name);
    }

    private void CursorPanel14_DragEnter(object sender, DragEventArgs e)
    {
        HandleDragEffects(e);
    }
    private void CursorPanel14_DragOver(object sender, DragEventArgs e)
    {
        HandleDragEffects(e);
    }
    private void CursorPanel14_DragDrop(object sender, DragEventArgs e)
    {
        HandleDragDrop(e, CursorPictureBox14.Name);
    }
    private void CursorSelectButton14_Click(object sender, EventArgs e)
    {
        HandleFilePick(CursorPictureBox14.Name);
    }

    private void CursorPanel15_DragEnter(object sender, DragEventArgs e)
    {
        HandleDragEffects(e);
    }
    private void CursorPanel15_DragOver(object sender, DragEventArgs e)
    {
        HandleDragEffects(e);
    }
    private void CursorPanel15_DragDrop(object sender, DragEventArgs e)
    {
        HandleDragDrop(e, CursorPictureBox15.Name);
    }
    private void CursorSelectButton15_Click(object sender, EventArgs e)
    {
        HandleFilePick(CursorPictureBox15.Name);
    }

    private void CursorPanel16_DragEnter(object sender, DragEventArgs e)
    {
        HandleDragEffects(e);
    }
    private void CursorPanel16_DragOver(object sender, DragEventArgs e)
    {
        HandleDragEffects(e);
    }
    private void CursorPanel16_DragDrop(object sender, DragEventArgs e)
    {
        HandleDragDrop(e, CursorPictureBox16.Name);
    }
    private void CursorSelectButton16_Click(object sender, EventArgs e)
    {
        HandleFilePick(CursorPictureBox16.Name);
    }

    private void CursorPanel17_DragEnter(object sender, DragEventArgs e)
    {
        HandleDragEffects(e);
    }
    private void CursorPanel17_DragOver(object sender, DragEventArgs e)
    {
        HandleDragEffects(e);
    }
    private void CursorPanel17_DragDrop(object sender, DragEventArgs e)
    {
        HandleDragDrop(e, CursorPictureBox17.Name);
    }
    private void CursorSelectButton17_Click(object sender, EventArgs e)
    {
        HandleFilePick(CursorPictureBox17.Name);
    }

    #endregion
}
