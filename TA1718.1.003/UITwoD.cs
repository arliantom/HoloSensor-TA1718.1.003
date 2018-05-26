using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Urho;
using Urho.Gui;

namespace TA1718._1._003
{
    class UITwoD
    {
        //init ui element
        UIElement ui_node;

        //init base color
        Color uicolor = Color.Green;
        Color uicolor2 = Color.Red;

        //aligment
        HorizontalAlignment horizontal = HorizontalAlignment.Left;
        VerticalAlignment vertical = VerticalAlignment.Center;

        //text
        Text x; /*Text x1; Text x2; Text x3;*/
        Text y; /*Text y1; Text y2; Text y3;*/
        Text z; /*Text z1; Text z2; Text z3;*/
        Text a; /*Text a1; Text a2; Text a3;*/



        //font
        Font ui_font = CoreAssets.Fonts.AnonymousPro;
        int font_small = 16;
        int font_medium = 30;
        int font_large = 40;
        
        // Sensor ID1
        int positionXx = 250;
        int positionYx = -200;

        int positionXy = 250;
        int positionYy = -175;

        int positionXz = 250;
        int positionYz = -150;

        int positionXa = 250;
        int positionYa = -125;

        //// Sensor ID2
        //int positionXx1 = 250;
        //int positionYx1 = -200;

        //int positionXy1 = 250;
        //int positionYy1 = -175;

        //int positionXz1 = 250;
        //int positionYz1 = -150;

        //int positionXa1 = 250;
        //int positionYa1 = -125;

        //// Sensor ID3
        //int positionXx2 = 250;
        //int positionYx2= -200;

        //int positionXy2 = 250;
        //int positionYy2 = -175;

        //int positionXz2 = 250;
        //int positionYz2 = -150;

        //int positionXa2 = 250;
        //int positionYa2 = -125;

        //// Sensor ID4
        //int positionXx3 = 250;
        //int positionYx3 = -200;

        //int positionXy3 = 250;
        //int positionYy3 = -175;

        //int positionXz3 = 250;
        //int positionYz3 = -150;

        //int positionXa3 = 250;
        //int positionYa3 = -125;

        public UITwoD(ref UIElement uielement)
        {
            ui_node = uielement;
            drawUI();
        }

        private void drawUI()
        {
            //Sensor ID1
            //draw font for X
            x = new Text();
            x.SetAlignment(horizontal, vertical);
            x.Position = new IntVector2(positionXx, positionYx);
            x.Name = "X";
            x.Value = "Temperatur = ";
            x.SetFont(ui_font, font_small);
            x.SetColor(uicolor);
            ui_node.AddChild(x);

            //draw font for Y
            y = new Text();
            y.SetAlignment(horizontal, vertical);
            y.Position = new IntVector2(positionXy, positionYy);
            y.Name = "Y";
            y.Value = "Kelembapan = ";
            y.SetFont(ui_font, font_small);
            y.SetColor(uicolor);
            ui_node.AddChild(y);

            //draw font for Z
            z = new Text();
            z.SetAlignment(horizontal, vertical);
            z.Position = new IntVector2(positionXz, positionYz);
            z.Name = "Z";
            z.Value = "Intensitas Cahaya = ";
            z.SetFont(ui_font, font_small);
            z.SetColor(uicolor);
            ui_node.AddChild(z);


            //draw font for a
            a = new Text();
            a.SetAlignment(horizontal, vertical);
            a.Position = new IntVector2(positionXa, positionYa);
            a.Name = "A";
            a.Value = "Kebisingan Suara" +
                " = ";
            a.SetFont(ui_font, font_small);
            a.SetColor(uicolor);
            ui_node.AddChild(a);

            ////Sensor ID2
            ////draw font for X
            //x1 = new Text();
            //x1.SetAlignment(horizontal, vertical);
            //x1.Position = new IntVector2(positionXx1, positionYx1);
            //x1.Name = "X";
            //x1.Value = "Temperatur = ";
            //x1.SetFont(ui_font, font_small);
            //x1.SetColor(uicolor);
            //ui_node.AddChild(x);

            ////draw font for Y
            //y1 = new Text();
            //y1.SetAlignment(horizontal, vertical);
            //y1.Position = new IntVector2(positionXy1, positionYy1);
            //y1.Name = "Y";
            //y1.Value = "Kelembapan = ";
            //y1.SetFont(ui_font, font_small);
            //y1.SetColor(uicolor);
            //ui_node.AddChild(y);

            ////draw font for Z
            //z1 = new Text();
            //z1.SetAlignment(horizontal, vertical);
            //z1.Position = new IntVector2(positionXz1, positionYz1);
            //z1.Name = "Z";
            //z1.Value = "Intensitas Cahaya = ";
            //z1.SetFont(ui_font, font_small);
            //z1.SetColor(uicolor);
            //ui_node.AddChild(z);


            ////draw font for a
            //a1 = new Text();
            //a1.SetAlignment(horizontal, vertical);
            //a1.Position = new IntVector2(positionXa1, positionYa1);
            //a1.Name = "A";
            //a1.Value = "Kebisingan Suara" +
            //    " = ";
            //a1.SetFont(ui_font, font_small);
            //a1.SetColor(uicolor);
            //ui_node.AddChild(a);

            ////Sensor ID3
            ////draw font for X
            //x2 = new Text();
            //x2.SetAlignment(horizontal, vertical);
            //x2.Position = new IntVector2(positionXx2, positionYx2);
            //x2.Name = "X";
            //x2.Value = "Temperatur = ";
            //x2.SetFont(ui_font, font_small);
            //x2.SetColor(uicolor);
            //ui_node.AddChild(x);

            ////draw font for Y
            //y2 = new Text();
            //y2.SetAlignment(horizontal, vertical);
            //y2.Position = new IntVector2(positionXy2, positionYy2);
            //y2.Name = "Y";
            //y2.Value = "Kelembapan = ";
            //y2.SetFont(ui_font, font_small);
            //y2.SetColor(uicolor);
            //ui_node.AddChild(y);

            ////draw font for Z
            //z2 = new Text();
            //z2.SetAlignment(horizontal, vertical);
            //z2.Position = new IntVector2(positionXz2, positionYz2);
            //z2.Name = "Z";
            //z2.Value = "Intensitas Cahaya = ";
            //z2.SetFont(ui_font, font_small);
            //z2.SetColor(uicolor);
            //ui_node.AddChild(z);


            ////draw font for a
            //a2 = new Text();
            //a2.SetAlignment(horizontal, vertical);
            //a2.Position = new IntVector2(positionXa2, positionYa2);
            //a2.Name = "A";
            //a2.Value = "Kebisingan Suara" +
            //    " = ";
            //a2.SetFont(ui_font, font_small);
            //a2.SetColor(uicolor);
            //ui_node.AddChild(a);

            ////Sensor ID4
            ////draw font for X
            //x3 = new Text();
            //x3.SetAlignment(horizontal, vertical);
            //x3.Position = new IntVector2(positionXx3, positionYx3);
            //x3.Name = "X";
            //x3.Value = "Temperatur = ";
            //x3.SetFont(ui_font, font_small);
            //x3.SetColor(uicolor);
            //ui_node.AddChild(x);

            ////draw font for Y
            //y3 = new Text();
            //y3.SetAlignment(horizontal, vertical);
            //y3.Position = new IntVector2(positionXy3, positionYy3);
            //y3.Name = "Y";
            //y3.Value = "Kelembapan = ";
            //y3.SetFont(ui_font, font_small);
            //y3.SetColor(uicolor);
            //ui_node.AddChild(y);

            ////draw font for Z
            //z3 = new Text();
            //z3.SetAlignment(horizontal, vertical);
            //z3.Position = new IntVector2(positionXz3, positionYz3);
            //z3.Name = "Z";
            //z3.Value = "Intensitas Cahaya = ";
            //z3.SetFont(ui_font, font_small);
            //z3.SetColor(uicolor);
            //ui_node.AddChild(z);


            ////draw font for a
            //a3 = new Text();
            //a3.SetAlignment(horizontal, vertical);
            //a3.Position = new IntVector2(positionXa3, positionYa3);
            //a3.Name = "A";
            //a3.Value = "Kebisingan Suara" +
            //    " = ";
            //a3.SetFont(ui_font, font_small);
            //a3.SetColor(uicolor);
            //ui_node.AddChild(a);

        }

