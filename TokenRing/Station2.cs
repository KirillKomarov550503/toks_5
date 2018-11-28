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
    public partial class Station2 : Form
    {
        public Station2()
        {
            InitializeComponent();
        }
        public bool isWriteEvent;

        public void Station2WriteEvent(object sender, MouseEventArgs e) //событие отправки сообщения станцией с адресом 1
        {
            this.Invoke((MethodInvoker)(delegate { isWriteEvent = true; }));
        }
       

        private void Station2_Load(object sender, EventArgs e)
        {
            isWriteEvent = false;
            button2.MouseClick += Station2WriteEvent;
            textBox10.ScrollBars = ScrollBars.Both;
            textBox11.ScrollBars = ScrollBars.Both;
        }
    }
}
