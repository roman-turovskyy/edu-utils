using System;
using System.Windows.Forms;
using Eleks.Demo;

namespace FormsFilesFinder
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            lbResults.Items.Clear();
            var filesFinder = new FilesFinder();
            string[] directories = tbPath.Text.Split(new[] {' '});
            var foundFilesAndCount = filesFinder.SearchWithSameNames(directories);
            lbResults.BeginUpdate();
            foreach (var fc in foundFilesAndCount)
            {
                lbResults.Items.Add(string.Format("{0}: {1}", fc.FileName, fc.Count));
            }
            lbResults.EndUpdate();
        }
    }
}
