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

namespace AsyncUtils
{
    public partial class AsyncProgressDialog : Form
    {
        public string SuccessButtonText;
        public string SuccessMessage;

        public CancellationTokenSource TheCancellationTokenSource = null;
        public AsyncProgressDialog()
        {
            InitializeComponent();
        }

        public DialogResult ShowDialog(string Title, string Message, string SuccessButtonText, string SuccessMessage)
        {
            this.SuccessButtonText = SuccessButtonText;
            this.SuccessMessage = SuccessMessage;
            this.Text = Title;
            this.label1.Text = Message;
            return base.ShowDialog();
        }

        public async void RunAsync(Action action, CancellationTokenSource cancelTokenSource)
        {
            bool wasCancelled = false;
            TheCancellationTokenSource = cancelTokenSource;
            try
            {
                await Task.Run(action, cancelTokenSource.Token);
            }
            catch (TaskCanceledException)
            {
                wasCancelled = true;
                this.Close();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString(), "Exception", MessageBoxButtons.OK, MessageBoxIcon.Error);

                wasCancelled = true;
                this.DialogResult = DialogResult.Abort;
                this.Close();
            }
            finally
            {
                TheCancellationTokenSource = null;

                if (!wasCancelled)
                {
                    // we are done, change the text
                    this.button1.Text = SuccessButtonText;
                    this.label1.Text = SuccessMessage;
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (TheCancellationTokenSource != null)
            {
                TheCancellationTokenSource.Cancel();
                this.DialogResult = DialogResult.Abort;
                this.Close();
            }
            else
            {
                this.Close();
                this.DialogResult = DialogResult.OK;
            }
        }

        private void Form2_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (TheCancellationTokenSource != null)
            {
                TheCancellationTokenSource.Cancel();
                this.DialogResult = DialogResult.Abort;
            }
        }
    }
}
