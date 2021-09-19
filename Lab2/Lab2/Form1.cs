using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Lab2
{
    public partial class Form1 : Form
    {

        private Assembly assembly;
        public Dictionary<string, Type> component = new Dictionary<string, Type>();

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }


        //Получение пути к сборке через OpenFileDialog
        private string selectAssemblyFile()
        {
            openFileDialog1.Filter = "Dll files (*.dll)|*.dll|Exe files (*.exe)|*.exe|All files(*.*) |*.*";
            openFileDialog1.Title = "Select assembly file";
            return (openFileDialog1.ShowDialog() == DialogResult.OK) ?
            openFileDialog1.FileName : null;
        }

        //Загрузка сборки
        private Assembly openAssembly(string path)
        {
           try
            {
                Assembly a = Assembly.LoadFrom(path);
                component.Clear();
                return a;
            }
            catch (Exception)
            {
                MessageBox.Show("Не удалось загрузить указанную сборку!",
                "Ошибка!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }
        }

        //Добавить все классы и интерфейсы сборки к узлу дерева
        void addRoot(TreeNode root, Type[] types)
        {
            TreeNode node = null;
            foreach (Type type in types)
            {
                node = new TreeNode();
                node.Text = type.ToString();
                //Если класс
                if (type.IsClass)
                {
                    node.ImageIndex = 0;
                    node.SelectedImageIndex = 0;
                    addFirstLevel(node, type);
                    root.Nodes.Add(node);
                }
                //Если интерфейс
                else if (type.IsInterface)
                {
                    node.ImageIndex = 1;
                    node.SelectedImageIndex = 1;
                    addFirstLevel(node, type);
                    root.Nodes.Add(node);
                }
                //Если перечисление
                else if (type.IsEnum)
                {
                    node.ImageIndex = 2;
                    node.SelectedImageIndex = 2;
                    addFirstLevel(node, type);
                    root.Nodes.Add(node);
                }
                //Если структура
                else if (type.IsValueType && !type.IsPrimitive)
                {
                    node.ImageIndex = 3;
                    node.SelectedImageIndex = 3;
                    addFirstLevel(node, type);
                    root.Nodes.Add(node);
                }
                try
                {
                    component.Add(type.ToString(), type);
                }
                catch { }
            }
        }

        //Загрузить все поля, конструкторы и методы
        private void addFirstLevel(TreeNode node, Type type)
        {
            TreeNode node1;

            FieldInfo[] fields = type.GetFields();
            FieldInfo[] privateFields = type.GetFields(BindingFlags.NonPublic | BindingFlags.Instance);
            MethodInfo[] methods = type.GetMethods();
            ConstructorInfo[] constructors = type.GetConstructors();

            //Загрузить поля
            foreach (FieldInfo field in fields)
            {
                node1 = new TreeNode();
                node1.Text = field.FieldType.Name + " " + field.Name;
                node1.ImageIndex = 6;
                node1.SelectedImageIndex = 6;
                node.Nodes.Add(node1);
                try
                {
                    component.Add(type.ToString()+ "publicfield" + node1.Text, type);
                }
                catch { }
            }

            //Загрузить не публичные поля
            foreach (FieldInfo field in privateFields)
            {
                node1 = new TreeNode();
                node1.Text = field.FieldType.Name + " " + field.Name;
                node1.ImageIndex = 7;
                node1.SelectedImageIndex = 7;
                node.Nodes.Add(node1);

                component.Add(type.ToString() + "privatefield" + node1.Text, type);
            }

            //Загрузить конструкторы
            foreach (ConstructorInfo constructor in constructors)
            {
                String s = "";
                ParameterInfo[] parametrs = constructor.GetParameters();
                foreach (ParameterInfo parametr in parametrs)
                {
                    s = s + parametr.ParameterType.Name + ", ";
                }
                s = s.Trim();
                s = s.TrimEnd(',');
                node1 = new TreeNode();
                node1.Text = node.Text + "(" + s + ")";
                node1.ImageIndex = 4;
                node1.SelectedImageIndex = 4;
                node.Nodes.Add(node1);
                try
                {
                    component.Add(type.ToString() + "constructor" + node1.Text, type);
                } catch { }
            }

            //Загрузить методы
            foreach (MethodInfo method in methods)
            {
                String s = "";
                ParameterInfo[] parametrs = method.GetParameters();
                foreach (ParameterInfo parametr in parametrs)
                {
                    s = s + parametr.ParameterType.Name + ", ";
                }
                s = s.Trim();
                s = s.TrimEnd(',');
                node1 = new TreeNode();
                node1.Text = method.ReturnType.Name + " " + method.Name + "("
                + s + ")";
                node1.ImageIndex = 5;
                node1.SelectedImageIndex = 5;
                node.Nodes.Add(node1);
                try
                {
                    component.Add(type.ToString() + "method" + node1.Text, type);
                } catch {}
            }
        }

        private void відкритиСбіркуToolStripMenuItem_Click(object sender, EventArgs e)
        {
            treeView1.Nodes.Clear();
            string path = selectAssemblyFile();
            if (path != null)
            {
                assembly = openAssembly(path);
            }
            if (assembly != null)
            {
                TreeNode root = new TreeNode();
                root.Text = assembly.GetName().Name;
                root.ImageIndex = 8;
                root.SelectedImageIndex = 8;
                treeView1.Nodes.Add(root);
                Type[] types = assembly.GetTypes();
                addRoot(root, types);
            }

        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void splitContainer1_Panel2_Paint(object sender, PaintEventArgs e)
        {

        }

        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            listView1.Clear();
            ListViewItem lvi = new ListViewItem();
            ListViewItem lvi2 = new ListViewItem();
            ListViewItem lvi3 = new ListViewItem();
            ListViewItem lvi4 = new ListViewItem();
            ListViewItem lvi5 = new ListViewItem();
            ListViewItem lvi6 = new ListViewItem();
            ListViewItem lvi7 = new ListViewItem();

            FieldInfo[] fields;
            FieldInfo[] privateFields;
            MethodInfo[] methods;
            ConstructorInfo[] constructors;
            if (e.Node.Parent == null)
                return;
            if (component.TryGetValue(e.Node.Text, out var comp))          
            {
                fields = comp.GetFields();
                privateFields = comp.GetFields(BindingFlags.NonPublic | BindingFlags.Instance);
                methods = comp.GetMethods();
                constructors = comp.GetConstructors();

                if (comp.IsPublic)
                {
                    lvi.Text = "Public ";
                }
                if (comp.IsClass)
                {
                    lvi.Text += "Class " + comp.Name;
                }
                else if (comp.IsInterface)
                {
                    lvi.Text += "Interface " + comp.Name;
                }
                else if (comp.IsEnum)
                {
                    lvi.Text += "Enum " + comp.Name;
                }
                else if (comp.IsValueType && !comp.IsPrimitive)
                {
                    lvi.Text += "Structure " + comp.Name;
                }
                
                if(comp.Namespace != null)
                    lvi2.Text = "Namespace: " + comp.Namespace;
                else
                    lvi2.Text = "Namespace: -";
                if (comp.BaseType != null)
                    lvi3.Text = "Base class: " + comp.BaseType.ToString();
                else
                    lvi3.Text = "Base class: -";

                String s = null;
                foreach (FieldInfo field in fields)
                {
                    s += field.Name + ", ";
                }
                
                if(s != null)
                {
                    s = s.Remove(s.Length - 2, 2);
                    lvi4.Text = "Public fields: " + s;
                } else
                {
                    lvi4.Text = "Public fields: -";
                }

                s = null;
                foreach (FieldInfo field in privateFields)
                {
                    s += field.Name + ", ";
                }

                if (s != null)
                {
                    s = s.Remove(s.Length - 2, 2);
                    lvi5.Text = "Private fields: " + s;
                }
                else
                {
                    lvi5.Text = "Private fields: -";
                }

                s = null; 
                foreach (ConstructorInfo constructor in constructors)
                {
                    s += comp.Name + ", ";
                }
                if (s != null)
                {
                    s = s.Remove(s.Length - 2, 2);
                    lvi6.Text = "Constructors: " + s;
                }
                else
                {
                    lvi6.Text = "Constructors: -";
                }

                s = null;
                foreach (MethodInfo method in methods)
                {
                    s += method.Name + "(), ";
                }
                if (s != null)
                {
                    s = s.Remove(s.Length - 2, 2);
                    lvi7.Text = "Methods: " + s;
                }
                else
                {
                    lvi7.Text = "Methods: -";
                }

                listView1.Items.Add(lvi);  
                listView1.Items.Add(lvi2);
                listView1.Items.Add(lvi3);
                listView1.Items.Add(lvi4);
                listView1.Items.Add(lvi5);
                listView1.Items.Add(lvi6);
                listView1.Items.Add(lvi7);
            }
            else if (component.TryGetValue(e.Node.Parent.Text + "publicfield" + e.Node.Text, out var comppubf))
            {
                fields = comppubf.GetFields();
                foreach (FieldInfo field in fields)
                {
                    if(e.Node.Text.Equals(field.FieldType.Name + " " + field.Name))
                        lvi.Text = "public " + field.FieldType.Name + " " + field.Name;
                }     
                listView1.Items.Add(lvi);
            }
            else if (component.TryGetValue(e.Node.Parent.Text + "privatefield" + e.Node.Text, out var compprf))
            {
                privateFields = compprf.GetFields(BindingFlags.NonPublic | BindingFlags.Instance);
                foreach (FieldInfo field in privateFields)
                {
                    if (e.Node.Text.Equals(field.FieldType.Name + " " + field.Name))
                        lvi.Text = "private " + field.FieldType.Name + " " + field.Name;
                }
                listView1.Items.Add(lvi);
            }
            else if (component.TryGetValue(e.Node.Parent.Text + "constructor" + e.Node.Text, out var compcon))
            {
                constructors = compcon.GetConstructors();
                String s = "";
                lvi.Text = "Constructor: " + compcon.Name;
                foreach (ConstructorInfo constructor in constructors)
                {
                    ParameterInfo[] parametrs = constructor.GetParameters();
                    foreach (ParameterInfo parametr in parametrs)
                    {
                        s += parametr.ParameterType.Name + ", ";
                    }
                    if (s.Length > 0 && s.Contains(", "))
                    {
                        s = s.Remove(s.Length - 2, 2);
                    }
                    if ((compcon.FullName + "(" + s + ")").Equals(e.Node.Text))
                    {
                        if (s == "")
                        {
                            s = "Parameters: -";
                            lvi2.Text = s;
                        }
                        else
                        {
                            lvi2.Text = "Parameters: " + s;
                        }
                    }
                    s = "";
                } 
                listView1.Items.Add(lvi);
                listView1.Items.Add(lvi2);
            }
            else if (component.TryGetValue(e.Node.Parent.Text + "method" + e.Node.Text, out var compmet))
            {
                methods = compmet.GetMethods();
                String s = "";
                foreach (MethodInfo method in methods)
                {
                    ParameterInfo[] parametrs = method.GetParameters();
                    foreach (ParameterInfo parametr in parametrs)
                    {
                        s = s + parametr.ParameterType.Name + ", ";
                    }
                    if (s.Length > 0 && s.Contains(", "))
                    {
                        s = s.Remove(s.Length - 2, 2);
                    }
                    if ((method.ReturnType.Name + " " + method.Name + "(" + s + ")").Equals(e.Node.Text))
                    {
                        lvi.Text = "Method: " + method.Name;
                        if (s == "")
                        {
                            s = "Parameters: -";
                            lvi2.Text = s;
                        }
                        else
                        {
                            lvi2.Text = "Parameters: " + s;
                        }
                        lvi3.Text = "Return type: " + method.ReturnType.Name;
                    }
                    s = "";
                }
                listView1.Items.Add(lvi);
                listView1.Items.Add(lvi2);
                listView1.Items.Add(lvi3);
            }
        }
    }
}
