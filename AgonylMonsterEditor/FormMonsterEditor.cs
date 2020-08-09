using System;
using System.IO;
using System.Windows.Forms;

namespace AgonylMonsterEditor
{
    public partial class FormMonsterEditor : Form
    {
        public string MonsterFile;

        private byte[] _monsterFileData;
        private A3NPC _currentQuestData;

        public FormMonsterEditor()
        {
            InitializeComponent();
        }

        private void FormMonsterEditor_Load(object sender, EventArgs e)
        {
            this.Text = "Monster Editor - " + Path.GetFileName(this.MonsterFile);
            this._monsterFileData = File.ReadAllBytes(this.MonsterFile);
            this.SaveMonsterFileDialog.FileName = Utils.BytesToUInt16(Utils.SkipAndTakeLinqShim(ref this._monsterFileData, 2, 20)).ToString();
            this.SaveMonsterFileDialog.InitialDirectory = new FileInfo(this.MonsterFile).Directory.Name;
            this._currentQuestData = new A3NPC()
            {
                Id = Utils.BytesToUInt16(Utils.SkipAndTakeLinqShim(ref this._monsterFileData, 2, 20)),
                Name = System.Text.Encoding.Default.GetString(Utils.SkipAndTakeLinqShim(ref this._monsterFileData, 20)).Trim(),
                RespawnRate = Utils.BytesToUInt16(Utils.SkipAndTakeLinqShim(ref this._monsterFileData, 2, 22)),
                Level = this._monsterFileData[60],
                HP = Utils.BytesToUInt32(Utils.SkipAndTakeLinqShim(ref this._monsterFileData, 4, 64)),
                Defense = this._monsterFileData[26],
                AdditionalDefense = this._monsterFileData[27],
                MovementSpeed = Utils.BytesToUInt16(Utils.SkipAndTakeLinqShim(ref this._monsterFileData, 2, 56)),
                PlayerExp = Utils.BytesToUInt16(Utils.SkipAndTakeLinqShim(ref this._monsterFileData, 2, 61)),
                BlueAttackDefense = Utils.BytesToUInt16(Utils.SkipAndTakeLinqShim(ref this._monsterFileData, 2, 70)),
                RedAttackDefense = Utils.BytesToUInt16(Utils.SkipAndTakeLinqShim(ref this._monsterFileData, 2, 72)),
                GreyAttackDefense = Utils.BytesToUInt16(Utils.SkipAndTakeLinqShim(ref this._monsterFileData, 2, 74)),
                MercenaryExp = Utils.BytesToUInt16(Utils.SkipAndTakeLinqShim(ref this._monsterFileData, 2, 76)),
                AttackSpeedLow = Utils.BytesToUInt16(Utils.SkipAndTakeLinqShim(ref this._monsterFileData, 2, 52)),
                AttackSpeedHigh = Utils.BytesToUInt16(Utils.SkipAndTakeLinqShim(ref this._monsterFileData, 2, 54)),
            };
            for (var i = 0; i < 3; i++)
            {
                this._currentQuestData.Attacks[i] = new A3NPCAttack()
                {
                    Range = this._monsterFileData[28 + (i * 8)],
                    Area = this._monsterFileData[29 + (i * 8)],
                    Damage = Utils.BytesToUInt16(Utils.SkipAndTakeLinqShim(ref this._monsterFileData, 2, 32 + (i * 8))),
                    AdditionalDamage = Utils.BytesToUInt16(Utils.SkipAndTakeLinqShim(ref this._monsterFileData, 2, 34 + (i * 8))),
                };
            }

            this.LoadTextboxesWithMonsterData();
        }

        private void Textbox_TextChanged(object sender, EventArgs e)
        {
            var textbox = sender as TextBox;
            if (System.Text.RegularExpressions.Regex.IsMatch(textbox.Text, "[^0-9]"))
            {
                _ = MessageBox.Show("Please enter only numbers", "Agonyl Monster Editor", MessageBoxButtons.OK, MessageBoxIcon.Error);
                textbox.Text = string.Empty;
            }
        }

