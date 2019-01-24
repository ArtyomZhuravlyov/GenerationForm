using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Xml;

namespace WorkWithXML
{

    public partial class Form1 : Form
    {
        public static int counterForms = 0;
        List<Form2> childForms = new List<Form2>(3);  //создание списка форм 
        List<Dblclick> Dblclicks = new List<Dblclick>(1); //создание списка узлов  Dblclick
        List<Label> labels = new List<Label>();
        List<Button> buttons = new List<Button>();
        List<TextBox> textBoxes = new List<TextBox>();
        List<ToolStripMenuItem> button = new List<ToolStripMenuItem>(); //кнопки на MenuStrip
        TextBox currentTextbox  = new TextBox(); 

        class Dblclick
        {
            public string[] Items { get; private set; }
            public string Name { get; private set; }

            public Dblclick(List<string> attrib)
            {
                Name = attrib[1];
                Items = attrib[2].Split(new char[] { ',' });
            }
        }

        //наш ТextBox имеет дополнительное свойство
        class TextBox1 : TextBox
        {
            public string DoubleClick1 { get; private set; } 

            public TextBox1(string doubleClick)
            {
                DoubleClick1 = doubleClick;
            }
        }

        //наша кнопка имеет дополнительное свойство
        class Button1 : Button
        {
            public string Form { get; private set; }

            public Button1(string form)
            {
                Form = form;
            }

        }

        //Добавляем кнопки ToolStripMenu 
        void AddToolStrip(List<Form2> Dateform)
        {
            button.Add(new ToolStripMenuItem());
            button[Dateform.Count - 1].Name = Dateform[Dateform.Count - 1].Name;
            button[Dateform.Count - 1].Text = Dateform[Dateform.Count - 1].Name;
            button[Dateform.Count - 1].Click += btn_Click;
            menuStrip1.Items.Add(button[Dateform.Count - 1]);
        }