        public void updateUI(decimal x_val, decimal y_val, int z_val, int a_val, int mode)
        {
            if (mode == 0)
            {
                if (x_val < 20 || x_val > 28)
                {
                    x.SetColor(uicolor2);
                }
                else
                {
                    x.SetColor(uicolor);
                }

                if (y_val < 30 || y_val > 80)
                {
                    y.SetColor(uicolor2);
                }
                else
                {
                    y.SetColor(uicolor);
                }

                if (z_val < 250 || x_val > 750)
                {
                    z.SetColor(uicolor2);
                }
                else
                {
                    z.SetColor(uicolor);
                }

                if (z_val > 40)
                {
                    a.SetColor(uicolor2);
                }
                else
                {
                    a.SetColor(uicolor);
                }
            }
            else if (mode == 1)
            {
                if (x_val < 20 || x_val > 28)
                {
                    x.SetColor(uicolor2);
                }
                else
                {
                    x.SetColor(uicolor);
                }

                if (y_val < 30 || y_val > 80)
                {
                    y.SetColor(uicolor2);
                }
                else
                {
                    y.SetColor(uicolor);
                }

                if (z_val < 10 || x_val > 250)
                {
                    z.SetColor(uicolor2);
                }
                else
                {
                    z.SetColor(uicolor);
                }

                if (z_val > 50)
                {
                    a.SetColor(uicolor2);
                }
                else
                {
                    a.SetColor(uicolor);
                }
            }
            else
            {
                if (x_val < 20 || x_val > 28)
                {
                    x.SetColor(uicolor2);
                }
                else
                {
                    x.SetColor(uicolor);
                }

                if (y_val < 30 || y_val > 80)
                {
                    y.SetColor(uicolor2);
                }
                else
                {
                    y.SetColor(uicolor);
                }

                if (z_val < 250 || x_val > 750)
                {
                    z.SetColor(uicolor2);
                }
                else
                {
                    z.SetColor(uicolor);
                }

                if (z_val > 60)
                {
                    a.SetColor(uicolor2);
                }
                else
                {
                    a.SetColor(uicolor);
                }
            }
            x.Value = "Temperatur = " + x_val + " Celsius";
            y.Value = "Kelembapan = " + y_val + " %";
            z.Value = "Intensitas Cahaya = " + z_val + " lx";
            a.Value = "Kebisingan Suara = " + a_val + " db";
        }

        public void eraseUI()
        {
            x.Value = "";
            y.Value = "";
            z.Value = "";
            a.Value = "";
        }

    }
}
