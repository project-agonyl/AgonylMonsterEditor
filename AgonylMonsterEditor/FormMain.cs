using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Windows.Forms;

namespace AgonylMonsterEditor
{
    public partial class FormMain : Form
    {
        private Queue<A3NPC> _a3Npcs = new Queue<A3NPC>();
        private BindingList<A3NPC> _a3NpcsBound = new BindingList<A3NPC>();

        public FormMain()
        {
            InitializeComponent();
        }

        private void FormMain_Load(object sender, EventArgs e)
        {
            this.dataGridView.AutoGenerateColumns = false;
            this.dataGridView.Columns.Add(new DataGridViewTextBoxColumn()
            {
                DataPropertyName = "Id",
                Name = "ID",
                Width = 50,
            });
            this.dataGridView.Columns.Add(new DataGridViewTextBoxColumn()
            {
                DataPropertyName = "Name",
                Name = "Name",
                Width = 100,
            });
            this.dataGridView.Columns.Add(new DataGridViewTextBoxColumn()
            {
                DataPropertyName = "Level",
                Name = "Level",
                Width = 50,
            });
            this.dataGridView.Columns.Add(new DataGridViewTextBoxColumn()
            {
                DataPropertyName = "Defense",
                Name = "Defense",
                Width = 50,
            });
            this.dataGridView.Columns.Add(new DataGridViewTextBoxColumn()
            {
                DataPropertyName = "Attack",
                Name = "Attack",
                Width = 150,
            });
            this.dataGridView.Columns.Add(new DataGridViewTextBoxColumn()
            {
                DataPropertyName = "HP",
                Name = "HP",
                Width = 100,
            });
            this.dataGridView.Columns.Add(new DataGridViewTextBoxColumn()
            {
                DataPropertyName = "RespawnRate",
                Name = "Respawn Rate",
                Width = 150,
            });
            this.dataGridView.DataSource = this._a3NpcsBound;
            this.ReloadDataButton.Enabled = false;
            Utils.LoadMonsterData();
        }

        private void ChooseNPCFolderButton_Click(object sender, EventArgs e)
        {
            if (this.FolderBrowser.ShowDialog() == DialogResult.OK)
            {
                this.CurrentMonsterFolderLabel.Text = this.FolderBrowser.SelectedPath;
                this.NPCDataLoaderBgWorker.RunWorkerAsync();
            }
        }

        private void ReloadDataButton_Click(object sender, EventArgs e)
        {
            if (this.NPCDataLoaderBgWorker.IsBusy)
            {
                _ = MessageBox.Show("Monster loader is busy!", "Agonyl Monster Editor", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            this._a3NpcsBound.Clear();
            this._a3Npcs.Clear();
            this.NPCDataLoaderBgWorker.RunWorkerAsync();
            this.ReloadDataButton.Enabled = false;
            this.ChooseNPCFolderButton.Enabled = false;
        }

        private void NPCDataLoaderBgWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            if (!Directory.Exists(this.CurrentMonsterFolderLabel.Text))
            {
                return;
            }

            foreach (var file in Directory.GetFiles(this.CurrentMonsterFolderLabel.Text, "*"))
            {
                try
                {
                    // We don't want files that have extension or not a number
                    if (Path.HasExtension(file) || !decimal.TryParse(Path.GetFileName(file), out _))
                    {
                        continue;
                    }

                    var fileBytes = File.ReadAllBytes(file);
                    var id = Utils.BytesToUInt16(Utils.SkipAndTakeLinqShim(ref fileBytes, 2, 20));

                    // ID above 1000 are NPCs hence ignore
                    if (id >= 1000)
                    {
                        continue;
                    }

                    var damage = Utils.BytesToUInt16(Utils.SkipAndTakeLinqShim(ref fileBytes, 2, 32));
                    var maxDamage = damage + Utils.BytesToUInt16(Utils.SkipAndTakeLinqShim(ref fileBytes, 2, 34));
                    var name = System.Text.Encoding.Default.GetString(Utils.SkipAndTakeLinqShim(ref fileBytes, 20)).Trim();
                    if (Utils.MonsterList.ContainsKey(id))
                    {
                        name = Utils.MonsterList[id].Name;
                    }

                    var monsterData = new A3NPC()
                    {
                        Id = id,
                        Name = name,
                        RespawnRate = Utils.BytesToUInt16(Utils.SkipAndTakeLinqShim(ref fileBytes, 2, 22)),
                        Level = fileBytes[60],
                        HP = Utils.BytesToUInt32(Utils.SkipAndTakeLinqShim(ref fileBytes, 4, 64)),
                        Attack = damage + " to " + maxDamage,
                        Defense = fileBytes[26],
                        File = file,
                    };
                    this._a3Npcs.Enqueue(monsterData);
                    this.NPCDataLoaderBgWorker.ReportProgress(monsterData.Id);
                }
                catch { }
            }
        }

        private void NPCDataLoaderBgWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            if (this._a3Npcs.Count != 0)
            {
                this._a3NpcsBound.Add(this._a3Npcs.Dequeue());
            }
        }

