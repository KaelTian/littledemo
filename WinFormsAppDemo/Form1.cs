using System.Windows.Forms;

namespace WinFormsAppDemo
{
    public partial class Form1 : AntdUI.Window
    {
        public Form1()
        {
            InitializeComponent();

            //// 创建一个带 Label 的 TextBox 组合控件
            //TextBox textBox = new TextBox();
            //LabeledControl labeledTextBox = new LabeledControl(textBox, "状态：");
            //labeledTextBox.Location = new Point(10, 10);
            //this.Controls.Add(labeledTextBox);

            //// 创建一个带 Label 的 CheckBox 组合控件
            //CheckBox checkBox = new CheckBox();
            //LabeledControl labeledCheckBox = new LabeledControl(checkBox, "启用：");
            //labeledCheckBox.Location = new Point(10, 50);
            //this.Controls.Add(labeledCheckBox);

            uploadDragger.DragChanged += (sender, e) =>
            {
                string[] files = e.Value;
                AntdUI.Modal.open(this, "上传文件", files);
            };
            uploadDragger.Click += (sender, e) => 
            {
            };
        }
    }
}
