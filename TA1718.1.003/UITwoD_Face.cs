using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Urho;
using Urho.Gui;

namespace TA1718._1._003
{
    class UITwoD_Face
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

        //font
        Font ui_font = CoreAssets.Fonts.AnonymousPro;
        int font_small = 16;
        int font_medium = 30;
        int font_large = 40;

        int positionXx = 250;
        int positionYx = -50;

        int positionXy = 250;
        int positionYy = -25;

        public UITwoD_Face(ref UIElement uielement)
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
            y.Value = "Ekspresi = ";
            y.SetFont(ui_font, font_small);
            y.SetColor(uicolor);
            ui_node.AddChild(y);


        }

        public void updateUI(string x_val, string y_val)
        {
            x.Value = "Nama = " + x_val;
            y.Value = "Ekspresi = " + y_val;
        }

        public void eraseUI()
        {
            x.Value = "";
            y.Value = "";
        }
    }
}

