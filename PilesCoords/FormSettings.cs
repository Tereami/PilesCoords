#region License
/*Данный код опубликован под лицензией Creative Commons Attribution-ShareAlike.
Разрешено использовать, распространять, изменять и брать данный код за основу для производных в коммерческих и
некоммерческих целях, при условии указания авторства и если производные лицензируются на тех же условиях.
Код поставляется "как есть". Автор не несет ответственности за возможные последствия использования.
Зуев Александр, 2020, все права защищены.
This code is listed under the Creative Commons Attribution-ShareAlike license.
You may use, redistribute, remix, tweak, and build upon this work non-commercially and commercially,
as long as you credit the author by linking back and license your new creations under the same terms.
This code is provided 'as is'. Author disclaims any implied warranty.
Zuev Aleksandr, 2020, all rigths reserved.*/
#endregion
#region Usings
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
#endregion

namespace PilesCoords
{
    public partial class FormSettings : Form
    {
        public FormSettings()
        {
            InitializeComponent();

            radioNumberingDown.Checked = Settings.numberingUpDown;
            radioNumberingUp.Checked = !Settings.numberingUpDown;

            numPileDepth.Value = (decimal)Settings.pileDepth;
            txtPileFamilyName.Text = Settings.pileFamilyName;

            checkBoxSortByBottomElev_FirstTable.Checked = Settings.sortByBottomElev_Table1;
            checkBoxSortByTopElev_FirstTable.Checked = Settings.sortByTopElev_Table1;
            checkBoxSortByCutLength_FirstTable.Checked = Settings.sortByCutLength_Table1;
            checkBoxSortBySlabElev_FirstTable.Checked = Settings.sortBySlabElev_Table1;

            checkBoxSortByBottomElev_Table2.Checked = Settings.sortByBottomElev_Table2;
            checkBoxSortByTopElev_Table2.Checked = Settings.sortByTopElev_Table2;
            checkBoxSortByCutLength_Table2.Checked = Settings.sortByCutLength_Table2;
            checkBoxSortBySlabElev_Table2.Checked = Settings.sortBySlabElev_Table2;

        }


        private void buttonCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void buttonOk_Click(object sender, EventArgs e)
        {
            Settings.numberingUpDown = radioNumberingDown.Checked;
            Settings.pileFamilyName = txtPileFamilyName.Text;

            Settings.sortByBottomElev_Table1 = checkBoxSortByBottomElev_FirstTable.Checked;
            Settings.sortByTopElev_Table1 = checkBoxSortByTopElev_FirstTable.Checked;
            Settings.sortByCutLength_Table1 = checkBoxSortByCutLength_FirstTable.Checked;
            Settings.sortBySlabElev_Table1 = checkBoxSortBySlabElev_FirstTable.Checked;

            Settings.sortByBottomElev_Table2 = checkBoxSortByBottomElev_Table2.Checked;
            Settings.sortByTopElev_Table2 = checkBoxSortByTopElev_Table2.Checked;
            Settings.sortByCutLength_Table2 = checkBoxSortByCutLength_Table2.Checked;
            Settings.sortBySlabElev_Table2 = checkBoxSortBySlabElev_Table2.Checked;


            Settings.pileDepth = (double)numPileDepth.Value;

            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void numPileDepth_ValueChanged(object sender, EventArgs e)
        {

        }
    }
}
