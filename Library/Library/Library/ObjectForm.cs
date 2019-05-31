using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Reflection;
using System.ComponentModel;
using System.Drawing;
using System.Collections;
using System.Threading;

namespace Library
{
    public enum FormType
    {
        Add,
        Edit
    }

    public class ObjectForm : Form
    {
        private Type ObjectType { get; set; }
        private Data Data { get; set; }
        private ListView ListView { get; set; }

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
        private readonly int checkedlistboxWidth = 225;
        private readonly int checkedlistboxHeight = 100;

        public ObjectForm(Type objectType, FormType formType, string formName, Data data, ListView listView)
        {
            ObjectType = objectType;
            Data = data;
            ListView = listView;

            Name = formName;
            MaximizeBox = false;
            StartPosition = FormStartPosition.CenterScreen;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            Width = formWidth;
            Height = formHeight;

            foreach (PropertyInfo property in objectType.GetProperties())
            {
                formHeight += paddingTop + labelHeight;
                Height += paddingTop + labelHeight;
                Label label = new Label();
                label.Height = labelHeight;
                label.Width = labelWidth;
                label.Text = (property.GetCustomAttribute(typeof(DescriptionAttribute)) as DescriptionAttribute).Description;
                label.Location = new Point(paddingLeft, formHeight - labelHeight);
                Controls.Add(label);

                if ((property.PropertyType == typeof(int)) || (property.PropertyType == typeof(float)) || (property.PropertyType == typeof(string)))
                {
                    formHeight += textboxHeight;
                    Height += textboxHeight;
                    TextBox textBox = new TextBox();
                    textBox.Name = property.Name;
                    textBox.Width = textboxWidth;
                    textBox.Height = textboxHeight;
                    textBox.Location = new Point(paddingLeft, formHeight - textboxHeight);
                    Controls.Add(textBox);
                }
                else if (property.PropertyType.IsGenericType)
                {
                    formHeight += checkedlistboxHeight;
                    Height += checkedlistboxHeight;
                    CheckedListBox checkedListBox = new CheckedListBox();
                    checkedListBox.Items.AddRange(Data.objects.Where(o => o.GetType() == property.PropertyType.GetGenericArguments()[0]).Concat(Data.objects.Where(o => o.GetType().IsSubclassOf(property.PropertyType.GetGenericArguments()[0]))).Select(o => o.GetId()).ToArray());
                    checkedListBox.Name = property.Name;
                    checkedListBox.Width = checkedlistboxWidth;
                    checkedListBox.Height = checkedlistboxHeight;
                    checkedListBox.Location = new Point(paddingLeft, formHeight - checkedlistboxHeight);
                    Controls.Add(checkedListBox);
                }
                else
                {
                    formHeight += comboboxHeight;
                    Height += comboboxHeight;
                    ComboBox comboBox = new ComboBox();
                    comboBox.Name = property.Name;
                    comboBox.Width = comboboxWidth;
                    comboBox.Height = comboboxHeight;
                    comboBox.Location = new Point(paddingLeft, formHeight - textboxHeight);
                    Controls.Add(comboBox);
                    if (property.PropertyType.IsEnum)
                    {
                        comboBox.Items.AddRange(property.PropertyType.GetEnumNames());
                    }
                    else
                    {
                        comboBox.Items.AddRange(Data.objects.Where(o => o.GetType() == property.PropertyType).Select(o => o.GetId()).Concat(Data.objects.Where(o => o.GetType().IsSubclassOf(property.PropertyType)).Select(o => o.GetId())).ToArray());
                    }
                }
            }

            formHeight += paddingTop + buttonHeight;
            Height += paddingTop + buttonHeight;
            Button buttonAction = new Button();
            buttonAction.Width = buttonWidth;
            buttonAction.Height = buttonHeight;
            buttonAction.Location = new Point(paddingLeft, formHeight - buttonHeight);
            switch (formType)
            {
                case FormType.Add:
                    buttonAction.Name = "buttonAddObject";
                    buttonAction.Text = "Add";
                    buttonAction.Click += ButtonAddObject_Click;
                    Text = "Add object";
                    break;
                case FormType.Edit:
                    buttonAction.Name = "buttonEditObject";
                    buttonAction.Text = "Edit";
                    buttonAction.Click += ButtonEditObject_Click;
                    Shown += FormEditObject_Shown;
                    Text = "Edit object";
                    break;
            }
            Controls.Add(buttonAction);

            Button buttonBack = new Button();
            buttonBack.Width = buttonWidth;
            buttonBack.Height = buttonHeight;
            buttonBack.Location = new Point(2 * paddingLeft + buttonWidth, formHeight - buttonHeight);
            buttonBack.Name = "buttonBackObject";
            buttonBack.Text = "Back";
            buttonBack.Click += ButtonBackObject_Click;
            Controls.Add(buttonBack);

            Height += paddingTop;
        }