        private void SaveQuestButton_Click(object sender, EventArgs e)
        {
            try
            {
                var outFileData = this._monsterFileData;
                if (!Utils.IsEmptyData(this.MonsterIdTb.Text))
                {
                    Utils.ReplaceBytesAt(ref outFileData, 20, BitConverter.GetBytes(Convert.ToUInt16(this.MonsterIdTb.Text)));
                }

                if (!Utils.IsEmptyData(this.RespawnRateTb.Text))
                {
                    Utils.ReplaceBytesAt(ref outFileData, 22, BitConverter.GetBytes(Convert.ToUInt16(this.RespawnRateTb.Text)));
                }

                if (!Utils.IsEmptyData(this.LevelTb.Text))
                {
                    outFileData[60] = Convert.ToByte(this.LevelTb.Text);
                }

                if (!Utils.IsEmptyData(this.PlayerExpTb.Text))
                {
                    Utils.ReplaceBytesAt(ref outFileData, 61, BitConverter.GetBytes(Convert.ToUInt16(this.PlayerExpTb.Text)));
                }

                if (!Utils.IsEmptyData(this.MercExpTb.Text))
                {
                    Utils.ReplaceBytesAt(ref outFileData, 76, BitConverter.GetBytes(Convert.ToUInt16(this.MercExpTb.Text)));
                }

                if (!Utils.IsEmptyData(this.HPTb.Text))
                {
                    Utils.ReplaceBytesAt(ref outFileData, 64, BitConverter.GetBytes(Convert.ToUInt32(this.HPTb.Text)));
                }

                if (!Utils.IsEmptyData(this.DefenseTb.Text))
                {
                    outFileData[26] = Convert.ToByte(this.DefenseTb.Text);
                }

                if (!Utils.IsEmptyData(this.AdditionalDefenseTb.Text))
                {
                    outFileData[27] = Convert.ToByte(this.AdditionalDefenseTb.Text);
                }

                if (!Utils.IsEmptyData(this.MovementSpeedTb.Text))
                {
                    Utils.ReplaceBytesAt(ref outFileData, 56, BitConverter.GetBytes(Convert.ToUInt16(this.MovementSpeedTb.Text)));
                }

                if (!Utils.IsEmptyData(this.MinAttackSpeedTb.Text))
                {
                    Utils.ReplaceBytesAt(ref outFileData, 52, BitConverter.GetBytes(Convert.ToUInt16(this.MinAttackSpeedTb.Text)));
                }

                if (!Utils.IsEmptyData(this.MaxAttackSpeedTb.Text))
                {
                    Utils.ReplaceBytesAt(ref outFileData, 54, BitConverter.GetBytes(Convert.ToUInt16(this.MaxAttackSpeedTb.Text)));
                }

                if (!Utils.IsEmptyData(this.BlueDefenseTb.Text))
                {
                    Utils.ReplaceBytesAt(ref outFileData, 70, BitConverter.GetBytes(Convert.ToUInt16(this.BlueDefenseTb.Text)));
                }

                if (!Utils.IsEmptyData(this.RedDefenseTb.Text))
                {
                    Utils.ReplaceBytesAt(ref outFileData, 72, BitConverter.GetBytes(Convert.ToUInt16(this.RedDefenseTb.Text)));
                }

                if (!Utils.IsEmptyData(this.GreyDefenseTb.Text))
                {
                    Utils.ReplaceBytesAt(ref outFileData, 74, BitConverter.GetBytes(Convert.ToUInt16(this.GreyDefenseTb.Text)));
                }

                if (!Utils.IsEmptyData(this.AttackRangeTb1.Text))
                {
                    outFileData[28] = Convert.ToByte(this.AttackRangeTb1.Text);
                }

                if (!Utils.IsEmptyData(this.AttackAreaTb1.Text))
                {
                    outFileData[29] = Convert.ToByte(this.AttackAreaTb1.Text);
                }

                if (!Utils.IsEmptyData(this.DamageTb1.Text))
                {
                    Utils.ReplaceBytesAt(ref outFileData, 32, BitConverter.GetBytes(Convert.ToUInt16(this.DamageTb1.Text)));
                }

                if (!Utils.IsEmptyData(this.AdditionalDamageTb1.Text))
                {
                    Utils.ReplaceBytesAt(ref outFileData, 34, BitConverter.GetBytes(Convert.ToUInt16(this.AdditionalDamageTb1.Text)));
                }

                if (!Utils.IsEmptyData(this.AttackRangeTb2.Text))
                {
                    outFileData[28 + 8] = Convert.ToByte(this.AttackRangeTb2.Text);
                }

                if (!Utils.IsEmptyData(this.AttackAreaTb2.Text))
                {
                    outFileData[29 + 8] = Convert.ToByte(this.AttackAreaTb2.Text);
                }

                if (!Utils.IsEmptyData(this.DamageTb2.Text))
                {
                    Utils.ReplaceBytesAt(ref outFileData, 32 + 8, BitConverter.GetBytes(Convert.ToUInt16(this.DamageTb2.Text)));
                }

                if (!Utils.IsEmptyData(this.AdditionalDamageTb2.Text))
                {
                    Utils.ReplaceBytesAt(ref outFileData, 34 + 8, BitConverter.GetBytes(Convert.ToUInt16(this.AdditionalDamageTb2.Text)));
                }

                if (!Utils.IsEmptyData(this.AttackRangeTb3.Text))
                {
                    outFileData[28 + 16] = Convert.ToByte(this.AttackRangeTb3.Text);
                }

                if (!Utils.IsEmptyData(this.AttackAreaTb3.Text))
                {
                    outFileData[29 + 16] = Convert.ToByte(this.AttackAreaTb3.Text);
                }

                if (!Utils.IsEmptyData(this.DamageTb3.Text))
                {
                    Utils.ReplaceBytesAt(ref outFileData, 32 + 16, BitConverter.GetBytes(Convert.ToUInt16(this.DamageTb3.Text)));
                }

                if (!Utils.IsEmptyData(this.AdditionalDamageTb3.Text))
                {
                    Utils.ReplaceBytesAt(ref outFileData, 34 + 16, BitConverter.GetBytes(Convert.ToUInt16(this.AdditionalDamageTb3.Text)));
                }

                if (this.SaveMonsterFileDialog.ShowDialog() != DialogResult.Cancel)
                {
                    File.WriteAllBytes(this.SaveMonsterFileDialog.FileName, outFileData);
                    _ = MessageBox.Show("Saved the file " + Path.GetFileName(this.SaveMonsterFileDialog.FileName), "Agonyl Monster Editor", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                _ = MessageBox.Show(ex.Message, "Agonyl Monster Editor", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ResetFieldsButton_Click(object sender, EventArgs e)
        {
            this.LoadTextboxesWithMonsterData();
        }

        private void LoadTextboxesWithMonsterData()
        {
            this.MonsterNameTb.Text = this._currentQuestData.Name;
            this.MonsterIdTb.Text = this._currentQuestData.Id.ToString();
            this.RespawnRateTb.Text = this._currentQuestData.RespawnRate.ToString();
            this.LevelTb.Text = this._currentQuestData.Level.ToString();
            this.PlayerExpTb.Text = this._currentQuestData.PlayerExp.ToString();
            this.MercExpTb.Text = this._currentQuestData.MercenaryExp.ToString();
            this.HPTb.Text = this._currentQuestData.HP.ToString();
            this.DefenseTb.Text = this._currentQuestData.Defense.ToString();
            this.AdditionalDefenseTb.Text = this._currentQuestData.AdditionalDefense.ToString();
            this.MovementSpeedTb.Text = this._currentQuestData.MovementSpeed.ToString();
            this.MinAttackSpeedTb.Text = this._currentQuestData.AttackSpeedLow.ToString();
            this.MaxAttackSpeedTb.Text = this._currentQuestData.AttackSpeedHigh.ToString();
            this.BlueDefenseTb.Text = this._currentQuestData.BlueAttackDefense.ToString();
            this.RedDefenseTb.Text = this._currentQuestData.RedAttackDefense.ToString();
            this.GreyDefenseTb.Text = this._currentQuestData.GreyAttackDefense.ToString();

            this.AttackRangeTb1.Text = this._currentQuestData.Attacks[0].Range.ToString();
            this.AttackAreaTb1.Text = this._currentQuestData.Attacks[0].Area.ToString();
            this.DamageTb1.Text = this._currentQuestData.Attacks[0].Damage.ToString();
            this.AdditionalDamageTb1.Text = this._currentQuestData.Attacks[0].AdditionalDamage.ToString();

            this.AttackRangeTb2.Text = this._currentQuestData.Attacks[1].Range.ToString();
            this.AttackAreaTb2.Text = this._currentQuestData.Attacks[1].Area.ToString();
            this.DamageTb2.Text = this._currentQuestData.Attacks[1].Damage.ToString();
            this.AdditionalDamageTb2.Text = this._currentQuestData.Attacks[1].AdditionalDamage.ToString();

            this.AttackRangeTb3.Text = this._currentQuestData.Attacks[2].Range.ToString();
            this.AttackAreaTb3.Text = this._currentQuestData.Attacks[2].Area.ToString();
            this.DamageTb3.Text = this._currentQuestData.Attacks[2].Damage.ToString();
            this.AdditionalDamageTb3.Text = this._currentQuestData.Attacks[2].AdditionalDamage.ToString();
        }
    }
}