        private void NPCDataLoaderBgWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            this.dataGridView.Refresh();
            if (this._a3NpcsBound.Count == 0)
            {
                _ = MessageBox.Show("Could not find any monster file", "Agonyl Monster Editor", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                this.FixEmptyCells();
                _ = MessageBox.Show("Loaded " + this._a3NpcsBound.Count + " monsters", "Agonyl Monster Editor", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

            this.ReloadDataButton.Enabled = true;
            this.ChooseNPCFolderButton.Enabled = true;
        }

        private void dataGridView_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            var dataIndexNo = this.dataGridView.Rows[e.RowIndex].Index;
            var questEditorForm = new FormMonsterEditor();
            questEditorForm.MonsterFile = this._a3NpcsBound[dataIndexNo].File;
            questEditorForm.ShowDialog();
        }

        private void dataGridView_DragEnter(object sender, DragEventArgs e)
        {
            if (!this.NPCDataLoaderBgWorker.IsBusy)
            {
                // Check if the Data format of the file(s) can be accepted
                // (we only accept file drops from Windows Explorer, etc.)
                if (e.Data.GetDataPresent(DataFormats.FileDrop))
                {
                    // modify the drag drop effects to Move
                    e.Effect = DragDropEffects.All;
                }
                else
                {
                    // no need for any drag drop effect
                    e.Effect = DragDropEffects.None;
                }
            }
        }

        private void dataGridView_DragDrop(object sender, DragEventArgs e)
        {
            if (!this.NPCDataLoaderBgWorker.IsBusy)
            {
                // still check if the associated data from the file(s) can be used for this purpose
                if (e.Data.GetDataPresent(DataFormats.FileDrop))
                {
                    // Fetch the file(s) names with full path here to be processed
                    var fileList = (string[])e.Data.GetData(DataFormats.FileDrop);
                    if (!Path.HasExtension(fileList[0]))
                    {
                        var questEditorForm = new FormMonsterEditor();
                        questEditorForm.MonsterFile = fileList[0];
                        questEditorForm.ShowDialog();
                    }
                }
            }
        }

        // This fix is needed as I cannot figure out why cell gets empty though data is available
        private void FixEmptyCells()
        {
            foreach (DataGridViewRow rw in this.dataGridView.Rows)
            {
                if (rw.Index >= this._a3NpcsBound.Count)
                {
                    break;
                }

                for (var i = 0; i < rw.Cells.Count; i++)
                {
                    if (rw.Cells[i].Value == null)
                    {
                        switch (i)
                        {
                            case 0:
                                rw.Cells[i].Value = this._a3NpcsBound[rw.Index].Id;
                                break;

                            case 1:
                                rw.Cells[i].Value = this._a3NpcsBound[rw.Index].Name;
                                break;

                            case 2:
                                rw.Cells[i].Value = this._a3NpcsBound[rw.Index].Level;
                                break;

                            case 3:
                                rw.Cells[i].Value = this._a3NpcsBound[rw.Index].Defense;
                                break;

                            case 4:
                                rw.Cells[i].Value = this._a3NpcsBound[rw.Index].Attack;
                                break;

                            case 5:
                                rw.Cells[i].Value = this._a3NpcsBound[rw.Index].HP;
                                break;

                            case 6:
                                rw.Cells[i].Value = this._a3NpcsBound[rw.Index].RespawnRate;
                                break;
                        }
                    }
                }
            }
        }
    }
}
