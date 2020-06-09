using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

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
