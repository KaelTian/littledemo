using System.Windows.Forms;

namespace WinFormsAppDemo
{
    public partial class Form1 : AntdUI.Window
    {
        public Form1()
        {
            InitializeComponent();

            //// ����һ���� Label �� TextBox ��Ͽؼ�
            //TextBox textBox = new TextBox();
            //LabeledControl labeledTextBox = new LabeledControl(textBox, "״̬��");
            //labeledTextBox.Location = new Point(10, 10);
            //this.Controls.Add(labeledTextBox);

            //// ����һ���� Label �� CheckBox ��Ͽؼ�
            //CheckBox checkBox = new CheckBox();
            //LabeledControl labeledCheckBox = new LabeledControl(checkBox, "���ã�");
            //labeledCheckBox.Location = new Point(10, 50);
            //this.Controls.Add(labeledCheckBox);

            uploadDragger.DragChanged += (sender, e) =>
            {
                string[] files = e.Value;
                AntdUI.Modal.open(this, "�ϴ��ļ�", files);
            };
            uploadDragger.Click += (sender, e) => 
            {
            };
        }
    }
}
