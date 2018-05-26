using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Urho;
using Urho.Gui;

namespace TA1718._1._003
{
    class UITwoD_Voice
    {
        //init ui element
        UIElement ui_node;

        //init base color
        Color uicolor = Color.Green;

        //aligment
        HorizontalAlignment horizontal = HorizontalAlignment.Left;
        VerticalAlignment vertical = VerticalAlignment.Center;

        //text
        Text x;
        Text y;
        Text z;

        //font
        Font ui_font = CoreAssets.Fonts.AnonymousPro;
        int font_small = 16;
        int font_medium = 30;
        int font_large = 40;

        int positionXx = 700;
        int positionYx = -200;

        int positionXy = 700;
        int positionYy = -175;

        int positionXz = 700;
        int positionYz = -150;

        public UITwoD_Voice(ref UIElement uielement)
        {
            ui_node = uielement;
            drawUI();
        }

        private void drawUI()
        {
            //draw font for X
            x = new Text();
            x.SetAlignment(horizontal, vertical);
            x.Position = new IntVector2(positionXx, positionYx);
            x.Name = "X";
            x.Value = "Nama = ";
            x.SetFont(ui_font, font_small);
            x.SetColor(uicolor);
            ui_node.AddChild(x);

            //draw font for Y
            y = new Text();
            y.SetAlignment(horizontal, vertical);
            y.Position = new IntVector2(positionXy, positionYy);
            y.Name = "Y";
            y.Value = "Perintah = ";
            y.SetFont(ui_font, font_small);
            y.SetColor(uicolor);
            ui_node.AddChild(y);

            //draw font for Y
            z = new Text();
            z.SetAlignment(horizontal, vertical);
            z.Position = new IntVector2(positionXz, positionYz);
            z.Name = "Z";
            z.Value = "Perintah = ";
            z.SetFont(ui_font, font_small);
            z.SetColor(uicolor);
            ui_node.AddChild(z);

        }

        public void updateUI(string x_val, string y_val, string z_val)
        {
            x.Value = "Nama = " + x_val;
            y.Value = "Perintah = " + y_val;
            z.Value = "Jarak = " + z_val; 
        }

        public void eraseUI()
        {
            x.Value = "";
            y.Value = "";
            z.Value = "";
        }
    }
}
