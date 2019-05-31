using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;
using System.ComponentModel;
using System.Reflection;
using System.IO;
using PluginInterface;

namespace Library
{
    public class SaveForm : Form
    {
        private Data Data { get; set; }

        private readonly string formName = "formSaveObject";
        private readonly int formWidth = 250;
        private readonly int formHeight = 0;
        private readonly int paddingTop = 5;
        private readonly int paddingLeft = 5;
        private readonly int labelHeight = 13;
        private readonly int labelWidth = 240;
        private readonly int textboxWidth = 225;
        private readonly int textboxHeight = 20;
        private readonly int comboboxWidth = 225;
        private readonly int comboboxHeight = 20;
        private readonly int buttonWidth = 110;
        private readonly int buttonHeight = 40;

        public SaveForm(Data data)
        {
            Data = data;

            Name = formName;
            Text = "Save data";
            MaximizeBox = false;
            StartPosition = FormStartPosition.CenterScreen;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            Width = formWidth;
            Height = formHeight;

            formHeight += paddingTop + labelHeight;
            Height += paddingTop + labelHeight;
            Label labelFileName = new Label();
            labelFileName.Height = labelHeight;
            labelFileName.Width = labelWidth;
            labelFileName.Location = new Point(paddingLeft, formHeight - labelHeight);
            labelFileName.Text = "File name";
            Controls.Add(labelFileName);

            formHeight += textboxHeight;
            Height += textboxHeight;
            TextBox textboxFileName = new TextBox();
            textboxFileName.Name = "textboxFileName";
            textboxFileName.Width = textboxWidth;
            textboxFileName.Height = textboxHeight;
            textboxFileName.Location = new Point(paddingLeft, formHeight - textboxHeight);
            Controls.Add(textboxFileName);

            formHeight += paddingTop + labelHeight;
            Height += paddingTop + labelHeight;
            Label labelSerializerName = new Label();
            labelSerializerName.Height = labelHeight;
            labelSerializerName.Width = labelWidth;
            labelSerializerName.Location = new Point(paddingLeft, formHeight - labelHeight);
            labelSerializerName.Text = "Serializer name";
            Controls.Add(labelSerializerName);

            formHeight += comboboxHeight;
            Height += comboboxHeight;
            ComboBox comboboxSerializer = new ComboBox();
            comboboxSerializer.Name = "comboboxSerializer";
            comboboxSerializer.Width = comboboxWidth;
            comboboxSerializer.Height = comboboxHeight;
            comboboxSerializer.Location = new Point(paddingLeft, formHeight - textboxHeight);
            comboboxSerializer.Items.AddRange(Data.serializerTypes.Select(t => (t.GetCustomAttribute(typeof(DescriptionAttribute)) as DescriptionAttribute).Description).ToArray());
            Controls.Add(comboboxSerializer);

            formHeight += paddingTop + labelHeight;
            Height += paddingTop + labelHeight;
            Label labelPluginName = new Label();
            labelPluginName.Height = labelHeight;
            labelPluginName.Width = labelWidth;
            labelPluginName.Location = new Point(paddingLeft, formHeight - labelHeight);
            labelPluginName.Text = "Plugin";
            Controls.Add(labelPluginName);

            formHeight += comboboxHeight;
            Height += comboboxHeight;
            ComboBox comboboxPlugin = new ComboBox();
            comboboxPlugin.Name = "comboboxPlugin";
            comboboxPlugin.Width = comboboxWidth;
            comboboxPlugin.Height = comboboxHeight;
            comboboxPlugin.Location = new Point(paddingLeft, formHeight - textboxHeight);
            comboboxPlugin.Items.AddRange(Data.pluginTypes.Select(t => (t.GetCustomAttribute(typeof(DescriptionAttribute)) as DescriptionAttribute).Description).ToArray());
            Controls.Add(comboboxPlugin);

            formHeight += paddingTop + buttonHeight;
            Height += paddingTop + buttonHeight;
            Button buttonSave = new Button();
            buttonSave.Width = buttonWidth;
            buttonSave.Height = buttonHeight;
            buttonSave.Location = new Point(paddingLeft, formHeight - buttonHeight);
            buttonSave.Name = "buttonSave";
            buttonSave.Text = "Save";
            buttonSave.Click += ButtonSave_Click;
            Controls.Add(buttonSave);

            Button buttonBack = new Button();
            buttonBack.Width = buttonWidth;
            buttonBack.Height = buttonHeight;
            buttonBack.Location = new Point(2 * paddingLeft + buttonWidth, formHeight - buttonHeight);
            buttonBack.Name = "buttonBack";
            buttonBack.Text = "Back";
            buttonBack.Click += ButtonBack_Click;
            Controls.Add(buttonBack);

            Height += paddingTop;
        }

        private void ButtonSave_Click(object sender, EventArgs e)
        {
            Control textboxFileName = Controls.Find("textboxFileName", true)[0];
            Control comboboxSerializer = Controls.Find("comboboxSerializer", true)[0];
            Control comboboxPlugin = Controls.Find("comboboxPlugin", true)[0];
            if ((textboxFileName.Text.Length == 0) || (comboboxSerializer.Text.Length == 0))
            {
                MessageBox.Show("Incorrect data!");
                return;
            }
            Type serializerType = Data.serializerTypes.First(t => (t.GetCustomAttribute(typeof(DescriptionAttribute)) as DescriptionAttribute).Description == comboboxSerializer.Text);
            IBaseSerializer serializer = Data.serializers.First(s => s.GetType() == serializerType);
            string fileName = textboxFileName.Text + "." + serializer.GetExtention();
            using (Stream outputStream = new MemoryStream())
            {
                serializer.Serialize(outputStream, Data.objects);
                if (comboboxPlugin.Text.Length == 0)
                    using (FileStream fileStream = new FileStream(fileName, FileMode.OpenOrCreate, FileAccess.Write))
                    {
                        outputStream.Seek(0, SeekOrigin.Begin);
                        byte[] bytes = new byte[outputStream.Length];
                        outputStream.Read(bytes, 0, (int)outputStream.Length);
                        fileStream.Write(bytes, 0, bytes.Length);
                    }
                else
                {
                    Type pluginType = Data.pluginTypes.First(t => (t.GetCustomAttribute(typeof(DescriptionAttribute)) as DescriptionAttribute).Description == comboboxPlugin.Text);
                    IPlugin plugin = Data.plugins.First(p => p.GetType() == pluginType);
                    fileName += "." + plugin.GetExt();
                    plugin.Archive(fileName, outputStream);
                }
            }
            MessageBox.Show("Data has successfully saved!");
            Dispose();
        }

        private void ButtonBack_Click(object sender, EventArgs e)
        {
            Dispose();
        }
    }
}
