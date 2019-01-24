using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace WorkWithXML
{
    public partial class Form2 : Form
    {
        
        public Form2()
        {
            InitializeComponent();           
        }

        public string ToolButton { get; private set; }

        //создание формы с атрибутами
        public Form2(List<string> AttributeNode)
        {
            Name = AttributeNode[1];
            Width = Convert.ToInt32(AttributeNode[2]);
            Height = Convert.ToInt32(AttributeNode[3]);
            MinimumSize = new Size(Convert.ToInt32(AttributeNode[4]), Convert.ToInt32(AttributeNode[5]));
            ToolButton = AttributeNode[6];
            this.Icon = new Icon(@"../../Icon/point_aletter_ai_7142.ico");                       
        }
        //так как формы при закрытии удаляются(не полностью),а они созданы динамически проще их просто скрывать
        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            if (e.CloseReason.ToString() == "UserClosing")
            {
                e.Cancel = true;
                Hide();
            }
        }

    }


}
