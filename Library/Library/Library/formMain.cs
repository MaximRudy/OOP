using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Reflection;
using System.IO;
using PluginInterface;

namespace Library
{
    public partial class formMain : Form
    {
        Data data = new Data();

        public formMain()
        {
            InitializeComponent();
            comboboxObject.Items.AddRange(data.objectTypes.Select(t => t.GetCustomAttribute<DescriptionAttribute>(true).Description).ToArray());
        }

        private void ButtonAdd_Click(object sender, EventArgs e)
        {
            Type type = null;
            try
            {
                type = data.objectTypes.First(t => t.GetCustomAttribute<DescriptionAttribute>(true).Description == comboboxObject.Text);
            }
            catch
            {
                MessageBox.Show("Incorrect data!");
                return;
            }
            Form formAddObject = new ObjectForm(type, FormType.Add, "formAddObject", data, listviewObject);
            formAddObject.Show();
        }

        private void ButtonEdit_Click(object sender, EventArgs e)
        {
            if (listviewObject.SelectedItems.Count == 0)
            {
                MessageBox.Show("Select object!");
                return;
            }
            Type type = data.objectTypes.First(t => t.GetCustomAttribute<DescriptionAttribute>(true).Description == listviewObject.SelectedItems[0].SubItems[1].Text);
            Form formEditObject = new ObjectForm(type, FormType.Edit, "formEditObject", data, listviewObject);
            formEditObject.Show();
        }

        private void ButtonRemove_Click(object sender, EventArgs e)
        {
            if (listviewObject.SelectedItems.Count == 0)
            {
                MessageBox.Show("Select object!");
                return;
            }
            data.objects.Remove(data.objects.Where(t => t.GetType().GetCustomAttribute<DescriptionAttribute>(true).Description == listviewObject.SelectedItems[0].SubItems[1].Text).First(o => o.GetId() == listviewObject.SelectedItems[0].SubItems[0].Text));
            data.FillListView(listviewObject);
        }

        private void ButtonSave_Click(object sender, EventArgs e)
        {
            Form formSave = new SaveForm(data);
            formSave.Show();
        }

        private void ButtonLoad_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialogLoad = new OpenFileDialog();
            if (openFileDialogLoad.ShowDialog() == DialogResult.Cancel)
                return;
            string fileName = openFileDialogLoad.FileName;
            string fileExtention = fileName.Split('.')[fileName.Split('.').Length - 1];
            if (fileName.Split('.').Length == 3)
            {
                IPlugin plugin = data.plugins.First(p => p.GetExt() == fileExtention);
                using (Stream inputStream = plugin.Dearchive(fileName))
                {
                    fileName = fileName.Remove(fileName.LastIndexOf('.')).Substring(fileName.LastIndexOf('\\') + 1);
                    fileExtention = fileName.Split('.')[fileName.Split('.').Length - 1];
                    data.objects.Clear();
                    IBaseSerializer serializer = data.serializers.First(s => s.GetExtention() == fileExtention);
                    data.objects = (List<IBaseObject>)serializer.Deserialize(inputStream);
                }
            }
            else if (fileName.Split('.').Length == 2)
            {
                IBaseSerializer serializer = data.serializers.First(s => s.GetExtention() == fileExtention);
                if (serializer == null)
                {
                    MessageBox.Show("Unknown extention");
                    return;
                }
                data.objects.Clear();
                using (Stream inputStream = new FileStream(fileName, FileMode.Open))
                {
                    data.objects = (List<IBaseObject>)serializer.Deserialize(inputStream);
                }
            }
            data.FillListView(listviewObject);
            MessageBox.Show("Data has successfully loaded!");
        }

        private void ButtonQuit_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