        public void ReadXML( string xml)
        {

            XmlDocument xDoc = new XmlDocument();
            try
            {
                xDoc.Load(xml);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                Environment.Exit(1);
            }

            List<string> AttribNode = new List<string>(2);

            XmlElement xRoot = xDoc.DocumentElement;
            // обход всех узлов в корневом элементе
            foreach (XmlNode xnode in xRoot)
            {
 
                if (xnode.Attributes.Count > 0) //отбор атрибутов
                { 
                    if (xnode.Attributes.GetNamedItem("Code") != null)
                        AttribNode.Add(xnode.Attributes.GetNamedItem("Code").Value);

                    if (xnode.Attributes.GetNamedItem("Name") != null)
                        AttribNode.Add(xnode.Attributes.GetNamedItem("Name").Value);

                    if (xnode.Attributes.GetNamedItem("Items") != null && AttribNode.Count == 2)
                    {
                        AttribNode.Add(xnode.Attributes.GetNamedItem("Items").Value);
                        Dblclicks.Add(new Dblclick(AttribNode));
                        AttribNode.Clear();
                        continue;
                    }

                    if (xnode.Attributes.GetNamedItem("Width") != null)
                        AttribNode.Add(xnode.Attributes.GetNamedItem("Width").Value);

                    if (xnode.Attributes.GetNamedItem("Height") != null)
                        AttribNode.Add(xnode.Attributes.GetNamedItem("Height").Value);

                    if (xnode.Attributes.GetNamedItem("MinWidth") != null)
                        AttribNode.Add(xnode.Attributes.GetNamedItem("MinWidth").Value);

                    if (xnode.Attributes.GetNamedItem("MinHeight") != null)
                        AttribNode.Add(xnode.Attributes.GetNamedItem("MinHeight").Value);

                    if (xnode.Attributes.GetNamedItem("ToolButton") != null)
                        AttribNode.Add(xnode.Attributes.GetNamedItem("ToolButton")?.Value);
                    
                    if (AttribNode.Count == 7 && AttribNode[6] == "Y")
                    {
                        childForms.Add(new Form2(AttribNode)); //запись атрибутов узла и добавление формы
                        AddToolStrip(childForms); //запись кнопки на меню
                    }
                    else if (AttribNode.Count == 7)
                    {
                        childForms.Add(new Form2(AttribNode)); //создание формы без создания кнопки на меню
                    }
                   //тут хотел сказать, что если меньше 7 значит недост атрибутов
                   else
                    {            
                        MessageBox.Show("в XML найдены не все атрибуты узла: " + xnode.Name);
                    }
                    
                    AttribNode.Clear(); // чистим список атрибутов для след. формы
                }
                
                if (xnode.ChildNodes.Count > 0)
                {// обходим все дочерние узлы элемента 
                    foreach (XmlNode childnode in xnode.ChildNodes)
                    { 
                        // если узел - Label
                        if (childnode.Name == "Label")
                        {
                            labels.Add(new Label());
                            labels[labels.Count - 1].Name = childnode.Attributes.GetNamedItem("Name")?.Value;
                            labels[labels.Count - 1].Text = childnode.Attributes.GetNamedItem("Caption")?.Value;
                            labels[labels.Count - 1].Top = Convert.ToInt32(childnode.Attributes.GetNamedItem("Top")?.Value);
                            labels[labels.Count - 1].Left = Convert.ToInt32(childnode.Attributes.GetNamedItem("Left")?.Value);
                            labels[labels.Count - 1].Height = Convert.ToInt32(childnode.Attributes.GetNamedItem("Height")?.Value);
                            labels[labels.Count - 1].Width = Convert.ToInt32(childnode.Attributes.GetNamedItem("Width")?.Value);
                            childForms[childForms.Count - 1].Controls.Add(labels[labels.Count - 1]);
                            
                        }
                        // если узел Edit//textbox
                        if (childnode.Name == "Edit")
                        {
                            if (childnode.Attributes.GetNamedItem("DblClickItem") != null)
                            {
                                textBoxes.Add(new TextBox1(childnode.Attributes.GetNamedItem("DblClickItem").Value));
                                textBoxes[textBoxes.Count - 1].MouseDoubleClick += Control1_MouseDoubleClick;
                            }

                            else textBoxes.Add(new TextBox());

                            if (childnode.Attributes.GetNamedItem("ReadOnly")?.Value == "Y")
                                textBoxes[textBoxes.Count - 1].ReadOnly = true;

                            textBoxes[textBoxes.Count - 1].Name = childnode.Attributes.GetNamedItem("Name")?.Value;
                            textBoxes[textBoxes.Count - 1].Top = Convert.ToInt32(childnode.Attributes.GetNamedItem("Top")?.Value);
                            textBoxes[textBoxes.Count - 1].Left = Convert.ToInt32(childnode.Attributes.GetNamedItem("Left")?.Value);
                            textBoxes[textBoxes.Count - 1].Height = Convert.ToInt32(childnode.Attributes.GetNamedItem("Height")?.Value);
                            textBoxes[textBoxes.Count - 1].Width = Convert.ToInt32(childnode.Attributes.GetNamedItem("Width")?.Value);

                            childForms[childForms.Count - 1].Controls.Add(textBoxes[textBoxes.Count - 1]);
                        }
                        // если узел Button
                        if (childnode.Name == "Button")
                        {
                            //присвоение кнопке нужного метода по заданию
                            if (childnode.Attributes.GetNamedItem("Result")?.Value == "3")
                            {
                                buttons.Add(new Button1(childnode.Attributes.GetNamedItem("Form")?.Value));
                                buttons[buttons.Count - 1].Click += btn_Click_OpenForm;
                            }
                            else buttons.Add(new Button());

                            if (childnode.Attributes.GetNamedItem("Result")?.Value == "2")
                                buttons[buttons.Count - 1].Click += btn_Click_ClosedForm;//

                            if (childnode.Attributes.GetNamedItem("Result")?.Value == "1")
                                buttons[buttons.Count - 1].Click += message_OK;//
                            
                            buttons[buttons.Count - 1].Name = childnode.Attributes.GetNamedItem("Name")?.Value;
                            buttons[buttons.Count - 1].Text = childnode.Attributes.GetNamedItem("Caption")?.Value;
                            buttons[buttons.Count - 1].Height = Convert.ToInt32(childnode.Attributes.GetNamedItem("Height")?.Value);
                            buttons[buttons.Count - 1].Width = Convert.ToInt32(childnode.Attributes.GetNamedItem("Width")?.Value);
                            buttons[buttons.Count - 1].Top = Convert.ToInt32(childnode.Attributes.GetNamedItem("Top")?.Value);
                            buttons[buttons.Count - 1].Left = Convert.ToInt32(childnode.Attributes.GetNamedItem("Left")?.Value);

                            childForms[childForms.Count - 1].Controls.Add(buttons[buttons.Count - 1]);
                        }
                    }
                }

            }

        }

        
        public Form1()
        {
            InitializeComponent();
            ReadXML(@"../../XML_Files/Test.xml");
        }


