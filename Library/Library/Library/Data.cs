using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.Windows.Forms;
using System.ComponentModel;
using PluginInterface;
using System.IO;

namespace Library
{
    public class Data
    {
        public List<Type> objectTypes;
        public List<IBaseObject> objects = new List<IBaseObject>();
        public List<Type> serializerTypes;
        public List<IBaseSerializer> serializers = new List<IBaseSerializer>();
        public List<Type> pluginTypes = new List<Type>();
        public List<IPlugin> plugins = new List<IPlugin>();
        private readonly string pluginPath = Path.Combine(Directory.GetCurrentDirectory(), "Plugins");

        public Data()
        {
            objectTypes = Assembly.GetExecutingAssembly().GetTypes().Where(t => !t.IsAbstract).Where(t => t.GetInterface(typeof(IBaseObject).ToString()) != null).ToList<Type>();
            serializerTypes = Assembly.GetExecutingAssembly().GetTypes().Where(t => !t.IsAbstract).Where(t => t.GetInterface(typeof(IBaseSerializer).ToString()) != null).ToList<Type>();
            serializers.Add(new BinarySerializer());
            serializers.Add(new JsonSerializer());
            serializers.Add(new MaximSerializer());
            RefreshPlugins();
        }

        public void FillListView(ListView listView)
        {
            listView.Items.Clear();
            foreach (IBaseObject @object in objects)
            {
                ListViewItem item = new ListViewItem(@object.GetId());
                item.SubItems.Add((@object.GetType().GetCustomAttribute(typeof(DescriptionAttribute)) as DescriptionAttribute).Description);
                listView.Items.Add(item);
            }
        }

        private void RefreshPlugins()
        {
            DirectoryInfo pluginDirectory = new DirectoryInfo(pluginPath);
            if (!pluginDirectory.Exists)
            {
                pluginDirectory.Create();
                return;
            }
            string[] pluginFiles = Directory.GetFiles(pluginPath, "*.dll");
            foreach (string file in pluginFiles)
            {
                Assembly asm = Assembly.LoadFrom(file);
                var types = asm.GetTypes().Where(t => t.GetInterfaces().Where(i => i.FullName == typeof(IPlugin).FullName).Any());
                foreach (var type in types)
                {
                    var plugin = asm.CreateInstance(type.FullName) as IPlugin;
                    pluginTypes.Add(type);
                    plugins.Add(plugin);
                }
            }
        }
    }
}
