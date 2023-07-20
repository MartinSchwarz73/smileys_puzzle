using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;

/// <summary>
/// Naming convention
/// prvni znak - promenna je ? globalni(g) , lokalni(l), atribut tridy(m), importni parametr(i)
/// druhy znak - datovy typ - value(v), array(a), objekt(o), bitmapa(b), ...
/// </summary>

namespace hadanka
{
    static class Constants
    {
        /// <summary>
        /// Konstanty
        /// protější strany mají kladné (oči) a záporné (pusa) číslo 
        /// pro snadné spojení/detekci jen na základě vynásobení -1
        /// </summary>

        public const int gc_red_top = 1;
        public const int gc_red_bottom = -1;
        public const int gc_blue_top = 2;
        public const int gc_blue_bottom = -2;
        public const int gc_yellow_top = 3;
        public const int gc_yellow_bottom = -3;
        public const int gc_green_top = 4;
        public const int gc_green_bottom = -4;
        public const int gc_nr_used_colors = 6;
        public enum gc_used_colors { unknown = 0, red = 1, blue = 2, yellow = 3, green = 4, black = 5 };
        public enum gc_tblr { top = 1, bottom = 2, left = 3, right = 4 };
    }


    public class PUZZLE_CARD
    {
        /// <summary>
        /// Třída jedné hrací karty
        /// Všechny atributy jsou privátní, přístup pouze přes getters/setters 
        /// </summary>

        /// <value><c>mv_used</c> - je karta použita ?</value>
        private bool mv_used;
        /// <value><c>mv_index</c> - index/pořadí karty</value>
        private int mv_index;
        /// <value><c>mb_img</c> - obrázek na kartě</value>
        private Bitmap mb_img;
        /// <value><c>mv_top, mv_right, mv_bottom, mv_left</c> - co je kde je</value>
        private int mv_top;
        private int mv_right;
        private int mv_bottom;
        private int mv_left;
        /// <value><c>mv_angle</c> - akt. uhel natoceni CW (0..3)</value>
        private int mv_angle;

