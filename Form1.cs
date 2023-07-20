using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace hadanka
{
    public partial class SmileysPuzzleForm : Form
    {
        private string ms_src_image_path = "";  // cesta ke zdrojovému obrázku
        public SmileysPuzzleForm()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog theDialog = new OpenFileDialog();
            theDialog.Title = "Otevřít obrázek";
            theDialog.InitialDirectory = @"C:\";
            if (theDialog.ShowDialog() == DialogResult.OK)
            {
                this.ms_src_image_path = theDialog.FileName;
                // ořezání názvu souboru vstupního obrázku (vč. cesty) na 55 znaků, aby u dlouhé cesty Label "neopustil" dialog
                if (this.ms_src_image_path.Length > 55)
                    lbl_path.Text = "..." + this.ms_src_image_path.Substring(this.ms_src_image_path.Length - 58);
                else
                    lbl_path.Text = this.ms_src_image_path;

                // nahrání zdrojového obrázku do "ikony" vpravo dole
                Bitmap lo_bitmap = new Bitmap(this.ms_src_image_path);
                pictureBox10.Image = lo_bitmap;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            PUZZLE lo_puzzle = new PUZZLE();
            if (this.ms_src_image_path != "")
            {
                lo_puzzle.load_image(this.ms_src_image_path);

                // bude si sám mapovat obrázky ze zdrojového souboru nebo z ručního zadání ?
                if (radioButton1.Checked == true)
                {
                    lo_puzzle.init_map(); // ruční zadání
                }
                else
                {
                    lo_puzzle.init_map_from_image();  // analýza vstupního obrázku ze souboru
                }
                lo_puzzle.solve();  // funkce pro nalezení řešení, pokud existuje

                try
                {
                    Image lo_image = null;
                    int lv_j = 0;
                    int lv_i = 0;
                    // rotace hracích karet podle výpočtu a uložení do dialogu/okna
                    for (int j = 1; j < 10; j++)
                    {
                        lv_j = (int)(j-1) / 3;
                        lv_i = (j-1) % 3;
                        lo_image = lo_puzzle.ma_list_res[lv_j, lv_i].get_image();

                        if (lo_image != null)
                        {
                            for (int i = 0; i < lo_puzzle.ma_list_res[lv_j, lv_i].get_angle(); i++)
                            {
                                lo_image.RotateFlip(RotateFlipType.Rotate90FlipNone);
                            }
                            ((PictureBox)this.Controls.Find("pictureBox" + j, true)[0]).Image = lo_image;
                        }
                        else
                        {
                            MessageBox.Show("Nepodařilo se najít řešení.");
                            return;

                        }
                    }
                }
                catch (NullReferenceException err)
                {

                    MessageBox.Show("Nepodařilo se najít řešení.");
                    return;
                }
            }
            else
            {
                if (radioButton2.Checked == true)
                {
                    MessageBox.Show("Zvolil(a) jste 'Mapování ze souboru', ale nenačetl(a) jste obrázek.");
                    return;
                }
                else
                {
                    lo_puzzle.without_image();
                    lo_puzzle.init_map();
                    lo_puzzle.solve();
                }

            }


            richTextBox1.Clear();
            richTextBox1.AppendText(String.Format("{0,2}{1,2} {2,2}\n", " ", lo_puzzle.ma_list_res[2, 0].get_top(), " "));
            richTextBox1.AppendText(String.Format("{0,2}{1,2} {2,2}\n", lo_puzzle.ma_list_res[2, 0].get_left(), 
                lo_puzzle.ma_list_res[2, 0].get_index(), lo_puzzle.ma_list_res[2, 0].get_right()));
            richTextBox1.AppendText(String.Format("{0,2}{1,2} {2,2}", " ", lo_puzzle.ma_list_res[2, 0].get_bottom()," "));
            

            richTextBox2.Clear();
            richTextBox2.AppendText(String.Format("{0,2}{1,2} {2,2}\n", " ", lo_puzzle.ma_list_res[2, 1].get_top(), " "));
            richTextBox2.AppendText(String.Format("{0,2}{1,2} {2,2}\n", lo_puzzle.ma_list_res[2, 1].get_left(), 
                lo_puzzle.ma_list_res[2, 1].get_index(), lo_puzzle.ma_list_res[2, 1].get_right()));
            richTextBox2.AppendText(String.Format("{0,2}{1,2} {2,2}", "  ", lo_puzzle.ma_list_res[2, 1].get_bottom(), " "));

            richTextBox3.Clear();
            richTextBox3.AppendText(String.Format("{0,2}{1,2} {2,2}\n", " ", lo_puzzle.ma_list_res[2, 2].get_top(), " "));
            richTextBox3.AppendText(String.Format("{0,2}{1,2} {2,2}\n", lo_puzzle.ma_list_res[2, 2].get_left(), 
                lo_puzzle.ma_list_res[2, 2].get_index(), lo_puzzle.ma_list_res[2, 2].get_right()));
            richTextBox3.AppendText(String.Format("{0,2}{1,2} {2,2}", " ", lo_puzzle.ma_list_res[2, 2].get_bottom(), " "));

            richTextBox4.Clear();
            richTextBox4.AppendText(String.Format("{0,2}{1,2} {2,2}\n", " ", lo_puzzle.ma_list_res[1, 0].get_top(), " "));
            richTextBox4.AppendText(String.Format("{0,2}{1,2} {2,2}\n", lo_puzzle.ma_list_res[1, 0].get_left(), 
                lo_puzzle.ma_list_res[1, 0].get_index(), lo_puzzle.ma_list_res[1, 0].get_right()));
            richTextBox4.AppendText(String.Format("{0,2}{1,2} {2,2}", " ", lo_puzzle.ma_list_res[1, 0].get_bottom(), " "));

            richTextBox5.Clear();
            richTextBox5.AppendText(String.Format("{0,2}{1,2} {2,2}\n", " ", lo_puzzle.ma_list_res[1, 1].get_top(), " "));
            richTextBox5.AppendText(String.Format("{0,2}{1,2} {2,2}\n", lo_puzzle.ma_list_res[1, 1].get_left(), 
                lo_puzzle.ma_list_res[1, 1].get_index(), lo_puzzle.ma_list_res[1, 1].get_right()));
            richTextBox5.AppendText(String.Format("{0,2}{1,2} {2,2}", " ", lo_puzzle.ma_list_res[1, 1].get_bottom(), " "));

            richTextBox6.Clear();
            richTextBox6.AppendText(String.Format("{0,2}{1,2} {2,2}\n", " ", lo_puzzle.ma_list_res[1, 2].get_top(), " "));
            richTextBox6.AppendText(String.Format("{0,2}{1,2} {2,2}\n", lo_puzzle.ma_list_res[1, 2].get_left(), 
                lo_puzzle.ma_list_res[1, 2].get_index(), lo_puzzle.ma_list_res[1, 2].get_right()));
            richTextBox6.AppendText(String.Format("{0,2}{1,2} {2,2}", " ", lo_puzzle.ma_list_res[1, 2].get_bottom(), " "));

            richTextBox7.Clear();
            richTextBox7.AppendText(String.Format("{0,2}{1,2} {2,2}\n", " ", lo_puzzle.ma_list_res[0, 0].get_top(), " "));
            richTextBox7.AppendText(String.Format("{0,2}{1,2} {2,2}\n", lo_puzzle.ma_list_res[0, 0].get_left(), 
                lo_puzzle.ma_list_res[0, 0].get_index(), lo_puzzle.ma_list_res[0, 0].get_right()));
            richTextBox7.AppendText(String.Format("{0,2}{1,2} {2,2}", " ", lo_puzzle.ma_list_res[0, 0].get_bottom(), " "));

            richTextBox8.Clear();
            richTextBox8.AppendText(String.Format("{0,2}{1,2} {2,2}\n", " ", lo_puzzle.ma_list_res[0, 1].get_top(), " "));
            richTextBox8.AppendText(String.Format("{0,2}{1,2} {2,2}\n", lo_puzzle.ma_list_res[0, 1].get_left(), 
                lo_puzzle.ma_list_res[0, 1].get_index(), lo_puzzle.ma_list_res[0, 1].get_right()));
            richTextBox8.AppendText(String.Format("{0,2}{1,2} {2,2}", " ", lo_puzzle.ma_list_res[0, 1].get_bottom(), " "));

            richTextBox9.Clear();
            richTextBox9.AppendText(String.Format("{0,2}{1,2} {2,2}\n", " ", lo_puzzle.ma_list_res[0, 2].get_top(), " "));
            richTextBox9.AppendText(String.Format("{0,2}{1,2} {2,2}\n", lo_puzzle.ma_list_res[0, 2].get_left(), 
                lo_puzzle.ma_list_res[0, 2].get_index(), lo_puzzle.ma_list_res[0, 2].get_right()));
            richTextBox9.AppendText(String.Format("{0,2}{1,2} {2,2}", " ", lo_puzzle.ma_list_res[0, 2].get_bottom(), " "));
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {

        }
    }
}