        private void FormEditObject_Shown(object sender, EventArgs e)
        {
            IBaseObject @object = Data.objects.Where(obj => obj.GetType() == ObjectType).First(obj => obj.GetId() == ListView.SelectedItems[0].SubItems[0].Text);
            foreach (Control control in Controls)
            {
                PropertyInfo property = ObjectType.GetProperty(control.Name);
                if (property == null)
                    continue;
                if ((control.GetType() == typeof(TextBox)) || (control.GetType() == typeof(ComboBox)))
                {
                    if (property.PropertyType.GetInterface(typeof(IBaseObject).ToString()) != null)
                        control.Text = (property.GetValue(@object) as IBaseObject).GetId();
                    else
                        control.Text = property.GetValue(@object).ToString();
                }
                else if (control.GetType() == typeof(CheckedListBox))
                {
                    for (int i = 0; i < (control as CheckedListBox).Items.Count; i++)
                        foreach (IBaseObject element in (property.GetValue(@object) as IEnumerable<IBaseObject>).Concat((property.GetValue(@object) as IEnumerable<IBaseObject>).Where(o => o.GetType().IsSubclassOf(property.GetValue(@object).GetType().GetGenericArguments()[0]))))
                            if (element.GetId() == (control as CheckedListBox).Items[i].ToString())
                                (control as CheckedListBox).SetItemChecked(i, true);
                }
            }
        }

        private void ButtonAddObject_Click(object sender, EventArgs e)
        {
            IBaseObject @object = (IBaseObject)Activator.CreateInstance(ObjectType);
            foreach (Control control in Controls)
            {
                if (!InitializeObject(ref @object, control))
                    return;
            }
            Data.objects.Add(@object);
            MessageBox.Show((@object.GetType().GetCustomAttribute(typeof(DescriptionAttribute)) as DescriptionAttribute).Description + " has successfully added!");
            Data.FillListView(ListView);
            Dispose();
        }

        private void ButtonEditObject_Click(object sender, EventArgs e)
        {
            IBaseObject @object = Data.objects.Where(obj => obj.GetType() == ObjectType).First(obj => obj.GetId() == ListView.SelectedItems[0].SubItems[0].Text);
            foreach (Control control in Controls)
            {
                if (!InitializeObject(ref @object, control))
                    return;
            }
            MessageBox.Show((@object.GetType().GetCustomAttribute(typeof(DescriptionAttribute)) as DescriptionAttribute).Description + " has successfully edited!");
            Data.FillListView(ListView);
            Dispose();
        }

        private void ButtonBackObject_Click(object sender, EventArgs e)
        {
            Dispose();
        }

        private bool InitializeObject(ref IBaseObject @object, Control control)
        {
            PropertyInfo property = ObjectType.GetProperty(control.Name);
            if (property == null)
                return true;
            try
            {
                if (control.GetType() == typeof(TextBox))
                {
                    if (property.PropertyType == typeof(string))
                        property.SetValue(@object, (control as TextBox).Text);
                    else if (property.PropertyType == typeof(int))
                        property.SetValue(@object, int.Parse((control as TextBox).Text));
                    else if (property.PropertyType == typeof(float))
                        property.SetValue(@object, float.Parse((control as TextBox).Text));
                }
                else if (control.GetType() == typeof(ComboBox))
                {
                    if (property.PropertyType.IsEnum)
                        property.SetValue(@object, Enum.Parse(property.PropertyType, (control as ComboBox).Text));
                    else
                        property.SetValue(@object, Data.objects.Where(o => o.GetType() == property.PropertyType).First(o => o.GetId() == (control as ComboBox).Text));
                }
                else if (control.GetType() == typeof(CheckedListBox))
                {
                    IList result = Activator.CreateInstance(property.PropertyType) as IList;
                    foreach (var item in (control as CheckedListBox).CheckedItems)
                        foreach (IBaseObject baseObject in Data.objects.Where(o => o.GetType() == property.PropertyType.GetGenericArguments()[0]).Concat(Data.objects.Where(o => o.GetType().IsSubclassOf(property.PropertyType.GetGenericArguments()[0]))))
                            if (item.ToString() == baseObject.GetId())
                                result.Add(baseObject);
                    property.SetValue(@object, result);
                }
            }
            catch
            {
                MessageBox.Show("Incorrect data!");
                return false;
            }
            return true;
        }
    }
}
