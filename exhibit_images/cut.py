import tkinter as tk
from PIL import Image, ImageTk, ImageDraw
from functools import partial
import sys
import os

if len(sys.argv) < 2:
    print('Usage: python cut.py <filename>')
    sys.exit(0)

FILENAME = sys.argv[1]
BASE = os.path.basename(FILENAME).split('.')[0]

scale_factor = 1/4
WIDTH = None
HEIGHT = None
LINE = None
IMAGE = None

root = tk.Tk()

def draw_line(canvas, event):
    global LINE
    if LINE is not None:
        canvas.delete(LINE)
    LINE = canvas.create_line(0, event.y, WIDTH, event.y, fill='red')

def wiped(image, delete_box):
    im = image.copy()
    mask = Image.new('L', im.size, color=255)
    draw = ImageDraw.Draw(mask) 
    draw.rectangle(delete_box, fill=0)
    im.putalpha(mask)
    return im    

def cut_image(event):
    root.quit()
    ycut = event.y / scale_factor
    w, h = IMAGE.size
    wiped(IMAGE, (0, 0, w, ycut)).save(BASE + 'LOWER.png')
    wiped(IMAGE, (0, ycut + 1, w, h)).save(BASE + 'UPPER.png')


class Application(tk.Frame):
    def __init__(self, master=None):
        super().__init__(master)
        global WIDTH
        global HEIGHT
        global IMAGE
        # image setup: load, scale down
        IMAGE = Image.open(FILENAME)
        self.pilImage = Image.open(FILENAME)
        width, height = self.pilImage.size
        width = WIDTH = round(width*scale_factor)
        height = HEIGHT = round(height*scale_factor)
        self.width = width
        self.height = height
        self.pilImage = self.pilImage.resize((width, height), Image.ANTIALIAS)
        self.image = ImageTk.PhotoImage(self.pilImage)

        self.canvas = tk.Canvas(master=self, width=width, height=height)
        self.canvas.pack()
        self.canvas.create_image((0, 0), image=self.image, anchor='nw')
        self.canvas.bind('<Motion>', partial(draw_line, self.canvas))
        self.canvas.bind('<Button-1>', cut_image)
        self.pack()



app = Application(master=root)
app.mainloop()

