using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Collections;
using System.Windows.Forms;

namespace IMGProcessor
{
    class State
    {
        Bitmap image;
        int blurAmount;
        int brightness;
        int contrast;
        int gamma;

        public static Stack<State> undoStack = new Stack<State>();
        public static Stack<State> redoStack = new Stack<State>();

        public State(Image image, int blurAmount, int brightness, int contrast, int gamma)
        {
            this.image = new Bitmap(image);
            this.blurAmount = blurAmount;
            this.brightness = brightness;
            this.contrast = contrast;
            this.gamma = gamma;
        }


        public Bitmap Image
        {
            get { return image;  }
            set { image = value; }
        }

        public int Blur
        {
            get { return blurAmount; }
            set { blurAmount = value; }
        }

        public int Brightness
        {
            get { return brightness; }
            set { brightness = value; }
        }

        public int Contrast
        {
            get { return contrast; }
            set { contrast = value; }
        }

        public int Gamma
        {
            get { return gamma; }
            set { gamma = value; }
        }

        public void save(Image image, int blurAmount, int brightness, int contrast, int gamma)
        {
            undoStack.Push(new State(this.image, this.blurAmount, this.brightness, this.contrast, this.gamma));

            Image = new Bitmap(image);
            Blur = blurAmount;
            Brightness = brightness;
            Contrast = contrast;
            Gamma = gamma;

        }

        public void undo()
        {
            redoStack.Push(new State(this.image, this.blurAmount, this.brightness, this.contrast, this.gamma));
            State state = undoStack.Pop();
            Image = new Bitmap(state.image);
            Blur = state.Blur;
            Brightness = state.Brightness;
            Contrast = state.Contrast;
            Gamma = state.Gamma;
        }

        public void redo()
        {
            undoStack.Push(new State(this.image, this.blurAmount, this.brightness, this.contrast, this.gamma));
            State state = redoStack.Pop();
            Image = new Bitmap(state.image);
            Blur = state.Blur;
            Brightness = state.Brightness;
            Contrast = state.Contrast;
            Gamma = state.Gamma;
        }
    }
}
