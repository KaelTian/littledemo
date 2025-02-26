using Newtonsoft.Json;
using Formatting = Newtonsoft.Json.Formatting;

namespace WinFormsAppDemo
{
    public partial class SplitControl : UserControl
    {
        public SplitControl()
        {
            InitializeComponent();
            splitContainer1.SplitterDistance = (int)(splitContainer1.Width * 0.35);
            splitContainer2.SplitterDistance = (int)(splitContainer1.Height * 0.7);
            uploadDragger1.Height = (int)(splitContainer2.Panel2.Height * 0.8);
            //tag1.Visible=false;

            // JSON 格式化内容的 TextBox
            RichTextBox textBoxJson = new RichTextBox
            {
                Dock = DockStyle.Fill,
                Multiline = true,
                ScrollBars = RichTextBoxScrollBars.Both,
                ReadOnly = true,
                WordWrap = false
            };

            splitContainer2.Panel1.Controls.Add(textBoxJson);
            uploadDragger1.DragChanged += (sender, e) =>
            {
                string[] files = e.Value;
                if (files == null || files.Length == 0) return;
                DisplayJsonInTextBox(files[0], textBoxJson);
            };
        }


        private void DisplayJsonInTextBox(string filePath, RichTextBox textBox)
        {
            try
            {
                string jsonContent = File.ReadAllText(filePath);
                string formattedJson = JsonConvert.SerializeObject(
                    JsonConvert.DeserializeObject(jsonContent),
                    Formatting.Indented
                );
                textBox.Text = formattedJson;
                tag1.Text = filePath;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"无法加载 JSON 文件: {ex.Message}");
            }
        }
    }
}
