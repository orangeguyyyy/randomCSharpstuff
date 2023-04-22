using Microsoft.Win32;
using System;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace setdefaultapp
{
    public partial class Form1 : Form
    {
        [ComImport]
        [Guid("1968106d-f3b5-44cf-890e-116fcb9ecef1")]
        class ApplicationAssociationRegistration { }

        [ComImport]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        [Guid("4e530b0a-e611-4c77-a3ac-9031d022281b")]
        interface IApplicationAssociationRegistration
        {
            void QueryCurrentDefault([MarshalAs(UnmanagedType.LPWStr)] string pszQuery, ASSOCIATIONTYPE atQueryType, ASSOCIATIONLEVEL alQueryLevel, [MarshalAs(UnmanagedType.LPWStr)] out string ppszAssociation);
            void QueryAppIsDefault([MarshalAs(UnmanagedType.LPWStr)] string pszQuery, ASSOCIATIONTYPE atQueryType, ASSOCIATIONLEVEL alQueryLevel, [MarshalAs(UnmanagedType.LPWStr)] string pszAppRegistryName, out bool pfDefault);
            void SetAppAsDefault([MarshalAs(UnmanagedType.LPWStr)] string pszAppRegistryName, [MarshalAs(UnmanagedType.LPWStr)] string pszSet, ASSOCIATIONTYPE atSetType);
            void SetAppAsDefaultAll([MarshalAs(UnmanagedType.LPWStr)] string pszAppRegistryName);
            void ClearUserAssociations();
        }

        enum ASSOCIATIONTYPE
        {
            AT_FILEEXTENSION,
            AT_URLPROTOCOL,
            AT_STARTMENUCLIENT,
            AT_MIMETYPE
        }

        enum ASSOCIATIONLEVEL
        {
            AL_MACHINE,
            AL_EFFECTIVE,
            AL_USER
        }

        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string fileExtension = textBox1.Text.Trim().ToLower();
            string appPath = textBox2.Text.Trim();
            string appName = System.IO.Path.GetFileNameWithoutExtension(appPath);

            if (!fileExtension.StartsWith("."))
            {
                fileExtension = "." + fileExtension;
            }

            if (!System.IO.File.Exists(appPath))
            {
                MessageBox.Show($"The specified application path '{appPath}' does not exist.");
                return;
            }

            using (RegistryKey key = Registry.ClassesRoot.OpenSubKey(fileExtension, true))
            {
                if (key != null)
                {
                    string progId = key.GetValue(null) != null ? key.GetValue(null).ToString() : null;
                    if (progId != null)
                    {
                        using (RegistryKey progIdKey = Registry.ClassesRoot.OpenSubKey(progId, true))
                        {
                            if (progIdKey != null)
                            {
                                progIdKey.SetValue("DefaultIcon", "");
                                progIdKey.SetValue("", $"{appName}.{fileExtension}");
                                using (RegistryKey shellKey = progIdKey.CreateSubKey("shell"))
                                {
                                    if (shellKey != null)
                                    {
                                        using (RegistryKey openKey = shellKey.CreateSubKey("open"))
                                        {
                                            if (openKey != null)
                                            {
                                                using (RegistryKey commandKey = openKey.CreateSubKey("command"))
                                                {
                                                    if (commandKey != null)
                                                    {
                                                        commandKey.SetValue("", $"\"{appPath}\" \"%1\"");
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            MessageBox.Show($"The default app for {fileExtension} has been set to {appPath}");
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
