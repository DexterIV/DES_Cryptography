using System;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;

namespace Cryptography.TripleDes
{
    /// <summary>
    ///     Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void DecryptButtonOnClick(object sender, RoutedEventArgs e)
        {
            var des = new DesEncryption();
            var res = des.Decrypt(Input.Text, Key1.Text);

            Output.Text = res;
        }

        private void EncryptButtonOnClick(object sender, RoutedEventArgs e)
        {
            var des = new DesEncryption();
            var res = des.Encrypt(Input.Text, Key1.Text);

            Output.Text = res;
        }

        private void ButtonToHexOnClick(object sender, RoutedEventArgs e)
        {
            Input.Text =  string.Join("",Encoding.ASCII.GetBytes(Input.Text).Select(c => c.ToString("X2")));
        }

        private void UtfButtonOnClick(object sender, RoutedEventArgs e)
        {

            try
            {
                Output.Text = Regex.Replace(new string(Encoding.ASCII.GetChars(FromHex(Output.Text))).Trim(), @"[^\u0020-\u007E]", string.Empty);
            }
            catch (Exception)
            {
            }

        }

        public static byte[] FromHex(string hex)
        {
            hex = hex.Replace("-", "");
            byte[] raw = new byte[hex.Length / 2];
            for (int i = 0; i < raw.Length; i++)
            {
                raw[i] = Convert.ToByte(hex.Substring(i * 2, 2), 16);
            }
            return raw;
        }
    }
}