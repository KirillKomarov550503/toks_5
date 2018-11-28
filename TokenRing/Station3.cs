using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TokenRing
{
    public partial class Station3 : Form
    {
        public Station3()
        {
            InitializeComponent();
        }
        public bool isWriteEvent;
        public bool isCreateToken;
        public void Station3WriteEvent(object sender, MouseEventArgs e) //событие отправки сообщения станцией с адресом 1
        {
            this.Invoke((MethodInvoker)(delegate { isWriteEvent = true; }));
        }

        public void CreateTokenEvent(object sender, MouseEventArgs e) //событие отправки сообщения станцией с адресом 1
        {
            this.Invoke((MethodInvoker)(delegate { Console.WriteLine("CreateTokenEvent"); isCreateToken = true; }));
        }

        private void Station3_Load(object sender, EventArgs e)
        {
            isWriteEvent = false;
            isCreateToken = false;
            button3.MouseClick += Station3WriteEvent;
            button4.MouseClick += CreateTokenEvent;
            textBox4.ScrollBars = ScrollBars.Both;
            textBox5.ScrollBars = ScrollBars.Both;
        }
    }
}
