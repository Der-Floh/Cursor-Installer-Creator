using Microsoft.Win32;
using System.Diagnostics;
using System.Security.Principal;
using System.Text.Json;

namespace Cursor_Installer_Creator;

public sealed partial class MainForm : Form
{
    private Dictionary<string, string> BoxCursorAssignment { get; set; } = new Dictionary<string, string>();
    private Dictionary<string, string> CursorBoxAssignment { get; set; }
    private List<CCursor>? CCursors { get; set; }
    private System.Windows.Forms.Timer timer = new System.Windows.Forms.Timer();
    private bool isResizing;

    public MainForm(List<CCursor>? cCursors = null)
    {
        timer.Interval = 1000;
        timer.Tick += (sender, e) =>
        {
            Refresh();
        };

        InitializeComponent();

        DarkModeCheckBox.Checked = IsDarkModeEnabled();
        DarkModeCheckBox_CheckedChanged(DarkModeCheckBox, EventArgs.Empty);
        AdminLabel.Visible = !IsRunningWithAdminPrivileges();

        SetUpBoxCursorAssignment();
        FillCursors(cCursors);
    }

    public static bool IsDarkModeEnabled()
    {
        const string keyPath = @"Software\Microsoft\Windows\CurrentVersion\Themes\Personalize";
        const string valueName = "AppsUseLightTheme";

        using var key = Registry.CurrentUser.OpenSubKey(keyPath);
        if (key is not null && key.GetValue(valueName) is int value)
            return value == 0;
        return false;
    }

