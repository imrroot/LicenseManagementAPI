using System.Reflection;
using LicenseManagementLibrary.src.Core;
using LicenseManagementLibrary.src.Security;

namespace User_App_Test
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            
        }
        private LicenseManager _licenseManager;
        private async void Form1_Load(object sender, EventArgs e)
        {
            _licenseManager = new LicenseManager("https://localhost:7113/api", "qS1fJkQTft", "321a783fa45449c3b7c665b9a29a7127", "y-V7zAZZfm8NfgXF");
            var msg = await _licenseManager.InitializeAsync("ZNYM-FVRY-BLGT-KQPL");
            rich_txt.Text = "Message : " + msg + "\n\n\n";
            if (_licenseManager.License != null)
            {
                rich_txt.Text += $"License ExpirationDate : {_licenseManager.License.ExpirationDate} " +
                                $"\nLicense Status : {_licenseManager.License.Status.ToString()}\n" +
                                $"License LastUsed : {_licenseManager.License.LastUsedDate}" +
                                $"\n\n\nSubscription : \nPlan Name : {_licenseManager.Subscription.Name}";
            }
            

        }
        
        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                var dialog = new Form2();
                AccessControl.EnsureAccess(dialog, _licenseManager);
                dialog.ShowDialog();
            }
            catch (UnauthorizedAccessException ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        [AccessLevel(1)]
        private void button2_Click(object sender, EventArgs e)
        {

            try
            {
                AccessControl.EnsureMethodAccess(this, _licenseManager);
                MessageBox.Show("Button clicked and access granted!");
            }
            catch (UnauthorizedAccessException ex)
            {
                MessageBox.Show(ex.Message, "Access Denied", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

    }
}
