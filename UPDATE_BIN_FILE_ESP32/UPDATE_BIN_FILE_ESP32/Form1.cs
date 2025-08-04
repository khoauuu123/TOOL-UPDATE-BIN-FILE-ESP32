using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace UPDATE_BIN_FILE_ESP32
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            try
            {
                comboBox_com.DataSource = SerialPort.GetPortNames();
            }
            catch (Exception ex)
            {
                // Hiển thị thông báo lỗi trong MessageBox
                MessageBox.Show("Có lỗi xảy ra: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                comboBox_com.DataSource = SerialPort.GetPortNames();
            }
            catch (Exception ex)
            {
                // Hiển thị thông báo lỗi trong MessageBox
                MessageBox.Show("Có lỗi xảy ra: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                OpenFileDialog openFileDialog = new OpenFileDialog
                {
                    Filter = "Firmware File (*.bin)|*.bin",
                    Title = "Chọn file firmware"
                };

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    tb_HexFilePath.Text = openFileDialog.FileName;
                }
            }
            catch (Exception ex)
            {
                // Hiển thị thông báo lỗi trong MessageBox
                MessageBox.Show("Có lỗi xảy ra: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            try
            {
                string binFilePath = tb_HexFilePath.Text;
                string comPort = comboBox_com.Text;

                if (string.IsNullOrEmpty(binFilePath) || string.IsNullOrEmpty(comPort))
                {
                    MessageBox.Show("Please select a HEX file and COM port.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Hộp thoại xác nhận từ người dùng
                DialogResult result = MessageBox.Show(
                    "Bạn có chắc chắn muốn cập nhật firmware không?",
                    "Xác nhận",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    // Bắt đầu quá trình upload
                    UploadFirmware(comPort, binFilePath);
                }
            }
            catch (Exception ex)
            {
                // Hiển thị thông báo lỗi trong MessageBox
                MessageBox.Show("Có lỗi xảy ra: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void UploadFirmware(string comPort, string firmwarePath)
        {
            string pythonPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "esptool.exe");
            string bootAppPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "boot_app0.bin");

            string arguments;

            arguments = $"--chip esp32 --port {comPort} --baud 921600  --before default_reset --after hard_reset write_flash  -z --flash_mode dio --flash_freq 80m --flash_size 4MB 0x10000 \"{firmwarePath}\" 0xe000 \"{bootAppPath}\"";

            Process process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = pythonPath,
                    Arguments = arguments,
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    CreateNoWindow = true
                }
            };

            process.Start();

            string output = process.StandardOutput.ReadToEnd();
            string error = process.StandardError.ReadToEnd();
            process.WaitForExit();

            if (process.ExitCode == 0)
            {
                tb_terminal.Text = "Upload successful:\n" + output;
            }
            else
            {
                tb_terminal.Text = "Upload failed:\n" + error;
            }
        }
    }
}
