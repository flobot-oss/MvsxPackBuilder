using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MvsxPackBuilder
{
    public partial class CategoryEditor : Form
    {
        private Hylo.Category TransientCategory = null;
        private Hylo.Category CategoryToEdit = null;

        public CategoryEditor()
        {
            InitializeComponent();
        }

        public DialogResult ShowDialog(string Title, Hylo.Category CategoryToEdit)
        {
            this.CategoryToEdit = CategoryToEdit;

            TransientCategory = new Hylo.Category();

            TransientCategory.CustomBackgroundPath = CategoryToEdit.CustomBackgroundPath;
            TransientCategory.CustomIndicatorPath = CategoryToEdit.CustomIndicatorPath;
            TransientCategory.DisplayName = CategoryToEdit.DisplayName;
            TransientCategory.FolderName = CategoryToEdit.FolderName;

            // setup our components
            this.BackgroundPicture.ImageLocation = CategoryToEdit.CustomBackgroundPath;
            this.IndicatorPicture.ImageLocation = CategoryToEdit.CustomIndicatorPath;
            this.CategoryNameTextBox1.Text = CategoryToEdit.DisplayName;

            this.Text = Title;
            return base.ShowDialog();
        }

        private void OKButton1_Click(object sender, EventArgs e)
        {
            // make sure the text is updated.
            TransientCategory.DisplayName = CategoryNameTextBox1.Text;

            // copy data back
            CategoryToEdit.CustomBackgroundPath = TransientCategory.CustomBackgroundPath;
            CategoryToEdit.CustomIndicatorPath = TransientCategory.CustomIndicatorPath;
            CategoryToEdit.DisplayName = TransientCategory.DisplayName;
            CategoryToEdit.FolderName = TransientCategory.FolderName;

            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void CancelButton2_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void SelectBackgroundButton_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "Image Files|*.jpg;*.jpeg;*.png";

            dialog.Title = "Please select an image for the category background.";
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                TransientCategory.CustomBackgroundPath = dialog.FileName;
                this.BackgroundPicture.ImageLocation = dialog.FileName;
            }
        }

        private void SelectIndicatorButton1_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "Image Files|*.jpg;*.jpeg;*.png";

            dialog.Title = "Please select an image for the category indicator image.";
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                TransientCategory.CustomIndicatorPath = dialog.FileName;
                this.IndicatorPicture.ImageLocation = dialog.FileName;
            }
        }
    }
}
