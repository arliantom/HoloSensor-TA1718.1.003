using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Urho;
using Urho.Gui;

namespace TA1718._1._003
{
    class UITwoD_Gesture
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

        Text x1;
        Text y1;
        Text z1;

        Text x2;
        Text y2;
        Text z2;

        Text x3;
        Text y3;
        Text z3;

        Text x4;
        Text y4;

        //font
        Font ui_font = CoreAssets.Fonts.AnonymousPro;
        int font_small = 12;
        int font_medium = 30;
        int font_large = 40;

        int positionXx1 = 700;
        int positionYx1 = -50;
        int positionXy1 = 840;
        int positionYy1 = -25;
        int positionXz1 = 1000;
        int positionYz1 = 0;
        int positionYa1 = 25;
        int positionYKinect = 50;



        public UITwoD_Gesture(ref UIElement uielement)
        {
            ui_node = uielement;
            drawUI();
        }

        private void drawUI()
        {
            //draw font for X
            x = new Text();
            x.SetAlignment(horizontal, vertical);
            x.Position = new IntVector2(positionXx1, positionYx1);
            x.Name = "X";
            x.Value = "Camera ID = ";
            x.SetFont(ui_font, font_small);
            x.SetColor(uicolor);
            ui_node.AddChild(x);

            //draw font for Y
            y = new Text();
            y.SetAlignment(horizontal, vertical);
            y.Position = new IntVector2(positionXy1, positionYx1);
            y.Name = "Y";
            y.Value = "Gestur = ";
            y.SetFont(ui_font, font_small);
            y.SetColor(uicolor);
            ui_node.AddChild(y);

            //draw font for Z
            z = new Text();
            z.SetAlignment(horizontal, vertical);
            z.Position = new IntVector2(positionXz1, positionYx1);
            z.Name = "Z";
            z.Value = "Confidence = ";
            z.SetFont(ui_font, font_small);
            z.SetColor(uicolor);
            ui_node.AddChild(z);

            //draw font for X
            x1 = new Text();
            x1.SetAlignment(horizontal, vertical);
            x1.Position = new IntVector2(positionXx1, positionYy1);
            x1.Name = "X1";
            x1.Value = "Camera ID = ";
            x1.SetFont(ui_font, font_small);
            x1.SetColor(uicolor);
            ui_node.AddChild(x1);

            //draw font for Y
            y1 = new Text();
            y1.SetAlignment(horizontal, vertical);
            y1.Position = new IntVector2(positionXy1, positionYy1);
            y1.Name = "Y1";
            y1.Value = "Gestur = ";
            y1.SetFont(ui_font, font_small);
            y1.SetColor(uicolor);
            ui_node.AddChild(y1);

            //draw font for Z
            z1 = new Text();
            z1.SetAlignment(horizontal, vertical);
            z1.Position = new IntVector2(positionXz1, positionYy1);
            z1.Name = "Z1";
            z1.Value = "Confidence = ";
            z1.SetFont(ui_font, font_small);
            z1.SetColor(uicolor);
            ui_node.AddChild(z1);

            //draw font for X
            x2 = new Text();
            x2.SetAlignment(horizontal, vertical);
            x2.Position = new IntVector2(positionXx1, positionYz1);
            x2.Name = "X2";
            x2.Value = "Camera ID = ";
            x2.SetFont(ui_font, font_small);
            x2.SetColor(uicolor);
            ui_node.AddChild(x2);

            //draw font for Y
            y2 = new Text();
            y2.SetAlignment(horizontal, vertical);
            y2.Position = new IntVector2(positionXy1, positionYz1);
            y2.Name = "Y2";
            y2.Value = "Gestur = ";
            y2.SetFont(ui_font, font_small);
            y2.SetColor(uicolor);
            ui_node.AddChild(y2);

            //draw font for Z
            z2 = new Text();
            z2.SetAlignment(horizontal, vertical);
            z2.Position = new IntVector2(positionXz1, positionYz1);
            z2.Name = "Z2";
            z2.Value = "Confidence = ";
            z2.SetFont(ui_font, font_small);
            z2.SetColor(uicolor);
            ui_node.AddChild(z2);

            //draw font for X
            x3 = new Text();
            x3.SetAlignment(horizontal, vertical);
            x3.Position = new IntVector2(positionXx1, positionYa1);
            x3.Name = "X3";
            x3.Value = "Camera ID = ";
            x3.SetFont(ui_font, font_small);
            x3.SetColor(uicolor);
            ui_node.AddChild(x3);

            //draw font for Y
            y3 = new Text();
            y3.SetAlignment(horizontal, vertical);
            y3.Position = new IntVector2(positionXy1, positionYa1);
            y3.Name = "Y3";
            y3.Value = "Gestur = ";
            y3.SetFont(ui_font, font_small);
            y3.SetColor(uicolor);
            ui_node.AddChild(y3);

            //draw font for Z
            z3 = new Text();
            z3.SetAlignment(horizontal, vertical);
            z3.Position = new IntVector2(positionXz1, positionYa1);
            z3.Name = "Z3";
            z3.Value = "Confidence = ";
            z3.SetFont(ui_font, font_small);
            z3.SetColor(uicolor);
            ui_node.AddChild(z3);

            //draw font for X
            x4 = new Text();
            x4.SetAlignment(horizontal, vertical);
            x4.Position = new IntVector2(positionXx1, positionYKinect);
            x4.Name = "X3";
            x4.Value = "Camera ID = ";
            x4.SetFont(ui_font, font_small);
            x4.SetColor(uicolor);
            ui_node.AddChild(x4);

            //draw font for Y
            y4 = new Text();
            y4.SetAlignment(horizontal, vertical);
            y4.Position = new IntVector2(positionXy1, positionYKinect);
            y4.Name = "Y3";
            y4.Value = "Gestur = ";
            y4.SetFont(ui_font, font_small);
            y4.SetColor(uicolor);
            ui_node.AddChild(y4);


        }

        public void updateUI(string x_val, string y_val, float z_val)
        {
            if (x_val == "1")
            {
                x.Value = "Camera ID = " + x_val;
                y.Value = "Gestur = " + y_val;
                z.Value = "Confidence = " + z_val;
            }
            else if (x_val == "2")
            {
                x1.Value = "Camera ID = " + x_val;
                y1.Value = "Gestur = " + y_val;
                z1.Value = "Confidence = " + z_val;
            }
            else if (x_val == "3")
            {
                x2.Value = "Camera ID = " + x_val;
                y2.Value = "Gestur = " + y_val;
                z2.Value = "Confidence = " + z_val;
            }
            else if (x_val == "3")
            {
                x3.Value = "Camera ID = " + x_val;
                y3.Value = "Gestur = " + y_val;
                z3.Value = "Confidence = " + z_val;
            }
        }

        public void updateUI_Kinect( string y_val)
        {
                x4.Value = "Camera ID = Kinect";
                y4.Value = "Gestur = " + y_val;
        }

        public void eraseUI()
        {
            x.Value = "";
            y.Value = "";
            z.Value = "";

            x1.Value = "";
            y1.Value = "";
            z1.Value = "";

            x2.Value = "";

            y2.Value = "";
            z2.Value = "";

            x3.Value = "";
            y3.Value = "";
            z3.Value = "";

            x4.Value = "";
            y4.Value = "";
        }


    }
}
