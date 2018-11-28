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
    public partial class Station1 : Form
    {
        public bool isWriteEvent;
        public Station1()
        {
            InitializeComponent();
        }

        public void Station1WriteEvent(object sender, MouseEventArgs e) //событие отправки сообщения станцией с адресом 1
        {
            this.Invoke((MethodInvoker)(delegate { isWriteEvent = true; }));
        }

        private void Station1_Load(object sender, EventArgs e)
        {
            isWriteEvent = false;
            button1.MouseClick += Station1WriteEvent;
            textBox3.ScrollBars = ScrollBars.Both;
            textBox2.ScrollBars = ScrollBars.Both;
        }
    }
}
