using System.ComponentModel;

namespace WinFormsAppDemo
{
    public partial class MainForm : AntdUI.Window
    {
        private DynamicForm? form;
        private DataGridView? dataGridView;
        private List<Dictionary<string, object>>? records;

        public MainForm()
        {
            InitializeComponent();
            //InitializeLayout();
            //LoadData();
        }

        private void InitializeLayout()
        {
            // 初始化 DataGridView
            dataGridView = new DataGridView
            {
                Dock = DockStyle.Left,
                Width = this.Width / 2,
                AutoGenerateColumns = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect
            };
            //dataGridView.Columns.Add(new DataGridViewTextBoxColumn { Name = "FirstName", HeaderText = "First Name" });
            //dataGridView.Columns.Add(new DataGridViewTextBoxColumn { Name = "LastName", HeaderText = "Last Name" });
            //dataGridView.Columns.Add(new DataGridViewTextBoxColumn { Name = "Email", HeaderText = "Email" });
            //dataGridView.Columns.Add(new DataGridViewTextBoxColumn { Name = "StartDate", HeaderText = "Start Date" });
            dataGridView.SelectionChanged += DataGridView_SelectionChanged;
            // 自动生成列
            dataGridView.AutoGenerateColumns = true;
            dataGridView.AllowUserToAddRows = false; // 禁止用户添加新行（已有空记录代替）
            this.Controls.Add(dataGridView);

            // 初始化动态表单
            form = new DynamicForm
            {
                Dock = DockStyle.Right,
                Width = (this.Width / 2) - 50,
                Margin = new Padding(30, 0, 0, 0)
            };
            form.OnSave = SaveRecord;
            this.Controls.Add(form);
        }

        private void LoadData()
        {
            if (records == null)
            {
                // 加载记录
                records = new List<Dictionary<string, object>>
                    {
                        new Dictionary<string, object> { { "recid", 1 }, { "FirstName", "John" }, { "LastName", "Doe" }, { "Email", "jdoe@gmail.com" }, { "StartDate", "4/3/2012" } },
                        new Dictionary<string, object> { { "recid", 2 }, { "FirstName", "Jane" }, { "LastName", "Smith" }, { "Email", "jsmith@gmail.com" }, { "StartDate", "4/3/2012" } },
                    };
            }
            // 添加一个空行
            records.Add(new Dictionary<string, object> { { "recid", 0 }, { "FirstName", "" }, { "LastName", "" }, { "Email", "" }, { "StartDate", "" } });

            // 将 records 转换为匿名对象的列表并绑定到 DataGridView
            dataGridView.DataSource = records.ConvertAll(r => new
            {
                FirstName = r.ContainsKey("FirstName") ? r["FirstName"].ToString() : string.Empty,
                LastName = r.ContainsKey("LastName") ? r["LastName"].ToString() : string.Empty,
                Email = r.ContainsKey("Email") ? r["Email"].ToString() : string.Empty,
                StartDate = r.ContainsKey("StartDate") ? r["StartDate"].ToString() : string.Empty
            });



        }

        private void DataGridView_SelectionChanged(object sender, EventArgs e)
        {
            if (dataGridView.SelectedRows.Count == 1)
            {
                int index = dataGridView.SelectedRows[0].Index;
                form.LoadRecord(records[index]);
            }
            else
            {
                form.Clear();
            }
        }

        private void SaveRecord(Dictionary<string, object> record)
        {
            int index = records.FindIndex(r => r["recid"].Equals(record["recid"]));
            if (index >= 0)
            {
                records[index] = record;
                LoadData();
            }
            else
            {
                record["recid"] = records.Count + 1;
                records.Add(record);
                LoadData();
            }
        }
    }

    public class DynamicForm : Panel
    {
        public Action<Dictionary<string, object>> OnSave;
        private Dictionary<string, TextBox> fields;
        private Dictionary<string, object> currentRecord;

        public DynamicForm()
        {
            fields = new Dictionary<string, TextBox>();
            this.AutoScroll = true;
            InitializeForm();
        }

        private void InitializeForm()
        {
            // 定义表单的字段配置
            var formFields = new[]
            {
                new { Name = "FirstName", Label = "First Name", Type = "text" },
                new { Name = "LastName", Label = "Last Name", Type = "text" },
                new { Name = "Email", Label = "Email", Type = "email" },
                new { Name = "StartDate", Label = "Start Date", Type = "date" }
            };

            int y = 10;
            foreach (var field in formFields)
            {
                Label label = new Label { Text = field.Label, Top = y, Left = 10, Width = 100 };
                this.Controls.Add(label);

                TextBox textBox = new TextBox { Top = y, Left = 120, Width = 150 };
                fields[field.Name] = textBox;
                this.Controls.Add(textBox);
                y += 30;
            }

            Button saveButton = new Button { Text = "Save", Top = y, Left = 120, Width = 80, Height = 30 };
            saveButton.Click += (s, e) => Save();
            this.Controls.Add(saveButton);
        }

        public void LoadRecord(Dictionary<string, object> record)
        {
            currentRecord = record;
            foreach (var field in fields.Keys)
            {
                fields[field].Text = record.ContainsKey(field) ? record[field]?.ToString() : string.Empty;
            }
        }

        public void Clear()
        {
            currentRecord = null;
            foreach (var field in fields.Values)
            {
                field.Text = string.Empty;
            }
        }

        private void Save()
        {
            var newRecord = new Dictionary<string, object>();
            if (currentRecord != null && currentRecord.ContainsKey("recid"))
            {
                newRecord["recid"] = currentRecord["recid"];
            }

            foreach (var field in fields)
            {
                newRecord[field.Key] = field.Value.Text;
            }

            OnSave?.Invoke(newRecord);
            Clear();
        }
    }
}