        public PUZZLE_CARD(Bitmap ib_img, int iv_index)
        {
            /// <summary>Konstruktor - vytvoří hrací kartu
            /// (<paramref name="ib_img"/>,<paramref name="iv_index"/>).
            /// </summary>
            /// <param name="ib_img">obrázek hrací karty</param>
            /// <param name="iv_index">inicializační index/pořadí hrací karty</param>
            this.mv_angle = 0;
            this.mb_img = ib_img;
            this.mv_index = iv_index;
            this.mv_used = false;
        }
        public bool is_used()
        {
            return this.mv_used;
        }
        public void set_use()
        {
            this.mv_used = true;
        }
        public void set_unuse()
        {
            this.mv_used = false;
        }
        public void set_map(int iv_top, int iv_right, int iv_bottom, int iv_left)
        {
            this.mv_top = iv_top;
            this.mv_right = iv_right;
            this.mv_bottom = iv_bottom;
            this.mv_left = iv_left;
        }
        public void rotate_right()
        {
            int lv_temp = this.mv_top;
            this.mv_top = this.mv_left;
            this.mv_left = this.mv_bottom;
            this.mv_bottom = this.mv_right;
            this.mv_right = lv_temp;

            this.mv_angle = ++this.mv_angle % 4;
        }
        public int get_index()
        {
            return this.mv_index;
        }
        public int get_top()
        {
            return this.mv_top;
        }
        public int get_left()
        {
            return this.mv_left;
        }
        public int get_bottom()
        {
            return this.mv_bottom;
        }
        public int get_right()
        {
            return this.mv_right;
        }
        public Bitmap get_image()
        {
            return this.mb_img;
        }
        public int get_angle()
        {
            return this.mv_angle;
        }
        public void analyze_card()
        {
            /// <summary>
            /// Analyzuje obraz karty aby zjistil jaké "obličeje" na kartě jsou
            /// </summary>

            int[] la_colors = new int[Constants.gc_nr_used_colors];
            int lv_black_px_cnt = 0;
            int lv_prev_color = (int)Constants.gc_used_colors.black;
            Color lo_px_color;
            int lv_recognized_color;

            int lv_middle = (int)this.mb_img.Width / 2;
            double lv_mouth_pos = (int)this.mb_img.Width * 0.26;  // cca kde by mohla být pusa, aby byla detekována
            int lv_w = 2;  // polovina šířky pásma pro detekci pusy
            
            // spodni část
            for (int y = this.mb_img.Height - 3; y > this.mb_img.Height - lv_mouth_pos; y--)
            {
                for (int x = lv_middle - lv_w; x < (lv_middle + lv_w); x++)
                {
                    lo_px_color = this.mb_img.GetPixel(x, y);
                    lv_recognized_color = this.get_color_pixel(lo_px_color.R, lo_px_color.G, lo_px_color.B);
                    la_colors[lv_recognized_color]++;
                    if ((lv_prev_color != (int)Constants.gc_used_colors.black || lv_prev_color != (int)Constants.gc_used_colors.unknown)
                        && lv_recognized_color == (int)Constants.gc_used_colors.black)
                    {
                        // lv_black_px_cnt je pro analyzu zda je to spodek smajlika (pusa) nebo ne. snaží se v půlce smajlika najít černou barvu.
                        lv_black_px_cnt++;
                    }
                    lv_prev_color = lv_recognized_color;
                }
            }
            this.recognize((int)Constants.gc_tblr.bottom, la_colors, lv_black_px_cnt);

            // horní část - nejdříve nastavit proměnné do výchozího stavu
            Array.Clear(la_colors, 0, la_colors.Length);
            lv_black_px_cnt = 0;
            lv_prev_color = (int)Constants.gc_used_colors.black;

            for (int y = 2; y < lv_mouth_pos; y++)
            {
                for (int x = lv_middle - lv_w; x < (lv_middle + lv_w); x++)
                {
                    lo_px_color = this.mb_img.GetPixel(x, y);
                    lv_recognized_color = this.get_color_pixel(lo_px_color.R, lo_px_color.G, lo_px_color.B);
                    la_colors[lv_recognized_color]++;
                    if ((lv_prev_color != (int)Constants.gc_used_colors.black || lv_prev_color != (int)Constants.gc_used_colors.unknown)
                        && lv_recognized_color == (int)Constants.gc_used_colors.black)
                    {
                        // lv_black_px_cnt je pro analyzu zda je to spodek smajlika (pusa) nebo ne. snaží se v půlce smajlika najít černou barvu.
                        lv_black_px_cnt++;
                    }
                    lv_prev_color = lv_recognized_color;
                }
            }
            this.recognize((int)Constants.gc_tblr.top, la_colors, lv_black_px_cnt);

            // levá část - nejdříve nastavit proměnné do výchozího stavu
            Array.Clear(la_colors, 0, la_colors.Length);
            lv_black_px_cnt = 0;
            lv_prev_color = (int)Constants.gc_used_colors.black;

            for (int x = 2; x < lv_mouth_pos; x++)
            {
                for (int y = lv_middle - lv_w; y < (lv_middle + lv_w); y++)
                {
                    lo_px_color = this.mb_img.GetPixel(x, y);
                    lv_recognized_color = this.get_color_pixel(lo_px_color.R, lo_px_color.G, lo_px_color.B);
                    la_colors[lv_recognized_color]++;
                    if ((lv_prev_color != (int)Constants.gc_used_colors.black || lv_prev_color != (int)Constants.gc_used_colors.unknown)
                        && lv_recognized_color == (int)Constants.gc_used_colors.black)
                    {
                        // lv_black_px_cnt je pro analyzu zda je to spodek smajlika (pusa) nebo ne. snaží se v půlce smajlika najít černou barvu.
                        lv_black_px_cnt++;
                    }
                    lv_prev_color = lv_recognized_color;
                }
            }
            this.recognize((int)Constants.gc_tblr.left, la_colors, lv_black_px_cnt);

            // pravá část - nejdříve nastavit proměnné do výchozího stavu
            lv_black_px_cnt = 0;
            lv_prev_color = (int)Constants.gc_used_colors.black;
            Array.Clear(la_colors, 0, la_colors.Length);

            for (int x = this.mb_img.Width - 3; x > (this.mb_img.Width - lv_mouth_pos); x--)
            {
                for (int y = lv_middle - lv_w; y < (lv_middle + lv_w); y++)
                {
                    lo_px_color = this.mb_img.GetPixel(x, y);
                    lv_recognized_color = this.get_color_pixel(lo_px_color.R, lo_px_color.G, lo_px_color.B);
                    la_colors[lv_recognized_color]++;
                    if ((lv_prev_color != (int)Constants.gc_used_colors.black || lv_prev_color != (int)Constants.gc_used_colors.unknown)
                        && lv_recognized_color == (int)Constants.gc_used_colors.black)
                    {
                        // lv_black_px_cnt je pro analyzu zda je to spodek smajlika (pusa) nebo ne. snaží se v půlce smajlika najít černou barvu.
                        lv_black_px_cnt++;
                    }
                    lv_prev_color = lv_recognized_color;
                }
            }
            this.recognize((int)Constants.gc_tblr.right, la_colors, lv_black_px_cnt);
        }
            
