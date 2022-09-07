using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO.Ports;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Rebar;
using System.Runtime.Remoting.Contexts;

namespace canbusgui
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            //Uart baudrate 921600 de sabit
            serialPort1.BaudRate = 921600;
            Control.CheckForIllegalCrossThreadCalls = false;
        }

        char[] cantxbuf = new char[21];
        char[] txbuf = new char[21];
        string rx;

        int i = 0;
        int j = 0;
        int verieklendimi = 0;

        private void Form1_Load(object sender, EventArgs e)
        {
            //Serial port bağlantı
            string[] ports = SerialPort.GetPortNames();
            comportcombo.Items.AddRange(ports);

            if(comportcombo.Items.Count > 0)
            {
                comportcombo.SelectedIndex = 0;
            }

            disconnectbutton.Enabled = false;
            sendcountbox.Text = "0";
            receivecountbox.Text = "0";

          
        }
        private void connectbutton_Click(object sender, EventArgs e)
        {
            canbaudratecombo.Enabled = false;
            connectbutton.Enabled = false;   
            disconnectbutton.Enabled=true;
            

            try
            {
                
                serialPort1.PortName = comportcombo.SelectedItem.ToString();
                serialPort1.Open();
                
                connectstatus.Text = "Connected";
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message,"MessageConnect",MessageBoxButtons.OK,MessageBoxIcon.Error);
            }


            //İlk olarak canbus baudrate değerini gönderiyoruz.

            string baud = canbaudratecombo.Text;
            char[] baudpart = baud.ToCharArray();

            switch (baud.Length)
            {
                case 2:
                    cantxbuf[0] = '0';
                    cantxbuf[1] = '0';
                    cantxbuf[2] = '0';
                    cantxbuf[3] = baudpart[0];
                    break;
                case 3:
                    cantxbuf[0] = '0';
                    cantxbuf[1] = '0';
                    cantxbuf[2] = baudpart[0];
                    cantxbuf[3] = baudpart[1];
                    break;
                case 4:
                    cantxbuf[0] = '0';
                    cantxbuf[1] = baudpart[0];
                    cantxbuf[2] = baudpart[1];
                    cantxbuf[3] = baudpart[2];
                    break;
                case 5:
                    cantxbuf[0] = baudpart[0];
                    cantxbuf[1] = baudpart[1];
                    cantxbuf[2] = baudpart[2];
                    cantxbuf[3] = baudpart[3];
                    break;
            }

            cantxbuf[4] = '*';

            serialPort1.Write(cantxbuf, 0, 21);
        }

        private void disconnectbutton_Click(object sender, EventArgs e)
        {
            canbaudratecombo.Enabled = true;
            connectbutton.Enabled = true;
            disconnectbutton.Enabled = false;
           

            try
            {
                serialPort1.Close();
                            
                connectstatus.Text = "Disconnected";
                
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void rtrcombo_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(rtrcombo.SelectedIndex == 1)
            {
                dlccombo.SelectedIndex = 0;
                dlccombo.Enabled = false;      
                d1box.Clear();
                d2box.Clear();
                d3box.Clear();
                d4box.Clear();
                d5box.Clear();
                d6box.Clear();
                d7box.Clear();
                d8box.Clear();
            }
            else
            {
                dlccombo.Enabled=true;
            }

        }

        private void dlccombo_SelectedIndexChanged(object sender, EventArgs e)
        {
            d1box.Clear();
            d2box.Clear();
            d3box.Clear();
            d4box.Clear();
            d5box.Clear();
            d6box.Clear();
            d7box.Clear();
            d8box.Clear();

            switch (dlccombo.SelectedIndex)
            {
                case 0:
                    d1box.Enabled = false;
                    d2box.Enabled = false;
                    d3box.Enabled = false;
                    d4box.Enabled = false;
                    d5box.Enabled = false;
                    d6box.Enabled = false;
                    d7box.Enabled = false;
                    d8box.Enabled = false;
                    break;
                case 1:
                    d1box.Enabled = true;
                    d2box.Enabled = false;
                    d3box.Enabled = false;
                    d4box.Enabled = false;
                    d5box.Enabled = false;
                    d6box.Enabled = false;
                    d7box.Enabled = false;
                    d8box.Enabled = false;
                    break;
                case 2:
                    d1box.Enabled = true;
                    d2box.Enabled = true;
                    d3box.Enabled = false;
                    d4box.Enabled = false;
                    d5box.Enabled = false;
                    d6box.Enabled = false;
                    d7box.Enabled = false;
                    d8box.Enabled = false;
                    break;
                case 3:
                    d1box.Enabled = true;
                    d2box.Enabled = true;
                    d3box.Enabled = true;
                    d4box.Enabled = false;
                    d5box.Enabled = false;
                    d6box.Enabled = false;
                    d7box.Enabled = false;
                    d8box.Enabled = false;
                    break;
                case 4:
                    d1box.Enabled = true;
                    d2box.Enabled = true;
                    d3box.Enabled = true;
                    d4box.Enabled = true;
                    d5box.Enabled = false;
                    d6box.Enabled = false;
                    d7box.Enabled = false;
                    d8box.Enabled = false;
                    break;
                case 5:
                    d1box.Enabled = true;
                    d2box.Enabled = true;
                    d3box.Enabled = true;
                    d4box.Enabled = true;
                    d5box.Enabled = true;
                    d6box.Enabled = false;
                    d7box.Enabled = false;
                    d8box.Enabled = false;
                    break;
                case 6:
                    d1box.Enabled = true;
                    d2box.Enabled = true;
                    d3box.Enabled = true;
                    d4box.Enabled = true;
                    d5box.Enabled = true;
                    d6box.Enabled = true;
                    d7box.Enabled = false;
                    d8box.Enabled = false;
                    break;
                case 7:
                    d1box.Enabled = true;
                    d2box.Enabled = true;
                    d3box.Enabled = true;
                    d4box.Enabled = true;
                    d5box.Enabled = true;
                    d6box.Enabled = true;
                    d7box.Enabled = true;
                    d8box.Enabled = false;
                    break;
                case 8:
                    d1box.Enabled = true;
                    d2box.Enabled = true;
                    d3box.Enabled = true;
                    d4box.Enabled = true;
                    d5box.Enabled = true;
                    d6box.Enabled = true;
                    d7box.Enabled = true;
                    d8box.Enabled = true;
                    break;

            }
        }
        private void addbutton_Click(object sender, EventArgs e)
        {
            if (rtrcombo.SelectedIndex == -1)
                rtrcombo.SelectedIndex = 0;
            if (dlccombo.SelectedIndex == -1)
                dlccombo.SelectedIndex = 0;

            if (idbox.Text == "")
            {
               MessageBox.Show("ID giriniz");
            }

            if (idbox.Text.Length > 3)
            {
                MessageBox.Show("Maksimum 3 değer giriniz.");
            }                   

            if(d1box.Text.Length > 2 || d2box.Text.Length > 2 || d3box.Text.Length > 2 || d4box.Text.Length > 2 || d5box.Text.Length > 2 || d6box.Text.Length > 2 || d7box.Text.Length > 2 || d8box.Text.Length > 2)
            {
                MessageBox.Show("Maksimum 2 değer giriniz.");
            }

            if (idbox.Text != "" && idbox.Text.Length <= 3)
            {
                senddatagrid.Rows.Add(idbox.Text, rtrcombo.Text, dlccombo.SelectedIndex, d1box.Text, d2box.Text, d3box.Text, d4box.Text, d5box.Text, d6box.Text, d7box.Text, d8box.Text);
                verieklendimi = 1;
            }

        }
        //göndereceğimiz değerleri textbox ve comboboxlardan alarak gönderdiğimiz için datagride eklenen değerlerden göndermek istediğimizi seçtiğimizde boxlara yazması lazım ki seçtiğimiz değeri gönderelim.
        //aşağıdaki cellclick bloğu da bu fonksiyon için yazıldı.
        private void senddatagrid_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            idbox.Text = senddatagrid.CurrentRow.Cells[0].Value.ToString();
            rtrcombo.Text = senddatagrid.CurrentRow.Cells[1].Value.ToString();
            dlccombo.Text = senddatagrid.CurrentRow.Cells[2].Value.ToString();
            d1box.Text = senddatagrid.CurrentRow.Cells[3].Value.ToString();
            d2box.Text = senddatagrid.CurrentRow.Cells[4].Value.ToString();
            d3box.Text = senddatagrid.CurrentRow.Cells[5].Value.ToString();
            d4box.Text = senddatagrid.CurrentRow.Cells[6].Value.ToString();
            d5box.Text = senddatagrid.CurrentRow.Cells[7].Value.ToString();
            d6box.Text = senddatagrid.CurrentRow.Cells[8].Value.ToString();
            d7box.Text = senddatagrid.CurrentRow.Cells[9].Value.ToString();
            d8box.Text = senddatagrid.CurrentRow.Cells[10].Value.ToString();
        }
        private void sendbutton_Click(object sender, EventArgs e)
        {
            if (verieklendimi == 1 && canbaudratecombo.Text !="")
            {
                try
                {
                    if (serialPort1.IsOpen)
                    {                        

                        if (idbox.Text != "")
                        {
                            //İkinci olarak id değerini göndermeliyiz.Id değeri string bir ifade olduğu için chara uyarlıyoruz.

                            if (idbox.Text.Length == 1)
                            {
                                string id = idbox.Text;
                                char idpart = char.Parse(id);
                                txbuf[0] = '0';
                                txbuf[1] = '0';
                                txbuf[2] = idpart;
                            }

                            else if(idbox.Text.Length == 2)
                            {
                                string id = idbox.Text;
                                char[] idpart = id.ToCharArray();
                                txbuf[0] = '0';
                                txbuf[1] = idpart[0];
                                txbuf[2] = idpart[1];
                            }

                            else if(idbox.Text.Length == 3)
                            {
                                string id = idbox.Text;
                                char[] idpart = id.ToCharArray();
                                txbuf[0] = idpart[0];
                                txbuf[1] = idpart[1];
                                txbuf[2] = idpart[2];
                            }
                                                                                   
                        }

                        //Üçüncü olarak RTR değerini gönderiyoruz.RTR=0->No , RTR=1->Yes

                        txbuf[3] = (char)rtrcombo.SelectedIndex;

                        //Dördüncü olarak DLC değerini gönderiyoruz.

                        txbuf[4] = (char)dlccombo.SelectedIndex;

                        //Beşinci olarak D1 aktise gönderiyoruz.

                        if (d1box.Text != "")
                        {
                            if (d1box.Text.Length == 1)
                            {                           
                                string d1 = d1box.Text;
                                char d1part = char.Parse(d1);
                                txbuf[5] = '0';
                                txbuf[6] = d1part;
                            }

                            else if(d1box.Text.Length == 2)
                            {
                                string d1 = d1box.Text;
                                char[] d1part = d1.ToCharArray();
                                txbuf[5] = d1part[0];
                                txbuf[6] = d1part[1];

                            }

                        }

                        //Altıncı olarak D2 aktifse gönderiyoruz.

                        if (d2box.Text != "")
                        {
                            if (d2box.Text.Length == 1)
                            {
                                string d2 = d2box.Text;
                                char d2part = char.Parse(d2);
                                txbuf[7] = '0';
                                txbuf[8] = d2part;
                            }

                            else if (d2box.Text.Length == 2)
                            {
                                string d2 = d2box.Text;
                                char[] d2part = d2.ToCharArray();
                                txbuf[7] = d2part[0];
                                txbuf[8] = d2part[1];

                            }
                      
                        }

                        //Yedinci olarak D3 aktifse gönderiyoruz.

                        if (d3box.Text != "")
                        {
                            if (d3box.Text.Length == 1)
                            {
                                string d3 = d3box.Text;
                                char d3part = char.Parse(d3);
                                txbuf[9] = '0';
                                txbuf[10] = d3part;
                            }

                            else if (d3box.Text.Length == 2)
                            {
                                string d3 = d3box.Text;
                                char[] d3part = d3.ToCharArray();
                                txbuf[9] = d3part[0];
                                txbuf[10] = d3part[1];

                            }

                        }

                        //Sekizinci olarak D4 aktifse gönderiyoruz.

                        if (d4box.Text != "")
                        {
                            if (d4box.Text.Length == 1)
                            {
                                string d4 = d4box.Text;
                                char d4part = char.Parse(d4);
                                txbuf[11] = '0';
                                txbuf[12] = d4part;
                            }

                            else if (d4box.Text.Length == 2)
                            {
                                string d4 = d4box.Text;
                                char[] d4part = d4.ToCharArray();
                                txbuf[11] = d4part[0];
                                txbuf[12] = d4part[1];

                            }
     
                        }

                        //Dokuzuncu olarak D5 aktifse gönderiyoruz.

                        if (d5box.Text != "")
                        {
                            if (d5box.Text.Length == 1)
                            {
                                string d5 = d5box.Text;
                                char d5part = char.Parse(d5);
                                txbuf[13] = '0';
                                txbuf[14] = d5part;
                            }

                            else if (d5box.Text.Length == 2)
                            {
                                string d5 = d5box.Text;
                                char[] d5part = d5.ToCharArray();
                                txbuf[13] = d5part[0];
                                txbuf[14] = d5part[1];

                            }
     
                        }

                        //Onuncu olarak D6 aktifse gönderiyoruz.

                        if (d6box.Text != "")
                        {
                            if (d6box.Text.Length == 1)
                            {
                                string d6 = d6box.Text;
                                char d6part = char.Parse(d6);
                                txbuf[15] = '0';
                                txbuf[16] = d6part;
                            }

                            else if (d6box.Text.Length == 2)
                            {
                                string d6 = d6box.Text;
                                char[] d6part = d6.ToCharArray();
                                txbuf[15] = d6part[0];
                                txbuf[16] = d6part[1];

                            }

                        }

                        //On birinci olarak D7 aktifse gönderiyoruz.

                        if (d7box.Text != "")
                        {
                            if (d7box.Text.Length == 1)
                            {
                                string d7 = d7box.Text;
                                char d7part = char.Parse(d7);
                                txbuf[17] = '0';
                                txbuf[18] = d7part;
                            }

                            else if (d7box.Text.Length == 2)
                            {
                                string d7 = d7box.Text;
                                char[] d7part = d7.ToCharArray();
                                txbuf[17] = d7part[0];
                                txbuf[18] = d7part[1];

                            }

                        }

                        //On ikinci olarak D8 aktifse gönderiyoruz.

                        if (d8box.Text != "")
                        {
                            if (d8box.Text.Length == 1)
                            {
                                string d8 = d8box.Text;
                                char d8part = char.Parse(d8);
                                txbuf[19] = '0';
                                txbuf[20] = d8part;
                            }

                            else if (d8box.Text.Length == 2)
                            {
                                string d8 = d8box.Text;
                                char[] d8part = d8.ToCharArray();
                                txbuf[19] = d8part[0];
                                txbuf[20] = d8part[1];

                            }
                            else
                            {
                                MessageBox.Show("Maksimum 2 değer giriniz.");
                            }
                        }

                        serialPort1.Write(txbuf, 0, 21);

                        i++;

                        sendcountbox.Text = i.ToString();

                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "SendMessage", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else if(canbaudratecombo.Text == "")
            {
                MessageBox.Show("CAN için baudrate seçimi yapınız");
            }
            else
            {
                MessageBox.Show("Add butonuna basarak seçimi ekleyiniz");
            }
        }
        private void deletebutton_Click(object sender, EventArgs e)
        {
            try
            {
                int selectedIndex = senddatagrid.CurrentCell.RowIndex;
                if (selectedIndex > -1)
                {
                    senddatagrid.Rows.RemoveAt(selectedIndex);
                    senddatagrid.Refresh();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Silmek istediğiniz satırı seçiniz.");
            }
        }
        private void clearcountbutton_Click(object sender, EventArgs e)
        {
            sendcountbox.Text = "0";
            i = 0;
        }
        private void pausebutton_Click(object sender, EventArgs e)
        {
            serialPort1.Close();
            serialPort1.DataReceived -= new SerialDataReceivedEventHandler(serialPort1_DataReceived);
        }
        private void receivebutton_Click(object sender, EventArgs e)
        {
            serialPort1.Open();
            serialPort1.DataReceived += new SerialDataReceivedEventHandler(serialPort1_DataReceived);

        }

        private void serialPort1_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {

            try
            {
                rx = serialPort1.ReadExisting();
                //rx = serialPort1.ReadLine();
                char[] rxbuf = rx.ToCharArray();

                //id
                char[] id = { rxbuf[0], rxbuf[1], rxbuf[2] };
                string idful = new string(id);

                //rtr->rxbuf[3]
                //dlc->rxbuf[4]

                switch(rxbuf[4])
                {
                    case '0':
                        rxbuf[5] = ' ';
                        rxbuf[6] = ' ';
                        rxbuf[7] = ' ';
                        rxbuf[8] = ' ';
                        rxbuf[9] = ' ';
                        rxbuf[10] = ' ';
                        rxbuf[11] = ' ';
                        rxbuf[12] = ' ';
                        rxbuf[13] = ' ';
                        rxbuf[14] = ' ';
                        rxbuf[15] = ' ';
                        rxbuf[16] = ' ';
                        rxbuf[17] = ' ';
                        rxbuf[18] = ' ';
                        rxbuf[19] = ' ';
                        rxbuf[20] = ' ';
                        break;
                    case '1':
                        rxbuf[7] = ' ';
                        rxbuf[8] = ' ';
                        rxbuf[9] = ' ';
                        rxbuf[10] = ' ';
                        rxbuf[11] = ' ';
                        rxbuf[12] = ' ';
                        rxbuf[13] = ' ';
                        rxbuf[14] = ' ';
                        rxbuf[15] = ' ';
                        rxbuf[16] = ' ';
                        rxbuf[17] = ' ';
                        rxbuf[18] = ' ';
                        rxbuf[19] = ' ';
                        rxbuf[20] = ' ';
                        break;
                    case '2':
                        rxbuf[9] = ' ';
                        rxbuf[10] = ' ';
                        rxbuf[11] = ' ';
                        rxbuf[12] = ' ';
                        rxbuf[13] = ' ';
                        rxbuf[14] = ' ';
                        rxbuf[15] = ' ';
                        rxbuf[16] = ' ';
                        rxbuf[17] = ' ';
                        rxbuf[18] = ' ';
                        rxbuf[19] = ' ';
                        rxbuf[20] = ' ';
                        break;
                    case '3':
                        rxbuf[11] = ' ';
                        rxbuf[12] = ' ';
                        rxbuf[13] = ' ';
                        rxbuf[14] = ' ';
                        rxbuf[15] = ' ';
                        rxbuf[16] = ' ';
                        rxbuf[17] = ' ';
                        rxbuf[18] = ' ';
                        rxbuf[19] = ' ';
                        rxbuf[20] = ' ';
                        break ;
                    case '4':
                        rxbuf[13] = ' ';
                        rxbuf[14] = ' ';
                        rxbuf[15] = ' ';
                        rxbuf[16] = ' ';
                        rxbuf[17] = ' ';
                        rxbuf[18] = ' ';
                        rxbuf[19] = ' ';
                        rxbuf[20] = ' ';
                        break;
                    case '5':
                        rxbuf[15] = ' ';
                        rxbuf[16] = ' ';
                        rxbuf[17] = ' ';
                        rxbuf[18] = ' ';
                        rxbuf[19] = ' ';
                        rxbuf[20] = ' ';
                        break;
                    case '6':
                        rxbuf[17] = ' ';
                        rxbuf[18] = ' ';
                        rxbuf[19] = ' ';
                        rxbuf[20] = ' ';
                        break;
                    case '7':
                        rxbuf[19] = ' ';
                        rxbuf[20] = ' ';
                        break;

                }

                //d1
                char[] d1r = { rxbuf[5], rxbuf[6] };
                string d1ful = new string(d1r);

                //d2
                char[] d2r = { rxbuf[7], rxbuf[8] };
                string d2ful = new string(d2r);

                //d3
                char[] d3r = { rxbuf[9], rxbuf[10] };
                string d3ful = new string(d3r);

                //d4
                char[] d4r = { rxbuf[11], rxbuf[12] };
                string d4ful = new string(d4r);

                //d5
                char[] d5r = { rxbuf[13], rxbuf[14] };
                string d5ful = new string(d5r);

                //d6
                char[] d6r = { rxbuf[15], rxbuf[16] };
                string d6ful = new string(d6r);

                //d7
                char[] d7r = { rxbuf[17], rxbuf[18] };
                string d7ful = new string(d7r);

                //d8
                char[] d8r = { rxbuf[19], rxbuf[20] };
                string d8ful = new string(d8r);

                receivedatagrid.Rows.Add(idful, rxbuf[4], rxbuf[5], d1ful, d2ful, d3ful, d4ful, d5ful,d6ful,d7ful,d8ful);
                j++;
                receivecountbox.Text = j.ToString();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "MessageDataReceived", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        private void clearbutton_Click(object sender, EventArgs e)
        {
            receivedatagrid.Rows.Clear();
            receivecountbox.Text = "0";
            j = 0;

        }
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (serialPort1.IsOpen)
            {
                serialPort1.Close();
                
            }
            serialPort1.DataReceived -= new SerialDataReceivedEventHandler(serialPort1_DataReceived);
        }


    }
}