    public void ChangeTheme(bool light, Control.ControlCollection container)
    {
        if (light)
        {
            BackColor = SystemColors.Control;
            ForeColor = SystemColors.ControlText;
        }
        else
        {
            BackColor = SystemColors.ControlDarkDark;
            ForeColor = SystemColors.Control;
        }
        foreach (Control component in container)
        {
            if (component is Panel || component is Button || component is TextBox)
            {
                if (component is Panel)
                {
                    var panel = component as Panel;
                    if (panel is not null)
                        if (light)
                        {
                            if (panel.HasBorderColor())
                            {
                                panel.BorderStyle = BorderStyle.FixedSingle;
                                panel.RemoveBorderColor();
                            }
                        }
                        else if (panel.BorderStyle == BorderStyle.FixedSingle)
                        {
                            panel.BorderStyle = BorderStyle.None;
                            panel.SetBorderColor(SystemColors.ControlDark);
                        }
                    ChangeTheme(light, component.Controls);
                }
                else if (component is Button)
                {
                    var button = component as Button;
                    if (button is not null)
                        if (light)
                            button.FlatStyle = FlatStyle.Standard;
                        else
                        {
                            button.FlatStyle = FlatStyle.Flat;
                            button.FlatAppearance.BorderColor = SystemColors.ControlDark;
                        }
                }
                if (light)
                {
                    component.BackColor = SystemColors.Control;
                    component.ForeColor = SystemColors.ControlText;
                }
                else
                {
                    component.BackColor = SystemColors.ControlDarkDark;
                    component.ForeColor = SystemColors.Control;
                }
            }
        }
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
        CursorBoxAssignment = BoxCursorAssignment.ToDictionary(x => x.Value, x => x.Key);
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
            Panel? panel = pictureBox?.Parent as Panel;
            Label? label = CursorsTableLayoutPanel.Controls.Find(pictureBoxName.Replace("CursorPictureBox", "CursorNameLabel"), true).FirstOrDefault() as Label;

            if (pictureBox is null || label is null)
                continue;

            pictureBox.Image?.Dispose();
            label.Text = cursorName;
            if (PreviewCheckBox.Checked && panel is not null)
            {
                string? cursorPath = CCursors?.Find(x => x.Name?.ToLower() == cursorName.ToLower())?.CursorPath;
                if (!string.IsNullOrEmpty(cursorPath))
                    panel.Cursor = AdvancedCursors.Create(cursorPath);
            }

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
        Panel? panel = pictureBox?.Parent as Panel;
        Label? label = CursorsTableLayoutPanel.Controls.Find(pictureBoxName.Replace("CursorPictureBox", "CursorNameLabel"), true).FirstOrDefault() as Label;

        if (pictureBox is null || label is null)
            return;

        pictureBox.Image?.Dispose();
        label.Text = cursorName;
        if (PreviewCheckBox.Checked && panel is not null)
        {
            string? cursorPath = CCursors?.Find(x => x.Name?.ToLower() == cursorName.ToLower())?.CursorPath;
            if (!string.IsNullOrEmpty(cursorPath))
                panel.Cursor = AdvancedCursors.Create(cursorPath);
        }

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

    private void ResetCursor(string boxname)
    {
        string? newCursorPath = CursorHelper.GetSelectedCursorPath(BoxCursorAssignment[boxname]);
        if (string.IsNullOrEmpty(newCursorPath))
            return;
        UpdateCCursor(newCursorPath, boxname);
        FillCursor(boxname);
    }

    private string? GetCursorFile()
    {
        using OpenFileDialog openFileDialog = new OpenFileDialog();
        openFileDialog.Filter = "Cursor Files (.cur, .ani)|*.cur;*.ani";
        openFileDialog.FilterIndex = 0;
        openFileDialog.RestoreDirectory = true;
        openFileDialog.AddToRecent = true;
        openFileDialog.AutoUpgradeEnabled = true;

        if (openFileDialog.ShowDialog() != DialogResult.OK)
            return null;

        return openFileDialog.FileName;
    }

    private string? GetCursorInstallerFile()
    {
        using OpenFileDialog openFileDialog = new OpenFileDialog();
        openFileDialog.Filter = "Cursor Installer (.inf)|*.inf";
        openFileDialog.FilterIndex = 0;
        openFileDialog.RestoreDirectory = true;
        openFileDialog.AddToRecent = true;
        openFileDialog.AutoUpgradeEnabled = true;

        if (openFileDialog.ShowDialog() != DialogResult.OK)
            return null;

        return openFileDialog.FileName;
    }

    private void ImportCursorFiles(string filePath)
    {
        string? selectedFolder = Path.GetDirectoryName(filePath);
        if (string.IsNullOrEmpty(selectedFolder))
            return;

        var fileAssigns = ParseInstallerInfStrings(filePath);
        foreach (var file in fileAssigns)
        {
            CursorBoxAssignment.TryGetValue(file.Key, out string? boxname);
            if (string.IsNullOrEmpty(boxname))
                continue;
            UpdateCCursor(Path.Combine(selectedFolder, file.Value), boxname);
            FillCursor(boxname);
        }
    }

    public Dictionary<string, string> ParseInstallerInfStrings(string filePath)
    {
        var stringsDictionary = new Dictionary<string, string>();
        string[] lines = File.ReadAllLines(filePath);
        bool isStringsSection = false;

        foreach (string line in lines)
        {
            if (line.Trim().Equals("[Strings]", StringComparison.OrdinalIgnoreCase))
            {
                isStringsSection = true;
                continue;
            }

            if (isStringsSection)
            {
                string[] parts = line.Split('=');

                if (parts.Length == 2)
                {
                    string key = parts[0].Trim();
                    string? keyT = CursorHelper.TransformCursorName(key);
                    if (!string.IsNullOrEmpty(keyT))
                        key = keyT;
                    string value = parts[1].Trim().TrimStart('\"').TrimEnd('\"');

                    stringsDictionary[key] = value;
                }
            }
        }

        return stringsDictionary;
    }

    private bool IsCursorFile(string filepath)
    {
        string? extension = Path.GetExtension(filepath)?.ToLower();
        return extension == ".cur" || extension == ".ani";
    }
    private bool IsCursorInstallerFile(string filepath)
    {
        string? extension = Path.GetExtension(filepath)?.ToLower();
        return extension == ".inf";
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

        string file = filepaths.Where(IsCursorFile).First();
        UpdateCCursor(file, boxname);
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

    private void MainForm_Resize(object sender, EventArgs e)
    {
        if (DarkModeCheckBox.Checked && !isResizing)
        {
            isResizing = true;
            timer.Start();
        }
    }
    private void MainForm_ResizeEnd(object sender, EventArgs e)
    {
        if (isResizing)
        {
            timer.Stop();
            isResizing = false;
            Refresh();
        }
    }

    private void DarkModeCheckBox_CheckedChanged(object sender, EventArgs e)
    {
        ChangeTheme(!DarkModeCheckBox.Checked, this.Controls);
    }

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

    private void PreviewCheckBox_CheckedChanged(object sender, EventArgs e)
    {
        if (PreviewCheckBox.Checked)
        {
            foreach (KeyValuePair<string, string> kvp in BoxCursorAssignment)
            {
                string pictureBoxName = kvp.Key;
                string cursorName = kvp.Value;

                PictureBox? pictureBox = CursorsTableLayoutPanel.Controls.Find(pictureBoxName, true).FirstOrDefault() as PictureBox;
                Panel? panel = pictureBox?.Parent as Panel;

                if (panel is null)
                    if (panel is null)
                        continue;

                string? cursorPath = CCursors?.Find(x => x.Name?.ToLower() == cursorName.ToLower())?.CursorPath;
                if (string.IsNullOrEmpty(cursorPath))
                    continue;

                panel.Cursor = AdvancedCursors.Create(cursorPath);
            }
        }
        else
        {
            foreach (KeyValuePair<string, string> kvp in BoxCursorAssignment)
            {
                string pictureBoxName = kvp.Key;
                string cursorName = kvp.Value.ToLower();

                PictureBox? pictureBox = CursorsTableLayoutPanel.Controls.Find(pictureBoxName, true).FirstOrDefault() as PictureBox;
                Panel? panel = pictureBox?.Parent as Panel;

                if (panel is null)
                    continue;

                panel.Cursor = Cursors.Default;
            }
        }
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
    private void CursorResetButton1_Click(object sender, EventArgs e)
    {
        ResetCursor(CursorPictureBox1.Name);
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
    private void CursorResetButton2_Click(object sender, EventArgs e)
    {
        ResetCursor(CursorPictureBox2.Name);
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
    private void CursorResetButton3_Click(object sender, EventArgs e)
    {
        ResetCursor(CursorPictureBox3.Name);
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
    private void CursorResetButton4_Click(object sender, EventArgs e)
    {
        ResetCursor(CursorPictureBox4.Name);
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
    private void CursorResetButton5_Click(object sender, EventArgs e)
    {
        ResetCursor(CursorPictureBox5.Name);
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
    private void CursorResetButton6_Click(object sender, EventArgs e)
    {
        ResetCursor(CursorPictureBox6.Name);
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
    private void CursorResetButton7_Click(object sender, EventArgs e)
    {
        ResetCursor(CursorPictureBox7.Name);
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
    private void CursorResetButton8_Click(object sender, EventArgs e)
    {
        ResetCursor(CursorPictureBox8.Name);
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
    private void CursorResetButton9_Click(object sender, EventArgs e)
    {
        ResetCursor(CursorPictureBox9.Name);
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
    private void CursorResetButton10_Click(object sender, EventArgs e)
    {
        ResetCursor(CursorPictureBox10.Name);
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
    private void CursorResetButton11_Click(object sender, EventArgs e)
    {
        ResetCursor(CursorPictureBox11.Name);
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
    private void CursorResetButton12_Click(object sender, EventArgs e)
    {
        ResetCursor(CursorPictureBox12.Name);
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
    private void CursorResetButton13_Click(object sender, EventArgs e)
    {
        ResetCursor(CursorPictureBox13.Name);
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
    private void CursorResetButton14_Click(object sender, EventArgs e)
    {
        ResetCursor(CursorPictureBox14.Name);
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
    private void CursorResetButton15_Click(object sender, EventArgs e)
    {
        ResetCursor(CursorPictureBox15.Name);
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
    private void CursorResetButton16_Click(object sender, EventArgs e)
    {
        ResetCursor(CursorPictureBox16.Name);
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
    private void CursorResetButton17_Click(object sender, EventArgs e)
    {
        ResetCursor(CursorPictureBox17.Name);
    }

    private void CursorsAllResetButton_Click(object sender, EventArgs e)
    {
        foreach (KeyValuePair<string, string> kvp in BoxCursorAssignment)
        {
            string pictureBoxName = kvp.Key;
            PictureBox? pictureBox = CursorsTableLayoutPanel.Controls.Find(pictureBoxName, true).FirstOrDefault() as PictureBox;
            if (pictureBox is not null)
                ResetCursor(pictureBox.Name);
        }
    }

    private void CursorsAllPanel_DragEnter(object sender, DragEventArgs e)
    {
        if (e.Data.GetDataPresent(DataFormats.FileDrop))
        {
            string[] filepaths = (string[])e.Data.GetData(DataFormats.FileDrop);

            if (filepaths.Any(IsCursorInstallerFile))
                e.Effect = DragDropEffects.Copy;
            else
                e.Effect = DragDropEffects.None;
        }
        else
        {
            e.Effect = DragDropEffects.None;
        }
    }
    private void CursorsAllPanel_DragOver(object sender, DragEventArgs e)
    {
        if (e.Data.GetDataPresent(DataFormats.FileDrop))
        {
            string[] filepaths = (string[])e.Data.GetData(DataFormats.FileDrop);

            if (filepaths.Any(IsCursorInstallerFile))
                e.Effect = DragDropEffects.Copy;
            else
                e.Effect = DragDropEffects.None;
        }
        else
        {
            e.Effect = DragDropEffects.None;
        }
    }
    private void CursorsAllPanel_DragDrop(object sender, DragEventArgs e)
    {
        string[]? filepaths = (string[]?)e.Data.GetData(DataFormats.FileDrop);

        if (filepaths is null || filepaths.Length == 0)
            return;

        string file = filepaths.Where(IsCursorInstallerFile).First();
        ImportCursorFiles(file);
    }
    private void CursorsAllImportButton_Click(object sender, EventArgs e)
    {
        string? file = GetCursorInstallerFile();
        if (string.IsNullOrEmpty(file))
            return;
        ImportCursorFiles(file);
    }

    #endregion
}