        private void recognize(int iv_pos, int[] ia_colors, int iv_black_px_cnt)
        {
            /// <summary>
            /// metoda přiřadí straně hrací karty zjištěnou barvu
            /// </summary>
            int lv_maxValue = ia_colors.Max();
            int lv_maxIndex = ia_colors.ToList().IndexOf(lv_maxValue);
            if (iv_black_px_cnt > 0)
            {
                lv_maxIndex *= -1; // násobení mínus jedničkou - viz nahoře - našly se černé px = protější strana (pusa)
            }
            switch (iv_pos)
            {
                case (int)Constants.gc_tblr.top: this.mv_top = lv_maxIndex; break;
                case (int)Constants.gc_tblr.bottom: this.mv_bottom = lv_maxIndex; break;
                case (int)Constants.gc_tblr.left: this.mv_left = lv_maxIndex; break;
                case (int)Constants.gc_tblr.right: this.mv_right = lv_maxIndex; break;
            }
        }
        private int get_color_pixel(int iv_red, int iv_green, int iv_blue)
        {
            /// <summary>
            /// hodnoty v podmínkách jsou jen hrubě naměřené hodnoty RGB, které určují žlutou, červenou ... barvu
            /// </summary>
            int iv_retval = 0;
            if (iv_red > 200 && iv_green > 200 && iv_blue < 150) iv_retval = (int)Constants.gc_used_colors.yellow;
            else if (iv_red > 200 && iv_green < 100 && iv_blue < 100) iv_retval = (int)Constants.gc_used_colors.red;
            else if (iv_red < 200 && iv_green > 200 && iv_blue < 200) iv_retval = (int)Constants.gc_used_colors.green;
            else if (iv_red < 200 && iv_green < 200 && iv_blue > 200) iv_retval = (int)Constants.gc_used_colors.blue;
            else if (iv_red < 100 && iv_green < 100 && iv_blue < 100) iv_retval = (int)Constants.gc_used_colors.black;
            return iv_retval;
        }

    }
    public class PUZZLE
    {
        /// <summary>
        /// třída pro hrací pole
        /// </summary>

        /// <value><c>ma_list</c> - pole vsech karet</value>
        public PUZZLE_CARD[] ma_list = new PUZZLE_CARD[9];
        /// <value><c>ma_list_res</c> - pole pro vysledek</value>
        public PUZZLE_CARD[,] ma_list_res = new PUZZLE_CARD[3, 3];

