namespace WinFormsAppDemo
{
    public partial class LabeledControl : UserControl
    {
        private Label descriptionLabel;
        private Control inputControl;

        public LabeledControl(Control control, string descriptionText)
        {
            InitializeComponent();
            // 设置 Label
            descriptionLabel = new Label();
            descriptionLabel.Text = descriptionText;
            descriptionLabel.AutoSize = true;
            descriptionLabel.Location = new Point(0, 0);

            // 动态调整 Label 的宽度
            using (Graphics g = CreateGraphics())
            {
                SizeF textSize = g.MeasureString(descriptionLabel.Text, descriptionLabel.Font);
                descriptionLabel.Width = (int)textSize.Width + 5; // 额外加边距
            }

            // 设置传入的 Control
            inputControl = control;
            inputControl.Location = new Point(descriptionLabel.Width + 10, 0); // 放置在 Label 旁边，增加间距

            // 调整控件的大小
            this.Size = new Size(descriptionLabel.Width + inputControl.Width + 10, Math.Max(descriptionLabel.Height, inputControl.Height));
            this.Controls.Add(descriptionLabel);
            this.Controls.Add(inputControl);
        }
    }
}