        //кнопка ToolStripMenu имеет тоже name, что и форма
        //если name совпадает вызывается нужная форма
        void btn_Click(object sender, EventArgs e)
        {
            string name = ((ToolStripMenuItem)(sender)).Name;
            
            foreach (Form2 date in childForms) //перебор форм
            {
                if (name == date.Name) //отбор нужной формы по имени
                {                    
                    //проверка visible, чтобы не открывать её ещё раз
                    if (!date.Visible)
                    {                          
                       date.MdiParent = this;
                       date.Show();
                       //this.LayoutMdi(MdiLayout.TileHorizontal);
                    }                    
                }
            }
        }

        //возможность выбора Items при DoubleClick по textbox
        private void Control1_MouseDoubleClick(Object sender, MouseEventArgs e)
        {
            bool findName = false; //подтверждает, что нашёл нужную форму
            string dblClickEdit = ((TextBox1)(sender)).DoubleClick1;
            foreach(Dblclick Dblclick in Dblclicks) //перебор объектов класса Dblclick
            {
                if (Dblclick.Name == dblClickEdit)
                {
                    findName = true;
                    ComboBox box = new ComboBox();
                    box.Location = ((TextBox1)(sender)).Location;
                    box.Size = ((TextBox1)(sender)).Size;
                    box.Items.AddRange(Dblclick.Items);
                    ((TextBox1)(sender)).Parent.Controls.Add(box); //позволяет узнать форму, откуда была нажата кнопка
                    ((TextBox1)(sender)).Visible = false;
                    currentTextbox = ((TextBox1)(sender)); // ссылка на данный textbox, чтобы забрать значение из появившегося combobox
                    box.SelectedIndexChanged += comboBox_SelectedIndexChanged;
                }
            }
            if(findName == false) MessageBox.Show("элемент с Именем " + dblClickEdit + " не найден" );
        }

        //выбор во временном combobox переходит в textbox
        private void comboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (((ComboBox)(sender)).SelectedIndex > -1)
            {
                currentTextbox.Visible = true;
                currentTextbox.Text = ((ComboBox)(sender)).SelectedItem.ToString();
                ((ComboBox)(sender)).Visible = false;              
            }
        }

        /***********************  действия по кнопке ****************/
        private void message_OK(object sender, EventArgs e)
        {
            MessageBox.Show("OK");
        }

        void btn_Click_ClosedForm(object sender, EventArgs e)
        {
            ((Button)(sender)).Parent.Hide();
        }
        
        void btn_Click_OpenForm(object sender, EventArgs e)
        {
            bool findname = false;
            string name_Form = ((Button1)(sender)).Form;

            foreach (Form2 child in childForms)
            {
                if (name_Form == child.Name)
                {
                    findname = true;
                    child.StartPosition = FormStartPosition.CenterScreen;
                    child.ShowDialog();
                }                   
            }
            if (findname == false) MessageBox.Show("форма с Именем " + name_Form + " не найдена");
        }

    }
}