        public PUZZLE()
        {
            // Konstruktor 
        }
        public void without_image()
        {
            /// <summary>
            /// metoda ktera vytvori objekty karet a ulozi je do pole ma_list, akorát bez obrázků
            /// </summary>
            for (int i = 0; i < 9; i++)
            {
                PUZZLE_CARD lo_puzzle_part = new PUZZLE_CARD(null, i);
                this.ma_list[i] = lo_puzzle_part;
                this.ma_list_res[(int)i / 3, i % 3] = null;
            }
        }
        public void load_image(string iv_path)
        {
            /// <summary>
            /// metoda, ktera nacte obrazek a rozdeli ho na 9 casti - na hraci karty
            /// vytvori jednotlive objekty pro karty a do objektu vlozi jednotlive casti obrazku
            /// </summary>
            Bitmap lo_bitmap = new Bitmap(iv_path);

            int lv_width_part = lo_bitmap.Width / 3;
            int lv_height_part = lo_bitmap.Height / 3;
            Rectangle lo_cloneRect;
            Bitmap lb_puzzle_image;

            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    lo_cloneRect = new Rectangle(j * lv_width_part, i * lv_height_part, lv_width_part, lv_height_part);
                    lb_puzzle_image = lo_bitmap.Clone(lo_cloneRect, lo_bitmap.PixelFormat);
                    PUZZLE_CARD lo_puzzle_part = new PUZZLE_CARD(lb_puzzle_image, i * 3 + j);
                    lo_puzzle_part.analyze_card();
                    this.ma_list[i * 3 + j] = lo_puzzle_part;
                    this.ma_list_res[i, j] = null;
                }
            }
        }
        /// <summary>
        /// naplnění "mapy" analýzou zdrojového souboru
        /// </summary>
        public void init_map_from_image()
        {
            for (int i = 0; i < 9; i++)
            {
                this.ma_list[i].analyze_card();
            }
        }
        /// <summary>
        /// naplnění "mapy" ručně podle zaslaného obrázku a konstant výše uvedených
        /// </summary>
        public void init_map()
        {
            this.ma_list[0].set_map(Constants.gc_red_bottom, Constants.gc_yellow_bottom, Constants.gc_red_top, Constants.gc_green_top);
            this.ma_list[1].set_map(Constants.gc_blue_top, Constants.gc_yellow_top, Constants.gc_blue_bottom, Constants.gc_green_bottom);
            this.ma_list[2].set_map(Constants.gc_red_top, Constants.gc_yellow_top, Constants.gc_blue_bottom, Constants.gc_yellow_bottom);

            this.ma_list[3].set_map(Constants.gc_red_bottom, Constants.gc_blue_top, Constants.gc_green_top, Constants.gc_red_bottom);
            this.ma_list[4].set_map(Constants.gc_blue_top, Constants.gc_green_top, Constants.gc_red_bottom, Constants.gc_yellow_bottom);
            this.ma_list[5].set_map(Constants.gc_blue_bottom, Constants.gc_yellow_bottom, Constants.gc_red_top, Constants.gc_green_top);

            this.ma_list[6].set_map(Constants.gc_blue_bottom, Constants.gc_green_bottom, Constants.gc_yellow_top, Constants.gc_blue_top);
            this.ma_list[7].set_map(Constants.gc_blue_top, Constants.gc_red_bottom, Constants.gc_blue_bottom, Constants.gc_yellow_top);
            this.ma_list[8].set_map(Constants.gc_yellow_top, Constants.gc_red_bottom, Constants.gc_green_bottom, Constants.gc_green_top);

        }
        /// <summary>
        /// metoda, ktera hleda kartu padnouci na urcite misto
        /// vyuziva i podminku, ktera overuje, ze je karta uz pouzita - máme každou hrací kartu jen jednu
        /// </summary>
        private bool find_puzzle_card(int iv_top, int iv_right, int iv_bottom, int iv_left, int iv_i)
        {
            for (int i = 0; i < 9; i++)
            {
                if (!this.ma_list[i].is_used())
                {
                    for (int j = 0; j < 4; j++)
                    {
                        if ((iv_top == (this.ma_list[i].get_top() * (-1)) || iv_top == 0)
                            && (iv_right == (this.ma_list[i].get_right() * (-1)) || iv_right == 0)
                            && (iv_bottom == (this.ma_list[i].get_bottom() * (-1)) || iv_bottom == 0)
                            && (iv_left == (this.ma_list[i].get_left() * (-1)) || iv_left == 0))
                        {

                            if (this.put_card(i, iv_i))
                            {
                                return true;
                            }

                        }
                        else
                        {
                            this.ma_list[i].rotate_right();
                        }
                    }
                }

            }
            return false;
        }
        /// <summary>
        /// hlavní metoda na vyřešení puzzle
        /// tato metoda vloží první kartu, čím spustí výpočet řešení
        /// </summary>
        public void solve()
        {
            for (int i = 0; i < 9; i++)
            {
                for (int k = 0; k < 4; k++)
                {
                    if (this.put_card(i, 0))
                    {
                        // hotovo
                        return;
                    }
                    else
                    {
                        // reinicializace potřebného v případě neúspěchu 
                        for (int j = 0; j < 9; j++)
                        {
                            this.ma_list[j].set_unuse();
                        }
                        Array.Clear(this.ma_list_res, 0, this.ma_list_res.Length);
                    }
                    // otočíme kartu o 90st CW a hledáme s tímto nastavením
                    this.ma_list[i].rotate_right();
                }

            }
        }
        /// <summary>
        /// metoda na vkladani jednotlivych karet do poli
        /// </summary>
        /// <param name="iv_index">kterou kartu</param>
        /// <param name="iv_i">na kterou pozici</param>
        /// <returns></returns>
        private bool put_card(int iv_index, int iv_i)
        {
            // přepočet na 2D
            int lv_j = (int)iv_i / 3;
            int lv_i = iv_i % 3;

            this.ma_list_res[lv_j, lv_i] = this.ma_list[iv_index];
            this.ma_list[iv_index].set_use();

            // prvni radek
            if (lv_j == 0)
            {
                switch (lv_i)
                {
                    case 0:
                    case 1:
                        if (!this.find_puzzle_card(0, 0, 0, this.ma_list[iv_index].get_right(), iv_i + 1))
                        {
                            this.ma_list_res[lv_j, lv_i] = null;
                            this.ma_list[iv_index].set_unuse();
                        }
                        else
                        {
                            return true;
                        }
                        break;
                    case 2:
                        if (!this.find_puzzle_card(0, 0, this.ma_list_res[0, 0].get_top(), 0, iv_i + 1))
                        {
                            this.ma_list_res[lv_j, lv_i] = null;
                            this.ma_list[iv_index].set_unuse();
                        }
                        else
                        {
                            return true;
                        }
                        break;
                }
            }
            //druhy a treti radek
            if (lv_j == 1 || lv_j == 2)
            {
                switch (lv_i)
                {
                    case 0:
                    case 1:
                        if (!this.find_puzzle_card(0, 0, this.ma_list_res[(lv_j - 1), (lv_i + 1)].get_top(), 
                            this.ma_list[iv_index].get_right(), iv_i + 1))
                        {
                            this.ma_list_res[lv_j, lv_i] = null;
                            this.ma_list[iv_index].set_unuse();
                        }
                        else
                        {
                            return true;
                        }
                        break;
                    case 2:
                        if (lv_j == 2)
                        {
                            return true;
                        }
                        else
                        {
                            if (!this.find_puzzle_card(0, 0, this.ma_list_res[1, 0].get_top(), 0, iv_i + 1))
                            {
                                this.ma_list_res[lv_j, lv_i] = null;
                                this.ma_list[iv_index].set_unuse();
                            }
                            else
                            {
                                return true;
                            }
                        }
                        break;

                }
            }
            return false;
        }
        static class Program
        {
            /// <summary>
            /// Hlavní vstupní bod aplikace.
            /// </summary>
            [STAThread]
            static void Main()
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new SmileysPuzzleForm());
            }
        }
    }
}